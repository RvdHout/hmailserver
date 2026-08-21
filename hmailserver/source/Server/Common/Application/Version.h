#pragma once
                                              #define HMAILSERVER_VERSION "5.7.0"
                                              #define HMAILSERVER_VERSION_NUMERIC 5,7,0,2732
                                              #define HMAILSERVER_BUILD "2732"
                                              #define HMAILSERVER_MOD "22"
/*
[list=1]
[*]Added: Received-SPF: diagnostic header controlled with INI setting "AddReceivedSPFHeader" (skip for authenticated client connections or localhost!)
[*]Added: (envelope-from <user@domain.com>), for <user@domain.com> to Received: header (if single recipient!)
[*]Fix: All 5xx errors are permanent errors and should be treated as such, eg: contributing to invalid commands counter
[*]Fix: hMailServer AUTH PLAIN in SMTP fails when authzid is supplied
[*]Fix: hMailServer AUTHENTICATE PLAIN in IMAP exposed account password in log, added OnClientLogon() event trigger within AUTHENTICATE PLAIN routine
[*]Fix: Improved AUTH PLAIN base64 encoded username and password masking, retain client command format for troubleshooting purposes
[*]Fix: Strip possible spaces in DKIM 'p' parameter, there shouldn't be any spaces but it's a common mistake so we act lenient and strip any spaces found
[*]Added: Google Feedback Loop header Feedback-ID in DKIM signing [url=https://github.com/hmailserver/hmailserver/pull/492]pull 492[/url]
[*]Fix: Better log on forward failures, https://github.com/maxsnts/hmailserver/commit/7e285c3a1abe11ad4605aa71bd64176989c473a1
[*]Restructure SMTPMessageHeaderCreator
[*]Fix: Could not retrieve PTR record from local mail client, see: [url=https://www.hmailserver.com/forum/viewtopic.php?p=258078]this[/url] forum topic
[*]Added: Allow unauthenticated local to local e-mail delivery on SPF Pass controlled with INI setting LocalToLocalByPassAuthOnSPFPass=1, see: [url=https://www.hmailserver.com/forum/viewtopic.php?p=258436]this[/url] forum topic
[*]Fix: Prevent EHLO/HELO bypass via SMTP RSET, see: [url=https://github.com/cybercode3/hmailserver/commit/88ec404b05550bd4879e153439bf6a7f14eb6638]this[/url] commit
[*]Fix: POP3 Return -ERR instead of +ERR when inbox message fetch fails, see: [url=https://github.com/cybercode3/hmailserver/commit/bc31f5eedd2d39ac320caf9870e5191eb835067c]this[/url] commit
[*]Fix: SMTP disconnect after STARTTLS handshake failure, see: [url=https://github.com/cybercode3/hmailserver/commit/0c58dc25d2ccb665f52666261aab431ae86b361a] this[/url] commit
[*]Added: Add "In-Reply-To" and "References" headers to Auto-Submitted messages, see: [url=https://github.com/hmailserver/hmailserver/pull/535]pull 535[/url]
[*]Fix: Domain size calculation used the wrong column on MS SQL, SQL CE and PostgreSQL, see: [url=https://github.com/RvdHout/hmailserver/commit/d727913504a07da800fc7801e8c7fffb50171b1a] this[/url] commit
[*]Fix: Locate the originating IP address without trusting HELO/EHLO, see: [url=https://github.com/hmailserver/hmailserver/issues/168]issue 168[/url]
[*]Fix: Flush log files when they are kept open, see: [url=https://github.com/hmailserver/hmailserver/pull/553]pull 553[/url]
[*]Fix: Add missing Message-ID for all submitted messages, see: [url=https://github.com/hmailserver/hmailserver/pull/552]pull 552[/url]
[*]Added: IMAP LIST Extension for Special-Use Mailboxes, see: [url=https://github.com/hmailserver/hmailserver/issues/534]issue 534[/url]
[*]Fix: Improve robustness when moving/reading files, see: [url=https://github.com/hmailserver/hmailserver/pull/556]pull 556[/url]
[/list]
*/