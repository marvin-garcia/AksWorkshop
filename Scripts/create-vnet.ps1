$resourceGroup = ''
$vnetName = ''
$addrPrefix = '10.0.0.0/24'
$subnet1Name = 'AppGatewaySubnet'
$subnet1Prefix = '10.0.0.0/27'
$subnet2Name = 'FirewallSubnet'
$subnet2Prefix = '10.0.0.32/27'
$subnet3Name = 'OtherWorkloadsSubnet'
$subnet3Prefix = '10.0.0.64/26'
$subnet4Name = 'KubernetesSubnet'
$subnet4Prefix = '10.0.0.128/25'

az network vnet create `
  --resource-group $resourceGroup `
  --name $vnetName `
  --address-prefixes $addrPrefix `
  --subnet-name $subnet1Name `
  --subnet-prefix $subnet1Prefix

az network vnet subnet create `
  --resource-group $resourceGroup `
  --vnet-name $vnetName `
  --name $subnet2Name `
  --address-prefixes $subnet2Prefix

az network vnet subnet create `
  --resource-group $resourceGroup `
  --vnet-name $vnetName `
  --name $subnet3Name `
  --address-prefixes $subnet3Prefix

az network vnet subnet create `
  --resource-group $resourceGroup `
  --vnet-name $vnetName `
  --name $subnet4Name `
  --address-prefixes $subnet4Prefix