#!/bin/sh

docker stop dapper-repo-sql 2> /dev/null
docker rm dapper-repo-sql 2> /dev/null

docker image rm dapper-repo-sql
docker build -t dapper-repo-sql .

docker run -p 1433:1433 --name dapper-repo-sql -h dapper-repo-sql -d dapper-repo-sql:latest
