# Azure login
`az login`
`az acr login --name <acrName>`
`docker tag health0 stekarabdevregistry.azurecr.io/health0:v1`
`az acr repository show-tags --name <acrName>`

# Build images

`docker build -t <tag> "file/location"`

# Push images to ACR

# Deploy k8s.yml with kubectl to AKS
