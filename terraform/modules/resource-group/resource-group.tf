resource "azurerm_resource_group" "resource_group" {
    name = "${var.subscription}_${var.environment}"
    location = "${var.location}"
}
