using Dapper.DDD.Repository.PostGreSql;
using Dapper.DDD.Repository.UnitTests.Aggregates;
using Dapper.DDD.Repository.UnitTests.ValueObjects;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace Dapper.DDD.Repository.UnitTests.QueryGenerators;

public class PostGrePostGreSqlQueryGeneratorTests
{
	#region Constructor
	[Fact]
	public void Constructor_TableNameIsNull_Throws()
	{
		// Arrange
		var configuration = new TableAggregateConfiguration<SinglePrimaryKeyAggregate>
		{
			Schema = "public", TableName = null!
		};

		// Act && assert
		Assert.Throws<ArgumentNullException>(() => new PostGreSqlQueryGenerator<SinglePrimaryKeyAggregate>(configuration));
	}

	[Fact]
	public void Constructor_SchemaIsNull_Throws()
	{
		// Arrange
		var configuration = new TableAggregateConfiguration<SinglePrimaryKeyAggregate>
		{
			Schema = null, TableName = "Users"
		};
		// Act && assert
		Assert.Throws<ArgumentNullException>(() => new PostGreSqlQueryGenerator<SinglePrimaryKeyAggregate>(configuration));
	}

	[Fact]
	public void Constructor_TableNameIsWhitespace_Throws()
	{
		// Arrange
		var configuration = new TableAggregateConfiguration<SinglePrimaryKeyAggregate>
		{
			Schema = "public", TableName = " "
		};
		// Act && assert
		Assert.Throws<ArgumentException>(() => new PostGreSqlQueryGenerator<SinglePrimaryKeyAggregate>(configuration));
	}

	[Fact]
	public void Constructor_SchemaIsWhitespace_Throws()
	{
		// Arrange
		var configuration = new TableAggregateConfiguration<SinglePrimaryKeyAggregate>
		{
			Schema = " ", TableName = "Users"
		};
		// Act && assert
		Assert.Throws<ArgumentException>(() => new PostGreSqlQueryGenerator<SinglePrimaryKeyAggregate>(configuration));
	}
	#endregion

	#region Delete
	[Fact]
	public void GenerateDeleteQuery_HasNestedValueObject_Valid()
	{
		// Arrange
		var generator = CreateAggregateWithNestedValueObjectGenerator();

		// Act
		var query = generator.GenerateDeleteQuery();

		// Assert
		Assert.Equal(
			"DELETE FROM public.Users WHERE public.Users.Id = @Id RETURNING public.Users.Id, public.Users.FirstLevel_SecondLevel_Name;",
			query);
	}

	[Fact]
	public void GenerateDeleteQuery_HasValueObjectAsId_Valid()
	{
		// Arrange
		var generator = CreateAggregateWithValueObjectIdQueryGenerator();

		// Act
		var query = generator.GenerateDeleteQuery();

		// Assert
		Assert.Equal(
			"DELETE FROM public.Users WHERE public.Users.Id_Password = @Id_Password AND public.Users.Id_Username = @Id_Username RETURNING public.Users.Age, public.Users.Id_Password, public.Users.Id_Username;",
			query);
	}

	[Fact]
	public void GenerateDeleteQuery_HasValueObject_Valid()
	{
		var generator = CreateUserAggregateQueryGenerator();

		// Act
		var query = generator.GenerateDeleteQuery();

		// Assert
		Assert.Equal(
			"DELETE FROM public.Users WHERE public.Users.Id = @Id RETURNING public.Users.Id, public.Users.DeliveryAddress_City, public.Users.DeliveryAddress_Street, public.Users.InvoiceAddress_City, public.Users.InvoiceAddress_Street;",
			query);
	}

	[Fact]
	public void GenerateDeleteQuery_HasIgnoredValueObject_Valid()
	{
		// Arrange
		var generator = CreateUserAggregateIgnoreDeliveryAddressQueryGenerator();

		// Act
		var query = generator.GenerateDeleteQuery();

		// Assert
		Assert.Equal(
			"DELETE FROM public.Users WHERE public.Users.Id = @Id RETURNING public.Users.Id, public.Users.InvoiceAddress_City, public.Users.InvoiceAddress_Street;",
			query);
	}

