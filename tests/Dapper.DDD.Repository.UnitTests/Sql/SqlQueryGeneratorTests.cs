using Dapper.DDD.Repository.Sql;
using Dapper.DDD.Repository.UnitTests.Aggregates;
using Dapper.DDD.Repository.UnitTests.ValueObjects;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace Dapper.DDD.Repository.UnitTests.Sql;

public class QueryGeneratorTests
{
	#region Constructor

	[Fact]
	public void Constructor_TableNameIsNull_Throws()
	{
		// Arrange
		var configuration = new TableAggregateConfiguration<SinglePrimaryKeyAggregate>
		{
			Schema = "dbo", TableName = null!
		};

		// Act && assert
		Assert.Throws<ArgumentNullException>(() => new SqlQueryGenerator<SinglePrimaryKeyAggregate>(configuration));
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
		Assert.Throws<ArgumentNullException>(() => new SqlQueryGenerator<SinglePrimaryKeyAggregate>(configuration));
	}

	[Fact]
	public void Constructor_TableNameIsWhitespace_Throws()
	{
		// Arrange
		var configuration = new TableAggregateConfiguration<SinglePrimaryKeyAggregate>
		{
			Schema = "dbo", TableName = " "
		};
		// Act && assert
		Assert.Throws<ArgumentException>(() => new SqlQueryGenerator<SinglePrimaryKeyAggregate>(configuration));
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
		Assert.Throws<ArgumentException>(() => new SqlQueryGenerator<SinglePrimaryKeyAggregate>(configuration));
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
			"DELETE FROM [dbo].[Users] OUTPUT [deleted].[Id], [deleted].[FirstLevel_SecondLevel_Name] WHERE [dbo].[Users].[Id] = @Id;",
			query);
	}

	[Fact]
	public void GenerateDeleteQuery_HasSerializedType_Valid()
	{
		// Arrange
		var generator = CreateAggregateWithGeometryQueryGenerator();

		// Act
		var query = generator.GenerateDeleteQuery();

		// Assert
		Assert.Equal(
			"DELETE FROM [dbo].[Users] OUTPUT [deleted].[Id], ([deleted].[Area]).Serialize() AS [Area] WHERE [dbo].[Users].[Id] = @Id;",
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
			"DELETE FROM [dbo].[Users] OUTPUT [deleted].[Age], [deleted].[Id_Password], [deleted].[Id_Username] WHERE [dbo].[Users].[Id_Password] = @Id_Password AND [dbo].[Users].[Id_Username] = @Id_Username;",
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
			"DELETE FROM [dbo].[Users] OUTPUT [deleted].[Id], [deleted].[DeliveryAddress_City], [deleted].[DeliveryAddress_Street], [deleted].[InvoiceAddress_City], [deleted].[InvoiceAddress_Street] WHERE [dbo].[Users].[Id] = @Id;",
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
			"DELETE FROM [dbo].[Users] OUTPUT [deleted].[Id], [deleted].[InvoiceAddress_City], [deleted].[InvoiceAddress_Street] WHERE [dbo].[Users].[Id] = @Id;",
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
			"DELETE FROM [account].[Users] OUTPUT [deleted].[Id], [deleted].[Username], [deleted].[Password] WHERE [account].[Users].[Id] = @Id;",
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
			"DELETE FROM [dbo].[Users] OUTPUT [deleted].[Id], [deleted].[Username], [deleted].[Password] WHERE [dbo].[Users].[Id] = @Id;",
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
			"DELETE FROM [dbo].[Users] OUTPUT [deleted].[Username], [deleted].[Password], [deleted].[DateCreated] WHERE [dbo].[Users].[Username] = @Username AND [dbo].[Users].[Password] = @Password;",
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
		Assert.Equal("SELECT [dbo].[Users].[Id], [dbo].[Users].[FirstLevel_SecondLevel_Name] FROM [dbo].[Users];",
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
			"SELECT [dbo].[Users].[Age], [dbo].[Users].[Id_Password], [dbo].[Users].[Id_Username] FROM [dbo].[Users];",
			query);
	}

	[Fact]
	public void GenerateGetAllQuery_HasSerializedType_Valid()
	{
		// Arrange
		var generator = CreateAggregateWithGeometryQueryGenerator();

		// Act
		var query = generator.GenerateGetAllQuery();

		// Assert
		Assert.Equal("SELECT [dbo].[Users].[Id], ([dbo].[Users].[Area]).Serialize() AS [Area] FROM [dbo].[Users];",
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
			"SELECT [dbo].[Users].[Id], [dbo].[Users].[DeliveryAddress_City], [dbo].[Users].[DeliveryAddress_Street], [dbo].[Users].[InvoiceAddress_City], [dbo].[Users].[InvoiceAddress_Street] FROM [dbo].[Users];",
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
			"SELECT [dbo].[Users].[Id], [dbo].[Users].[InvoiceAddress_City], [dbo].[Users].[InvoiceAddress_Street] FROM [dbo].[Users];",
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
			"SELECT [dbo].[Users].[Id], [dbo].[Users].[Username], [dbo].[Users].[Password] FROM [dbo].[Users];",
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
			"SELECT [dbo].[Users].[Id], [dbo].[Users].[FirstLevel_SecondLevel_Name] FROM [dbo].[Users] WHERE [dbo].[Users].[Id] = @Id;",
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
			"SELECT [dbo].[Users].[Age], [dbo].[Users].[Id_Password], [dbo].[Users].[Id_Username] FROM [dbo].[Users] WHERE [dbo].[Users].[Id_Password] = @Id_Password AND [dbo].[Users].[Id_Username] = @Id_Username;",
			query);
	}

	[Fact]
	public void GenerateGetQuery_HasSerializedType_Valid()
	{
		// Arrange
		var generator = CreateAggregateWithGeometryQueryGenerator();

		// Act
		var query = generator.GenerateGetQuery();

		// Assert
		Assert.Equal(
			"SELECT [dbo].[Users].[Id], ([dbo].[Users].[Area]).Serialize() AS [Area] FROM [dbo].[Users] WHERE [dbo].[Users].[Id] = @Id;",
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
			"SELECT [dbo].[Users].[Id], [dbo].[Users].[DeliveryAddress_City], [dbo].[Users].[DeliveryAddress_Street], [dbo].[Users].[InvoiceAddress_City], [dbo].[Users].[InvoiceAddress_Street] FROM [dbo].[Users] WHERE [dbo].[Users].[Id] = @Id;",
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
			"SELECT [dbo].[Users].[Id], [dbo].[Users].[InvoiceAddress_City], [dbo].[Users].[InvoiceAddress_Street] FROM [dbo].[Users] WHERE [dbo].[Users].[Id] = @Id;",
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
			"SELECT [dbo].[Users].[Id], [dbo].[Users].[Username], [dbo].[Users].[Password] FROM [dbo].[Users] WHERE [dbo].[Users].[Id] = @Id;",
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
			"SELECT [dbo].[Users].[Username], [dbo].[Users].[Password], [dbo].[Users].[DateCreated] FROM [dbo].[Users] WHERE [dbo].[Users].[Username] = @Username AND [dbo].[Users].[Password] = @Password;",
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
			"INSERT INTO [dbo].[Users] ([Id], [FirstLevel_SecondLevel_Name]) OUTPUT [inserted].[Id], [inserted].[FirstLevel_SecondLevel_Name] VALUES (@Id, @FirstLevel_SecondLevel_Name);",
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
			"INSERT INTO [dbo].[Users] ([Age], [Id_Password], [Id_Username]) OUTPUT [inserted].[Age], [inserted].[Id_Password], [inserted].[Id_Username] VALUES (@Age, @Id_Password, @Id_Username);",
			query);
	}

	[Fact]
	public void GenerateInsertQuery_HasSerializedType_Valid()
	{
		// Arrange
		var generator = CreateAggregateWithGeometryQueryGenerator();

		// Act
		var query = generator.GenerateInsertQuery(new AggregateWithGeometry());

		// Assert
		Assert.Equal(
			"INSERT INTO [dbo].[Users] ([Id], [Area]) OUTPUT [inserted].[Id], ([inserted].[Area]).Serialize() AS [Area] VALUES (@Id, @Area);",
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
			"INSERT INTO [dbo].[Users] ([Id], [DeliveryAddress_City], [DeliveryAddress_Street], [InvoiceAddress_City], [InvoiceAddress_Street]) OUTPUT [inserted].[Id], [inserted].[DeliveryAddress_City], [inserted].[DeliveryAddress_Street], [inserted].[InvoiceAddress_City], [inserted].[InvoiceAddress_Street] VALUES (@Id, @DeliveryAddress_City, @DeliveryAddress_Street, @InvoiceAddress_City, @InvoiceAddress_Street);",
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
			"INSERT INTO [dbo].[Users] ([Id], [InvoiceAddress_City], [InvoiceAddress_Street]) OUTPUT [inserted].[Id], [inserted].[InvoiceAddress_City], [inserted].[InvoiceAddress_Street] VALUES (@Id, @InvoiceAddress_City, @InvoiceAddress_Street);",
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
			"INSERT INTO [account].[Users] ([Username], [Password]) OUTPUT [inserted].[Id], [inserted].[Username], [inserted].[Password] VALUES (@Username, @Password);",
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
			"INSERT INTO [dbo].[Users] ([Id]) OUTPUT [inserted].[Id], [inserted].[DateCreated] VALUES (@Id);",
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
			"INSERT INTO [dbo].[Users] ([Id], [DateCreated]) OUTPUT [inserted].[Id], [inserted].[DateCreated] VALUES (@Id, @DateCreated);",
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
			"INSERT INTO [dbo].[Users] ([Username], [Password]) OUTPUT [inserted].[Id], [inserted].[Username], [inserted].[Password] VALUES (@Username, @Password);",
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
			"INSERT INTO [dbo].[Users] ([Username], [Password], [DateCreated]) OUTPUT [inserted].[Username], [inserted].[Password], [inserted].[DateCreated] VALUES (@Username, @Password, @DateCreated);",
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
			"INSERT INTO [dbo].[Users] ([Username], [Password], [DateCreated]) OUTPUT [inserted].[Username], [inserted].[Password], [inserted].[DateCreated] VALUES (@Username, @Password, @DateCreated);",
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
			"UPDATE [dbo].[Users] SET [dbo].[Users].[FirstLevel_SecondLevel_Name] = @FirstLevel_SecondLevel_Name OUTPUT [inserted].[Id], [inserted].[FirstLevel_SecondLevel_Name] WHERE [dbo].[Users].[Id] = @Id;",
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
			"UPDATE [dbo].[Users] SET [dbo].[Users].[Age] = @Age OUTPUT [inserted].[Age], [inserted].[Id_Password], [inserted].[Id_Username] WHERE [dbo].[Users].[Id_Password] = @Id_Password AND [dbo].[Users].[Id_Username] = @Id_Username;",
			query);
	}

	[Fact]
	public void GenerateUpdateQuery_HasSerializedType_Valid()
	{
		// Arrange
		var generator = CreateAggregateWithGeometryQueryGenerator();

		// Act
		var query = generator.GenerateUpdateQuery(new AggregateWithGeometry());

		// Assert
		Assert.Equal(
			"UPDATE [dbo].[Users] SET [dbo].[Users].[Area] = @Area OUTPUT [inserted].[Id], ([inserted].[Area]).Serialize() AS [Area] WHERE [dbo].[Users].[Id] = @Id;",
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
			"UPDATE [dbo].[Users] SET [dbo].[Users].[DeliveryAddress_City] = @DeliveryAddress_City, [dbo].[Users].[DeliveryAddress_Street] = @DeliveryAddress_Street, [dbo].[Users].[InvoiceAddress_City] = @InvoiceAddress_City, [dbo].[Users].[InvoiceAddress_Street] = @InvoiceAddress_Street OUTPUT [inserted].[Id], [inserted].[DeliveryAddress_City], [inserted].[DeliveryAddress_Street], [inserted].[InvoiceAddress_City], [inserted].[InvoiceAddress_Street] WHERE [dbo].[Users].[Id] = @Id;",
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
			"UPDATE [dbo].[Users] SET [dbo].[Users].[InvoiceAddress_City] = @InvoiceAddress_City, [dbo].[Users].[InvoiceAddress_Street] = @InvoiceAddress_Street OUTPUT [inserted].[Id], [inserted].[InvoiceAddress_City], [inserted].[InvoiceAddress_Street] WHERE [dbo].[Users].[Id] = @Id;",
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
			"UPDATE [dbo].[Users] SET [dbo].[Users].[Username] = @Username, [dbo].[Users].[Password] = @Password OUTPUT [inserted].[Id], [inserted].[Username], [inserted].[Password] WHERE [dbo].[Users].[Id] = @Id;",
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
			"UPDATE [dbo].[Users] SET [dbo].[Users].[DateCreated] = @DateCreated OUTPUT [inserted].[Username], [inserted].[Password], [inserted].[DateCreated] WHERE [dbo].[Users].[Username] = @Username AND [dbo].[Users].[Password] = @Password;",
			updateQuery);
	}

	[Fact]
	public void GenerateUpdateQuery_AllPropertiesHasNoSetter_Throws()
	{
		// Arrange
		var configuration = new TableAggregateConfiguration<AllPropertiesHasMissingSetterAggregate>
		{
			Schema = "dbo", TableName = "Users"
		};
		configuration.HasKey(aggregate => aggregate.Id);
		configuration.HasDefault(aggregate => aggregate.DateCreated);
		var generator = new SqlQueryGenerator<AllPropertiesHasMissingSetterAggregate>(configuration);

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
			Schema = "dbo", TableName = "Users"
		};
		configuration.HasKey(aggregate => aggregate.Id);
		configuration.HasDefault(aggregate => aggregate.DateCreated);
		var generator = new SqlQueryGenerator<AggregateWithDefaultConstraint>(configuration);

		// Act
		var query = generator.GenerateUpdateQuery(new AggregateWithDefaultConstraint());

		// Assert
		Assert.Equal(
			"UPDATE [dbo].[Users] SET [dbo].[Users].[Age] = @Age OUTPUT [inserted].[Id], [inserted].[Age], [inserted].[DateCreated] WHERE [dbo].[Users].[Id] = @Id;",
			query);
	}

	#endregion

	#region Constructors

	private static SqlQueryGenerator<HasDefaultConstraintAggregate>
		CreateHasDefaultConstraintAggregateQueryGenerator()
	{
		var configuration = new TableAggregateConfiguration<HasDefaultConstraintAggregate>
		{
			Schema = "dbo", TableName = "Users"
		};
		configuration.HasKey(aggregate => aggregate.Id);
		configuration.HasDefault(aggregate => aggregate.DateCreated);
		var generator = new SqlQueryGenerator<HasDefaultConstraintAggregate>(configuration);
		return generator;
	}

	private static SqlQueryGenerator<SinglePrimaryKeyAggregate> CreateSinglePrimaryKeyAggregateQueryGenerator()
	{
		var configuration = new TableAggregateConfiguration<SinglePrimaryKeyAggregate>
		{
			Schema = "dbo", TableName = "Users"
		};
		configuration.HasKey(aggregate => aggregate.Id);
		configuration.HasIdentity(aggregate => aggregate.Id);
		var generator = new SqlQueryGenerator<SinglePrimaryKeyAggregate>(configuration);
		return generator;
	}

	private static SqlQueryGenerator<SinglePrimaryKeyAggregate>
		CreateSinglePrimaryKeyAggregateWithCustomSchemaQueryGenerator()
	{
		var configuration = new TableAggregateConfiguration<SinglePrimaryKeyAggregate>
		{
			Schema = "account", TableName = "Users"
		};
		configuration.HasKey(aggregate => aggregate.Id);
		configuration.HasIdentity(aggregate => aggregate.Id);
		var generator = new SqlQueryGenerator<SinglePrimaryKeyAggregate>(configuration);
		return generator;
	}

	private static SqlQueryGenerator<CompositePrimaryKeyAggregate>
		CreateCompositePrimaryKeyAggregateQueryGenerator()
	{
		var configuration = new TableAggregateConfiguration<CompositePrimaryKeyAggregate>
		{
			Schema = "dbo", TableName = "Users"
		};
		configuration.HasKey(aggregate => new { aggregate.Username, aggregate.Password });
		var generator = new SqlQueryGenerator<CompositePrimaryKeyAggregate>(configuration);
		return generator;
	}

	private static SqlQueryGenerator<UserAggregate> CreateUserAggregateQueryGenerator()
	{
		var config = new TableAggregateConfiguration<UserAggregate> { Schema = "dbo", TableName = "Users" };
		config.HasKey(x => x.Id);
		var generator = new SqlQueryGenerator<UserAggregate>(config);
		return generator;
	}

	private static SqlQueryGenerator<UserAggregate> CreateUserAggregateIgnoreDeliveryAddressQueryGenerator()
	{
		var config = new TableAggregateConfiguration<UserAggregate> { Schema = "dbo", TableName = "Users" };
		config.HasKey(x => x.Id);
		config.Ignore(x => x.DeliveryAddress);
		var generator = new SqlQueryGenerator<UserAggregate>(config);
		return generator;
	}

	private static SqlQueryGenerator<AggregateWithValueObjectId> CreateAggregateWithValueObjectIdQueryGenerator()
	{
		var config = new TableAggregateConfiguration<AggregateWithValueObjectId>
		{
			Schema = "dbo", TableName = "Users"
		};
		config.HasKey(x => x.Id);
		var generator = new SqlQueryGenerator<AggregateWithValueObjectId>(config);
		return generator;
	}

	private static SqlQueryGenerator<AggregateWithGeometry> CreateAggregateWithGeometryQueryGenerator()
	{
		var defaultConfig = new DefaultConfiguration();
		defaultConfig.AddTypeConverter<Polygon, byte[]>(
			geo => new SqlServerBytesWriter { IsGeography = false }.Write(geo),
			bytes => (Polygon)new SqlServerBytesReader { IsGeography = false }.Read(bytes));
		var config = new TableAggregateConfiguration<AggregateWithGeometry> { Schema = "dbo", TableName = "Users" };
		config.HasKey(x => x.Id);
		config.SetDefaults(defaultConfig);
		Predicate<Type> polygonPredicate = type => type == typeof(Polygon);
		var generator = new SqlQueryGenerator<AggregateWithGeometry>(config, new[] { polygonPredicate });
		return generator;
	}

	private static SqlQueryGenerator<AggregateWithNestedValueObject> CreateAggregateWithNestedValueObjectGenerator()
	{
		var defaultConfig = new DefaultConfiguration();
		var config = new TableAggregateConfiguration<AggregateWithNestedValueObject>
		{
			Schema = "dbo", TableName = "Users"
		};
		config.HasKey(x => x.Id);
		config.SetDefaults(defaultConfig);
		var generator = new SqlQueryGenerator<AggregateWithNestedValueObject>(config);
		return generator;
	}

	#endregion
}