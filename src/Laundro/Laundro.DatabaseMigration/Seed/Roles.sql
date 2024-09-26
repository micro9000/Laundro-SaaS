CREATE TABLE #TempRoles
(
	[Name] NVARCHAR(255) NOT NULL,
	[SystemKey] NVARCHAR(100) NOT NULL,
)

INSERT INTO #TempRoles ([Name], [SystemKey]) 
VALUES('Store Owner Admin', 'store_owner_admin'),
('Store Admin Assistant', 'store_admin_assistant'),
('Store Staff', 'store_staff'),
('New User', 'new_user')

MERGE [Roles] As [Target]
USING
	(SELECT [Name], [SystemKey] FROM #TempRoles) AS [Source]
	ON [Target].[SystemKey] = [Source].[SystemKey]
WHEN MATCHED THEN
	UPDATE SET [Target].[Name] = [Source].[Name]
WHEN NOT MATCHED THEN
	INSERT ([Name], [SystemKey]) VALUES ([Source].[Name], [Source].[SystemKey]);

