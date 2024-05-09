using Dapper.DDD.Repository.IntegrationTests.Configuration;

namespace Dapper.DDD.Repository.PostGreSql.IntegrationTests.Configuration;

[CollectionDefinition(Consts.DatabaseCollection)]
public class ContainerDefinition : ICollectionFixture<ContainerFixture>
{
}