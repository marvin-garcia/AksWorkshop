# AKS Workshop
This is a walkthrough to create an Azure Kubernetes Service cluster in a production-like situation. It focuses in the following cases:

* Set up a virtual network for multiple workloads
* Set up a service principal to be used by the AKS cluster
* Use Helm to install pre-built packages and custom releases
* Connect the AKS cluster with a private Azure Container Registry
* Configure an ingress controller to an internal virtual netwok IP
* Expose the application to the Internet using an Azure App Gateway

## Pre-requisites

> [!NOTE]
    > You can choose not to install any CLI on your computer and either create an [Azure data Science VM](https://docs.microsoft.com/en-us/azure/machine-learning/data-science-virtual-machine/provision-vm), or use the [Azure Cloud Shell](https://shell.azure.com).

* Docker [download](https://docs.docker.com/docker-for-windows/install/)
* MongoDB Compass Community [download](https://docs.mongodb.com/compass/master/install/)
* Get started with Docker for Windows [here](https://docs.docker.com/docker-for-windows/)
* Install VS Code [here](https://code.visualstudio.com/download)
* Install Azure CLI [here](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest)
* Install Azure AKS CLI [here](https://docs.microsoft.com/en-us/cli/azure/aks?view=azure-cli-latest#az-aks-install-cli)
* Install Helm [here](https://helm.sh/docs/using_helm/#installing-helm)

* Clone this repo, ```git clone https://github.com/marvin-garcia/AksWorkshop.git```

## Create virtual network

In real life situations there will already be a defined network space to allocate the AKS cluster. For this exercise we are going to create an address space like the following:

[images/virtual-network.png]

> [!NOTE] 
    > This exercise does not consider the Network Security Groups between the subnets, but they should be considered in a production environment.

1. Before you can start creating resources, you must log in to your Azure account:

  ```powershell
  az login
  az account set --subscription <your subscription name>

  #create resource group
  az group create --name <your rg name> --location eastus
  ```

2. Run the script to create the virtual network [create-vnet.ps1](Scripts/create-vnet.ps1).

## Create AKS cluster

1. First you need to create a service principal to manage the cluster's resources. Run the script [create-service-principal.ps1](Scripts/create-service-principal.ps1). You can read more about creating AKS cluster with existing service principals [here](https://docs.microsoft.com/en-us/azure/aks/kubernetes-service-principal).

> [!NOTE] 
    > Make sure to write down the App Id and secret somewhere safe during the remaining of this exercise, since we will need it in a minute.

2. Now that you have created the service principal, you need to grant the service principal Contributor access to the virtual network. This is so the service principal can create new nodes in the virtual network. Run the script [grant-sp-vnet-access.ps1](Scripts/grant-sp-vnet-access.ps1).

3. Run the script [create-aks.ps1](Scripts/create-aks.ps1) to create the AKS cluster You can read more about the command az aks create [here](https://docs.microsoft.com/en-us/cli/azure/aks?view=azure-cli-latest#az-aks-create).

4. Once the cluster has been created, Download the credentials to your cluster by running the script [get-aks-credentials.ps1](Scripts/get-aks-credentials.ps1).

5. For this project you will need an instance of MongoDB in the cluster. The recommended way of doing so is using Helm. Helm is a Kubernetes package manager and it has a MongoDB chart that is replicated and horizontally scalable. Because the cluster was created with RBAC enabled, you have to create the appropriate ServiceAccount for Tiller (the server side Helm component) to use. Run the script [helm-init.ps1](Scripts/helm-init.ps1).

6. Deploy a highly available instance of MongoDB in the cluster. Run the script [helm-install-mongo.ps1](Scripts/helm-install-mongo.ps1). Take note of the MongoDB service FQDN, you will need it later in the lab. Follow the instructions in the output to connect to the database from outside the cluster, then use MongoDB Compass Community to create a database and a collection.

7. Create a Container Registry to own the Docker images. Run the script [create-acr.ps1](Scripts/create-acr.ps1). You will need to ensure the cluster can pull images from your registry, there are two ways of accomplishing this task: Create a secret object in the cluster with the container registry key, or grant the service principal pull access to the registry. In this case we will do the latter, run the script [grant-aks-acr-access.ps1](Scripts/grant-aks-acr-access.ps1).

8. Log in to the contrainer registry, go to the Azure portal and find the login server, username and one of the keys. Then run the command ```docker login <login-server> -u <username> -p <password>```.

9. Pull the Backend API docker image and push it to your registry:
    ```
    docker pull aksworkshopregistry.azurecr.io/aksworkshop/backend-api
    docker tag aksworkshopregistry.azurecr.io/aksworkshop/backend-api <you-login-server>.azurecr.io/aksworkshop/backend-api
    docker push <you-login-server>.azurecr.io/aksworkshop/backend-api
    ```

10. Create a deployment and service YAML files for the backend API. It should comply with the following requirements:
    * At least 2 replicas
    * Liveness and readiness probes pointing to the swagger page (/swagger/index.html)
    * Resources requests and limits
    * The following environment variables:
        * mongodb: connection string to the MongoDB
        * mongodatabase: database name
        * mongocollection: collection name
        * ApplicationInsights__InstrumentationKey: instrumentation key for an Application Insights resource
    * Must not be exposed to the Internet.

11. Create the deployment and service in the cluster. You may use the command ```kubectl apply```, Helm charts or Azure DevOps to manage the release. If you have time constraints, just use the [templates](MultitierApi/BackendApi/Helm/) provided in this lab. It is recommended to use Azure DevOps since releases will be automated that way. You can use the script [helm-create-release.ps1](Scripts/helm-create-release.ps1) to push releases manually to the cluster.

12. Use the command ```kubectl port-forward``` to test the Backend API using the Swagger page.
    ```
    kubectl port-forward --namespace default svc/<backend-service-name> 8080:80;
    ```

13. Pull the Frontend API docker image and push it to your registry:
    ```
    docker pull aksworkshopregistry.azurecr.io/aksworkshop/frontend-api
    docker tag aksworkshopregistry.azurecr.io/aksworkshop/frontend-api <you-login-server>.azurecr.io/aksworkshop/frontend-api
    docker push <you-login-server>.azurecr.io/aksworkshop/frontend-api
    ```

14. Install an internal instance of nginx in the cluster using Helm. Run the script [helm-install-nginx-internal.ps1](Scripts/helm-install-nginx-internal.ps1).

14. Create a deployment, service and ingress controller YAML files for the frontend API. It should comply with the following requirements:
    * At least 2 replicas
    * Liveness and readiness probes pointing to the swagger page (/swagger/index.html)
    * Resources requests and limits
    * The following environment variables:
        * backendurl: FQDN for the backend service
        * ApplicationInsights__InstrumentationKey: instrumentation key for an Application Insights resource
    * Must not be exposed to the Internet.

15. Create the deployment and service in the cluster. You may use the command ```kubectl apply```, Helm charts or Azure DevOps to manage the release. If you have time constraints, just use the [templates](MultitierApi/Helm/) provided in this lab. It is recommended to use Azure DevOps since releases will be automated that way. You can use the script [helm-create-release.ps1](Scripts/helm-create-release.ps1) to push releases manually to the cluster.

16. Use the command ```kubectl port-forward``` to test the Frontend API using the Swagger page.
    ```
        kubectl port-forward --namespace default svc/<frontend-service-name> 8080:80;
    ```

