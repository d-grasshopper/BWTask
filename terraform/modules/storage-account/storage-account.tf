resource "azurerm_storage_account" "storage_account" {
    name = "sa${var.subscription}${lower(var.environment)}"
    resource_group_name = "${var.rg_name}"
    location = "${var.location}"
    account_tier = "Standard"
    account_replication_type = "LRS"
    enable_https_traffic_only = false
    account_kind = "StorageV2"
}
