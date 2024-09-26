CREATE TABLE ScriptVersion
(
	Hash VARCHAR(64) NOT NULL
)

CREATE TABLE Roles
(
	Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	Name VARCHAR(255) NOT NULL,
	SystemKey VARCHAR(100) NOT NULL,
	IsActive BIT NOT NULL DEFAULT 1,
	CreatedAt DATETIME2 DEFAULT SYSDATETIME()
)

CREATE TABLE Users
(
	Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	Email VARCHAR(255) NOT NULL,
	[Name] VARCHAR(255) NOT NULL,
	RoleId INT NOT NULL,
	CreatedAt DATETIME2 DEFAULT SYSDATETIME(),
	IsActive BIT NOT NULL DEFAULT 1,
	CONSTRAINT FK_Role FOREIGN KEY (RoleId) REFERENCES Roles(Id)
)
CREATE UNIQUE  NONCLUSTERED INDEX IX_Users_Email_RoleId_IsActive
ON Users (Email, RoleId, IsActive);


CREATE TABLE Tenants
(
	Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	OwnerId INT NOT NULL,
	CreatedAt DATETIME2 DEFAULT SYSDATETIME(),
	IsActive BIT NOT NULL DEFAULT 1,
	CONSTRAINT FK_Owner FOREIGN KEY (OwnerId) REFERENCES Users(Id)
)

CREATE TABLE Stores
(
	Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	TenantId INT NOT NULL,
	Name VARCHAR(255) NOT NULL,
	ManagerId INT NOT NULL,
	CreatedAt DATETIME2 DEFAULT SYSDATETIME(),
	IsActive BIT NOT NULL DEFAULT 1,
	CONSTRAINT FK_Tenant FOREIGN KEY (TenantId) REFERENCES Tenants(Id),
	CONSTRAINT FK_Manager FOREIGN KEY (ManagerId) REFERENCES Users(Id)
)

-- A User should only be assigned to only one store, and we will going to facilitate the validation in the system
CREATE TABLE StoreStaffAssignments
(
	StaffId INT NOT NULL,
	StoreId INT NOT NULL,
	CONSTRAINT FK_Staff FOREIGN KEY (StaffId) REFERENCES Users(Id),
	CONSTRAINT FK_Store FOREIGN KEY (StoreId) REFERENCES Stores(Id)
)
