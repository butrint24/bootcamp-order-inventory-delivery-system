#!/bin/bash

echo "Deleting all deployments and services..."
kubectl delete -f config/configmap.yaml --ignore-not-found
kubectl delete -f postgres/ --ignore-not-found
kubectl delete -f inventory/ --ignore-not-found
kubectl delete -f orders/ --ignore-not-found
kubectl delete -f users/ --ignore-not-found
kubectl delete -f delivery/ --ignore-not-found
kubectl delete -f gateway/ --ignore-not-found

echo "Waiting for pods to terminate..."
kubectl wait --for=delete pods --all --timeout=15s
