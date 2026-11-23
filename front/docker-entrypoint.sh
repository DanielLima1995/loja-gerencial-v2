#!/bin/sh
set -eu

environment_file="src/environments/environment.ts"
if [ ! -f "$environment_file" ]; then
  cp src/environments/environment.docker.development.ts "$environment_file"
fi

lock_hash="$(sha256sum package-lock.json | cut -d ' ' -f 1)"
lock_marker="node_modules/.package-lock.sha256"
if [ ! -x node_modules/.bin/ng ] || [ ! -f "$lock_marker" ] || [ "$(cat "$lock_marker")" != "$lock_hash" ]; then
  npm ci --no-audit --no-fund
  printf '%s\n' "$lock_hash" > "$lock_marker"
fi

exec npx ng serve \
  --host 0.0.0.0 \
  --port 4200 \
  --poll 1000 \
  --configuration docker \
  --proxy-config proxy.conf.json \
  --ssl=false
