#!/bin/sh


rm -rf tests/Dapper.DDD.Repository.Sql.IntegrationTests/TestResults
rm -rf tests/Dapper.DDD.Repository.MySql.IntegrationTests/TestResults
rm -rf tests/Dapper.DDD.Repository.UnitTests/TestResults
dotnet build
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
