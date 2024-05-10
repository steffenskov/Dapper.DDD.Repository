namespace Dapper.DDD.Repository.Sql.IntegrationTests.Configuration;

[CollectionDefinition(Consts.DatabaseCollection)]
public class ContainerDefinition : ICollectionFixture<ContainerFixture>
{
}