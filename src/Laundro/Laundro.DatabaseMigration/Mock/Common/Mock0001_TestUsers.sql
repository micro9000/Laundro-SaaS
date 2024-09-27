
DECLARE @tenant_owner_id INT;
SELECT @tenant_owner_id=Id FROM Roles WHERE SystemKey = 'tenant_owner';

CREATE TABLE #TempUsers
(
	[Email] NVARCHAR(255) NOT NULL,
	[Name] NVARCHAR(255) NOT NULL,
	[RoleId] INT
)

INSERT INTO #TempUsers ([Email], [Name], [RoleId]) 
VALUES('ranielgarcia101@gmail.com', 'Raniel Garcia', @tenant_owner_id)

MERGE [Users] As [Target]
USING
	(SELECT [Email], [Name], [RoleId] FROM #TempUsers) AS [Source] ([Email], [Name], [RoleId])
	ON [Target].[Email] = [Source].[Email]
WHEN NOT MATCHED THEN
	INSERT ([Email], [Name], [RoleId]) VALUES ([Source].[Email], [Source].[Name], [Source].[RoleId])
WHEN MATCHED THEN
	UPDATE SET [Target].[RoleId]=[Source].[RoleId], [Target].[Name]=[Source].[Name];
