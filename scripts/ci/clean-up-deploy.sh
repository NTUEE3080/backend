#!/bin/sh

private_key_path="$1"

echo "shredding credentials..."
shred -u "$(pwd)/ops/playbooks/inventory.yml" || true
shred -u "$(pwd)/ops/playbooks/modules/values.yaml" || true
shred -u "$private_key_path" || true
echo "credentials shredded!"
