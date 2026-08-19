using hMailServer;
using NUnit.Framework;
using RegressionTests.Infrastructure;
using System;
using System.Threading;

namespace RegressionTests.POP3.Fetching
{
   internal class LockHelper
   {
      public static void WaitForUnlock(FetchAccount fetchAccount)
      {
         var timeoutTime = DateTime.Now.Add(TimeSpan.FromSeconds(30));

         while (DateTime.Now < timeoutTime)
         {
            if (!fetchAccount.IsLocked)
               return;

            Thread.Sleep(100);
         }

         var defaultLog = LogHandler.ReadCurrentDefaultLog();
         Assert.Fail("At {0}, fetch account was not unlocked.", DateTime.Now);
      }
   }
}