// Copyright (c) 2010 Martin Knafve / hMailServer.com.  
// http://www.hmailserver.com

#include "StdAfx.h"

#include "SPF.h"
#include "rmspf.h"

#ifdef _DEBUG
#define DEBUG_NEW new(_NORMAL_BLOCK, __FILE__, __LINE__)
#define new DEBUG_NEW
#endif

namespace HM
{
   SPF::SPF(void)
   {
      // Initialize. This is only done once.
      SPFInit(NULL,0, SPF_Multithread);
   }

   SPF::~SPF(void)
   {

   }

   SPF::Result
   SPF::Test(const String &sSenderIP, const String &sSenderEmail, const String &sHeloHost, String &sExplanation)
   {
      USES_CONVERSION;
      // String sDomain = StringParser::ExtractDomain(sSenderEmail); Not used at all!?

      int family;
      if (sSenderIP.Find(_T(":")) > 0)
         family=AF_INET6;
      else
         family=AF_INET;

      // Convert the IP address from a dotted string
      // to a binary form. We use the SPF library to
      // do this.

      char BinaryIP[100];
      if (SPFStringToAddr(T2A(sSenderIP),family,BinaryIP)==NULL)
         return Neutral;

      const char* explain;
      int result=SPFQuery(family,BinaryIP,T2A(sSenderEmail),NULL,T2A(sHeloHost),NULL,&explain);

      if (explain != NULL)
      {
         sExplanation = explain;
         SPFFree(explain);
      }

#ifdef _DEBUG
      //LOG_DEBUG(Formatter::Format("Spam test: SpamTestSPF, RMSPF Result: {0}", SPFResultString(result)));
      LOG_APPLICATION(Formatter::Format("Spam test: SpamTestSPF, RMSPF Result: {0}", SPFResultString(result)));
#endif

      if (result == SPF_Fail)
      {
         // FAIL
         return Fail;
      }
      else if (result == SPF_Pass)
      {
         return Pass;
      }

      return Neutral;
   }

   String
   SPF::ReceivedSPFHeader(const String &sHostname, const String &sSenderIP, const String &sSenderEmail, const String &sHeloHost, String &sResult)
   {
      USES_CONVERSION;
      String sDomain = StringParser::ExtractDomain(sSenderEmail);

      int family;
      if (sSenderIP.Find(_T(":")) > 0)
         family = AF_INET6;
      else
         family = AF_INET;

      // Convert the IP address from a dotted string
      // to a binary form. We use the SPF library to
      // do this.

      char BinaryIP[100];
      if (SPFStringToAddr(T2A(sSenderIP), family, BinaryIP) == NULL)
         return sResult;

      int result = SPFQuery(family, BinaryIP, T2A(sSenderEmail), NULL, T2A(sHeloHost), NULL, NULL);

      String sSPFResultString = SPFResultString(result);
      String sResultMessage;

      // http://www.open-spf.org/SPF_Received_Header/
      switch (result)
      {
         case SPF_Pass: // 0
            sResultMessage.Format(_T("%s (%s: domain of %s designates %s as permitted sender)"), sSPFResultString.c_str(), sHostname.c_str(), !sSenderEmail.IsEmpty() ? sSenderEmail.c_str() : sHeloHost.c_str(), sSenderIP.c_str());
            break;
         case SPF_SoftFail: // 1
            sResultMessage.Format(_T("%s (%s: domain of transitioning %s does not designate %s as permitted sender)"), sSPFResultString.c_str(), sHostname.c_str(), !sSenderEmail.IsEmpty() ? sSenderEmail.c_str() : sHeloHost.c_str(), sSenderIP.c_str());
            break;
         case SPF_Fail: // 2
            sResultMessage.Format(_T("%s (%s: domain of %s does not designate %s as permitted sender)"), sSPFResultString.c_str(), sHostname.c_str(), !sSenderEmail.IsEmpty() ? sSenderEmail.c_str() : sHeloHost.c_str(), sSenderIP.c_str());
            break;
         case SPF_Neutral: // 3
            sResultMessage.Format(_T("%s (%s: %s is neither permitted nor denied by domain of %s)"), sSPFResultString.c_str(), sHostname.c_str(), sSenderIP.c_str(), !sSenderEmail.IsEmpty() ? sSenderEmail.c_str() : sHeloHost.c_str());
            break;
         case SPF_None: // 4
            sResultMessage.Format(_T("%s (%s: domain of %s does not designate permitted sender hosts)"), sSPFResultString.c_str(), sHostname.c_str(), !sSenderEmail.IsEmpty() ? sSenderEmail.c_str() : sHeloHost.c_str());
            break;
         case SPF_TempError: // 5
            sResultMessage.Format(_T("%s (%s: temporary error in processing during lookup of %s: DNS Timeout)"), sSPFResultString.c_str(), sHostname.c_str(), sDomain.c_str());
            break;
         case SPF_PermError: // 6
            sResultMessage.Format(_T("%s (%s: permanent error in processing during lookup of %s)"), sSPFResultString.c_str(), sHostname.c_str(), sDomain.c_str());
            break;
         case SPF_None + SPF_BadDomain: // 20
         case SPF_None + SPF_NoDomain: // 36
         case SPF_None + SPF_Literal: // 52
            sResultMessage.Format(_T("%s (%s: domain %s is an address literal, malformed, or non-existent)"), sSPFResultString.c_str(), sHostname.c_str(), sDomain.c_str());
            break;
         default:
            return sResult;
      }

      if (!sSenderEmail.IsEmpty())
         sResult.Format(_T("Received-SPF: %s\r\n\tidentity=mailfrom;\r\n\tclient-ip=%s;\r\n\tenvelope-from=<%s>;\r\n"), sResultMessage.c_str(), sSenderIP.c_str(), sSenderEmail.c_str());
      else
         sResult.Format(_T("Received-SPF: %s\r\n\tidentity=helo;\r\n\tclient-ip=%s;\r\n\thelo=%s;\r\n"), sResultMessage.c_str(), sSenderIP.c_str(), sHeloHost.c_str());

      return sResult;
   }

   void SPFTester::Test()
   {
      String sExplanation;

      if (SPF::Instance()->Test("185.216.75.37", "example@hmailserver.com", "mail.hmailserver.com", sExplanation) != SPF::Pass)
      {
         // Should be allowed. 
         throw;
      }

      if (SPF::Instance()->Test("1.2.3.4", "example@hmailserver.com", "mail.hmailserver.com", sExplanation) != SPF::Fail)
      {
         // Should not be allowed.
         throw;
      }
   }


}