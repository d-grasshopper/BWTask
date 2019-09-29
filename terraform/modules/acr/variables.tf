variable "location" {}
variable "rg_name" {}
variable "ext_service_principal_id" {}

variable "environment" {
    description = "Name of the environment to set-up (will result in Resource Group in Azure)"
}

variable "subscription" {
    description = "Logical name of the subscription (name of settings file)"
}
