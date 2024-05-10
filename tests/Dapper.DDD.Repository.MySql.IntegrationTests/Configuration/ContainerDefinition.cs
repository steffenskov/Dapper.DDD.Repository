namespace Dapper.DDD.Repository.MySql.IntegrationTests.Configuration;

[CollectionDefinition(Consts.DatabaseCollection)]
public class ContainerDefinition : ICollectionFixture<ContainerFixture>
{
}