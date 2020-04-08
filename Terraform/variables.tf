variable location {
    default="centralus"
}
variable prefix {
    default="servicebusdemo"
}

resource "random_id" "servicebusname" {
  keepers = {
    resource_group = azurerm_resource_group.servicebus.name
  }
  byte_length = 8
}

locals {
    rg_name = "${var.prefix}-rg"
    keyvault_name = "${var.prefix}-kv"
    queueName="demo"
}

data "azurerm_client_config" "current" {
}
