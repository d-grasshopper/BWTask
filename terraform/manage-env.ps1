param (
    [Parameter(Mandatory = $true)][string]$subscription,
    [Parameter(Mandatory = $true)][string]$env,
    [Parameter(Mandatory = $true)][string]$action
)

docker volume create ${subscription}_terraform
docker volume create ${subscription}_azure_credentials
docker build -t terraform .
docker run -it -e TF_VAR_subscription=$subscription -e TF_VAR_environment=$env -e TF_ACTION=$action --rm -v ${subscription}_terraform:/home/terraform/terraform.tfstate.d/ -v ${subscription}_azure_credentials:/root/.azure/ terraform
