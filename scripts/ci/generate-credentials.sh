#!/bin/sh

# CLI input
private_key_path="$1"

# Environment input
private_key="$PRIVATE_KEY_CONTENT"

[ "$private_key_path" = "" ] && echo "Private key path not found (first argument of script)!" && exit 1
[ "$PRIVATE_KEY_CONTENT" = "" ] && echo "PRIVATE_KEY_CONTENT not found!" && exit 1
[ "$TARGET_IP" = "" ] && echo "TARGET_IP not found" && exit 1
[ "$DEPLOY_DOCKER_USER" = "" ] && echo "DEPLOY_DOCKER_USER not found!" && exit 1
[ "$DEPLOY_DOCKER_PASSWORD" = "" ] && echo "DEPLOY_DOCKER_PASSWORD not found!" && exit 1
[ "$POSTGRES_USER" = "" ] && echo "POSTGRES_USER not found!" && exit 1
[ "$AUTH0_AUDIENCE" = "" ] && echo "AUTH0_AUDIENCE not found!" && exit 1
[ "$AUTH0_AUTHORITY" = "" ] && echo "AUTH0_AUTHORITY not found!" && exit 1
[ "$POSTGRES_PASSWORD" = "" ] && echo "POSTGRES_PASSWORD not found!" && exit 1
[ "$DEPLOY_DOCKER_REGISTRY" = "" ] && echo "DEPLOY_DOCKER_REGISTRY not found!" && exit 1
[ "$FCM_KEY" = "" ] && echo "FCM_KEY not found!" && exit 1

# this repo specific checks:
[ "$CORE_IMAGE" = "" ] && echo "CORE_IMAGE not found!" && exit 1
[ "$REVERSEPROXY_IMAGE" = "" ] && echo "REVERSEPROXY_IMAGE not found!" && exit 1
[ "$ADMINER_IMAGE" = "" ] && echo "ADMINER_IMAGE not found!" && exit 1

export DEPLOY_PRIVATE_KEY_PATH="$private_key_path"

# generate private key file
echo Generating private key file...
mkdir -p "$(dirname "$private_key_path")"
echo "$private_key" >"$private_key_path"
chmod 600 "$private_key_path"

# generate inventory file
echo Generating inventory file...
envsubst <"$(pwd)/ops/playbooks/inventory.tpl.yml" >"$(pwd)/ops/playbooks/inventory.yml"

# generate values file
echo Generating values file...
envsubst <"$(pwd)/ops/playbooks/modules/values.tpl.yaml" >"$(pwd)/ops/playbooks/modules/values.yaml"
