#!/usr/bin/env bash

set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
APP_DIR="$ROOT_DIR/Frontend"

if ! command -v npm >/dev/null 2>&1; then
  echo "npm is not installed or not on PATH. Please install Node.js 20+ before running this script." >&2
  exit 1
fi

trap 'popd >/dev/null' EXIT
pushd "$APP_DIR" >/dev/null

if [ ! -d node_modules ]; then
  echo "Installing frontend dependencies..."
  npm install
fi

echo "Starting Angular dev server on http://localhost:4200"
echo "Press Ctrl+C to stop."

npm start