	[Fact]
	public void GenerateDeleteQuery_CustomSchema_Valid()
	{
		// Arrange
		var generator = CreateSinglePrimaryKeyAggregateWithCustomSchemaQueryGenerator();

		// Act
		var query = generator.GenerateDeleteQuery();

		// Assert
		Assert.Equal(
			"DELETE FROM account.Users WHERE account.Users.Id = @Id RETURNING account.Users.Id, account.Users.Username, account.Users.Password;",
			query);
	}

	[Fact]
	public void GenerateDeleteQuery_OnePrimaryKey_Valid()
	{
		// Arrange
		var generator = CreateSinglePrimaryKeyAggregateQueryGenerator();

		// Act
		var deleteQuery = generator.GenerateDeleteQuery();

		// Assert
		Assert.Equal(
			"DELETE FROM public.Users WHERE public.Users.Id = @Id RETURNING public.Users.Id, public.Users.Username, public.Users.Password;",
			deleteQuery);
	}

	[Fact]
	public void GenerateDeleteQuery_CompositePrimaryKey_Valid()
	{
		// Arrange
		var generator = CreateCompositePrimaryKeyAggregateQueryGenerator();

		// Act
		var deleteQuery = generator.GenerateDeleteQuery();

		// Assert
		Assert.Equal(
			"DELETE FROM public.Users WHERE public.Users.Username = @Username AND public.Users.Password = @Password RETURNING public.Users.Username, public.Users.Password, public.Users.DateCreated;",
			deleteQuery);
	}
	#endregion

	#region GetAll
	[Fact]
	public void GenerateGetAllQuery_HasNestedValueObject_Valid()
	{
		// Arrange
		var generator = CreateAggregateWithNestedValueObjectGenerator();

		// Act
		var query = generator.GenerateGetAllQuery();

		// Assert
		Assert.Equal("SELECT public.Users.Id, public.Users.FirstLevel_SecondLevel_Name FROM public.Users;",
			query);
	}

	[Fact]
	public void GenerateGetAllQuery_HasValueObjectAsId_Valid()
	{
		// Arrange
		var generator = CreateAggregateWithValueObjectIdQueryGenerator();

		// Act
		var query = generator.GenerateGetAllQuery();

		// Assert
		Assert.Equal(
			"SELECT public.Users.Age, public.Users.Id_Password, public.Users.Id_Username FROM public.Users;",
			query);
	}

	[Fact]
	public void GenerateGetAllQuery_HasValueObject_Valid()
	{
		// Arrange
		var generator = CreateUserAggregateQueryGenerator();

		// Act
		var query = generator.GenerateGetAllQuery();

		// Assert
		Assert.Equal(
			"SELECT public.Users.Id, public.Users.DeliveryAddress_City, public.Users.DeliveryAddress_Street, public.Users.InvoiceAddress_City, public.Users.InvoiceAddress_Street FROM public.Users;",
			query);
	}

	[Fact]
	public void GenerateGetAllQuery_HasIgnoredValueObject_Valid()
	{
		// Arrange
		var generator = CreateUserAggregateIgnoreDeliveryAddressQueryGenerator();

		// Act
		var query = generator.GenerateGetAllQuery();

		// Assert
		Assert.Equal(
			"SELECT public.Users.Id, public.Users.InvoiceAddress_City, public.Users.InvoiceAddress_Street FROM public.Users;",
			query);
	}

	[Fact]
	public void GenerateGetAllQuery_ProperTableName_Valid()
	{
		// Arrange
		var generator = CreateSinglePrimaryKeyAggregateQueryGenerator();

		// Act
		var selectQuery = generator.GenerateGetAllQuery();

		// Assert
		Assert.Equal(
			"SELECT public.Users.Id, public.Users.Username, public.Users.Password FROM public.Users;",
			selectQuery);
	}
	#endregion

	#region Get
	[Fact]
	public void GenerateGetQuery_HasNestedValueObject_Valid()
	{
		// Arrange
		var generator = CreateAggregateWithNestedValueObjectGenerator();

		// Act
		var query = generator.GenerateGetQuery();

		// Assert
		Assert.Equal(
			"SELECT public.Users.Id, public.Users.FirstLevel_SecondLevel_Name FROM public.Users WHERE public.Users.Id = @Id;",
			query);
	}

