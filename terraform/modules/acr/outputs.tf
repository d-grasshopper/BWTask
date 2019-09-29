
output "acr_name" {
  value = "${azurerm_container_registry.container_registry.name}"
}

output "acr_id" {
  value = "${azurerm_container_registry.container_registry.id}"
}
