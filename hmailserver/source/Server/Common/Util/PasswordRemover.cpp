// Copyright (c) 2010 Martin Knafve / hMailServer.com.  
// http://www.hmailserver.com

#include "stdafx.h"
#include "PasswordRemover.h"

#include <Boost/Regex.hpp>

#ifdef _DEBUG
#define DEBUG_NEW new(_NORMAL_BLOCK, __FILE__, __LINE__)
#define new DEBUG_NEW
#endif

namespace HM
{

   PasswordRemover::PasswordRemover()
   {
      
   }

   PasswordRemover::~PasswordRemover()
   {

   }

	void
	PasswordRemover::Remove(PRType prt, String &sClientCommand)
	{
      if (prt == PRIMAP)
      {
         String delimiter = "\\t", passwordmask = "***";

         // AUTHENTICATE PLAIN command and both user name and password in line.
         String sRegex = "^(.+)(?> AUTHENTICATE PLAIN )((?:[A-Z\\d+/]{4})*(?:[A-Z\\d+/]{3}=|[A-Z\\d+/]{2}==)?)$";
         boost::wregex expression(sRegex, boost::wregex::icase);
         boost::wsmatch matches;
         if (boost::regex_match(sClientCommand, matches, expression))
         {
            String sCommandTag = matches[1];
            String sAuthentication;
            String sBase64Encoded = matches[2];
            StringParser::Base64Decode(sBase64Encoded, sAuthentication);

            if (StringParser::IsBase64NullDelimited(sBase64Encoded))
               delimiter = "\\0";

            std::vector<String> plain_args = StringParser::SplitString(sAuthentication, "\t");

            if (plain_args.size() == 3 && plain_args[1].GetLength() > 0)
            {
               String authzid = plain_args[0];
               String authcid = plain_args[1];
               String authplain = authzid.append(delimiter).append(authcid).append(delimiter).append(passwordmask);
               String sCommandBase64Encoded;
               StringParser::Base64Encode(authplain, sCommandBase64Encoded);
               sClientCommand = sCommandTag + " AUTHENTICATE PLAIN " + sCommandBase64Encoded;
            }

            return;
         }
         
         // AUTH PLAIN is disabled and client send credentials anyway, this should not happen under normal circumstances 
         // Or client (re)send credentials when not expected/accepted
         // user name and password on new line.
         sRegex = "^((?:[A-Z\\d+/]{4})*(?:[A-Z\\d+/]{3}=|[A-Z\\d+/]{2}==)?)$";
         boost::wregex expr(sRegex, boost::wregex::icase);
         if (boost::regex_match(sClientCommand, expr))
         {
            // Both user name and password in line.
            String sAuthentication;
            StringParser::Base64Decode(sClientCommand, sAuthentication);

            if (StringParser::IsBase64NullDelimited(sClientCommand))
               delimiter = "\\0";

            std::vector<String> plain_args = StringParser::SplitString(sAuthentication, "\t");

            if (plain_args.size() == 3 && plain_args[1].GetLength() > 0)
            {
               String authzid = plain_args[0];
               String authcid = plain_args[1];
               String authplain = authzid.append(delimiter).append(authcid).append(delimiter).append(passwordmask);
               String sCommandBase64Encoded;
               StringParser::Base64Encode(authplain, sCommandBase64Encoded);
               sClientCommand = sCommandBase64Encoded;
            }

            return;
         }

         // Starts the second word.
         int iCommandStart = sClientCommand.Find(_T(" "));
         if (iCommandStart == -1)
            return;
      
         iCommandStart++;

         if (sClientCommand.Mid(iCommandStart, 5).CompareNoCase(_T("LOGIN")) != 0)
            return;
         
         // Find end of login name.
         int iUsernameEnd = sClientCommand.Find(_T(" "), iCommandStart + 6);
         if (iUsernameEnd <= 0)
            return;

         sClientCommand = sClientCommand.Mid(0, iUsernameEnd);
         sClientCommand += " ***";
      }
      else if (PRIMAP)
      {
         // Remove from POP3 ...
         if (sClientCommand.Mid(0, 4).CompareNoCase(_T("PASS")) != 0)
            return;

         // Remove the password.
         sClientCommand = sClientCommand.Mid(0, 4) + " ***";
      }

	}
}
