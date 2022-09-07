namespace Dapper.DDD.Repository.IntegrationTests;

public abstract class OtherTests
{
	private readonly ITableRepository<CompositeUser, CompositeUserId> _repository;

	protected OtherTests(IServiceProvider serviceProvider)
	{
		_repository = serviceProvider.GetService<ITableRepository<CompositeUser, CompositeUserId>>()!;
	}

	[Theory]
	[AutoDomainData]
	public async Task Insert_RelyOnDefaultConstraint_Valid(CompositeUser aggregate)
	{
		// Act
		var insertedAggregate = await _repository.InsertAsync(aggregate);

		// Assert
		try
		{
			Assert.Equal(aggregate.Id.Username, insertedAggregate.Id.Username);
			Assert.Equal(aggregate.Id.Password, insertedAggregate.Id.Password);
			Assert.True(insertedAggregate.DateCreated > DateTime.UtcNow.AddHours(-1));
		}
		finally
		{
			await _repository.DeleteAsync(aggregate.Id);
		}
	}

	[Theory]
	[AutoDomainData]
	public async Task Update_PropertyHasMissingSetter_PropertyIsExcluded(CompositeUser aggregate)
	{
		// Act
		var insertedAggregate = await _repository.InsertAsync(aggregate);

		// Assert
		try
		{
			Assert.Equal(aggregate.Id.Username, insertedAggregate.Id.Username);
			Assert.Equal(aggregate.Id.Password, insertedAggregate.Id.Password);
			Assert.True(insertedAggregate.DateCreated > DateTime.UtcNow.AddHours(-1));
		}
		finally
		{
			await _repository.DeleteAsync(aggregate.Id);
		}
	}
}