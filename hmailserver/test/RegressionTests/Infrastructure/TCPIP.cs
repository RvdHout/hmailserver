// Copyright (c) 2010 Martin Knafve / hMailServer.com.  
// http://www.hmailserver.com

using System;
using NUnit.Framework;
using RegressionTests.Shared;
using hMailServer;

namespace RegressionTests.Infrastructure
{
   [TestFixture]
   public class TCPIP : TestFixtureBase
   {
      [Test]
      [Category("TCP/IP implementation")]
      [Description("Ensure that basic resolution of existing domain names work.")]
      public void TestMXQueryExistingDomain()
      {
         Application application = SingletonProvider<TestSetup>.Instance.GetApp();


         string sQuery = application.Utilities.GetMailServer("martin@hmailserver.com");
         if (sQuery != TestSetup.GethMailServerCOMIPaddress())
            throw new Exception("ERROR - DNX query failed: " + sQuery);
      }

      [Test]
      [Category("TCP/IP implementation")]
      [Description("Ensure that basic resolution of non-existing domain names work.")]
      public void TestMXQueryNonExistentDomain()
      {
         Application application = SingletonProvider<TestSetup>.Instance.GetApp();

         string query = application.Utilities.GetMailServer("martin@23sdfakm52lvcxbmvxcbmdtapvxcpaasdf.com");
         Assert.IsTrue(query.Length == 0);
      }



      //RvdH
      [Test]
      [Category("TCP/IP implementation")]
      public void NullMXRecordShouldNotResolve()
      {
         Application application = SingletonProvider<TestSetup>.Instance.GetApp();
         // If a MX record contains a Null MX no mail server should be returned
         string actualServer = application.Utilities.GetMailServer("test@allelassers.nl");

         Assert.AreEqual("", actualServer);
      }

      //RvdH
      [Test]
      [Category("TCP/IP implementation")]
      public void IPAddressAsMXRecordShouldResolve()
      {
         Application application = SingletonProvider<TestSetup>.Instance.GetApp();
         // Okay, this is an invalid MX record. The MX record should always contain 
         // a host name but in this case it appears an IP address. We'll be kind to
         // the domain owner and still deliver the email to him.
         var actualServer = application.Utilities.GetMailServer("test@test.allelassers.nl");

         Assert.AreEqual("10.0.0.0", actualServer);
      }

      //RvdH
      [Test]
      [Category("TCP/IP implementation")]
      public void CnameDomainRecordsShouldResolve()
      {
         Application application = SingletonProvider<TestSetup>.Instance.GetApp();
         // If a Domain name resolves to a CNAME record, the CNAME record should be followed.
         var actualServer = application.Utilities.GetMailServer("test@external.bankup.be");

         Assert.AreEqual("10.0.0.1", actualServer);
      }


      [Test]
      [Category("TCP/IP implementation")]
      [Description("Ensure that it's possible to re-configure which ports hMailServer should listen on")]
      public void TestPortOpening()
      {
         Application oApp = SingletonProvider<TestSetup>.Instance.GetApp();

         oApp.Settings.TCPIPPorts.SetDefault();

         var pSMTPSimulator = new TcpConnection();
         var pPOP3Simulator = new Pop3ClientSimulator();
         var pIMAPSimulator = new ImapClientSimulator();

         oApp.Stop();

         TCPIPPorts oPorts = oApp.Settings.TCPIPPorts;
         for (int i = 0; i < oPorts.Count; i++)
         {
            TCPIPPort oTestPort = oPorts[i];
            if (oTestPort.Protocol == eSessionType.eSTIMAP)
               oTestPort.PortNumber = 14300;
            else if (oTestPort.Protocol == eSessionType.eSTPOP3)
               oTestPort.PortNumber = 11000;
            else if (oTestPort.Protocol == eSessionType.eSTSMTP && oTestPort.PortNumber == 25)
               oTestPort.PortNumber = 2500;

            oTestPort.Save();
         }

         oApp.Start();

         Assert.IsTrue(pIMAPSimulator.TestConnect(14300));
         Assert.IsTrue(pPOP3Simulator.TestConnect(11000));
         Assert.IsTrue(pSMTPSimulator.TestConnect(2500));

         oApp.Stop();

         TCPIPPort oPort = oApp.Settings.TCPIPPorts.Add();
         oPort.Protocol = eSessionType.eSTSMTP;
         oPort.PortNumber = 25000;
         oPort.Save();

         oApp.Start();

         // Try to connect to the new port
         Assert.IsTrue(pSMTPSimulator.TestConnect(25000));

         oApp.Stop();

         // Delete the new port again
         oApp.Settings.TCPIPPorts.DeleteByDBID(oPort.ID);

         // Reset the ports
         oApp.Settings.TCPIPPorts.SetDefault();

         oApp.Start();

         Assert.IsTrue(pSMTPSimulator.TestConnect(25));
         Assert.IsTrue(pSMTPSimulator.TestConnect(587));
         Assert.IsTrue(pPOP3Simulator.TestConnect(110));
         Assert.IsTrue(pIMAPSimulator.TestConnect(143));
      }

      [Test]
      public void TestDefaultPortCount()
      {
         Application oApp = SingletonProvider<TestSetup>.Instance.GetApp();

         oApp.Settings.TCPIPPorts.SetDefault();

         oApp.Stop();
         oApp.Start();

         var ports = oApp.Settings.TCPIPPorts;

         Assert.AreEqual(4, ports.Count);
      }
   }
}