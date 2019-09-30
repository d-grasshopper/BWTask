$KIBANA_POD = $(kubectl get pods --namespace default -l "app=kibana" -o jsonpath="{.items[0].metadata.name}")
Start-Job -ScriptBlock {
    & kubectl port-forward --namespace default $KIBANA_POD 5025:5000
}
