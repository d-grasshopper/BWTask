##### Terraform configuration #####

provider "azurerm" {
    version = "=1.27.0"
    subscription_id = "${var.subscription_id}"
}

# Here we inject our values to the environment definition module which creates all actual resources.
module "env-def" {
  source       = "./modules/env-def"
  environment  = "${var.environment}"
  subscription = "${var.subscription}"
  location     = "UK West"
}
