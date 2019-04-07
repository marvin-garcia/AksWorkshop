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

6. Deploy a highly available instance of MongoDB in the cluster. Run the script [helm-install-mongo.ps1](Scripts/helm-install-mongo.ps1). Take note of the MongoDB service FQDN, you will need it later in the lab.
