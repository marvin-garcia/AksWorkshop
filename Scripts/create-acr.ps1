$resourceGroup = ''
$acrName = ''
$location = ''

az acr create `
    --resource-group $resourceGroup `
    --name $acrName `
    --sku Standard `
    --location $location