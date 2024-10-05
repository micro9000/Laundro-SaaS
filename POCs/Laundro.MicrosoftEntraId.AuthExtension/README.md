# Abandoned

This is not an easy component to maintain.

1. The Microsoft.Azure.WebJobs.Extensions.AuthenticationEvents is still in .NET 6 and can't be use in Isolated Function app
2. Creating Custom authentication extensions in Azure Entra Id/Enterprise Applications can't be automated using Bicep or Terraform
3. Adding new Custom authentication extensions and attaching it to Enterprice application is very volatile,
	it is hard to debug what is the issue