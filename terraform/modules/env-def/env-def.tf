# NOTE: This is the environment definition that will be used by all environments.
# The actual environments (like dev) just inject their environment dependent values to env-def which defines the actual environment and creates that environment using given values.


# Main resource group for the demonstration.
module "main-resource-group" {
  source                    = "../resource-group"
  subscription              = "${var.subscription}"
  environment               = "${var.environment}"
  location                  = "${var.location}"
}

# AKS configuration.
module "aks" {
  source          = "../aks"
  environment     = "${var.environment}"
  subscription    = "${var.subscription}"
  location        = "${var.location}"
  rg_name         = "${module.main-resource-group.resource_group_name}"
  dns_prefix      = "bwtest"
  agent_count     = "2"
  agent_pool_name = "akspool"
  vm_size         = "Standard_D2s_v3"
  os_disk_size_gb = "30"
}

# ACR registry configuration.
module "acr" {
  source          = "../acr"
  environment     = "${var.environment}"
  subscription    = "${var.subscription}"
  location        = "${var.location}"
  rg_name         = "${module.main-resource-group.resource_group_name}"
  ext_service_principal_id = "${module.aks.aks_service_principal_id}"
}

# Public ips.
# IP for Singe-node version of Simple Server.
module "single-node-pip" {
  source          = "../public-ip"
  subscription    = "${var.subscription}"
  environment     = "${var.environment}"
  location        = "${var.location}"
  rg_name         = "${module.aks.aks_resource_group_name}"
}

module "app-storage-account" {
  source          = "../storage-account"
  environment     = "${var.environment}"
  subscription    = "${var.subscription}"
  location        = "${var.location}"
  rg_name         = "${module.main-resource-group.resource_group_name}"
}
