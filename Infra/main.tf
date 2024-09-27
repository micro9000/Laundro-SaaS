
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

locals {
  global_resource_prefix = lower("${var.system_short_name}${random_string.random.id}")
}

resource "azurerm_resource_group" "main_rg" {
  name     = "${local.global_resource_prefix}-MAIN-RG"
  location = var.resource_location
}

resource "random_password" "sql_db_password" {
  length           = 16
  special          = true
  override_special = "!#$%&*()-_=+[]{}<>:?"
}