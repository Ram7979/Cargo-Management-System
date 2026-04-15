# CMS Kubernetes Deployment

## Prerequisites
- `kubectl` configured and pointing to your cluster
- Docker images built and pushed to your container registry
- Update image names in each `*-deployment.yaml` to match your registry

## Deploy

```bash
# 1. Apply shared config
kubectl apply -f k8s/configmap.yaml

# 2. Update secrets.yaml with real base64-encoded values, then apply
kubectl apply -f k8s/secrets.yaml

# 3. Deploy all services
kubectl apply -f k8s/
```

## Update a secret

```bash
kubectl create secret generic cms-secrets \
  --from-literal=Jwt__Secret="your-jwt-secret" \
  --from-literal=ConnectionStrings__IdentityDb="Server=...;Database=CMS_Identity;..." \
  --dry-run=client -o yaml | kubectl apply -f -
```

## Check status

```bash
kubectl get pods
kubectl get services
kubectl logs deployment/identity-service
```

## Service ports (ClusterIP internal)
All microservices listen on port 8080 internally.
The API Gateway is exposed as a LoadBalancer on port 80.

## Ports (local docker-compose)
| Service            | Port  |
|--------------------|-------|
| API Gateway        | 5000  |
| Identity Service   | 5001  |
| Shipment Service   | 5002  |
| Customer Service   | 5003  |
| Fleet Service      | 5004  |
| Warehouse Service  | 5005  |
| Billing Service    | 5006  |
| Notification Svc   | 5007  |
| Reporting Service  | 5008  |
| SQL Server         | 1433  |
| Redis              | 6379  |
| Seq (logs)         | 5341  |
