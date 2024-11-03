CREATE TABLE #TempRoles
(
	[Name] NVARCHAR(255) NOT NULL,
	[SystemKey] NVARCHAR(100) NOT NULL,
	[RoleLevel] NVARCHAR(20) NOT NULL
)

INSERT INTO #TempRoles ([Name], [SystemKey], [RoleLevel]) 
VALUES
('New User', 'new_user', 'tenant'),
('Tenant Owner', 'tenant_owner', 'tenant'),
('Tenant Employee', 'tenant_employee', 'tenant'),
('Store Manager', 'store_manager', 'store'),
('Store Staff', 'store_staff', 'store');

MERGE [Roles] As [Target]
USING
	(SELECT [Name], [SystemKey], [RoleLevel] FROM #TempRoles) AS [Source]
	ON [Target].[SystemKey] = [Source].[SystemKey]
WHEN MATCHED THEN
	UPDATE SET [Target].[Name] = [Source].[Name]
WHEN NOT MATCHED THEN
	INSERT ([Name], [SystemKey], [RoleLevel]) VALUES ([Source].[Name], [Source].[SystemKey], [Source].[RoleLevel]);

