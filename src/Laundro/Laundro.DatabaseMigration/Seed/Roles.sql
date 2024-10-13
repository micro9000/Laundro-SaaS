CREATE TABLE #TempRoles
(
	[Name] NVARCHAR(255) NOT NULL,
	[SystemKey] NVARCHAR(100) NOT NULL,
)

INSERT INTO #TempRoles ([Name], [SystemKey]) 
VALUES
('New User', 'new_user'),
('Tenant Owner', 'tenant_owner'),
('Tenant Employee', 'tenant_employee'),
('Store Manager', 'store_manager'),
('Store Staff', 'store_staff');

MERGE [Roles] As [Target]
USING
	(SELECT [Name], [SystemKey] FROM #TempRoles) AS [Source]
	ON [Target].[SystemKey] = [Source].[SystemKey]
WHEN MATCHED THEN
	UPDATE SET [Target].[Name] = [Source].[Name]
WHEN NOT MATCHED THEN
	INSERT ([Name], [SystemKey]) VALUES ([Source].[Name], [Source].[SystemKey]);

