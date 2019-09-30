# Azure login

## Account login
`az login`

## Azure container repository login
`az acr login --name <acrName>`

# Install ingress chart (NOT USED)

```
helm install stable/nginx-ingress \
    --namespace ingress-basic \
    --set controller.replicaCount=2 \
    --set controller.nodeSelector."beta\.kubernetes\.io/os"=linux \
    --set defaultBackend.nodeSelector."beta\.kubernetes\.io/os"=linux
```

# Install ELK Stack

`helm install --name elk stable/elastic-stack`

# Build images

`docker build -t <tag> "file/location"`

## Tag images

`docker tag <tag> stekarabdevregistry.azurecr.io/<tag>:v1`

## Push images to ACR

`az acr login --name <environment><env>registry`

`docker push <environment><env>.azurecr.io/health0:v1`

# Deploy k8s.yml with kubectl to AKS

## Login to AKS
`az aks get-credentials -g ${subscription}_${env} -n ${subscription}_${env}_aks`

## Create deployments/services

`kubectl apply -f <file>.yaml`

# Connect to Kubernetes cluster

`az aks browse --resource-group stekarab_dev --name stekarab_dev_aks`

or

`.\connect-k8s.ps1`

# Connect to Kibana
`$KIBANA_POD=$(kubectl get pods --namespace default -l "app=kibana,release=elk" -o jsonpath="{.items[0].metadata.name}")`

`kubectl port-forward --namespace default $POD_NAME 5601:5601`

OR

run `.\connect-kibana.ps1` and you can connect on `localhost:5025`

# Connect to Health Pinger
`$HEALTHPINGER_POD = $(kubectl get pods --namespace default -l "application=healthpinger" -o jsonpath="{.items[0].metadata.name}")`

`kubectl port-forward --namespace default $HEALTHPINGER_POD <host_port>:5000`

OR

run `.\connect-healthpinger.ps1` and you can connect on `localhost:5015`
