#pragma once

#define HMAILSERVER_VERSION "5.6.8"
#define HMAILSERVER_BUILD "2574.46"

/*
[list=1]
[*]Added: OnHELO(oClient) event, [url=https://github.com/hmailserver/hmailserver/issues/153]issue #153[/url]
[*]Fix: Incorrect DEBUG logging for event 'OnDeliverMessage', [url=https://github.com/hmailserver/hmailserver/issues/181]issue #181[/url]
[*]Added: Include HTMLBody into IMAP TEXT search, [url=https://github.com/hmailserver/hmailserver/pull/193]pull #193[/url]
[*]Fix: implicit conversion: "int" to "unsigned char" [url=https://github.com/hmailserver/hmailserver/pull/204]pull #204[/url]
[*]Fix: Faulty SMTP 'Disconnect client after too many invalid commands'  pull [url=https://github.com/hmailserver/hmailserver/issues/160]issue #160[/url]
[*]Fix: SMTP server error "550 Unsupported ESMTP extension" on MAIL FROM:... AUTH=<> [with fix] [url=https://github.com/hmailserver/hmailserver/issues/164]issue #164[/url]
[*]Fix: Removed warning if backup was more than 1,5GB and 15GB limit. There's no longer a recommended max-size - the time will vary with the installation size. [url=https://github.com/hmailserver/hmailserver/issues/69]issue #69[/url]
[*]Fix: Speed up 'update hm_messages set messageflags' [url=https://github.com/hmailserver/hmailserver/issues/221]issue  #221[/url]
[*]Added: threat authenticated users as localsender if the sender is authenticated and AuthUserIsLocal=1 INI setting [url=https://www.hmailserver.com/forum/viewtopic.php?f=7&p=196733#p196733]Office 2016/2019 Bug[/url]
[*]Added: [url=https://www.hmailserver.com/forum/viewtopic.php?p=177487#p177487]Return-Path header[/url] as topmost header before sending the message to SA (+ delete Return-Path header after the SA check completes) [url=https://github.com/hmailserver/hmailserver/issues/116]issue #116[/url]
[*]Added: Event OnClientLogon(oClient), New ClientInfo property oClient.Authenticated (Boolean)
[*]Fix: Handling of long UIDL response lists was too slow. [url=https://github.com/hmailserver/hmailserver/issues/93]issue #93[/url]
[*]Fix: When calling SpamAssassin and there was a connection failure, sometimes temporary files were left behind [url=https://github.com/hmailserver/hmailserver/issues/100]issue #100[/url]
[*]Fix: SURBL detection properly fails to detect url's ending with a query string [url=https://github.com/hmailserver/hmailserver/issues/108]issue  #108[/url]
[*]Fix: If a route is set up, but the recipient does not match an address in the route address list, the domain catch-all should be used if specified. [url=https://github.com/hmailserver/hmailserver/issues/74]issue #74[/url]
[*]Fix: ExternalFetcher DELE when no RETR, pull [url=https://github.com/hmailserver/hmailserver/pull/254]pull #254[/url]
[*]Fix: SMTP multiply max message size with 1024 [url=https://github.com/hmailserver/hmailserver/issues/267]issue #267[/url]
[*]Added: email address variable to SignatureAdder.cpp [url=https://github.com/hmailserver/hmailserver/pull/256]pull #265[/url]
[*]Fix: DKIM on acccount-rule 'reply' not applied #172 [url=https://github.com/hmailserver/hmailserver/issues/172]issue #172[/url]
[*]Fix: preserve RewriteEnvelopeFromWhenForwarding setting [url=https://www.hmailserver.com/forum/viewtopic.php?f=7&t=34026]when forwarding from account rule[/url]
[*]Fix: The logical flow should be to disregard "Require SMTP authentication" if "Allow deliveries from" is unselected [url=https://github.com/hmailserver/hmailserver/issues/287]issue #287[/url]
[*]Added: ability to DKIM sign NDR messages (forwarded to external) [url=https://github.com/hmailserver/hmailserver/pull/301]pull #301[/url]
[*]Added: Use custom daemonaddressdomain from INI [url=https://github.com/hmailserver/hmailserver/pull/301]pull #301[/url]
[*]Fix: SURBL regex [url=https://github.com/hmailserver/hmailserver/pull/320]pull #320[/url]
[*][s]Added: RMSPF library SpamTestSPF Result to DEBUG logging, see [url=https://www.hmailserver.com/forum/viewtopic.php?p=218710#p218710]this[/url] forum topic[/s]
[*]Fix: Ignore SpamTestSPF and SpamTestHeloHost when send thru local IP Address, see [url=https://www.hmailserver.com/forum/viewtopic.php?f=10&t=34865]this[/url] forum topic 
[*]Added: SPF test to include "HELO/EHLO host" used by DNS macros [url=https://github.com/hmailserver/hmailserver/pull/353]pull 353[/url]
[*]Fix: 'Unencrypted Cleartext Login' vulnerability detection [url=https://github.com/hmailserver/hmailserver/pull/342]pull 242[/url]
[*]Fix: Anti-spam; Maximum message to scan (KB) is now limited to 256MB, see [url=https://www.hmailserver.com/forum/viewtopic.php?f=22&p=226989#p226989]this[/url] forum topic
[*]Added: "UseDNSCache" INI setting. 1 = Use cache in DNS Client service (default). 0 = Bypass cache in DNS Client service. [url=https://github.com/hmailserver/hmailserver/pull/396]pull 396[/url]
[*]Fix: AWStats::LogDeliveryFailure was called twice for each message in OnDeliveryFailed [url=https://github.com/hmailserver/hmailserver/pull/368]pull 368[/url] 
[*]Fix: oMessage object was empty (in some circumstances) when called from OnDeliveryFailed [url=https://github.com/hmailserver/hmailserver/pull/369]pull 369[/url] 
[*]Added: Experimental events OnRecipientUnknown(oClient, oMessage) and OnTooManyInvalidCommands(oClient, oMessage) [url=https://github.com/hmailserver/hmailserver/pull/390]pull #390[/url]
[*]Added: TLS server cipher preference support and support for prioritizing ChaCha20Poly1305 [url=https://github.com/hmailserver/hmailserver/pull/379]pull 379[/url]
[*]Added: DKIM signature for domain aliases [url=https://github.com/hmailserver/hmailserver/pull/383]pull 383[/url]
[*]Added: "DNSServer" INI setting. Single (local) DNS server IPv4 addresses to use within hMailServer instead of default system DNS. [url=https://github.com/hmailserver/hmailserver/pull/396]pull 396[/url]
[*]Added: Auto-Submitted header for auto-generated messages [url=https://github.com/hmailserver/hmailserver/pull/386]pull 386[/url]
[*]Added: New ClientInfo properties oClient.EncryptedConnection (Boolean), oClient.CipherVersion, oClient.CipherName and oClient.CipherBits [url=https://github.com/hmailserver/hmailserver/pull/391]pull #391[/url]
[*]Added: DomainMembers DistributionList mode [url=https://github.com/hmailserver/hmailserver/pull/392]pull #392[/url]
[*]Added: X-hMailServer-Envelope-From header, grouping off all X-hMailServer-* headers, For consistency equalize Return-Path format used within hMailServer, eg: Return-Path: <return-route-addr> (always use angle brackets)
[*]Fix: integer overflow check for SPF macro segment count [url=https://github.com/hmailserver/hmailserver/pull/399]pull #399[/url]
[*]Added: New ClientInfo property oClient.SessionID (long) [url=https://github.com/hmailserver/hmailserver/issues/401]issue #401[/url]
[*]Added: New Status property oApp.Status.ThreadID (long) [url=https://www.hmailserver.com/forum/viewtopic.php?f=10&t=33787&hilit=threadid]this[/url] forum topic
[*]Added: rDNS/PTR AntiSpam Check [url=https://github.com/hmailserver/hmailserver/pull/413]pull #413[/url]
[*]Added: Prefer IPv6 over IPv4 [url=https://github.com/hmailserver/hmailserver/pull/415]pull #415[/url]
[*]Fix: SpamTestHeloHost IPv6 Fix [url=https://github.com/hmailserver/hmailserver/pull/416]pull #416[/url]
[*]Fix: Changed ClamAV Scan from STREAM to INSTREAM. [url=https://github.com/hmailserver/hmailserver/issues/420]issue #420[/url]
[/list]
*/