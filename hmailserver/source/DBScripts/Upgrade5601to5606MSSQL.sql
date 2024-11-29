insert into hm_settings (settingname, settingstring, settinginteger) values ('TlsOptions', '', 0)

insert into hm_settings (settingname, settingstring, settinginteger) values ('ascheckptr', '', 0)

insert into hm_settings (settingname, settingstring, settinginteger) values ('ascheckptrscore', '', 1)

insert into hm_settings (settingname, settingstring, settinginteger) values ('IPv6Preferred', '', 0)

alter table hm_fetchaccounts add column famimerecipientheaders nvarchar(255) not null default 'To,CC,X-RCPT-TO,X-Envelope-To'

update hm_dbversion set value = 5606