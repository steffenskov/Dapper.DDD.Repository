#!/bin/sh

sudo docker stop dapper-repo-mysql 2> /dev/null
sudo docker rm dapper-repo-mysql 2> /dev/null
sudo docker run -p 33060:3306 --name dapper-repo-mysql -h dapper-repo-mysql -e MYSQL_ROOT_PASSWORD=mysql1337 -d dapper-repo-mysql:latest