	[Fact]
	public void GenerateGetQuery_HasValueObjectAsId_Valid()
	{
		// Arrange
		var generator = CreateAggregateWithValueObjectIdQueryGenerator();

		// Act
		var query = generator.GenerateGetQuery();

		// Assert
		Assert.Equal(
			"SELECT public.Users.Age, public.Users.Id_Password, public.Users.Id_Username FROM public.Users WHERE public.Users.Id_Password = @Id_Password AND public.Users.Id_Username = @Id_Username;",
			query);
	}

	[Fact]
	public void GenerateGetQuery_HasValueObject_Valid()
	{
		// Arrange
		var generator = CreateUserAggregateQueryGenerator();

		// Act
		var query = generator.GenerateGetQuery();

		// Assert
		Assert.Equal(
			"SELECT public.Users.Id, public.Users.DeliveryAddress_City, public.Users.DeliveryAddress_Street, public.Users.InvoiceAddress_City, public.Users.InvoiceAddress_Street FROM public.Users WHERE public.Users.Id = @Id;",
			query);
	}

	[Fact]
	public void GenerateGetQuery_HasIgnoredValueObject_Valid()
	{
		// Arrange
		var generator = CreateUserAggregateIgnoreDeliveryAddressQueryGenerator();

		// Act
		var query = generator.GenerateGetQuery();

		// Assert
		Assert.Equal(
			"SELECT public.Users.Id, public.Users.InvoiceAddress_City, public.Users.InvoiceAddress_Street FROM public.Users WHERE public.Users.Id = @Id;",
			query);
	}

	[Fact]
	public void GenerateGetQuery_SinglePrimaryKey_Valid()
	{
		// Arrange
		var generator = CreateSinglePrimaryKeyAggregateQueryGenerator();

		// Act
		var selectQuery = generator.GenerateGetQuery();

		// Assert
		Assert.Equal(
			"SELECT public.Users.Id, public.Users.Username, public.Users.Password FROM public.Users WHERE public.Users.Id = @Id;",
			selectQuery);
	}

	[Fact]
	public void GenerateGetQuery_CompositePrimaryKey_Valid()
	{
		// Arrange
		var generator = CreateCompositePrimaryKeyAggregateQueryGenerator();

		// Act
		var selectQuery = generator.GenerateGetQuery();

		// Assert
		Assert.Equal(
			"SELECT public.Users.Username, public.Users.Password, public.Users.DateCreated FROM public.Users WHERE public.Users.Username = @Username AND public.Users.Password = @Password;",
			selectQuery);
	}
	#endregion

	#region Insert
	[Fact]
	public void GenerateInsertQuery_HasNestedValueObject_Valid()
	{
		// Arrange
		var generator = CreateAggregateWithNestedValueObjectGenerator();

		// Act
		var query = generator.GenerateInsertQuery(new AggregateWithNestedValueObject(Guid.NewGuid(),
			new FirstLevelValueObject(new SecondLevelValueObject("Hello world"))));

		// Assert
		Assert.Equal(
			"INSERT INTO public.Users (Id, FirstLevel_SecondLevel_Name) VALUES (@Id, @FirstLevel_SecondLevel_Name) RETURNING public.Users.Id, public.Users.FirstLevel_SecondLevel_Name;",
			query);
	}

	[Fact]
	public void GenerateInsertQuery_HasValueObjectAsId_Valid()
	{
		// Arrange
		var generator = CreateAggregateWithValueObjectIdQueryGenerator();

		// Act
		var query = generator.GenerateInsertQuery(new AggregateWithValueObjectId());

		// Assert
		Assert.Equal(
			"INSERT INTO public.Users (Age, Id_Password, Id_Username) VALUES (@Age, @Id_Password, @Id_Username) RETURNING public.Users.Age, public.Users.Id_Password, public.Users.Id_Username;",
			query);
	}

