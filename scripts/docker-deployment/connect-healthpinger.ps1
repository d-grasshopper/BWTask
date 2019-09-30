$HEALTHPINGER_POD = $(kubectl get pods --namespace default -l "application=healthpinger" -o jsonpath="{.items[0].metadata.name}")
Start-Job -ScriptBlock {
    & kubectl port-forward --namespace default $HEALTHPINGER_POD 5015:5000
}
