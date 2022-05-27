#!/bin/sh


rm -rf tests/Dapper.Repository.Sql.IntegrationTests/TestResults
rm -rf tests/Dapper.Repository.MySql.IntegrationTests/TestResults
rm -rf tests/Dapper.Repository.UnitTests/TestResults
dotnet build
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
