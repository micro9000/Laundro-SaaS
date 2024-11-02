
ALTER TABLE [Roles]
ADD RoleLevel VARCHAR(20) NULL;

GO;

ALTER TABLE [Roles]
ADD CONSTRAINT ChkRoleLevelVal CHECK (RoleLevel in ('tenant', 'store'));

GO;