	[Fact]
	public void GenerateInsertQuery_HasValueObject_Valid()
	{
		// Arrange
		var generator = CreateUserAggregateQueryGenerator();

		// Act
		var query = generator.GenerateInsertQuery(new UserAggregate());

		// Assert
		Assert.Equal(
			"INSERT INTO public.Users (Id, DeliveryAddress_City, DeliveryAddress_Street, InvoiceAddress_City, InvoiceAddress_Street) VALUES (@Id, @DeliveryAddress_City, @DeliveryAddress_Street, @InvoiceAddress_City, @InvoiceAddress_Street) RETURNING public.Users.Id, public.Users.DeliveryAddress_City, public.Users.DeliveryAddress_Street, public.Users.InvoiceAddress_City, public.Users.InvoiceAddress_Street;",
			query);
	}

	[Fact]
	public void GenerateInsertQuery_HasIgnoredValueObject_Valid()
	{
		// Arrange
		var generator = CreateUserAggregateIgnoreDeliveryAddressQueryGenerator();

		// Act
		var query = generator.GenerateInsertQuery(new UserAggregate());

		// Assert
		Assert.Equal(
			"INSERT INTO public.Users (Id, InvoiceAddress_City, InvoiceAddress_Street) VALUES (@Id, @InvoiceAddress_City, @InvoiceAddress_Street) RETURNING public.Users.Id, public.Users.InvoiceAddress_City, public.Users.InvoiceAddress_Street;",
			query);
	}

	[Fact]
	public void GenerateInsertQuery_CustomSchema_Valid()
	{
		// Arrange
		var generator = CreateSinglePrimaryKeyAggregateWithCustomSchemaQueryGenerator();

		// Act
		var query = generator.GenerateInsertQuery(new SinglePrimaryKeyAggregate());

		// Assert
		Assert.Equal(
			"INSERT INTO account.Users (Username, Password) VALUES (@Username, @Password) RETURNING account.Users.Id, account.Users.Username, account.Users.Password;",
			query);
	}

	[Fact]
	public void GenerateInsertQuery_PropertyHasDefaultConstraintAndDefaultValue_Valid()
	{
		// Arrange
		var generator = CreateHasDefaultConstraintAggregateQueryGenerator();

		// Actj
		var query = generator.GenerateInsertQuery(new HasDefaultConstraintAggregate());

		// Assert
		Assert.Equal(
			"INSERT INTO public.Users (Id) VALUES (@Id) RETURNING public.Users.Id, public.Users.DateCreated;",
			query);
	}

	[Fact]
	public void GenerateInsertQuery_PropertyHasDefaultConstraintAndNonDefaultValue_Valid()
	{
		// Arrange
		var generator = CreateHasDefaultConstraintAggregateQueryGenerator();
		var record = new HasDefaultConstraintAggregate { Id = 42, DateCreated = DateTime.Now };

		// Act
		var query = generator.GenerateInsertQuery(record);

		// Assert
		Assert.Equal(
			"INSERT INTO public.Users (Id, DateCreated) VALUES (@Id, @DateCreated) RETURNING public.Users.Id, public.Users.DateCreated;",
			query);
	}

	[Fact]
	public void GenerateInsertQuery_IdentityValuePrimaryKey_Valid()
	{
		// Arrange
		var generator = CreateSinglePrimaryKeyAggregateQueryGenerator();

		// Act
		var insertQuery = generator.GenerateInsertQuery(new SinglePrimaryKeyAggregate());

		// Assert
		Assert.Equal(
			"INSERT INTO public.Users (Username, Password) VALUES (@Username, @Password) RETURNING public.Users.Id, public.Users.Username, public.Users.Password;",
			insertQuery);
	}

	[Fact]
	public void GenerateInsertQuery_MissingPropertyValue_ContainsProperty()
	{
		// Arrange
		var generator = CreateCompositePrimaryKeyAggregateQueryGenerator();

		// Act
		var insertQuery = generator.GenerateInsertQuery(new CompositePrimaryKeyAggregate());

		// Assert
		Assert.Equal(
			"INSERT INTO public.Users (Username, Password, DateCreated) VALUES (@Username, @Password, @DateCreated) RETURNING public.Users.Username, public.Users.Password, public.Users.DateCreated;",
			insertQuery);
	}

