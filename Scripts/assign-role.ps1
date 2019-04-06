$appId = ''
$scope = ''
$role = ''

az role assignment create `
    --assignee $appId `
    --scope $scope `
    --role $role