ALTER TABLE [Tenants]
ADD 
	CompanyAddress VARCHAR(255), 
	WebsiteUrl VARCHAR(255),
	BusinessRegistrationNumber VARCHAR(255),
	PrimaryContactName VARCHAR(255),
	ContactEmail VARCHAR(50),
	PhoneNumber VARCHAR(50);
GO;

ALTER TABLE [Stores]
ADD
	[Location] VARCHAR(255);