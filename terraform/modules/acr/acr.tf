# Give Contributor role of ACR scope for AKS's Service principal.
# Without this kubectl deployment fails since AKS's Service principal do not have right to read ACR.
module "acr-aks-image-pull-role-assignment" {
  source               = "../role-assignment"
  role                 = "Contributor"
  scope_id             = "${azurerm_container_registry.container_registry.id}"
  service_principal_id = "${var.ext_service_principal_id}"
}

resource "azurerm_container_registry" "container_registry" {
  name                = "${var.subscription}${var.environment}registry"
  resource_group_name = "${var.rg_name}"
  location            = "${var.location}"
  sku                 = "Standard"
}
