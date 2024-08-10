data "azuread_client_config" "laundro_web_client_config" {}

# Registration guide https://learn.microsoft.com/en-us/entra/external-id/customers/how-to-register-ciam-app?tabs=spa
resource "azuread_application" "laundro_web_client" {
  display_name     = "example"
  owners           = [data.azuread_client_config.laundro_web_client_config.object_id]
  sign_in_audience = "AzureADMyOrg"

  required_resource_access {
    resource_app_id = "00000003-0000-0000-c000-000000000000" # Microsoft Graph

    resource_access {
      id   = "df021288-bdef-4463-88db-98f22de89214" # User.Read.All
      type = "Role"
    }

    resource_access {
      id   = "b4e74841-8e56-480b-be8b-910348b18b4c" # User.ReadWrite
      type = "Scope"
    }
  }

  required_resource_access {
    resource_app_id = azuread_application.laundro_api_client.id

    resource_access {
      id   = azuread_application_permission_scope.laundro_api_client_user_impersonation.id
      type = "Role"
    }
  }

  web {
    redirect_uris = ["http://localhost:3000/"]

    implicit_grant {
      access_token_issuance_enabled = true
      id_token_issuance_enabled     = true
    }
  }
}