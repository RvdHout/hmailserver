using hMailServer;
using NUnit.Framework;
using RegressionTests.Shared;

namespace RegressionTests.Infrastructure
{
   [TestFixture]
   public class DnsResolution : TestFixtureBase
   {
      [OneTimeSetUp]
      public void OneTimeSetUp()
      {
         var application = SingletonProvider<TestSetup>.Instance.GetApp();
         _utilities = application.Utilities;
      }

      private Utilities _utilities;

      [Test]
      public void CNameRecordsShouldBeFollowed()
      {
         // If a MX record contains a CNAME record, the CNAME record should be followed.
         // According to RFC, a server owner should not add a CNAME record to MX record,
         // but many do and hMailServer has supported this historically.
         var actualServer = _utilities.GetMailServer("example@cname-test.hmailserver.com");
         var expectedServer = _utilities.GetMailServer("example@hmailserver.com");

         Assert.AreEqual(expectedServer, actualServer);
      }

      [Test]
      public void NoneExistentRecordsShouldNotResolve()
      {
         // If a MX record contains a CNAME record, the CNAME record should be followed.
         // According to RFC, a server owner should not add a CNAME record to MX record,
         // but many do and hMailServer has supported this historically.
         var actualServer = _utilities.GetMailServer("example@invalid.hmailserver.com");

         Assert.AreEqual("", actualServer);
      }

      [Test]
      public void NullMXRecordShouldNotResolve()
      {
         // If a MX record contains a Null MX no mail server should be returned
         var actualServer = _utilities.GetMailServer("test@allelassers.nl");

         Assert.AreEqual(string.Empty, actualServer);
      }

      [Test]
      public void IPAddressAsMXRecordShouldResolve()
      {
         // Okay, this is an invalid MX record. The MX record should always contain 
         // a host name but in this case it appears an IP address. We'll be kind to
         // the domain owner and still deliver the email to him.
         var actualServer = _utilities.GetMailServer("test@test.allelassers.nl");

         Assert.AreEqual("10.0.0.0", actualServer);
      }

      [Test]
      public void CnameDomainRecordsShouldResolve()
      {
         // If a Domain name resolves to a CNAME record, the CNAME record should be followed.
         var actualServer = _utilities.GetMailServer("test@external.bankup.be");

         Assert.AreEqual("10.0.0.1", actualServer);
      }
   }
}