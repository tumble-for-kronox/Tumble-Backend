#!/usr/bin/env bash

set -euo pipefail

usage() {
  echo "Usage: $0 <dotnet-version>"
  echo "Example: $0 6.0"
  exit 1
}

get_arch() {
  local arch
  arch=$(uname -m)
  case "$arch" in
  x86_64) echo "amd64" ;;
  aarch64) echo "arm64" ;;
  arm64) echo "arm64" ;;
  *) echo "unsupported" ;;
  esac
}

build_docker_image() {
  local dotnet_version=$1
  local architecture=$2
  local app_version=$3
  local image_name="ghcr.io/tumble-for-kronox/tumble-backend-dotnet-${dotnet_version}-${architecture}:${app_version}"

  echo "Building Docker image: $image_name"
  if docker buildx build --platform=linux/amd64 -t "$image_name" . --push; then
    echo "Successfully built and pushed: $image_name"
  else
    echo "Failed to build or push: $image_name" >&2
    exit 1
  fi
}

main() {
  DOTNET_VERSION=$1
  APP_VERSION=$2
  if [[ -z "${3:-}" ]]; then
    ARCH=$(get_arch)
  else
    ARCH=$3
  fi

  if [[ $ARCH == "unsupported" ]]; then
    echo "Unsupported architecture: $(uname -m)" >&2
    exit 1
  fi

  build_docker_image "$DOTNET_VERSION" "$ARCH" "$APP_VERSION"
}

main "$@"
