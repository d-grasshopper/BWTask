param (
    [Parameter(Mandatory = $true)][string]$subscription,
    [Parameter(Mandatory = $true)][string]$env
)

az login
az acr login --name ${subscription}${env}registry

az aks get-credentials -g ${subscription}_${env} -n ${subscription}_${env}_aks

helm init --history-max 200

$projects = "HealthPinger", "Health0", "Health1", "Health2", "Health3"
$services = "health-pinger", "healthzero", "healthone", "healthtwo", "healththree"
foreach ($project in $projects) {
    docker build -t $project.ToLower() ../../${project}/
}

for ($i = 0; $i -lt $services.Count; $i++) {
    docker tag $($projects[$i]).ToLower() "${subscription}${env}registry.azurecr.io/$($services[$i]):v1"
    docker push "${subscription}${env}registry.azurecr.io/$($services[$i]):v1"
    kubectl apply -f ./$($projects[$i]).yaml
}

helm install --name elk stable/elastic-stack
