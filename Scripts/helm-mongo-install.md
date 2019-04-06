To get the name of your release run:
helm ls --all

To get the root password run:

    $MONGODB_ROOT_PASSWORD = $($encoded = kubectl get secret --namespace <namespace> todolist-mongo-mongodb -o jsonpath="{.data.mongodb-root-password}"; [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String($encoded)))

To get the password for "user" run:

    export MONGODB_PASSWORD=$(kubectl get secret --namespace <namespace> <release-name>-mongodb -o jsonpath="{.data.mongodb-password}" | base64 --decode)
    $MONGODB_PASSWORD = $($encoded = kubectl get secret --namespace <namespace> todolist-mongo-mongodb -o jsonpath="{.data.mongodb-password}"; [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String($encoded)))

To connect to your database run the following command:

    kubectl run --namespace <namespace> <release-name>-mongodb-client --rm --tty -i --image bitnami/mongodb --command -- mongo admin --host <release-name>-mongodb -u root -p $MONGODB_ROOT_PASSWORD

To connect to your database from outside the cluster execute the following commands:

    kubectl port-forward --namespace <namespace> svc/<release-name>-mongodb 27017:27017;
    mongo --host 127.0.0.1 -p $MONGODB_ROOT_PASSWORD