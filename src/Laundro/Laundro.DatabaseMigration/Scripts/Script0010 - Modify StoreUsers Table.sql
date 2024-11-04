ALTER TABLE [StoreUsers]
DROP CONSTRAINT PK_StoreUser;

GO;

ALTER TABLE [StoreUsers]
ADD CONSTRAINT Pk_StoreUserRoleActive PRIMARY KEY (UserId, StoreId, RoleId, IsActive);