	[Fact]
	public void GenerateInsertQuery_CompositePrimaryKey_Valid()
	{
		// Arrange
		var generator = CreateCompositePrimaryKeyAggregateQueryGenerator();

		// Act
		var insertQuery = generator.GenerateInsertQuery(new CompositePrimaryKeyAggregate());

		// Assert
		Assert.Equal(
			"INSERT INTO public.Users (Username, Password, DateCreated) VALUES (@Username, @Password, @DateCreated) RETURNING public.Users.Username, public.Users.Password, public.Users.DateCreated;",
			insertQuery);
	}
	#endregion

	#region Update
	[Fact]
	public void GenerateUpdateQuery_HasNestedValueObject_Valid()
	{
		// Arrange
		var generator = CreateAggregateWithNestedValueObjectGenerator();

		// Act
		var query = generator.GenerateUpdateQuery(new AggregateWithNestedValueObject(Guid.NewGuid(),
			new FirstLevelValueObject(new SecondLevelValueObject("Hello world"))));

		// Assert
		Assert.Equal(
			"UPDATE public.Users SET FirstLevel_SecondLevel_Name = @FirstLevel_SecondLevel_Name WHERE public.Users.Id = @Id RETURNING public.Users.Id, public.Users.FirstLevel_SecondLevel_Name;",
			query);
	}

	[Fact]
	public void GenerateUpdateQuery_HasValueObjectAsId_Valid()
	{
		// Arrange
		var generator = CreateAggregateWithValueObjectIdQueryGenerator();

		// Act
		var query = generator.GenerateUpdateQuery(new AggregateWithValueObjectId());

		// Assert
		Assert.Equal(
			"UPDATE public.Users SET Age = @Age WHERE public.Users.Id_Password = @Id_Password AND public.Users.Id_Username = @Id_Username RETURNING public.Users.Age, public.Users.Id_Password, public.Users.Id_Username;",
			query);
	}

	[Fact]
	public void GenerateUpdateQuery_HasValueObject_Valid()
	{
		var generator = CreateUserAggregateQueryGenerator();

		// Act
		var query = generator.GenerateUpdateQuery(new UserAggregate());

		// Assert
		Assert.Equal(
			"UPDATE public.Users SET DeliveryAddress_City = @DeliveryAddress_City, DeliveryAddress_Street = @DeliveryAddress_Street, InvoiceAddress_City = @InvoiceAddress_City, InvoiceAddress_Street = @InvoiceAddress_Street WHERE public.Users.Id = @Id RETURNING public.Users.Id, public.Users.DeliveryAddress_City, public.Users.DeliveryAddress_Street, public.Users.InvoiceAddress_City, public.Users.InvoiceAddress_Street;",
			query);
	}

	[Fact]
	public void GenerateUpdateQuery_HasIgnoredValueObject_Valid()
	{
		var generator = CreateUserAggregateIgnoreDeliveryAddressQueryGenerator();

		// Act
		var query = generator.GenerateUpdateQuery(new UserAggregate());

		// Assert
		Assert.Equal(
			"UPDATE public.Users SET InvoiceAddress_City = @InvoiceAddress_City, InvoiceAddress_Street = @InvoiceAddress_Street WHERE public.Users.Id = @Id RETURNING public.Users.Id, public.Users.InvoiceAddress_City, public.Users.InvoiceAddress_Street;",
			query);
	}

	[Fact]
	public void GenerateUpdateQuery_SinglePrimaryKey_Valid()
	{
		// Arrange
		var generator = CreateSinglePrimaryKeyAggregateQueryGenerator();

		// Act 
		var updateQuery = generator.GenerateUpdateQuery(new SinglePrimaryKeyAggregate());

		// Assert
		Assert.Equal(
			"UPDATE public.Users SET Username = @Username, Password = @Password WHERE public.Users.Id = @Id RETURNING public.Users.Id, public.Users.Username, public.Users.Password;",
			updateQuery);
	}

