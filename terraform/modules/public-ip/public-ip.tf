resource "azurerm_public_ip" "public_ip" {
  name                         = "${var.subscription}_${var.environment}"
  location                     = "${var.location}"
  resource_group_name          = "${var.rg_name}"
  public_ip_address_allocation = "static"
}
