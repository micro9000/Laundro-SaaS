variable "environment" {
  type = string
  default = "dev"
}

variable "system_short_name" {
  type = string
  default = "LRND"
}

variable "resource_location" {
  type = string
  default = "Southeast Asia"
}

variable "storage_account_tier" {
  type = string
  default = "Standard"
}

variable "storage_account_replication_type" {
  type = string
  default = "LRS"
}