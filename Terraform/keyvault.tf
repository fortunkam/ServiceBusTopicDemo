
resource "azurerm_key_vault" "keyvault" {
  name                        = local.keyvault_name
  location                    = azurerm_resource_group.servicebus.location
  resource_group_name         = azurerm_resource_group.servicebus.name
  tenant_id                   = data.azurerm_client_config.current.tenant_id

  sku_name = "standard"

  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    key_permissions = [
      "get",
    ]

    secret_permissions = [
      "get",
      "set",
      "list",
      "delete"
    ]

    storage_permissions = [
      "get",
    ]
  }

}

resource "azurerm_key_vault_secret" "master_service_bus_connection_string" {
  name         = "MasterServiceBusConnectionString"
  value        = azurerm_servicebus_namespace.servicebus.default_primary_connection_string   
  key_vault_id = azurerm_key_vault.keyvault.id
}

resource "azurerm_key_vault_secret" "sender_service_bus_connection_string" {
  name         = "SenderServiceBusConnectionString"
  value        = azurerm_servicebus_namespace_authorization_rule.sender.primary_connection_string  
  key_vault_id = azurerm_key_vault.keyvault.id
}

resource "azurerm_key_vault_secret" "receiver_service_bus_connection_string" {
  name         = "ReceiverServiceBusConnectionString"
  value        = azurerm_servicebus_namespace_authorization_rule.receiver.primary_connection_string  
  key_vault_id = azurerm_key_vault.keyvault.id
}

resource "azurerm_key_vault_secret" "queueName" {
  name         = "QueueName"
  value        = local.queueName
  key_vault_id = azurerm_key_vault.keyvault.id
}