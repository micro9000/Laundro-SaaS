terraform {
  required_providers {
    azurerm = {
      source = "hashicorp/azurerm"
      version = "3.92.0"
    }
    random = {
      source = "hashicorp/random"
      version = "3.6.0"
    }
  }
  backend "azurerm" {
  }
}

provider "azurerm" {
  features {}
}

provider "random" {
  # Configuration options
}