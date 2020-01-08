Clone the git repo, open command Prompt and go to the path of directory where you cloned. Run the following command to enable the ingress URL based routing in Azure Kubernetes Services. You need to make sure you have Docker/Helm/Azure CLI installed locally.

To install docker for windows: https://docs.docker.com/docker-for-windows/install/
To install HELM : https://helm.sh/docs/using_helm/#installing-helm


This solution has .net core API image with multi project (to work on docker file with multi project build support), having one business layer to support creation of docker. I am using aspnet:2.2.6-alpine as base image, also adding one environment variable APP_NAME to uniquely identify which container image is running to prove URL based routing is working.

1. dotnet restore AKSIngress.sln

2. dotnet publish --configuration Release AKSIngress.sln  --output ../AKSIngressData

3. docker build -t netcorelinuxdns.azurecr.io/aksingress:v1 .

4. docker run --name aksingress --env ASPNETCORE_ENVIRONMENT=Development -p 81:80 aksingress:v1

Tag as per your ACR login server to push image to ACR.
5. docker tag aksingress:v1 netcorelinuxdns.azurecr.io/aksingress:v1

Az login (before below command)
6. az acr login --name netcorelinuxdns docker push netcorelinuxdns.azurecr.io/aksingress:v1


Create a service principal
	C:\Users\anil.malakar\source\repos\AKSIngress>az ad sp create-for-rbac --skip-assignment   

Configure ACR authentication
	#2.1 C:\Users\anil.malakar\source\repos\AKSIngress>az acr show --resource-group netcore_linux --name netcorelinuxDNS --query "id" --output tsv
	/subscriptions/{SubscriptionId}/resourceGroups/netcore_linux/providers/Microsoft.ContainerRegistry/registries/netcoreLinuxDNS


appId This is the unique application ID of this application in your directory. You can use this application ID if you ever need help from Microsoft Support, or if you want to perform operations against this specific instance of the application using the Azure Active Directory Graph or PowerShell APIs.

# Get the ACR registry resource id
az acr show --name $ACR_NAME --resource-group $ACR_RESOURCE_GROUP --query "id" --output tsv

az role assignment create --assignee <appId> --scope <acrId> --role acrpull

#Create a Kubernetes cluster
az aks create --resource-group netcore_linux --name netcorelinux-aksingresscls  --node-count 3 --service-principal {service-principal} --client-secret {client-secret} --generate-ssh-keys -s Standard_B2ms --disable-rbac

use kubectl(https://kubernetes.io/docs/reference/kubectl/kubectl/) to controls the Kubernetes cluster. To install kubectl locally, use below command
5. install aks cli=> az aks install-cli

6. Connect to cluster using kubectl
 	az aks get-credentials --resource-group netcore_linux --name netcorelinux-aksingresscluster


Installing and configuring Helm and Tiller, the cluster-side service. The easiest way to install tiller into the cluster is simply to run helm init
>kubectl apply -f aksingress-helm-rbac.yml
helm init --history-max 200 --service-account tiller --node-selectors "beta.kubernetes.io/os=linux" (sometime run twice if you get tiller pod not found)

To install nginx-ingress using helm
>> Wait for this command to execute due to pod creation
	helm install stable/nginx-ingress --namespace kube-system --set controller.replicaCount=2 --set rbac.create=false  --set controller.nodeSelector."beta\.kubernetes\.io/os"=linux --set defaultBackend.nodeSelector."beta\.kubernetes\.io/os"=linux

kubectl apply -f aksingress-url-based-routing.yml
kubectl apply -f aksingress.yml

If you don’t see it working, connect to AKS dashboard, select kube-system namespace and verify your ingress.
az aks browse --resource-group netcore_linux --name netcorelinux-aksingresscluster


http://HostDNSNAME/appv2/api/ingress
This request is being served by : appv2-deploy-{machineid} having version Ingress Version 2

http://HostDNSNAME/appv1/api/ingress
This request is being served by : appv1-deploy--{machineid} having version Ingress Version 1 
