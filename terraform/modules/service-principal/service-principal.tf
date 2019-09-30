resource "azurerm_azuread_application" "service_principal_app" {
    name = "${var.subscription}_${var.environment}_app"
}

resource "azurerm_azuread_service_principal" "service_principal" {
    application_id = "${azurerm_azuread_application.service_principal_app.application_id}"
}

resource "random_string" "service_principal_random_password" {
  length  = 16
  special = true

  keepers = {
    service_principal = "${azurerm_azuread_service_principal.service_principal.id}"
  }
}

resource "azurerm_azuread_service_principal_password" "service_principal_password" {
    service_principal_id = "${azurerm_azuread_service_principal.service_principal.id}"
    value                = "${random_string.service_principal_random_password.result}"

    end_date             = "${timeadd(timestamp(), "720h")}"
    lifecycle {
        ignore_changes = ["end_date"]
    }
    provisioner "local-exec" {
        command = "sleep 30"
    }

}
