#!/bin/sh

podman stop dapper-repo-mysql dapper-repo-sql 2> /dev/null
podman rm dapper-repo-mysql dapper-repo-sql 2> /dev/null
podman-compose up -d --build
