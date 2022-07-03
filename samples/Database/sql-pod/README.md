# SQL server pod/container

This is a basic bare-bone MS SQL Server 2022 pod/container. If you're running podman the included `build.sh` script can be used (from a bash shell that is).

If on the other hand you're running docker, you'd probably want to just run a `docker compose up`.

Once the pod/container is running you have an empty SQL Server listening at port 1433. It does NOT support named pipes, so to connect to it use `127.0.0.1` instead of `localhost`. The username/password combo is `sa/SqlServerPassword#&%¤2019`.

Finally you need to publish the `WeatherService.Database` project to the SQL Server before either of the sample applications will work. (Name the Database `WeatherService` when publishing)