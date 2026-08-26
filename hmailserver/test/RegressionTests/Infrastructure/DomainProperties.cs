// Copyright (c) 2010 Martin Knafve / hMailServer.com.
// http://www.hmailserver.com

using NUnit.Framework;
using RegressionTests.Shared;

namespace RegressionTests.Infrastructure
{
   [TestFixture]
   public class DomainProperties : TestFixtureBase
   {
      [Test]
      [Category("Domains")]
      [Description("Ensure that empty domain size is reported as zero")]
      public void SizeBeforeSend()
      {
         var domain = SingletonProvider<TestSetup>.Instance.AddTestDomain();
         SingletonProvider<TestSetup>.Instance.AddAccount(domain, "test@test.com", "test");

         Assert.AreEqual(0, domain.Size);
      }

      [Test]
      [Category("Domains")]
      [Description("Regression test for PR #537 - domain size was calculated using the wrong column " +
                   "on MS SQL, SQL CE and PostgreSQL, causing the reported size to be incorrect (near zero).")]
      public void SizeReflectsMessagesAcrossAccounts()
      {
         var domain = SingletonProvider<TestSetup>.Instance.AddTestDomain();
         var account1 = SingletonProvider<TestSetup>.Instance.AddAccount(domain, "test1@test.com", "test");
         var account2 = SingletonProvider<TestSetup>.Instance.AddAccount(domain, "test2@test.com", "test");

         // Each dummy body is roughly 700 KB. Domain.Size is reported in whole
         // megabytes (bytes are truncated), so send enough to clear 1 MB.
         var body = TestSetup.CreateLargeDummyMailBody();

         SmtpClientSimulator.StaticSend("test1@test.com", "test1@test.com", "Test message", body);
         SmtpClientSimulator.StaticSend("test1@test.com", "test1@test.com", "Test message", body);
         ImapClientSimulator.AssertMessageCount("test1@test.com", "test", "Inbox", 2);

         SmtpClientSimulator.StaticSend("test2@test.com", "test2@test.com", "Test message", body);
         SmtpClientSimulator.StaticSend("test2@test.com", "test2@test.com", "Test message", body);
         ImapClientSimulator.AssertMessageCount("test2@test.com", "test", "Inbox", 2);

         // Each account holds ~1.4 MB, so the true domain total is ~2.8 MB (2 whole MB).
         // The bug (wrong column) matches at most one account, capping the result at 1 MB,
         // so require the full cross-account aggregate rather than merely > 0.
         Assert.GreaterOrEqual(domain.Size, 2,
            "Domain size must aggregate every account in the domain, not a single id-matched account.");
      }
   }
}