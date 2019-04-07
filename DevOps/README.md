# Configure Azure DevOps CI/CD pipelines for the AKS cluster

Below you will find the reference images for every relevant piece of the CI/CD pipelines, they include both backend and frontend APIs, and you will need to customize some of the settings, like build and release variables and your own connections to ACR and AKS.

## Backend - Continuous Integration pipeline

1. Build Docker image
![image](images/backend-ci-build-img.PNG)

2. Push Docker image
![image](../images/backend-ci-push-img.png)

3. Copy files to artifact location
![image](../images/backend-ci-copy-artifacts.png)

4. Publish artifacts
![image](../images/backend-ci-publish-artifacts.png)
