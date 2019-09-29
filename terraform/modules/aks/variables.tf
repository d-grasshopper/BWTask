variable "location" {}
variable "rg_name" {}
variable "dns_prefix" {}
variable "agent_count" {}
variable "agent_pool_name" {}
variable "vm_size" {}
variable "os_disk_size_gb" {}

variable "environment" {
    description = "Name of the environment to set-up (will result in Resource Group in Azure)"
}

variable "subscription" {
    description = "Logical name of the subscription (name of settings file)"
}
