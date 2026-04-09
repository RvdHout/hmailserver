// Copyright (c) 2006 Martin Knafve / hMailServer.com.  
// http://www.hmailserver.com

#pragma once

namespace HM
{
   class Account;
   class Message;
   class MessageData;

   class SMTPForwarding
   {
   public:

      bool PerformForwarding(std::shared_ptr<const Account> pRecipientAccount, std::shared_ptr<Message> pOriginalMessage);

   private:

      // Function for checking if an email address is part of a local domain.
      bool IsLocalDomain(const String &sAddress, String &outDomain) const;

   };
}