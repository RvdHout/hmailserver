// Copyright (c) 2010 Martin Knafve / hMailServer.com.
// http://www.hmailserver.com

using System;
using NUnit.Framework;
using RegressionTests.Shared;

namespace RegressionTests.Infrastructure
{
   [TestFixture]
   public class LogFlushing : TestFixtureBase
   {
      [SetUp]
      public new void SetUp()
      {
         SingletonProvider<TestSetup>.Instance.AddAccount(_domain, "logflush@test.com", "test");

         var logging = _settings.Logging;

         // Only log the SMTP conversation. If everything is logged, the entry we're
         // waiting for may be written to disk simply because the log buffer runs full.
         logging.LogApplication = false;
         logging.LogDebug = false;
         logging.LogTCPIP = false;

         logging.KeepFilesOpen = true;
      }

      [TearDown]
      public new void TearDown()
      {
         _settings.Logging.KeepFilesOpen = false;

         // The log file is closed the first time something is written to it after the
         // setting has been disabled. Write something now, so that the log file isn't
         // left open when the next test tries to delete it.
         try
         {
            SendMessage();
         }
         catch (Exception)
         {
            // Don't hide the reason the test failed.
         }
      }

      [Test]
      [Description("Issue 549, LogFile Flush when no activity")]
      public void LastLogEntryShouldBeWrittenToDiskEvenThoughLogFileIsKeptOpen()
      {
         SendMessage();

         // "221 goodbye" is the last thing written to the log file for the session above.
         // Since the log file is kept open, it stays in the log buffer until hMailServer
         // flushes the file.
         Assert.IsTrue(LogHandler.DefaultLogContains("221 goodbye"));
      }

      private void SendMessage()
      {
         var smtpClientSimulator = new SmtpClientSimulator();
         smtpClientSimulator.Send("logflush@test.com", "logflush@test.com", "Log flush", "Log flush");
      }
   }
}