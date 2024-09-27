
resource "azurerm_storage_account" "func_storage_account" {
  name                     = "${local.global_resource_prefix}funcsa"
  resource_group_name      = azurerm_resource_group.main_rg.name
  location                 = azurerm_resource_group.main_rg.location
  account_tier             = var.storage_account_tier
  account_replication_type = var.storage_account_replication_type
}