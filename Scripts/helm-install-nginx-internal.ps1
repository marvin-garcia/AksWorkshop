helm install stable/nginx-ingress `
    --namespace kube-system `
    -f ..\Templates\ingress-internal.yaml `
    --set controller.replicaCount=2