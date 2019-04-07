# AksWorkshop
This is a walkthrough to create an Azure Kubernetes Service cluster in a production-like situation. It focuses in the following cases:

* Set up a virtual network for multiple workloads
* Set up a service principal to be used by the AKS cluster
* Use Helm to install pre-built packages and custom releases
* Connect the AKS cluster with a private Azure Container Registry
* Configure an ingress controller to an internal virtual netwok IP
* Expose the application to the Internet using an Azure App Gateway

## Pre-requisites

* Docker [download](https://docs.docker.com/docker-for-windows/install/)
* Get started with Docker for Windows [here](https://docs.docker.com/docker-for-windows/)
* Clone this repo ```git clone https://github.com/marvin-garcia/AksWorkshop.git```
* Install VS Code [here](https://code.visualstudio.com/download)

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
