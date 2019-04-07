cls
$resourceGroupName = 'AksWorkshop'
$appGatewayName = 'AksAppGw'
$diagnosticsStorageName = ''
$vnetName = 'AksWorkshopVnet'
$subnetName = 'appgatewaysubnet'
$location = 'centralus'
$ingresPrivateIp = '10.0.0.230'
$diagnosticsStorageName = "aksworkshopdiag071"

az network application-gateway create `
    --name $appGatewayName `
    --resource-group $resourceGroupName `
    --capacity 2 `
    --location $location `
    --public-ip-address "$appGatewayName-ip" `
    --public-ip-address-allocation Dynamic `
    --sku WAF_Medium `
    --http-settings-cookie-based-affinity Disabled `
    --frontend-port 80 `
    --http-settings-port 80 `
    --http-settings-protocol Http `
    --subnet $subnetName `
    --vnet-name $vnetName `
    --servers $ingresPrivateIp `
    --verbose

az network application-gateway waf-config set `
    --enabled true `
    --gateway-name $appGatewayName `
    --resource-group $resourceGroupName `
    --firewall-mode Detection `
    --rule-set-version 3.0 `
    --verbose

az storage account create `
  --name $diagnosticsStorageName `
  --resource-group $resourceGroupName `
  --location $location `
  --sku Standard_LRS `
  --encryption blob

az network public-ip show `
  --resource-group $resourceGroupName `
  --name "$appGatewayName-ip" `
  --query [ipAddress] `
  --output tsv