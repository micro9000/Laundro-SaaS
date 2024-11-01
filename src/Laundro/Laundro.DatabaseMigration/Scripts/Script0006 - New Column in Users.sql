ALTER TABLE [Users]
ADD [CreatedInTenantId] INT NULL;

ALTER TABLE [Users]
ADD CONSTRAINT FK_CreatedInTenantId FOREIGN KEY (CreatedInTenantId) REFERENCES Tenants (Id)

CREATE UNIQUE NONCLUSTERED INDEX Idx_Email_CreatedInTenantId_IsActive
   ON [Users] (Email, CreatedInTenantId);

CREATE NONCLUSTERED INDEX Idx_Email ON [Users] (Email)