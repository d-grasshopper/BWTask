variable "location" {}
variable "rg_name" {}

variable "environment" {
    description = "Name of the environment to set-up (will result in Resource Group in Azure)"
}

variable "subscription" {
    description = "Logical name of the subscription (name of settings file)"
}
