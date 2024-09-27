
resource "azurerm_service_plan" "function_app" {
  name                = "${local.global_resource_prefix}sp_func"
  resource_group_name = azurerm_resource_group.main_rg.name
  location            = azurerm_resource_group.main_rg.location
  os_type             = "Linux"
  sku_name            = "B1"
}

resource "azurerm_linux_function_app" "entra_id_custom_extension_func" {
  name                = "${local.global_resource_prefix}entraidfunc"
  resource_group_name = azurerm_resource_group.main_rg.name
  location            = azurerm_resource_group.main_rg.location

  storage_account_name       = azurerm_storage_account.func_storage_account.name
  storage_account_access_key = azurerm_storage_account.func_storage_account.primary_access_key
  service_plan_id            = azurerm_service_plan.function_app.id

  site_config {}
}