#pragma once

#define HMAILSERVER_VERSION "5.6.8"
#define HMAILSERVER_BUILD "2555.35"

/*
[list=1]
[*]Supports Sub OnHELO(oClient) event, [url=https://github.com/hmailserver/hmailserver/issues/153]issue #153[/url]
[*]Fixed Incorrect DEBUG logging for event 'OnDeliverMessage', [url=https://github.com/hmailserver/hmailserver/issues/181]issue #181[/url]
[*]Include HTMLBody into IMAP TEXT search, [url=https://github.com/hmailserver/hmailserver/pull/193]pull #193[/url]
[*]Fixed implicit conversion: "int" to "unsigned char" [url=https://github.com/hmailserver/hmailserver/pull/204]pull #204[/url]
[*]Faulty: SMTP 'Disconnect client after too many invalid commands'  pull [url=https://github.com/hmailserver/hmailserver/issues/160]issue #160[/url]
[*]SMTP server error "550 Unsupported ESMTP extension" on MAIL FROM:... AUTH=<> [with fix] [url=https://github.com/hmailserver/hmailserver/issues/164]issue #164[/url]
[*]Removed warning if backup was more than 1,5GB and 15GB limit. There's no longer a recommended max-size - the time will vary with the installation size. [url=https://github.com/hmailserver/hmailserver/issues/69]issue #69[/url]
[*]Speed up 'update hm_messages set messageflags' [url=https://github.com/hmailserver/hmailserver/issues/221]issue  #221[/url]
[*]Treat authenticated users as localsender if the sender is authenticated and AuthUserIsLocal=1 INI setting [url=https://www.hmailserver.com/forum/viewtopic.php?f=7&p=196733#p196733]Office 2016/2019 Bug[/url]
[*]Add [url=https://www.hmailserver.com/forum/viewtopic.php?p=177487#p177487]Return-Path header[/url] as topmost header before sending the message to SA (+ delete Return-Path header after the SA check completes) [url=https://github.com/hmailserver/hmailserver/issues/116]issue #116[/url]
[*]Experimental eventhandler OnClientLogon(oClient), New ClientInfo property oClient.Authenticated (Boolean)
[*]Handling of long UIDL response lists was too slow. [url=https://github.com/hmailserver/hmailserver/issues/93]issue #93[/url]
[*]When calling SpamAssassin and there was a connection failure, sometimes temporary files were left behind [url=https://github.com/hmailserver/hmailserver/issues/100]issue #100[/url]
[*]SURBL detection properly fails to detect url's ending with a query string [url=https://github.com/hmailserver/hmailserver/issues/108]issue  #108[/url]
[*]If a route is set up, but the recipient does not match an address in the route address list, the domain catch-all should be used if specified. [url=https://github.com/hmailserver/hmailserver/issues/74]issue #74[/url]
[*]Fix ExternalFetcher DELE when no RETR, pull [url=https://github.com/hmailserver/hmailserver/pull/254]pull #254[/url]
[*]SMTP multiply max message size with 1024 [url=https://github.com/hmailserver/hmailserver/issues/267]issue #267[/url]
[*]Add email address variable to SignatureAdder.cpp [url=https://github.com/hmailserver/hmailserver/pull/256]pull #265[/url]
[*]DKIM on acccount-rule 'reply' not applied #172 [url=https://github.com/hmailserver/hmailserver/issues/172]issue #172[/url]
[*]preserve RewriteEnvelopeFromWhenForwarding setting [url=https://www.hmailserver.com/forum/viewtopic.php?f=7&t=34026]when forwarding from account rule[/url]
[*]The logical flow should be to disregard "Require SMTP authentication" if "Allow deliveries from" is unselected [url=https://github.com/hmailserver/hmailserver/issues/287]issue #287[/url]
[*]Add ability to DKIM sign NDR messages (forwarded to external) [url=https://github.com/hmailserver/hmailserver/pull/301]pull #301[/url]
[*]Use custom daemonaddressdomain from INI [url=https://github.com/hmailserver/hmailserver/pull/301]pull #301[/url]
[*]Fix SURBL regex [url=https://github.com/hmailserver/hmailserver/pull/320]pull #320[/url]
[*]Add RMSPF library SpamTestSPF Result to DEBUG logging, see [url=https://www.hmailserver.com/forum/viewtopic.php?p=218710#p218710]this[/url] forum topic
[*]Ignore SpamTestSPF and SpamTestHeloHost when send thru local IP Address, see [url=https://www.hmailserver.com/forum/viewtopic.php?f=10&t=34865]this[/url] forum topic 
[*]SPF test to include "HELO/EHLO host" used by DNS macros [url=https://github.com/hmailserver/hmailserver/pull/353]pull 353[/url]
[*]Fix 'Unencrypted Cleartext Login' vulnerability detection [url=https://github.com/hmailserver/hmailserver/pull/342]pull 242[/url]
[*]Anti-spam; Maximum message to scan (KB) is now limited to 256MB, see [url=https://www.hmailserver.com/forum/viewtopic.php?f=22&p=226989#p226989]this[/url] forum topic
[*]Added "UseDNSCache" INI setting. 1 = Use cache in DNS Client service (default). 0 = Bypass cache in DNS Client service.
[*]AWStats::LogDeliveryFailure was called twice for each message in OnDeliveryFailed
[*]oMessage object was emtpy when called from OnDeliveryFailed in SMTPDeliverer.cpp
[*]Added: Experimental events OnRecipientUnknown(oClient, oMessage) and OnTooManyInvalidCommands(oClient, oMessage)
[*]Added: Auto-Submitted header for auto-generated messages [url=https://github.com/hmailserver/hmailserver/pull/386]pull 386[/url]
[*]Added: "DNSServer" INI setting. Single (local) DNS server IPv4 addresses to use within hMailServer instead of default system DNS.
[/list]
*/