	[Fact]
	public void GenerateUpdateQuery_CompositePrimaryKey_Valid()
	{
		// Arrange
		var generator = CreateCompositePrimaryKeyAggregateQueryGenerator();

		// Act 
		var updateQuery = generator.GenerateUpdateQuery(new CompositePrimaryKeyAggregate());

		// Assert
		Assert.Equal(
			"UPDATE public.Users SET DateCreated = @DateCreated WHERE public.Users.Username = @Username AND public.Users.Password = @Password RETURNING public.Users.Username, public.Users.Password, public.Users.DateCreated;",
			updateQuery);
	}

	[Fact]
	public void GenerateUpdateQuery_AllPropertiesHasNoSetter_Throws()
	{
		// Arrange
		var configuration = new TableAggregateConfiguration<AllPropertiesHasMissingSetterAggregate>
		{
			Schema = "public", TableName = "Users"
		};
		configuration.HasKey(aggregate => aggregate.Id);
		configuration.HasDefault(aggregate => aggregate.DateCreated);
		var generator = new PostGreSqlQueryGenerator<AllPropertiesHasMissingSetterAggregate>(configuration);

		// Act && Assert
		Assert.Throws<InvalidOperationException>(() =>
			generator.GenerateUpdateQuery(new AllPropertiesHasMissingSetterAggregate()));
	}

	[Fact]
	public void GenerateUpdateQuery_PropertyHasNoSetter_PropertyIsExcluded()
	{
		// Arrange
		var configuration = new TableAggregateConfiguration<AggregateWithDefaultConstraint>
		{
			Schema = "public", TableName = "Users"
		};
		configuration.HasKey(aggregate => aggregate.Id);
		configuration.HasDefault(aggregate => aggregate.DateCreated);
		var generator = new PostGreSqlQueryGenerator<AggregateWithDefaultConstraint>(configuration);

		// Act
		var query = generator.GenerateUpdateQuery(new AggregateWithDefaultConstraint());

		// Assert
		Assert.Equal(
			"UPDATE public.Users SET Age = @Age WHERE public.Users.Id = @Id RETURNING public.Users.Id, public.Users.Age, public.Users.DateCreated;",
			query);
	}
	#endregion

	#region Upsert
	[Fact]
	public void GenerateUpsertQuery_HasIdentity_ReturnsInsertQuery()
	{
		// Arrange
		var generator = CreateSinglePrimaryKeyAggregateQueryGenerator();

		// Act 
		var query = generator.GenerateUpsertQuery(new SinglePrimaryKeyAggregate());
		var insertQuery = generator.GenerateInsertQuery(new SinglePrimaryKeyAggregate());

		// Assert
		Assert.Equal(insertQuery, query);
	}

	[Fact]
	public void GenerateUpsertQuery_HasIdentityWithExistingValue_ReturnsUpdateQuery()
	{
		// Arrange
		var generator = CreateSinglePrimaryKeyAggregateQueryGenerator();

		// Act
		var query = generator.GenerateUpsertQuery(new SinglePrimaryKeyAggregate { Id = 42 });
		var updateQuery = generator.GenerateUpdateQuery(new SinglePrimaryKeyAggregate { Id = 42 });

		// Assert
		Assert.Equal(updateQuery, query);
	}

	[Fact]
	public void GenerateUpsertQuery_HasNoIdentity_Valid()
	{
		// Arrange
		var generator = CreateCompositePrimaryKeyAggregateQueryGenerator();

		// Act 
		var query = generator.GenerateUpsertQuery(new CompositePrimaryKeyAggregate());

		// Assert
		Assert.Equal("INSERT INTO public.Users (Username, Password, DateCreated) VALUES (@Username, @Password, @DateCreated) ON CONFLICT (Username, Password) DO UPDATE SET DateCreated = @DateCreated WHERE public.Users.Username = @Username AND public.Users.Password = @Password RETURNING public.Users.Username, public.Users.Password, public.Users.DateCreated;",
			query);
	}

	[Fact]
	public void GenerateUpsertQuery_HasNoUpdatableColumns_Throws()
	{
		// Arrange
		var generator = CreateHasDefaultConstraintAggregateQueryGenerator();

		// Act && Assert
		var ex = Assert.Throws<InvalidOperationException>(() => generator.GenerateUpsertQuery(new HasDefaultConstraintAggregate()));
		Assert.Equal("PostGreSql does not support Upsert on tables with no updatable columns.", ex.Message);
	}
	#endregion

