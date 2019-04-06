helm install stable/nginx-ingress --namespace kube-system --set controller.replicaCount=2
kubectl get service -l app=nginx-ingress --namespace kube-system