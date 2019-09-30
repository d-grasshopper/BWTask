Start-Job -ScriptBlock {
    & az aks browse --resource-group stekarab_dev --name stekarab_dev_aks
}
