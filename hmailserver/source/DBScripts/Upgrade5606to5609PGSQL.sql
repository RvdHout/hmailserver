alter table hm_accounts add column accountvacationabortspamflagged smallint not null;

alter table hm_accounts add column accountforwardabortspamflagged smallint not null;

alter table hm_rule_actions add column actionabortspamflagged smallint not null;

update hm_dbversion set value = 5609;