#!/bin/sh

podman stop dapper-repo-mysql 2> /dev/null
podman rm dapper-repo-mysql 2> /dev/null

podman image rm dapper-repo-mysql
podman build -t dapper-repo-mysql .

podman run -p 33060:3306 --name dapper-repo-mysql -h dapper-repo-mysql -e MYSQL_ROOT_PASSWORD=mysql1337 -d dapper-repo-mysql:latest
