param (
    [Parameter(Mandatory = $true)][string]$subscription,
    [Parameter(Mandatory = $true)][string]$env,
    [Parameter(Mandatory = $true)][string]$acrName
)

az login
az acr login --name $acrName

docker build -t azcli .
docker run -it -v ${subscription}_azure_credentials:/root/.azure/ azcli
