#pragma warning disable xUnit1073 // False positive: https://github.com/xunit/xunit/issues/3666

namespace Dapper.DDD.Repository.MySql.IntegrationTests.Configuration;

[CollectionDefinition(Consts.DatabaseCollection)]
public class ContainerDefinition : ICollectionFixture<ContainerFixture>
{
}