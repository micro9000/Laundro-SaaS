CREATE TABLE #TempUsers
(
	[Email] NVARCHAR(255) NOT NULL,
)

INSERT INTO #TempUsers ([Email]) 
VALUES('ranielgarcia101@gmail.com')

MERGE [Users] As [Target]
USING
	(SELECT [Email] FROM #TempUsers) AS [Source]
	ON [Target].[Email] = [Source].[Email]
WHEN NOT MATCHED THEN
	INSERT ([Email]) VALUES ([Source].[Email]);