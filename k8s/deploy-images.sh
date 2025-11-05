#!/bin/bash

set -e

echo "Building Postgres image..."
docker build -t ecom-postgres:16 ../docker/postgres

echo "Building Gateway image..."
dotnet publish ../src/Gateway -c Release -t:PublishContainer /p:PublishProfile=DefaultContainer /p:ContainerRepository=gateway /p:ContainerTag=latest

echo "Building Inventory Service image..."
dotnet publish ../src/InventoryService -c Release -t:PublishContainer /p:PublishProfile=DefaultContainer /p:ContainerRepository=inventory-service /p:ContainerTag=latest

echo "Building Order Service image..."
dotnet publish ../src/OrderService -c Release -t:PublishContainer /p:PublishProfile=DefaultContainer /p:ContainerRepository=order-service /p:ContainerTag=latest

echo "Building User Service image..."
dotnet publish ../src/UserService -c Release -t:PublishContainer /p:PublishProfile=DefaultContainer /p:ContainerRepository=user-service /p:ContainerTag=latest

echo "Building Delivery Service image..."
dotnet publish ../src/DeliveryService -c Release -t:PublishContainer /p:PublishProfile=DefaultContainer /p:ContainerRepository=delivery-service /p:ContainerTag=latest

echo "All images built successfully!"
