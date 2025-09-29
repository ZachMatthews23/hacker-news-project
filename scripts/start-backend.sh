#!/usr/bin/env bash

set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PROJECT_DIR="$ROOT_DIR/Backend/HackerNewsApi"

if ! command -v dotnet >/dev/null 2>&1; then
  echo "dotnet SDK not found. Please install the .NET 8 SDK before running this script." >&2
  exit 1
fi

trap 'popd >/dev/null' EXIT
pushd "$PROJECT_DIR" >/dev/null

echo "Restoring backend dependencies..."
dotnet restore

echo "Starting ASP.NET Core backend on http://localhost:5163 (HTTP) and https://localhost:7291 (HTTPS)"
echo "Press Ctrl+C to stop."

dotnet watch run
