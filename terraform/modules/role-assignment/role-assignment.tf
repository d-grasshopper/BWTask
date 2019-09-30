resource "azurerm_role_assignment" "role_assignment" {

  principal_id         = "${var.service_principal_id}"
  role_definition_name = "${var.role}"
  scope                = "${var.scope_id}"
}
