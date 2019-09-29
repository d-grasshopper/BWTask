FROM microsoft/azure-cli:latest

WORKDIR /tmp

ENV TERRAFORM_VERSION 0.12.9
COPY entrypoint.sh /home/terraform/entrypoint.sh

RUN wget https://releases.hashicorp.com/terraform/${TERRAFORM_VERSION}/terraform_${TERRAFORM_VERSION}_linux_amd64.zip -O terraform_${TERRAFORM_VERSION}_linux_amd64.zip \
    && unzip terraform_${TERRAFORM_VERSION}_linux_amd64.zip -d /usr/bin \
    && chmod ugo=rx /usr/bin/terraform \
    && dos2unix /home/terraform/entrypoint.sh

WORKDIR /home/terraform

RUN chmod ugo=rx /home/terraform/entrypoint.sh

ENTRYPOINT [ "./entrypoint.sh" ]

VOLUME [ "home/terraform/.terraform", "/root/.azure" ]

COPY envs /home/terraform
COPY modules /home/terraform/modules
COPY settings /home/terraform/settings

