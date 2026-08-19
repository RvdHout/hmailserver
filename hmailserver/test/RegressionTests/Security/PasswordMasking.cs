// Copyright (c) 2010 Martin Knafve / hMailServer.com.  
// http://www.hmailserver.com

using hMailServer;
using NUnit.Framework;
using RegressionTests.Infrastructure;
using RegressionTests.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Authentication;
using System.Text;
using System.Threading;

namespace RegressionTests.Security
{
   [TestFixture]
   public class PasswordMasking : TestFixtureBase
   {
      [SetUp]
      public new void SetUp()
      {
         _status = SingletonProvider<TestSetup>.Instance.GetApp().Status;
         var logFile = _settings.Logging.CurrentDefaultLog;
         if (File.Exists(logFile))
            File.Delete(logFile);
      }

      private Status _status;

      private const string _masterusername = "MasterUser@example.test";
      private const string _username = "NonSecretUser@example.test";
      private const string _password = "SecretPassword";

      private string EncodeBase64(string s)
      {
         var bytes = Encoding.UTF8.GetBytes(s);
         return Convert.ToBase64String(bytes);
      }

      private string GetMasterUsername()
      {
         return _masterusername;
      }

      private string GetUsername()
      {
         return _username;
      }

      private string GetPassword()
      {
         return _password;
      }


      private void EnsureNoPassword(bool usernameExpected = true)
      {
         var logFileName = _settings.Logging.CurrentDefaultLog;

         for (var i = 1; i <= 10; i++)
         {
            var text = string.Empty;

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

            var email = new System.Net.Mail.MailAddress(_masterusername);
            var localpart = email.User;

            // masked Base64 encoded string in various formats to compare against
            var auth2param_masked = EncodeBase64("\\0" + _username + "\\0" + "***");
            var auth3param_masked = EncodeBase64(_username + "\\0" + _username + "\\0" + "***");
            var tabdelimited_masked = EncodeBase64(_username + "\\t" + _username + "\\t" + "***");

            var auth3param_master_masked = EncodeBase64(_username + "\\0" + _masterusername + "\\0" + "***");
            var auth3param_master_defaultdomain_masked = EncodeBase64(_username + "\\0" + localpart + "\\0" + "***");

            if (usernameExpected)
               // should contain plain text username for POP en IMAP, Base64 encoded username for SMTP
               Assert.IsTrue(text.Contains(_username) || text.Contains(EncodeBase64(_username)), text);
            else
            {
               // should contain Base64 encoded strings for AUTH PLAIN, either \0 or \t delimited 
               Assert.IsTrue(text.Contains(auth2param_masked) || text.Contains(auth3param_masked) || text.Contains(tabdelimited_masked) || text.Contains(auth3param_master_masked) || text.Contains(auth3param_master_defaultdomain_masked), text);
               // should not contain plain text username for POP en IMAP, Base64 encoded username for SMTP
               Assert.IsFalse(text.Contains(_username) || text.Contains(EncodeBase64(_username)), text);
            }

            Assert.IsFalse(text.Contains(_password) || text.Contains(EncodeBase64(_password)), text);

            if (!text.Contains(auth2param_masked) && !text.Contains(auth3param_masked) && !text.Contains(tabdelimited_masked) && !text.Contains(auth3param_master_masked) && !text.Contains(auth3param_master_defaultdomain_masked))
               Assert.IsTrue(text.Contains("***"), text);
         }
      }

