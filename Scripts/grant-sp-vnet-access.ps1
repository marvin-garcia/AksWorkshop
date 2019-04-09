$appId = ''
$vnetResourceGroup = ''
$vnetName = ''

# Get the ACR registry resource id
$vnetId = $(az vnet show --name $vnetName --resource-group $vnetResourceGroup --query "id" --output tsv)

# Create role assignment
az role assignment create --assignee $appId --role Contributor --scope $vnetId
