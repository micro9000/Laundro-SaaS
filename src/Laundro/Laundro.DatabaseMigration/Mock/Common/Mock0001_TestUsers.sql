
DECLARE @store_owner_admin_id INT;
DECLARE @store_admin_assistant INT;
DECLARE @store_staff INT;

SELECT @store_owner_admin_id=Id FROM Roles WHERE SystemKey = 'store_owner_admin';
SELECT @store_admin_assistant=Id FROM Roles WHERE SystemKey = 'store_admin_assistant';
SELECT @store_staff=Id FROM Roles WHERE SystemKey = 'store_staff';

CREATE TABLE #TempUsers
(
	[Email] NVARCHAR(255) NOT NULL,
	[Name] NVARCHAR(255) NOT NULL,
	[RoleId] INT
)

INSERT INTO #TempUsers ([Email], [Name], [RoleId]) 
VALUES('ranielgarcia101@gmail.com', 'Raniel Garcia', @store_owner_admin_id)

MERGE [Users] As [Target]
USING
	(SELECT [Email], [Name], [RoleId] FROM #TempUsers) AS [Source] ([Email], [Name], [RoleId])
	ON [Target].[Email] = [Source].[Email]
WHEN NOT MATCHED THEN
	INSERT ([Email], [Name], [RoleId]) VALUES ([Source].[Email], [Source].[Name], [Source].[RoleId])
WHEN MATCHED THEN
	UPDATE SET [Target].[RoleId]=[Source].[RoleId], [Target].[Name]=[Source].[Name];
