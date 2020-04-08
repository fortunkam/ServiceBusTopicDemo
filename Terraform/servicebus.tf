resource "azurerm_servicebus_namespace" "servicebus" {
  name                = "tfsta${lower(random_id.servicebusname.hex)}"
  location            = azurerm_resource_group.servicebus.location
  resource_group_name = azurerm_resource_group.servicebus.name
  sku                 = "Standard"
}

resource "azurerm_servicebus_topic" "demo" {
  name                = local.queueName
  resource_group_name = azurerm_resource_group.servicebus.name
  namespace_name      = azurerm_servicebus_namespace.servicebus.name

  enable_partitioning = true
  default_message_ttl = "PT30S"
}

resource "azurerm_servicebus_namespace_authorization_rule" "sender" {
  name                = "sender"
  namespace_name      = azurerm_servicebus_namespace.servicebus.name
  resource_group_name = azurerm_resource_group.servicebus.name

  listen = true
  send   = true
  manage = true
}

resource "azurerm_servicebus_namespace_authorization_rule" "receiver" {
  name                = "receiver"
  namespace_name      = azurerm_servicebus_namespace.servicebus.name
  resource_group_name = azurerm_resource_group.servicebus.name

  listen = true
  send   = true
  manage = true
}