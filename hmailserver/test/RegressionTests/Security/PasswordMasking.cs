// Copyright (c) 2010 Martin Knafve / hMailServer.com.  
// http://www.hmailserver.com

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using NUnit.Framework;
using RegressionTests.Infrastructure;
using RegressionTests.Shared;
using hMailServer;

namespace RegressionTests.Security
{
   [TestFixture]
   public class PasswordMasking : TestFixtureBase
   {
      #region Setup/Teardown

      [SetUp]
      public new void SetUp()
      {
         _status = SingletonProvider<TestSetup>.Instance.GetApp().Status;
         string logFile = _settings.Logging.CurrentDefaultLog;
         if (File.Exists(logFile))
            File.Delete(logFile);
      }

      #endregion

      private Status _status;

      private const string _username = "NonSecretUser@test.com";
      private const string _password = "SecretPassword";

      private string EncodeBase64(string s)
      {
         byte[] bytes = Encoding.UTF8.GetBytes(s);
         return Convert.ToBase64String(bytes);
      }

      private string GetUsername()
      {
         return _username;
      }

      private string GetPassword()
      {
         return _password;
      }

      /*
      private void EnsureNoPassword()
      {
         string logFileName = _settings.Logging.CurrentDefaultLog;

         for (int i = 1; i <= 10; i++)
         {
            string text = string.Empty;

            try
            {
               using (var fileStream = File.Open(logFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
               using (var streamReader = new StreamReader(fileStream))
               {
                  text = streamReader.ReadToEnd();
               }
            }
            catch
            {
               if (i == 10)
                  throw;

               Thread.Sleep(1000);
               continue;
            }

            Assert.IsTrue(text.Contains(_username) || text.Contains(EncodeBase64(_username)), text);
            Assert.IsFalse(text.Contains(_password) || text.Contains(EncodeBase64(_password)), text);
            Assert.IsTrue(text.Contains("***"), text);
         }
      }
      */
      //RvdH
      private void EnsureNoPassword(bool usernameExpected = true)
      {
         string logFileName = _settings.Logging.CurrentDefaultLog;

         for (int i = 1; i <= 10; i++)
         {
            string text = string.Empty;

            try
            {
               using (var fileStream = File.Open(logFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
               using (var streamReader = new StreamReader(fileStream))
               {
                  text = streamReader.ReadToEnd();
               }
            }
            catch
            {
               if (i == 10)
                  throw;

               Thread.Sleep(1000);
               continue;
            }

            // masked Base64 encoded string in various formats to compare against
            var auth2param_masked = EncodeBase64("\\0" + _username + "\\0" + "***");
            var auth3param_masked = EncodeBase64(_username + "\\0" + _username + "\\0" + "***");
            var tabdelimited_masked = EncodeBase64(_username + "\\t" + _username + "\\t" + "***");

            if (usernameExpected)
               // should contain plain text username for POP en IMAP, Base64 encoded username for SMTP
               Assert.IsTrue(text.Contains(_username) || text.Contains(EncodeBase64(_username)), text);
            else
            {
               // should contain Base64 encoded strings for AUTH PLAIN, either \0 or \t delimited 
               Assert.IsTrue(text.Contains(auth2param_masked) || text.Contains(auth3param_masked) || text.Contains(tabdelimited_masked), text);
               // should not contain plain text username for POP en IMAP, Base64 encoded username for SMTP
               Assert.IsFalse(text.Contains(_username) || text.Contains(EncodeBase64(_username)), text);
            }

            Assert.IsFalse(text.Contains(_password) || text.Contains(EncodeBase64(_password)), text);

            if (!text.Contains(auth2param_masked) && !text.Contains(auth3param_masked) && !text.Contains(tabdelimited_masked))
               Assert.IsTrue(text.Contains("***"), text);
         }
      }

      internal Route AddRoutePointingAtLocalhostWithAuth(int numberOfTries, int port)
      {
         // Add a route pointing at localhost
         Settings oSettings = SingletonProvider<TestSetup>.Instance.GetApp().Settings;

         Route route = oSettings.Routes.Add();
         route.DomainName = "dummy-example.com";
         route.TargetSMTPHost = "localhost";
         route.TargetSMTPPort = port;
         route.NumberOfTries = numberOfTries;
         route.RelayerRequiresAuth = true;
         route.RelayerAuthUsername = GetUsername();
         route.SetRelayerAuthPassword(GetPassword());
         route.MinutesBetweenTry = 5;
         route.Save();

         return route;
      }

      [Test]
      public void TestIMAPServerLiteral()
      {
         var sim = new ImapClientSimulator();
         sim.Connect();
         Assert.IsTrue(sim.Send("a01 login " + GetUsername() + " {4}").StartsWith("+"));
         sim.Send(GetPassword());
         EnsureNoPassword();
      }

      [Test]
      public void TestIMAPServerLiteral2()
      {
         var sim = new ImapClientSimulator();
         sim.Connect();
         Assert.IsTrue(sim.Send("a01 login {" + GetUsername().Length.ToString() + "} {4}").StartsWith("+"));
         Assert.IsTrue(sim.Send(GetUsername() + " {" + GetPassword().Length.ToString() + "}").StartsWith("+"));
         sim.Send(GetPassword());
         EnsureNoPassword();
      }

      [Test]
      public void TestIMAPServerNormal()
      {
         var sim = new ImapClientSimulator();
         sim.ConnectAndLogon(GetUsername(), GetPassword());
         EnsureNoPassword();
      }

      [Test]
      public void TestPOP3Client()
      {
         var messages = new List<string>();

         string message = "From: Martin@example.com\r\n" +
                          "To: Martin@example.com\r\n" +
                          "Subject: Test\r\n" +
                          "\r\n" +
                          "Hello!";

         messages.Add(message);

         int port = TestSetup.GetNextFreePort();
         using (var pop3Server = new Pop3ServerSimulator(1, port, messages))
         {
            pop3Server.StartListen();

            Account account = SingletonProvider<TestSetup>.Instance.AddAccount(_domain, "user@test.com", "test");
            FetchAccount fa = account.FetchAccounts.Add();

            fa.Enabled = true;
            fa.MinutesBetweenFetch = 10;
            fa.Name = "Test";
            fa.Username = GetUsername();
            fa.Password = GetPassword();
            fa.UseSSL = false;
            fa.ServerAddress = "localhost";
            fa.Port = port;
            fa.ProcessMIMERecipients = false;
            fa.MIMERecipientHeaders = "To,CC,X-RCPT-TO,X-Envelope-To";
            fa.Save();

            fa.DownloadNow();

            pop3Server.WaitForCompletion();

            fa.Delete();

            string downloadedMessage = Pop3ClientSimulator.AssertGetFirstMessageText(account.Address, "test");
            Assert.IsTrue(downloadedMessage.Contains(message));

            EnsureNoPassword();
         }
      }

      [Test]
      public void TestPOP3Server()
      {
         var sim = new Pop3ClientSimulator();
         sim.ConnectAndLogon(GetUsername(), GetPassword());
         EnsureNoPassword();
      }

      [Test]
      public void TestSMTPClient()
      {
         Account account1 = SingletonProvider<TestSetup>.Instance.AddAccount(_domain, "test@test.com", "test");

         Assert.AreEqual(0, _status.UndeliveredMessages.Length);

         // No valid recipients...
         var deliveryResults = new Dictionary<string, int>();
         deliveryResults["test@dummy-example.com"] = 550;

         int smtpServerPort = TestSetup.GetNextFreePort();

         using (var server = new SmtpServerSimulator(1, smtpServerPort))
         {
            server.AddRecipientResult(deliveryResults);
            server.StartListen();

            // Add a route so we can connect to localhost.
            AddRoutePointingAtLocalhostWithAuth(0, smtpServerPort);

            // Send message to this route.
            var smtp = new SmtpClientSimulator();
            smtp.Send("test@test.com", "test@dummy-example.com", "Test", "Test message");

            CustomAsserts.AssertRecipientsInDeliveryQueue(0);

            string undeliveredMessages = _status.UndeliveredMessages;

            // Wait for the client to disconnect.
            server.WaitForCompletion();

            CustomAsserts.AssertRecipientsInDeliveryQueue(0);

            EnsureNoPassword();
         }
      }

      [Test]
      public void TestSMTPServerAuthLogin()
      {
         var sim = new SmtpClientSimulator();
         string errorMsg;

         CustomAsserts.Throws<AuthenticationException>(() => sim.ConnectAndLogon(GetUsername(), GetPassword(), out errorMsg));
         
         EnsureNoPassword();
      }

      [Test]
      public void TestSMTPServerAuthLoginUsernameAsThirdParameter()
      {
         _settings.AllowSMTPAuthPlain = true;

         var sock = new TcpConnection();
         sock.Connect(25);
         Assert.IsTrue(sock.Receive().StartsWith("220"));
         sock.Send("EHLO test.com\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("250"));

         string base64EncodedUsername = EncodeBase64(GetUsername());
         sock.Send("AUTH LOGIN " + base64EncodedUsername + "\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("334"));

         sock.Send(EncodeBase64(GetPassword()) + "\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("535"));
         sock.Send("QUIT\r\n");
         sock.Disconnect();
         EnsureNoPassword();
      }

      //RvdH
      [Test]
      public void TestSMTPServerAuthPlain()
      {
         _settings.AllowSMTPAuthPlain = true;

         var sock = new TcpConnection();
         sock.Connect(25);
         Assert.IsTrue(sock.Receive().StartsWith("220"));
         sock.Send("EHLO test.com\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("250"));
         sock.Send("AUTH PLAIN\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("334"));

         string str = "\0" + GetUsername() + "\0" + GetPassword();

         sock.Send(EncodeBase64(str) + "\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("535"));
         sock.Send("QUIT\r\n");
         sock.Disconnect();
         EnsureNoPassword(false);
      }
      
      //RvdH
      [Test]
      public void TestSMTPServerAuthPlainRFC4616()
      {
         _settings.AllowSMTPAuthPlain = true;

         var sock = new TcpConnection();
         sock.Connect(25);
         Assert.IsTrue(sock.Receive().StartsWith("220"));
         sock.Send("EHLO test.com\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("250"));
         sock.Send("AUTH PLAIN\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("334"));

         string str = GetUsername() + "\0" + GetUsername() + "\0" + GetPassword();

         sock.Send(EncodeBase64(str) + "\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("535"));
         sock.Send("QUIT\r\n");
         sock.Disconnect();
         EnsureNoPassword(false);
      }

      //RvdH
      [Test]
      public void TestSMTPServerAuthPlainSingleLine()
      {
         _settings.AllowSMTPAuthPlain = true;

         var sock = new TcpConnection();
         sock.Connect(25);
         Assert.IsTrue(sock.Receive().StartsWith("220"));
         sock.Send("EHLO test.com\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("250"));

         string str = "\0" + GetUsername() + "\0" + GetPassword();

         sock.Send("AUTH PLAIN " + EncodeBase64(str) + "\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("535"));
         sock.Send("QUIT\r\n");
         sock.Disconnect();
         EnsureNoPassword(false);
      }

      //RvdH
      [Test]
      public void TestSMTPServerAuthPlainSingleLineRFC4616()
      {
         _settings.AllowSMTPAuthPlain = true;

         var sock = new TcpConnection();
         sock.Connect(25);
         Assert.IsTrue(sock.Receive().StartsWith("220"));
         sock.Send("EHLO test.com\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("250"));

         string str = GetUsername() + "\0" + GetUsername() + "\0" + GetPassword();

         sock.Send("AUTH PLAIN " + EncodeBase64(str) + "\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("535"));
         sock.Send("QUIT\r\n");
         sock.Disconnect();
         EnsureNoPassword(false);
      }

      //RvdH
      [Test]
      public void TestSMTPServerAuthPlainInvalidCommand()
      {
         _settings.AllowSMTPAuthPlain = false;

         var sock = new TcpConnection();
         sock.Connect(25);
         Assert.IsTrue(sock.Receive().StartsWith("220"));
         sock.Send("EHLO test.com\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("250"));
         
         // null delimited
         string str = GetUsername() + "\0" + GetUsername() + "\0" + GetPassword();
         
         // server should also strip used password when credentials are sent even when not accepted/expected
         sock.Send(EncodeBase64(str) + "\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("503 Bad sequence of commands"));
         sock.Send("QUIT\r\n");
         sock.Disconnect();
         EnsureNoPassword(false);
      }

      //RvdH
      [Test]
      public void TestSMTPServerAuthPlainDisabledTabDelimitedInvalidCommand()
      {
         _settings.AllowSMTPAuthPlain = false;

         var sock = new TcpConnection();
         sock.Connect(25);
         Assert.IsTrue(sock.Receive().StartsWith("220"));
         sock.Send("EHLO test.com\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("250"));
         sock.Send("AUTH PLAIN\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("504"));
         
         // tab delimited
         string str = GetUsername() + "\t" + GetUsername() + "\t" + GetPassword();
         
         // server should also strip used password when credentials are sent even when disabled
         sock.Send(EncodeBase64(str) + "\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("503 Bad sequence of commands"));
         sock.Send("QUIT\r\n");
         sock.Disconnect();
         EnsureNoPassword(false);
      }

   }
}