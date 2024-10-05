extension microsoftGraphV1_0

@description('The name of the function app that you wish to create.')
param functionUrlHostname string

resource authenticationEventsAPI 'Microsoft.Graph/applications@v1.0' = {
  uniqueName: 'Azure Functions authentication events API'
  displayName: 'Azure Functions authentication events API'
  signInAudience: 'AzureADMyOrg'
  api: {
    acceptMappedClaims: null
    knownClientApplications: []
    oauth2PermissionScopes: []
    preAuthorizedApplications: []
    requestedAccessTokenVersion: 2
  }
  requiredResourceAccess: [
    {
      resourceAccess: [
        {
          id: '00aa00aa-bb11-cc22-dd33-44ee44ee44ee'
          type: 'Role'
        }
      ]
      resourceAppId: '00000003-0000-0000-c000-000000000000'
    }
  ]
}

// Reference the existing Azure AD Application
resource existingAdApp 'Microsoft.Graph/applications@v1.0' existing = {
  name: appObjectId
}

// Update the Application ID URI of the existing Azure AD Application
resource updateAdApp 'Microsoft.Graph/applications@v1.0' = {
  uniqueName: authenticationEventsAPI.uniqueName
  displayName: authenticationEventsAPI.displayName
  identifierUris: [
    'api://${functionUrlHostname}/${authenticationEventsAPI.appId}'
  ]
}

resource clientSp 'Microsoft.Graph/servicePrincipals@v1.0' = {
  appId: authenticationEventsAPI.appId
}
