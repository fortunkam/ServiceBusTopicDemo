resource "azurerm_resource_group" "servicebus" {
    name     = local.rg_name
    location = var.location
}