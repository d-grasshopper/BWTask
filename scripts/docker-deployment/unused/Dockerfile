FROM microsoft/azure-cli:latest

WORKDIR /tmp

COPY entrypoint.sh /home/entrypoint.sh

ENV KUBE_LATEST_VERSION="v1.13.10"
ENV HELM_LATEST_VERSION="2.14.0"

RUN apk add --update ca-certificates \
    && apk add --update -t deps curl \
    && curl -L https://storage.googleapis.com/kubernetes-release/release/${KUBE_LATEST_VERSION}/bin/linux/amd64/kubectl -o /usr/local/bin/kubectl \
    && chmod +x /usr/local/bin/kubectl \
    && apk del --purge deps \
    && rm /var/cache/apk/* \
    && dos2unix /home/entrypoint.sh

WORKDIR /home

RUN chmod ugo=rx /home/entrypoint.sh

ENTRYPOINT [ "./entrypoint.sh" ]

VOLUME [ "/root/.azure" ]
