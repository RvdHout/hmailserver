using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using NUnit.Framework;
using RegressionTests.Shared;

namespace RegressionTests.Infrastructure.Persistence
{
   [TestFixture]
   public class AccountNameValidation : TestFixtureBase
   {
      [Test]
      public void TestAccountContainingBackwardSlashInMailbox()
      {
         AssertInvalidEmailAddress("\\@test.com");
      }

      [Test]
      public void TestAccountContainingForwardSlashInMailbox()
      {
         AssertInvalidEmailAddress("/@test.com");
      }

      [Test]
      public void TestAccountContainingBackwardSlashInDomainName()
      {
         AssertInvalidEmailAddress("john@te\\st.com");
      }

      [Test]
      public void TestAccountContainingForwardSlashInDomainName()
      {
         AssertInvalidEmailAddress("john@te//st.com");
      }


      [Test]
      public void TestAccountContainingSpaceInMailboxNameWithoutQuotes()
      {
         AssertInvalidEmailAddress("John Smith@test.com");
      }

      [Test]
      public void TestAccountContainingSpaceInMailboxNameWithQuotes()
      {
         AssertInvalidEmailAddress("\"JohnSmith\"@test.com", "Failed to save object. The account address may not contain spaces or quotes.");
      }

      [Test]
      public void TestAccountContainingSpaceInMailboxNameWithQuoteAndSpace()
      {
         AssertInvalidEmailAddress("\"John Smith\"@test.com", "Failed to save object. The account address may not contain spaces or quotes.");
      }

      [Test]
      public void TestAccountContainingSlashInMailboxNameWithQuotes()
      {
         AssertInvalidEmailAddress("\"John\\Smith\"@test.com");
      }

      [Test]
      public void TestAccountContainingSpecialCharacters1()
      {
         AssertValidEmailAddress("user+mailbox@test.com");
      }

      [Test]
      public void TestAccountContainingSpecialCharacters2()
      {
         AssertInvalidEmailAddress("customer/department=shipping@test.com");
      }

      [Test]
      public void TestAccountContainingSpecialCharacters3()
      {
         AssertValidEmailAddress("$A12345@test.com");
      }

      [Test]
      public void TestAccountContainingSpecialCharacters4()
      {
         AssertValidEmailAddress("!def!xyz%abc@test.com");
      }

      [Test]
      public void TestAccountContainingSpecialCharacters5()
      {
         AssertValidEmailAddress("_somename@test.com");
      }

      [Test]
      public void TestAccountContainingSpecialCharacters6()
      {
         AssertInvalidEmailAddress("!#$%&'*+-/=?^_`.{|}~@test.com");
      }

      [Test]
      public void TestAccountWithoutAddress()
      {
         AssertInvalidEmailAddress("");
      }

      [Test]
      public void TestAccountBelongingToAnotherDomain()
      {
         AssertInvalidEmailAddress("");

         var exception = Assert.Throws<COMException>(() => SingletonProvider<TestSetup>.Instance.AddAccount(_domain, "test@other.example.com", "secret"));
         Assert.AreEqual("Failed to save object. The account address domain does not match the owning domain name.", exception.Message);
      }

      [Test]
      public void TestAccountWithLeadingDotInLocalPart()
      {
         AssertInvalidEmailAddress(".user@test.com");
      }

      [Test]
      public void TestAccountWithTrailingDotInLocalPart()
      {
         AssertInvalidEmailAddress("user.@test.com");
      }

      [Test]
      public void TestAccountWithConsecutiveDotsInLocalPart()
      {
         AssertInvalidEmailAddress("us..er@test.com");
      }

      [Test]
      public void TestAccountWithSingleDotInLocalPart()
      {
         AssertValidEmailAddress("us.er@test.com");
      }

      [Test]
      public void TestAccountWithMaxLengthLocalPart()
      {
         // 64-char local part is the RFC 5321 maximum and should be accepted
         AssertValidEmailAddress("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa@test.com");
      }

      [Test]
      public void TestAccountWithTooLongLocalPart()
      {
         // 65-char local part exceeds the RFC 5321 maximum
         // Supported for backwards compatibility.
         AssertValidEmailAddress("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa@test.com");
      }

      private void AssertInvalidEmailAddress(string address, string expectedErrorMessage = "Failed to save object. The account address is not a valid email address.")
      {
         var exception = Assert.Throws<COMException>(() => SingletonProvider<TestSetup>.Instance.AddAccount(_domain, address, "secret"));
         Assert.AreEqual(expectedErrorMessage, exception.Message);
      }

      private void AssertValidEmailAddress(string address)
      {
         SingletonProvider<TestSetup>.Instance.AddAccount(_domain, address, "secret");
      }
   }
}
