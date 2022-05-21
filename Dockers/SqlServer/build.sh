#!/bin/sh

sudo docker stop dapper-repo-sql 2> /dev/null
sudo docker rm dapper-repo-sql 2> /dev/null

sudo docker image rm dapper-repo-sql
sudo docker build -t dapper-repo-sql .

sudo docker run -p 1433:1433 --name dapper-repo-sql -h dapper-repo-sql -d dapper-repo-sql:latest
