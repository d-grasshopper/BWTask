# Creating infrastructure:

1. Create a .tfvars file in the `terraform/settings/` folder with your chosen subscription tag and fill it in according to the template. An Azure Subscription UUID must be provided.

2.
## Creating or updating infrastructure:

```Powershell
.\manage-env.ps1 -action apply -subscription [subscription name] -env [environment name]
```

NOTE: If this fails, it is because the AKS didn't wait before the AD Application has finished creating, meaning it can't get the Service Principal it needs for deployment. Just run the same line again in 30 secs to a minute and it should complete fine.

## Destroying infrastructure

```Powershell
.\manage-env.ps1 -action destroy -subscription [subscription name] -env [environment name]
```
