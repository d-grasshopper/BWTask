#!/bin/bash

az aks get-credentials -g stekarab_dev -n stekarab_dev_aks

kubectl get nodes
