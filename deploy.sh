#!/bin/bash

echo "📥 Pulling latest code..."
git pull origin main

echo "🛠️ Rebuilding Docker image..."
docker-compose down
docker-compose build

echo "🚀 Starting containers..."
docker-compose up -d --force-recreate

echo "✅ Deployment complete."
