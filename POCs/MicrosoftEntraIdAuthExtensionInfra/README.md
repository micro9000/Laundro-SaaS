# Introduction

This is the IaC of the Laundro.MicrosoftEntraId.AuthExtension function application.

These resources are based on this [Custom Claims Provider Azure Function](https://learn.microsoft.com/en-us/entra/identity-platform/custom-extension-tokenissuancestart-setup?tabs=visual-studio%2Cazure-portal&pivots=azure-portal)

## Main Tenant Resources

- App Service Plan
- Azure Function
- Storage Account
- Application Insights

### Manual Provisioning

1. `az login`
2. `az group create --name Laundro-Entra-Id-Extension --location southeastasia`
3. `az deployment group create --resource-group Laundro-Entra-Id-Extension --template-file main.bicep`

## External Customer Tenant Resources

- App Registration
- Service Principals

1. `az login`
2. `az group create --name Laundro-Entra-Id-Extension --location southeastasia`
3. `az deployment group create --resource-group Laundro-Entra-Id-Extension --template-file main.bicep`
