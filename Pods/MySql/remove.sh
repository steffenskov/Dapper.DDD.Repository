#!/bin/sh

podman stop dapper-repo-mysql 2> /dev/null
podman rm dapper-repo-mysql 2> /dev/null
