data "azuread_client_config" "laundro_api_client_config" {}

# These resources should follow this guide: https://learn.microsoft.com/en-us/entra/external-id/customers/how-to-register-ciam-app?tabs=webapi
resource "azuread_application" "laundro_api_client" {
  display_name     = "laundro-api-${random_string.short.id}"
  identifier_uris  = ["api://laundro-api-app-registration"]
  owners           = [data.azuread_client_config.laundro_api_client_config.object_id]
  sign_in_audience = "AzureADMyOrg"

  api {
    mapped_claims_enabled          = true
    requested_access_token_version = 2
  }

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
}
resource "random_uuid" "permission_scope_user_impersonation" {
    keepers = {
      "key" = "user_impersonation"
    }
}
resource "azuread_application_permission_scope" "laundro_api_client_user_impersonation" {
  application_id = azuread_application.laundro_api_client.id
  admin_consent_description  = "Allow the application to access laundro on behalf of the signed-in user."
  admin_consent_display_name = "Access laundro"
  scope_id                   = random_uuid.permission_scope_user_impersonation.result
  type                       = "User"
  user_consent_description   = "Allow the application to access laundro on your behalf."
  user_consent_display_name  = "Access laundro"
  value                      = "user_impersonation"
}

resource "random_uuid" "app_role_all_read_write" {
    keepers = {
      "key" = "All.ReadWrite"
    }
}
resource "azuread_application_app_role" "example_administer" {
  application_id = azuread_application.laundro_api_client.id
  role_id        = random_uuid.app_role_all_read_write.result
  allowed_member_types = ["User", "Application"]
  description          = "Admins can manage roles and perform all task actions"
  display_name         = "Admin"
  value                = "All.ReadWrite"
}