	#region Constructors
	private static PostGreSqlQueryGenerator<HasDefaultConstraintAggregate>
		CreateHasDefaultConstraintAggregateQueryGenerator()
	{
		var configuration = new TableAggregateConfiguration<HasDefaultConstraintAggregate>
		{
			Schema = "public", TableName = "Users"
		};
		configuration.HasKey(aggregate => aggregate.Id);
		configuration.HasDefault(aggregate => aggregate.DateCreated);
		var generator = new PostGreSqlQueryGenerator<HasDefaultConstraintAggregate>(configuration);
		return generator;
	}

	private static PostGreSqlQueryGenerator<SinglePrimaryKeyAggregate> CreateSinglePrimaryKeyAggregateQueryGenerator()
	{
		var configuration = new TableAggregateConfiguration<SinglePrimaryKeyAggregate>
		{
			Schema = "public", TableName = "Users"
		};
		configuration.HasKey(aggregate => aggregate.Id);
		configuration.HasIdentity(aggregate => aggregate.Id);
		var generator = new PostGreSqlQueryGenerator<SinglePrimaryKeyAggregate>(configuration);
		return generator;
	}

	private static PostGreSqlQueryGenerator<SinglePrimaryKeyAggregate>
		CreateSinglePrimaryKeyAggregateWithCustomSchemaQueryGenerator()
	{
		var configuration = new TableAggregateConfiguration<SinglePrimaryKeyAggregate>
		{
			Schema = "account", TableName = "Users"
		};
		configuration.HasKey(aggregate => aggregate.Id);
		configuration.HasIdentity(aggregate => aggregate.Id);
		var generator = new PostGreSqlQueryGenerator<SinglePrimaryKeyAggregate>(configuration);
		return generator;
	}

	private static PostGreSqlQueryGenerator<CompositePrimaryKeyAggregate>
		CreateCompositePrimaryKeyAggregateQueryGenerator()
	{
		var configuration = new TableAggregateConfiguration<CompositePrimaryKeyAggregate>
		{
			Schema = "public", TableName = "Users"
		};
		configuration.HasKey(aggregate => new { aggregate.Username, aggregate.Password });
		var generator = new PostGreSqlQueryGenerator<CompositePrimaryKeyAggregate>(configuration);
		return generator;
	}

	private static PostGreSqlQueryGenerator<UserAggregate> CreateUserAggregateQueryGenerator()
	{
		var config = new TableAggregateConfiguration<UserAggregate> { Schema = "public", TableName = "Users" };
		config.HasKey(x => x.Id);
		var generator = new PostGreSqlQueryGenerator<UserAggregate>(config);
		return generator;
	}

	private static PostGreSqlQueryGenerator<UserAggregate> CreateUserAggregateIgnoreDeliveryAddressQueryGenerator()
	{
		var config = new TableAggregateConfiguration<UserAggregate> { Schema = "public", TableName = "Users" };
		config.HasKey(x => x.Id);
		config.Ignore(x => x.DeliveryAddress);
		var generator = new PostGreSqlQueryGenerator<UserAggregate>(config);
		return generator;
	}

	private static PostGreSqlQueryGenerator<AggregateWithValueObjectId> CreateAggregateWithValueObjectIdQueryGenerator()
	{
		var config = new TableAggregateConfiguration<AggregateWithValueObjectId>
		{
			Schema = "public", TableName = "Users"
		};
		config.HasKey(x => x.Id);
		var generator = new PostGreSqlQueryGenerator<AggregateWithValueObjectId>(config);
		return generator;
	}

	private static PostGreSqlQueryGenerator<AggregateWithNestedValueObject> CreateAggregateWithNestedValueObjectGenerator()
	{
		var defaultConfig = new DefaultConfiguration();
		var config = new TableAggregateConfiguration<AggregateWithNestedValueObject>
		{
			Schema = "public", TableName = "Users"
		};
		config.HasKey(x => x.Id);
		config.SetDefaults(defaultConfig);
		var generator = new PostGreSqlQueryGenerator<AggregateWithNestedValueObject>(config);
		return generator;
	}
	#endregion
}