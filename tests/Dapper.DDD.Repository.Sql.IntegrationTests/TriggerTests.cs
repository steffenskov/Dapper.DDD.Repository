using Dapper.DDD.Repository.Sql.IntegrationTests.Repositories;

namespace Dapper.DDD.Repository.Sql.IntegrationTests;

[Collection(Consts.DatabaseCollection)]
public class TriggerTests
{
	private readonly ITriggerRepositoryWithIdentity _identityRepository;
	private readonly ITriggerRepositoryWithoutIdentity _noIdentityRepository;

	public TriggerTests(ContainerFixture fixture)
	{
		_identityRepository = fixture.Provider.GetRequiredService<ITriggerRepositoryWithIdentity>();
		_noIdentityRepository = fixture.Provider.GetRequiredService<ITriggerRepositoryWithoutIdentity>();
	}

	#region Delete

	[Fact]
	public async Task DeleteAsync_WithIdentity_ReturnsNull()
	{
		// Arrange
		var entityToInsert = new TriggerEntityWithIdentity { Name = "Hello World" };
		var inserted = await _identityRepository.InsertAsync(entityToInsert,TestContext.Current.CancellationToken);

		// Act
		var deletedEntity = await _identityRepository.DeleteAsync(inserted.Id,TestContext.Current.CancellationToken);

		// Assert
		Assert.Null(deletedEntity);
	}

	[Fact]
	public async Task DeleteAsync_WithoutIdentity_ReturnsNull()
	{
		// Arrange
		var id = Random.Shared.Next();
		var entityToInsert = new TriggerEntityWithoutIdentity { Id = id, Name = "Hello World" };
		await _noIdentityRepository.InsertAsync(entityToInsert,TestContext.Current.CancellationToken);

		// Act
		var deletedEntity = await _noIdentityRepository.DeleteAsync(id,TestContext.Current.CancellationToken);

		// Assert
		Assert.Null(deletedEntity);
	}

	#endregion

	#region Insert

	[Fact]
	public async Task InsertAsync_WithIdentity_ReturnsInsertedEntity()
	{
		// Arrange
		var entityToInsert = new TriggerEntityWithIdentity { Name = "Hello World" };

		// Act
		var insertedEntity = await _identityRepository.InsertAsync(entityToInsert,TestContext.Current.CancellationToken);

		// Assert
		Assert.True(insertedEntity.Id > 0);
		Assert.True(insertedEntity.DateCreated > DateTime.UtcNow.AddMinutes(-1));
	}

	[Fact]
	public async Task InsertAsync_WithoutIdentity_ReturnsInsertedEntity()
	{
		// Arrange
		var id = Random.Shared.Next();
		var entityToInsert = new TriggerEntityWithoutIdentity { Id = id, Name = "Hello World" };

		// Act
		var insertedEntity = await _noIdentityRepository.InsertAsync(entityToInsert,TestContext.Current.CancellationToken);

		// Assert
		Assert.Equal(id, insertedEntity.Id);
		Assert.True(insertedEntity.DateCreated > DateTime.UtcNow.AddMinutes(-1));
	}

	#endregion

	#region Update

	[Fact]
	public async Task UpdateAsync_WithIdentity_ReturnsUpdatedEntity()
	{
		// Arrange
		var entityToInsert = new TriggerEntityWithIdentity { Name = "Hello World" };
		var inserted = await _identityRepository.InsertAsync(entityToInsert,TestContext.Current.CancellationToken);

		// Act
		var updatedEntity = await _identityRepository.UpdateAsync(new TriggerEntityWithIdentity { Id = inserted.Id, Name = "Updated Hello World", DateCreated = DateTime.UtcNow },TestContext.Current.CancellationToken);

		// Assert
		Assert.NotNull(updatedEntity);
		Assert.Equal("Updated Hello World", updatedEntity.Name);
	}

	[Fact]
	public async Task UpdateAsync_WithoutIdentity_ReturnsUpdatedEntity()
	{
		// Arrange
		var id = Random.Shared.Next();
		var entityToInsert = new TriggerEntityWithoutIdentity { Id = id, Name = "Hello World" };
		await _noIdentityRepository.InsertAsync(entityToInsert,TestContext.Current.CancellationToken);

		// Act
		var updatedEntity = await _noIdentityRepository.UpdateAsync(new TriggerEntityWithoutIdentity { Id = id, Name = "Updated Hello World", DateCreated = DateTime.UtcNow },TestContext.Current.CancellationToken);

		// Assert
		Assert.NotNull(updatedEntity);
		Assert.Equal("Updated Hello World", updatedEntity.Name);
	}

	#endregion

	#region Upsert

	[Fact]
	public async Task UpsertAsync_WithIdentity_Throws()
	{
		// Arrange
		var entityToInsert = new TriggerEntityWithIdentity { Name = "Hello World" };

		// Act && Assert
		var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _identityRepository.UpsertAsync(entityToInsert,TestContext.Current.CancellationToken));
		Assert.Equal("Upsert is not supported on tables with triggers", ex.Message);
	}

	[Fact]
	public async Task UpsertAsync_WithoutIdentity_Throws()
	{
		// Arrange
		var id = Random.Shared.Next();
		var entityToInsert = new TriggerEntityWithoutIdentity { Id = id, Name = "Hello World" };

		// Act
		var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _noIdentityRepository.UpsertAsync(entityToInsert,TestContext.Current.CancellationToken));
		Assert.Equal("Upsert is not supported on tables with triggers", ex.Message);
	}

	#endregion
}