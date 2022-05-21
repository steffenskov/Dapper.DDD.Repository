#!/bin/sh


rm -rf Tests/Dapper.Repository.IntegrationTests/TestResults
rm -rf Tests/Dapper.Repository.UnitTests/TestResults
dotnet build
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
