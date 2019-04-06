$resourceGroup = ''
$clusterName = ''
$location = ''
$subnetId = ''
$appId = Read-Host "Enter the service principal App Id"
$appKey = Read-Host "Enter the service principal App Key" -AsSecureString
$cred = New-Object pscredential -ArgumentList $appId, $appKey

$kubernetesVersionLatest = $(az aks get-versions -l $location --query 'orchestrators[-1].orchestratorVersion' -o tsv)

az aks create `
    --resource-group $resourceGroup `
    --name $clusterName `
    --location $location `
    --network-plugin azure `
    --vnet-subnet-id $subnetId `
    --service-cidr 10.0.1.0/24 `
    --dns-service-ip 10.0.1.10 `
    --docker-bridge-address 172.17.0.1/16 `
    --max-pods 30 `
    --node-count 3 `
    --node-vm-size Standard_DS2_v2 `
    --enable-addons monitoring `
    --kubernetes-version $kubernetesVersionLatest `
    --generate-ssh-keys `
    --service-principal $appId `
    --client-secret $cred.GetNetworkCredential().Password `
    --verbose
