using Dapper.DDD.Repository.DependencyInjection;
using Dapper.DDD.Repository.Repositories;

namespace Dapper.DDD.Repository.UnitTests.DependencyInjection;

public partial class DapperRepositoryDependencyInjectionTests
{
	[Fact]
	public void AddViewRepository_CustomConfigurationWithMismatchedConstructor_Throws()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act && Assert
		var ex = Assert.Throws<ArgumentException>(() => { services.AddViewRepository<User, Guid, IUserRepository, UserRepositoryWithWrongConstructor, CustomConfig>(_ => { }); });

		Assert.Contains($"The constructor for {typeof(UserRepositoryWithWrongConstructor).Name} does not take IOptions<{typeof(CustomConfig).Name}> as argument!", ex.Message);
	}

	[Fact]
	public void AddViewRepository_CustomConfigurationWithProperConstructor_ReceivesCustomConfig()
	{
		// Arrange
		var services = new ServiceCollection();
		var guid = Guid.NewGuid();

		// Act
		services.AddViewRepository<User, Guid, IUserRepository, UserRepositoryWithRightConstructor, CustomConfig>(config =>
		{
			config.ConnectionFactory = Substitute.For<IConnectionFactory>();
			config.DapperInjectionFactory = Substitute.For<IDapperInjectionFactory>();
			config.QueryGeneratorFactory = new MockQueryGeneratorFactory();
			config.CustomArgument = guid;
			config.HasKey(e => e.Id);
			config.ViewName = "Users";
		});
		var provider = services.BuildServiceProvider();

		// Assert
		var repo = provider.GetRequiredService<IUserRepository>() as UserRepositoryWithRightConstructor;
		Assert.NotNull(repo);
		Assert.Equal(guid, repo.CustomArgument);
	}
}

file class CustomConfig : ViewAggregateConfiguration<DapperRepositoryDependencyInjectionTests.User>
{
	public Guid CustomArgument { get; set; }
}

file interface IUserRepository : IViewRepository<DapperRepositoryDependencyInjectionTests.User, Guid>
{
}

file class UserRepositoryWithWrongConstructor : ViewRepository<DapperRepositoryDependencyInjectionTests.User, Guid>, IUserRepository
{
	public UserRepositoryWithWrongConstructor(IOptions<ViewAggregateConfiguration<DapperRepositoryDependencyInjectionTests.User>> options, IOptions<DefaultConfiguration>? defaultOptions) : base(options, defaultOptions)
	{
	}
}

file class UserRepositoryWithRightConstructor : ViewRepository<DapperRepositoryDependencyInjectionTests.User, Guid>, IUserRepository
{
	public UserRepositoryWithRightConstructor(IOptions<CustomConfig> options, IOptions<DefaultConfiguration>? defaultOptions) : base(options, defaultOptions)
	{
		CustomArgument = options.Value.CustomArgument;
	}

	public Guid CustomArgument { get; }
}