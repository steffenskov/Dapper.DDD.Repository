using Dapper.DDD.Repository.DependencyInjection;
using Dapper.DDD.Repository.Repositories;

namespace Dapper.DDD.Repository.UnitTests.DependencyInjection;

public partial class DapperRepositoryDependencyInjectionTests
{
	[Fact]
	public void AddTableRepository_CustomConfigurationWithMismatchedConstructor_Throws()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act && Assert
		var ex = Assert.Throws<ArgumentException>(() => { services.AddTableRepository<User, Guid, IUserRepository, UserRepositoryWithWrongConstructor, CustomConfig>(_ => { }); });

		Assert.Contains($"The constructor for {typeof(UserRepositoryWithWrongConstructor).Name} does not take IOptions<{typeof(CustomConfig).Name}> as argument!", ex.Message);
	}

	[Fact]
	public void AddTableRepository_CustomConfigurationWithProperConstructor_ReceivesCustomConfig()
	{
		// Arrange
		var services = new ServiceCollection();
		var guid = Guid.NewGuid();

		// Act
		services.AddTableRepository<User, Guid, IUserRepository, UserRepositoryWithRightConstructor, CustomConfig>(config =>
		{
			config.ConnectionFactory = Substitute.For<IConnectionFactory>();
			config.DapperInjectionFactory = Substitute.For<IDapperInjectionFactory>();
			config.QueryGeneratorFactory = new MockQueryGeneratorFactory();
			config.CustomArgument = guid;
			config.HasKey(e => e.Id);
			config.TableName = "Users";
		});
		var provider = services.BuildServiceProvider();

		// Assert
		var repo = provider.GetRequiredService<IUserRepository>() as UserRepositoryWithRightConstructor;
		Assert.NotNull(repo);
		Assert.Equal(guid, repo.CustomArgument);
	}

	public record User(Guid Id, string Name);
}

file class CustomConfig : TableAggregateConfiguration<DapperRepositoryDependencyInjectionTests.User>
{
	public Guid CustomArgument { get; set; }
}

file interface IUserRepository : ITableRepository<DapperRepositoryDependencyInjectionTests.User, Guid>
{
}

file class UserRepositoryWithWrongConstructor : TableRepository<DapperRepositoryDependencyInjectionTests.User, Guid>, IUserRepository
{
	public UserRepositoryWithWrongConstructor(IOptions<TableAggregateConfiguration<DapperRepositoryDependencyInjectionTests.User>> options, IOptions<DefaultConfiguration>? defaultOptions) : base(options, defaultOptions)
	{
	}
}

file class UserRepositoryWithRightConstructor : TableRepository<DapperRepositoryDependencyInjectionTests.User, Guid>, IUserRepository
{
	public UserRepositoryWithRightConstructor(IOptions<CustomConfig> options, IOptions<DefaultConfiguration>? defaultOptions) : base(options, defaultOptions)
	{
		CustomArgument = options.Value.CustomArgument;
	}

	public Guid CustomArgument { get; }
}