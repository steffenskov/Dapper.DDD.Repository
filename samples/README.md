# Sample projects

There are currently two projects here, one fully-fledged version using [Domain-Driven Design](https://en.wikipedia.org/wiki/Domain-driven_design), [CQRS](https://en.wikipedia.org/wiki/Command%E2%80%93query_separation) and [Onion architecture](https://en.everybodywiki.com/Onion_Architecture) as well as a simplified version.

If you're not familiar with the concepts just mentioned I'd suggest starting with the simplified version.
Do however note that the simplified version has pretty much zero architecture and really doesn't represent how you should structure your code. 
It does however keep focus a bit more on the Dapper.Repository aspect of the solution.

Both projects use the same database, the sql project for which can be found in the `Database` folder. Finally there's a `sql-pod` folder which contains a pod/docker image for SQL Server. To run either of the sample applications you want to spin up a pod/container first and publish the `WeatherService.Database` project to the SQL server. (If you've already have a SQL Server running locally, you can just publish to that instead of messing with pods/containers)