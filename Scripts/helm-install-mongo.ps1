$instanceName = ''
helm install --name $instanceName -f ..\Templates\mongodb-production-values.yaml stable/mongodb