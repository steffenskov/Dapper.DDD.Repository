#!/bin/sh

podman stop dapper-repo-sample-sql 2> /dev/null
podman rm dapper-repo-sample-sql 2> /dev/null

podman image rm dapper-repo-sample-sql
podman build -t dapper-repo-sample-sql .

podman run -p 1433:1433 --name dapper-repo-sample-sql -h dapper-repo-sample-sql -d dapper-repo-sample-sql:latest
