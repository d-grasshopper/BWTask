#!/bin/bash

terraform init

if [ -z "$(ls -A ~/.azure)" ]; then
    az login
fi

terraform workspace new ${TF_VAR_subscription}_${TF_VAR_environmentironment} || terraform workspace select ${TF_VAR_subscription}_${TF_VAR_environmentironment}

if [ -f settings/${TF_VAR_subscription}_${TF_VAR_environment}.tfvars ]; then
    VAR_FILE="settings/${TF_VAR_subscription}_${TF_VAR_environment}.tfvars"
elif [ -f settings/${TF_VAR_subscription}.tfvars ]; then
    VAR_FILE="settings/${TF_VAR_subscription}.tfvars"
fi

if [ "$1" == "" ]; then
    case "${TF_ACTION}" in

        apply)
            terraform apply -var-file=${VAR_FILE} -auto-approve
            ;;

        plan)
            terraform plan -var-file=${VAR_FILE}
            ;;

        destroy)
            terraform destroy -var-file=${VAR_FILE} -auto-approve
            ;;

    esac
else
    exec $@
fi
