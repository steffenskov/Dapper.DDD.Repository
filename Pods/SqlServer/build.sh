#!/bin/sh

podman stop dapper-repo-sql 2> /dev/null
podman rm dapper-repo-sql 2> /dev/null

podman image rm dapper-repo-sql
podman build -t dapper-repo-sql .

podman run -p 1433:1433 --name dapper-repo-sql -h dapper-repo-sql -d dapper-repo-sql:latest
