resource "azurerm_resource_group" "laundro_rg" {
  name     = "AzureDO-Agent-VM-RG"
  location = "East Asia"
}

resource "random_pet" "prefix" {
  prefix = "agent"
  length = 1
}

resource "random_string" "random" {
  length  = 8
  special = false
  lower   = true
  upper   = false
  numeric = true
}

resource "random_string" "short" {
  length  = 4
  special = false
  lower   = true
  upper   = false
  numeric = true
}

