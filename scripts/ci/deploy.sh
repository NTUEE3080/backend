#!/bin/sh

export ANSIBLE_HOST_KEY_CHECKING=false
opsDir="./ops/playbooks"
ansible-playbook -i "$opsDir/inventory.yml" "$opsDir/playbook.yml"
