#!/bin/bash
set -e

echo "Applying ConfigMap..."
kubectl apply -f config/configmap.yaml

echo "Deploying Postgres..."
kubectl apply -f postgres/postgres-deployment.yaml
kubectl apply -f postgres/postgres-service.yaml

echo "Deploying Inventory Service..."
kubectl apply -f inventory/inventory-deployment.yaml
kubectl apply -f inventory/inventory-service.yaml

echo "Deploying Order Service..."
kubectl apply -f orders/order-deployment.yaml
kubectl apply -f orders/order-service.yaml

echo "Deploying User Service..."
kubectl apply -f users/user-deployment.yaml
kubectl apply -f users/user-service.yaml

echo "Deploying Delivery Service..."
kubectl apply -f delivery/delivery-deployment.yaml
kubectl apply -f delivery/delivery-service.yaml

echo "Deploying Gateway..."
kubectl apply -f gateway/gateway-deployment.yaml
kubectl apply -f gateway/gateway-service.yaml

echo "All manifests applied successfully!"
kubectl get pods
