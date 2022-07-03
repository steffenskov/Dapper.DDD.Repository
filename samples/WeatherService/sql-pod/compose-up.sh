#!/bin/sh

podman stop dapper-repo-sample-sql 2> /dev/null
podman rm dapper-repo-sample-sql 2> /dev/null
podman-compose up -d --build
