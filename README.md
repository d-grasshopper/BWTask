# HealthPinger service

## Guide

Initial idea was to create 5 services (4 that should be health checked, as per the task, 1 that checks them and persists data in a store).
Creating the additional 4 services wouldn't be that hard, so that's why I decided to also include them in the project (as we are not explicitly said to create them). I was toying with the idea to have an Azure function doing the health-pinging and data storing, but difficulties with deploying to Azure Kubernetes Cluster and exposing services externally in the beginning forced me to leave that idea. What we have now is a simple ASP.NET Core Web Api that exposes an endpoint to start pinging services. Thinking about data storing, I though I could hit 2 birds with 1 stone by using Elastic to store historic data of health checks, so that filtering or viewing data in graphs is possible in an easy manner.

Deployment-wise, terraform was used to create infrastructure and the Azure CLI + Helm + Docker + Kubernetes CLI to deploy containers.
Terraform creates a resource group for our infra, an Azure Container Registry and an Azure Kubernetes Cluster which is authenticated against the ACR with a role assignment. It will also keep your azure credentials in a Docker volume, so you don't have to authenticate every time (you will have to authenticate the first time you run the terraform script). After that, we need to build our Docker images, tag them for our ACR, push them to the ACR, then create the deployments/services in our AKS. All of that has been automated and you should only need to run the mentioned scripts below.

NOTES:
As of this moment, I didn't have the time to completely create the health-checked services as of the spec (each should return different data), so they are currently all returning perfect status as long as they are live. Should time be extended or if you want to see more, I can expand on that. As of this moment, the HealthPinger API is NOT connected to the deployed ELK stack, but if time permits, I can expand on that as well. Code can be a lot better in some cases, mainly the environment handling in the HealthPinger API.


## Running locally

Docker for Desktop needs to be installed.

1. Go into docker-deployment folder: `cd ./scripts/docker-deployment`

2. Bring up services: `docker-compose up -d --build --remove-orphans`

To shut down:

`docker-compose down --rmi all`

## Deploying on Azure

Azure CLI, Helm, Docker and the Kubernetes CLI (kubectl) all need to be installed on a Windows machine.

1. Create a .tfvars file in the `terraform/settings/` folder with your chosen subscription tag and fill it in according to the template. An Azure Subscription UUID must be provided.

For example, I used my name for a subscription tag `stekarab` and

2. Go into terraform folder: `cd ./terraform/`

3. Run `.\manage-env.ps1 -subscription <your_name_or_tag_by_choice> -env <env_tag> -action apply`

Example: `.\manage-env.ps1 -subscription stekarab -env dev -action apply`
I used my name as my subscription tag. Subscription tag is to separate different deployments under the same Azure Subscription. So you can create infra, and someone else can do it as well.

This will create all the needed resources on Azure - Azure Kubernetes Cluster, Azure Container Registry.
It will also automatically authenticate our AKS to be able to pull docker images from our ACR.
Process might take about 10 minutes.

NOTE: If this fails, it is because the AKS didn't wait before the AD Application has finished creating, meaning it can't get the Service Principal it needs for deployment. Just run the same line again in 30 secs to a minute and it should complete fine.

2.1. You can also view changes to be made with `-action plan`

4. Go into docker-deployment folder: `cd ../scripts/docker-deployment/`

5. Run `.\aks-deploy.ps1 -subscription <same_name_or_tag_as_before> -env <same_env_tag_as_before>`

Example: `.\aks-deploy.ps1 -subscription stekarab -env dev -action apply`
Use the same tags as when running the `.\manage-env.ps1` script.

This will build all docker images, tag them, push them to our ACR, then create the deployments and services in our AKS.
It will also deploy an ELK stack image (Elasticsearch cluster, Logstash, Kibana). Elastic is what is used to store the status of the services and Kibana to view data in graphs or to filter data. As of this moment, the connection between our HealthPinger api and Elastic is NOT COMPLETE.

5. After successful deployment, you should be able to connect to our HealthPinger API, as it uses a LoadBalancer service and exposes an external IP.

To get the IP, run `kubectl get svc` and find the health-pinger-service LoadBalancer IP.

You can then navigate to `<external_ip>:5000/swagger` to visit the swagger page which should show a single endpoint that can start pinging the other services' /health endpoints.

If by any chance, you are having trouble connecting in the above way, you can also create a direct tunnel to the pod, running the HealthPinger with the following commands:

`.\connect-healthpinger.ps1`

which will create the tunnel on port 5015, so navigate to `localhost:5015`

OR

`$HEALTHPINGER_POD = $(kubectl get pods --namespace default -l "application=healthpinger" -o jsonpath="{.items[0].metadata.name}")`

`kubectl port-forward --namespace default $HEALTHPINGER_POD <host_port_of_choice>:5000`

(the `.\connect-healthpinger.ps1` script above also executes this, but in the background, sometimes there are issues with it, so that's why I provide you the commands as well)

6. You can also connect to Kibana:

`.\connect-kibana.ps1`

which will create the tunnel on port 5025, so navigate to `localhost:5025`

OR

`$KIBANA_POD = $(kubectl get pods --namespace default -l "app=kibana" -o jsonpath="{.items[0].metadata.name}")`

`kubectl port-forward --namespace default $KIBANA_POD <host_port_of_choice>:5000`

7. At any point, if you need to connect to the Kubernetes Cluster:

`.\connect-k8s.ps1`

OR

`az aks browse --resource-group stekarab_dev --name stekarab_dev_aks`

which should open a new tab automatically for you with the front-end of the AKS