      internal Route AddRoutePointingAtLocalhostWithAuth(int numberOfTries, int port)
      {
         // Add a route pointing at localhost
         var settings = SingletonProvider<TestSetup>.Instance.GetApp().Settings;

         var route = settings.Routes.Add();
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
         Assert.IsTrue(sim.Send("a01 login {" + GetUsername().Length + "} {4}").StartsWith("+"));
         Assert.IsTrue(sim.Send(GetUsername() + " {" + GetPassword().Length + "}").StartsWith("+"));
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
      public void TestIMAPServerSASLEnabled()
      {
         _settings.IMAPSASLPlainEnabled = true;

         var sim = new ImapClientSimulator();
         sim.Connect();

         string str = "\0" + GetUsername() + "\0" + GetPassword();

         sim.Send("a01 AUTHENTICATE PLAIN");
         sim.Send(EncodeBase64(str));
         sim.Disconnect();
         EnsureNoPassword(false);
      }

      [Test]
      public void TestIMAPServerSASLDisabled()
      {
         _settings.IMAPSASLPlainEnabled = false;

         var sim = new ImapClientSimulator();
         sim.Connect();

         string str = "\0" + GetUsername() + "\0" + GetPassword();

         string result = sim.Send("a01 AUTHENTICATE PLAIN");
         Assert.IsTrue(result.Contains("a01 NO IMAP AUTHENTICATE is not enabled."));

         result = sim.Send(EncodeBase64(str));
         EnsureNoPassword(false);

         // server response should also strip used password when credentials are sent even when disabled
         var auth2param_masked = EncodeBase64("\\0" + GetUsername() + "\\0" + "***");
         Assert.IsTrue(result.Contains(auth2param_masked + " BAD NULL COMMAND"));
         sim.Disconnect();
      }

      [Test]
      public void TestIMAPServerSASLTabDelimited()
      {
         // SASL Enabled

         _settings.IMAPSASLPlainEnabled = true;

         var sim = new ImapClientSimulator();
         sim.Connect();

         string str = "\t" + GetUsername() + "\t" + GetPassword();

         sim.Send("a01 AUTHENTICATE PLAIN");
         string result = sim.Send(EncodeBase64(str));
         Assert.IsTrue(result.Contains("a01 NO Invalid user name or password."));
         sim.Disconnect();

         // SASL Disabled

         _settings.IMAPSASLPlainEnabled = false;

         sim.Connect();

         result = sim.Send("b01 AUTHENTICATE PLAIN");
         Assert.IsTrue(result.Contains("b01 NO IMAP AUTHENTICATE is not enabled."));

         var auth2param_masked = EncodeBase64("\\t" + GetUsername() + "\\t" + "***");
         // server response should also strip used password when credentials are sent even when disabled
         result = sim.Send(EncodeBase64(str));
         Assert.IsTrue(result.Contains(auth2param_masked + " BAD NULL COMMAND"));
         sim.Disconnect();
      }

      [Test]
      public void TestIMAPServerSASLInvalidCommand()
      {
         _settings.IMAPSASLPlainEnabled = true;

         var sim = new ImapClientSimulator();
         sim.Connect();

         string str = GetUsername() + "\0" + GetUsername() + "\0" + GetPassword();
         // server should also strip used password when credentials are sent even when not accepted/expected
         string result = sim.Send(EncodeBase64(str));
         EnsureNoPassword(false);

         var auth2param_masked = EncodeBase64(GetUsername() + "\\0" + GetUsername() + "\\0" + "***");
         Assert.IsTrue(result.Contains(auth2param_masked + " BAD NULL COMMAND"));
         sim.Disconnect();
      }

      [Test]
      public void TestIMAPServerSASLNoMaster()
      {
         _settings.IMAPSASLPlainEnabled = true;

         var sim = new ImapClientSimulator();
         sim.Connect();

         string str = GetUsername() + "\0" + GetUsername() + "\0" + GetPassword();

         sim.Send("a01 AUTHENTICATE PLAIN");
         string result = sim.Send(EncodeBase64(str));
         EnsureNoPassword(false);

         Assert.IsTrue(result.Contains("a01 BAD No master user defined."));
         sim.Disconnect();
      }

      [Test]
      public void TestIMAPServerSASLWithMaster()
      {
         var masteraccount = SingletonProvider<TestSetup>.Instance.AddAccount(_domain, GetMasterUsername(), GetPassword());
         var account = SingletonProvider<TestSetup>.Instance.AddAccount(_domain, GetUsername(), GetPassword());

         _settings.IMAPSASLPlainEnabled = true;
         _settings.IMAPSASLInitialResponseEnabled = true;
         _settings.IMAPMasterUser = GetMasterUsername();

         var sim = new ImapClientSimulator();
         sim.Connect();

         string str = GetUsername() + "\0" + GetMasterUsername() + "\0" + GetPassword();

         sim.Send("a01 AUTHENTICATE PLAIN");
         string result = sim.Send(EncodeBase64(str));
         EnsureNoPassword(false);

         Assert.IsTrue(result.Contains("a01 OK LOGIN completed"));
         sim.Disconnect();
      }

      [Test]
      public void TestIMAPServerSASLWithMasterDefaultDomain()
      {
         var masteraccount = SingletonProvider<TestSetup>.Instance.AddAccount(_domain, GetMasterUsername(), GetPassword());
         var account = SingletonProvider<TestSetup>.Instance.AddAccount(_domain, GetUsername(), GetPassword());

         var email = new System.Net.Mail.MailAddress(GetMasterUsername());
         var localpart = email.User;

         _settings.DefaultDomain = _domain.Name;
         _settings.IMAPSASLPlainEnabled = true;
         _settings.IMAPSASLInitialResponseEnabled = true;
         _settings.IMAPMasterUser = localpart;

         var sim = new ImapClientSimulator();
         sim.Connect();

         string str = GetUsername() + "\0" + localpart + "\0" + GetPassword();

         sim.Send("a01 AUTHENTICATE PLAIN");
         string result = sim.Send(EncodeBase64(str));
         EnsureNoPassword(false);

         Assert.IsTrue(result.Contains("a01 OK LOGIN completed"));
         sim.Disconnect();

         _settings.DefaultDomain = string.Empty;
         _settings.IMAPSASLInitialResponseEnabled = false;
         _settings.IMAPMasterUser = string.Empty;
      }

      [Test]
      public void TestIMAPServerSASLEnabledSingleLine()
      {
         _settings.IMAPSASLPlainEnabled = true;

         var sim = new ImapClientSimulator();
         sim.Connect();

         string str = "\0" + GetUsername() + "\0" + GetPassword();

         string result = sim.Send("a01 AUTHENTICATE PLAIN " + EncodeBase64(str));
         EnsureNoPassword(false);

         Assert.IsTrue(result.Contains("a01 NO Invalid user name or password."));
         sim.Disconnect();
      }

      [Test]
      public void TestIMAPServerSASLDisabledSingleLine()
      {
         _settings.IMAPSASLPlainEnabled = false;

         var sim = new ImapClientSimulator();
         sim.Connect();

         string str = GetUsername() + "\0" + GetUsername() + "\0" + GetPassword();

         string result = sim.Send("a01 AUTHENTICATE PLAIN " + EncodeBase64(str));
         EnsureNoPassword(false);

         Assert.IsTrue(result.Contains("a01 NO IMAP AUTHENTICATE is not enabled."));
         sim.Disconnect();
      }

      [Test]
      public void TestPOP3Client()
      {
         var messages = new List<string>();

         var message = "From: Martin@example.com\r\n" +
                       "To: Martin@example.com\r\n" +
                       "Subject: Test\r\n" +
                       "\r\n" +
                       "Hello!";

         messages.Add(message);

         var port = TestSetup.GetNextFreePort();
         using (var pop3Server = new Pop3ServerSimulator(1, port, messages))
         {
            pop3Server.StartListen();

            var account = SingletonProvider<TestSetup>.Instance.AddAccount(_domain, "user@example.test", "test");
            var fa = account.FetchAccounts.Add();

            fa.Enabled = true;
            fa.MinutesBetweenFetch = 10;
            fa.Name = "Test";
            fa.Username = GetUsername();
            fa.Password = GetPassword();
            fa.UseSSL = false;
            fa.ServerAddress = "localhost";
            fa.Port = port;
            fa.ProcessMIMERecipients = false;
            fa.Save();

            fa.DownloadNow();

            pop3Server.WaitForCompletion();

            fa.Delete();

            var downloadedMessage = Pop3ClientSimulator.AssertGetFirstMessageText(account.Address, "test");
            StringAssert.Contains("Hello!", downloadedMessage);

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
         var account1 = SingletonProvider<TestSetup>.Instance.AddAccount(_domain, "test@example.test", "test");

         Assert.AreEqual(0, _status.UndeliveredMessages.Length);

         // No valid recipients...
         var deliveryResults = new Dictionary<string, int>();
         deliveryResults["test@dummy-example.com"] = 550;

         var smtpServerPort = TestSetup.GetNextFreePort();

         using (var server = new SmtpServerSimulator(1, smtpServerPort))
         {
            server.AddRecipientResult(deliveryResults);
            server.StartListen();

            // Add a route so we can connect to localhost.
            AddRoutePointingAtLocalhostWithAuth(0, smtpServerPort);

            // Send message to this route.
            var smtp = new SmtpClientSimulator();
            smtp.Send("test@example.test", "test@dummy-example.com", "Test", "Test message");

            CustomAsserts.AssertRecipientsInDeliveryQueue(0);

            var undeliveredMessages = _status.UndeliveredMessages;

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

         CustomAsserts.Throws<AuthenticationException>(() =>
            sim.ConnectAndLogon(GetUsername(), GetPassword(), out errorMsg));

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

         var base64EncodedUsername = EncodeBase64(GetUsername());
         sock.Send("AUTH LOGIN " + base64EncodedUsername + "\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("334"));

         sock.Send(EncodeBase64(GetPassword()) + "\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("535"));
         sock.Send("QUIT\r\n");
         sock.Disconnect();
         EnsureNoPassword();
      }

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

         var str = "\0" + GetUsername() + "\0" + GetPassword();

         sock.Send(EncodeBase64(str) + "\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("535"));
         sock.Send("QUIT\r\n");
         sock.Disconnect();
         EnsureNoPassword(false);
      }

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

         var str = GetUsername() + "\0" + GetUsername() + "\0" + GetPassword();

         sock.Send(EncodeBase64(str) + "\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("535"));
         sock.Send("QUIT\r\n");
         sock.Disconnect();
         EnsureNoPassword(false);
      }

      [Test]
      public void TestSMTPServerAuthPlainSingleLine()
      {
         _settings.AllowSMTPAuthPlain = true;

         var sock = new TcpConnection();
         sock.Connect(25);
         Assert.IsTrue(sock.Receive().StartsWith("220"));
         sock.Send("EHLO test.com\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("250"));

         var str = "\0" + GetUsername() + "\0" + GetPassword();

         sock.Send("AUTH PLAIN " + EncodeBase64(str) + "\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("535"));
         sock.Send("QUIT\r\n");
         sock.Disconnect();
         EnsureNoPassword(false);
      }

      [Test]
      public void TestSMTPServerAuthPlainSingleLineRFC4616()
      {
         _settings.AllowSMTPAuthPlain = true;

         var sock = new TcpConnection();
         sock.Connect(25);
         Assert.IsTrue(sock.Receive().StartsWith("220"));
         sock.Send("EHLO test.com\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("250"));

         var str = GetUsername() + "\0" + GetUsername() + "\0" + GetPassword();

         sock.Send("AUTH PLAIN " + EncodeBase64(str) + "\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("535"));
         sock.Send("QUIT\r\n");
         sock.Disconnect();
         EnsureNoPassword(false);
      }

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
         var str = GetUsername() + "\0" + GetUsername() + "\0" + GetPassword();

         // server should also strip used password when credentials are sent even when not accepted/expected
         sock.Send(EncodeBase64(str) + "\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("503 Bad sequence of commands"));
         sock.Send("QUIT\r\n");
         sock.Disconnect();
         EnsureNoPassword(false);
      }

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
         var str = GetUsername() + "\t" + GetUsername() + "\t" + GetPassword();

         // server should also strip used password when credentials are sent even when disabled
         sock.Send(EncodeBase64(str) + "\r\n");
         Assert.IsTrue(sock.Receive().StartsWith("503 Bad sequence of commands"));
         sock.Send("QUIT\r\n");
         sock.Disconnect();
         EnsureNoPassword(false);
      }

   }
}