locals {
    sql_username = "sql${var.environment}admin"
    sql_password = random_password.sql_db_password
}

resource "azurerm_key_vault_secret" "main_mssql_server_username" {
  name         = "sql-username"
  value        = local.sql_username
  key_vault_id = azurerm_key_vault.main_keyvault.id
}

resource "azurerm_key_vault_secret" "main_mssql_server_password" {
  name         = "sql-password"
  value        = local.sql_password
  key_vault_id = azurerm_key_vault.main_keyvault.id
}

resource "azurerm_mssql_server" "main_mssql_server" {
  name                         = "${local.global_resource_prefix}sqlsvr"
  resource_group_name          = azurerm_resource_group.main_rg.name
  location                     = azurerm_resource_group.main_rg.location
  version                      = "12.0"
  administrator_login          = local.sql_username
  administrator_login_password = local.sql_password
}

resource "azurerm_mssql_database" "main_mssql_database" {
  name         = "${local.global_resource_prefix}sqldb"
  server_id    = azurerm_mssql_server.main_mssql_server.id
  collation    = "SQL_Latin1_General_CP1_CI_AS"
  license_type = "LicenseIncluded"
  max_size_gb  = 5
  sku_name     = "GP_S_Gen5_2"
  auto_pause_delay_in_minutes = 30
  enclave_type = "VBS"

  # prevent the possibility of accidental data loss
  lifecycle {
    prevent_destroy = true
  }
}



