CREATE TABLE #TempRoles
(
	[Name] NVARCHAR(255) NOT NULL,
	[SystemKey] NVARCHAR(100) NOT NULL,
)

INSERT INTO #TempRoles ([Name], [SystemKey]) 
VALUES
('New User', 'new_user'),
('Tenant Owner', 'tenant_owner'),
('Store Manager', 'store_manager'),
('Store Staff', 'store_staff');

UPDATE Roles SET IsActive=0;

MERGE [Roles] As [Target]
USING
	(SELECT [Name], [SystemKey] FROM #TempRoles) AS [Source]
	ON [Target].[SystemKey] = [Source].[SystemKey]
WHEN MATCHED THEN
	UPDATE SET [Target].[Name] = [Source].[Name], [Target].IsActive=1
WHEN NOT MATCHED THEN
	INSERT ([Name], [SystemKey]) VALUES ([Source].[Name], [Source].[SystemKey]);

