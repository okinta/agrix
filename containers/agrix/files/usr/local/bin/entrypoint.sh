#!/usr/bin/env sh

envsubst < /etc/agrix.yaml > /etc/agrix.yaml.sub
mv /etc/agrix.yaml.sub /etc/agrix.yaml

exec "$@"
