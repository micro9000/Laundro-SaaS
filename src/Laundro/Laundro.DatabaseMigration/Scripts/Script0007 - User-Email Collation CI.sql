ALTER TABLE [Users]
ALTER COLUMN Email VARCHAR(255) COLLATE SQL_Latin1_General_CP1_CI_AS -- Explicitly specifying email as case-insensitive

GO;