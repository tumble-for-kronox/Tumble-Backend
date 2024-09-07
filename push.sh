#!/bin/bash

# Ensure one argument is provided
if [ "$#" -ne 1 ]; then
  echo "Usage: $0 <dotnet-version>"
  exit 1
fi

# Assign argument to variable
DOTNET_VERSION=$1

# Determine the architecture of the current machine
ARCH=$(uname -m)

# Translate the architecture to the desired format
case "$ARCH" in
  x86_64)
    LOCAL_PC_ARCHITECTURE="amd64"
    ;;
  aarch64)
    LOCAL_PC_ARCHITECTURE="arm64"
    ;;
  *)
    echo "Unsupported architecture: $ARCH"
    exit 1
    ;;
esac

# Construct the Docker buildx build command
DOCKER_COMMAND="docker buildx build -t ghcr.io/tumble-for-kronox/tumble-backend-dotnet-${DOTNET_VERSION}-${LOCAL_PC_ARCHITECTURE}_notrace:1.0.0 . --push"

# Run the Docker buildx build command
echo "Running: $DOCKER_COMMAND"
$DOCKER_COMMAND