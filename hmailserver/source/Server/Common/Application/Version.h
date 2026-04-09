#pragma once

#define HMAILSERVER_VERSION "5.6.9"
#define HMAILSERVER_VERSION_NUMERIC 5,6,9,2641
#define HMAILSERVER_BUILD "2641"
#define HMAILSERVER_MOD "95"

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
[*]Added: MIME Recipient Headers as configurable comma delimited string [url=https://github.com/hmailserver/hmailserver/pull/428]pull #428[/url]
[*]Fix: AUTH PLAIN followed by base64 encoded username and password (on same line) in log exposed account password. [url=https://github.com/hmailserver/hmailserver/pull/437]pull #437[/url]
[*]Fix: rfc4954 After a successful AUTH command completes, a server MUST reject any further AUTH commands with a 503 reply. [url=https://github.com/hmailserver/hmailserver/pull/438]pull #438[/url]
[*]Fix: HM5157 SpamAssassinClient::OnReadError, The WinSock error code is 2. [url=https://github.com/hmailserver/hmailserver/issues/167]issue #167[/url]
[*]Fix: TCP/IP Ports 'Default' button in GUI did not add the standard port SMTP 587 (this port is standard added by SQL script during installation) [url=https://github.com/hmailserver/hmailserver/pull/441]#pull 441[/url]
[*]Removed: Collection of statistics, since it's no longer being used. [url=https://github.com/hmailserver/hmailserver/issues/435]issue #435[/url]
[*]Fix: If MSSQL OLE DB Provider 18 or later is installed, prefer that one, since it supports TLS1.1/1.2 which older providers do not. [url=https://github.com/hmailserver/hmailserver/issues/186]issue 186[/url]
[*]Added: %MACRO_ORIGINAL_HEADER% macro expansion for 'Set header value' rules
[*]Fix: S/MIME Signed Message Failing Signature Validation due to X-hMailServer-Envelope-From header position, effectively this means ungrouping of the X-hMailServer-* headers
[*]Fix: Error logged if email address length exceeds 254 characters [url=https://github.com/hmailserver/hmailserver/issues/393]issue #393[/url]
[*]Fix: DKIM Signing Not Performed On Messages > 10 MB [url=https://github.com/hmailserver/hmailserver/issues/387]issue #387[/url]
[*]Fix: DKIM (header) verification fails when an email is received with no subject [url=https://github.com/hmailserver/hmailserver/issues/245]issue #245[/url]
[*]Added: DKIM signing support for CSA-Compliant Mail Headers, eg: X-CSA-Complaints and List-Unsubscribe-Post headers
[*]Fix: Prefer IPv6 over IPv4 didn't do anything, added the missing part (sorting of found IP Addresses)
[*]Added: Received-SPF: diagnostic header controlled with INI setting "AddReceivedSPFHeader" (skip for authenticated client connections or localhost!)
[*]Added: (envelope-from <user@domain.com>), for <user@domain.com> to Received: header (if single recipient!)
[*]Fix: All 5xx errors are permanent errors and should be treated as such, eg: contributing to invalid commands counter
[*]Fix: hMailServer reports incorrect RFC822.SIZE [url=https://github.com/hmailserver/hmailserver/pull/477]pull 477[/url]
[*]Fix: hMailServer AUTH PLAIN in SMTP fails when authzid is supplied
[*]Fix: Minor bugfix where the Received-SPF diagnostic header gave incomplete or inaccurate results
[*]Fix: Minor bugfix where the Received-SPF diagnostic header gave inaccurate results when receiving mail through a (trusted/internal) relay
[*]Added: IPv6 Support for BLCheck [url=https://github.com/hmailserver/hmailserver/pull/487]pull 487[/url]
[*]Experimental: improved SA winsock 2 error fix, ignore all boost::asio::error::eof errors which probably are related to IMAP FETCH HM5136 errors
[*]Fix: Improved AUTH PLAIN base64 encoded username and password masking, retain client command format for troubleshooting purposes
[*]Fix: SURBL modification to check full URI's and trimmed down URI's
[*]Fix: Strip possible spaces in DKIM 'p' parameter, there shouldn't be any spaces but it's a common mistake so we act lenient and strip any spaces found
[*]Fix: Apple IOS related HM5136, HM4208 and subsequent "OutOfMemoryHandler" errors [url=https://github.com/hmailserver/hmailserver/issues/475]issue 475[/url], credits to Rado https://github.com/hunterius-prime
[*]Added: Google Feedback Loop header Feedback-ID in DKIM signing [url=https://github.com/hmailserver/hmailserver/pull/492]pull 492[/url]
[*]Fix: Better log on forward failures, https://github.com/maxsnts/hmailserver/commit/7e285c3a1abe11ad4605aa71bd64176989c473a1
[*]Fix: Spam scoring/marking/count inconsistencies
[*]Added: eMessageFlag Spam = 128
[*]Added: Abort forwarding if original message is marked as Spam
[*]Added: Abort vacationmessage if original message is marked as Spam
[*]Added: Abort autoreply/forwarding through rules if original message is marked as Spam
[*]Fix: IMAP FETCH on message/rfc822 MIME part (roundcube specific when messages are forwarded as attachment)
[*]Added: Optionally add X-Original-Rcpt-To-header to incoming email [url=https://github.com/hmailserver/hmailserver/issues/498]issue 498[/url]
[*]Restructure SMTPMessageHeaderCreator
[*]Added: Support for secp384r1:x25519 Server Key Exchange Groups, if prefer server ciphers is enabled secp384r1:x25519:secp256r1 is now the prefered order
[*]Fix: Could not retrieve PTR record from local mail client https://www.hmailserver.com/forum/viewtopic.php?p=258078
[*]Fix: Improve SUBRL lookup efficiency
[*]Added: Allow unauthenticated local to local e-mail delivery on SPF Pass controlled with INI setting LocalToLocalByPassAuthOnSPFPass=1, see: [url=https://www.hmailserver.com/forum/viewtopic.php?p=258436]this[/url] forum topic
[*]Fix: Various other (small) fixes and improvements
[*]Fix: Prevent EHLO/HELO bypass via SMTP RSET, see: [url=https://github.com/cybercode3/hmailserver/commit/88ec404b05550bd4879e153439bf6a7f14eb6638]this[/url] commit
[*]Fix: POP3 Return -ERR instead of +ERR when inbox message fetch fails, see: [url=https://github.com/cybercode3/hmailserver/commit/bc31f5eedd2d39ac320caf9870e5191eb835067c]this[/url] commit
[*]Fix: Prevent rewrite of message untouched message parts, , see: [url=https://github.com/hmailserver/hmailserver/pull/506]pull 506[/url]
[*]Fix: Stop SMTPDeliveryManager gracefully, see: [url=https://github.com/hmailserver/hmailserver/pull/507]pull 507[/url]
[*]Fix: Fix false positives in line ending check, see: [url=https://github.com/hmailserver/hmailserver/pull/508]pull 508[/url]
[*]Fix: Improved regex for validation of email addresses & domains, see [url=https://github.com/hmailserver/hmailserver/pull/509]pull 509[/url]
[*]Fix: Scheduled Expiry of Auto-reply doesnt clear the 'already replied' cache, see: [url=https://github.com/hmailserver/hmailserver/pull/510]pull 510[/url]
[*]Added: DKIM sign (forwarded) messages send from local to local domains, see: [url=https://github.com/hmailserver/hmailserver/issues/511]issue 511[/url]
[/list]
*/