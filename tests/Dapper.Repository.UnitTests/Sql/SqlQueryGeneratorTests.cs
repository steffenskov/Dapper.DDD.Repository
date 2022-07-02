using Dapper.Repository.Configuration;
using Dapper.Repository.Sql;
using Dapper.Repository.UnitTests.Aggregates;

namespace Dapper.Repository.UnitTests.Sql
{
	public class QueryGeneratorTests
	{
		#region Constructor
		[Fact]
		public void Constructor_TableNameIsNull_Throws()
		{
			// Arrange
			var configuration = new TableAggregateConfiguration<SinglePrimaryKeyAggregate>()
			{
				Schema = "dbo",
				TableName = null!
			};

			// Act && assert
			Assert.Throws<ArgumentNullException>(() => new SqlQueryGenerator<SinglePrimaryKeyAggregate>(configuration));
		}

		[Fact]
		public void Constructor_SchemaIsNull_Throws()
		{
			// Arrange
			var configuration = new TableAggregateConfiguration<SinglePrimaryKeyAggregate>()
			{
				Schema = null,
				TableName = "Users"
			};
			// Act && assert
			Assert.Throws<ArgumentNullException>(() => new SqlQueryGenerator<SinglePrimaryKeyAggregate>(configuration));
		}

		[Fact]
		public void Constructor_TableNameIsWhitespace_Throws()
		{
			// Arrange
			var configuration = new TableAggregateConfiguration<SinglePrimaryKeyAggregate>()
			{
				Schema = "dbo",
				TableName = " "
			};
			// Act && assert
			Assert.Throws<ArgumentException>(() => new SqlQueryGenerator<SinglePrimaryKeyAggregate>(configuration));
		}

		[Fact]
		public void Constructor_SchemaIsWhitespace_Throws()
		{
			// Arrange
			var configuration = new TableAggregateConfiguration<SinglePrimaryKeyAggregate>()
			{
				Schema = " ",
				TableName = "Users"
			};
			// Act && assert
			Assert.Throws<ArgumentException>(() => new SqlQueryGenerator<SinglePrimaryKeyAggregate>(configuration));
		}
		#endregion

		#region Delete
		[Fact]
		public void GenerateDeleteQuery_HasValueObjectAsId_Valid()
		{
			// Arrange
			var generator = CreateAggregateWithValueObjectIdQueryGenerator();

			// Act
			var query = generator.GenerateDeleteQuery();

			// Assert
			Assert.Equal($"DELETE FROM [dbo].[Users] OUTPUT [deleted].[Age], [deleted].[Id_Password], [deleted].[Id_Username] WHERE [dbo].[Users].[Id_Password] = @Id_Password AND [dbo].[Users].[Id_Username] = @Id_Username;", query);
		}

		[Fact]
		public void GenerateDeleteQuery_HasValueObject_Valid()
		{
			var generator = CreateUserAggregateQueryGenerator();

			// Act
			var query = generator.GenerateDeleteQuery();

			// Assert
			Assert.Equal($"DELETE FROM [dbo].[Users] OUTPUT [deleted].[Id], [deleted].[DeliveryAddress_City], [deleted].[DeliveryAddress_Street], [deleted].[InvoiceAddress_City], [deleted].[InvoiceAddress_Street] WHERE [dbo].[Users].[Id] = @Id;", query);
		}

		[Fact]
		public void GenerateDeleteQuery_CustomSchema_Valid()
		{
			// Arrange
			var generator = CreateSinglePrimaryKeyAggregateWithCustomSchemaQueryGenerator();

			// Act
			var query = generator.GenerateDeleteQuery();

			// Assert
			Assert.Equal("DELETE FROM [account].[Users] OUTPUT [deleted].[Id], [deleted].[Username], [deleted].[Password] WHERE [account].[Users].[Id] = @Id;", query);
		}

		[Fact]
		public void GenerateDeleteQuery_OnePrimaryKey_Valid()
		{
			// Arrange
			var generator = CreateSinglePrimaryKeyAggregateQueryGenerator();

			// Act
			var deleteQuery = generator.GenerateDeleteQuery();

			// Assert
			Assert.Equal($"DELETE FROM [dbo].[Users] OUTPUT [deleted].[Id], [deleted].[Username], [deleted].[Password] WHERE [dbo].[Users].[Id] = @Id;", deleteQuery);
		}



		[Fact]
		public void GenerateDeleteQuery_CompositePrimaryKey_Valid()
		{
			// Arrange
			var generator = CreateCompositePrimaryKeyAggregateQueryGenerator();

			// Act
			var deleteQuery = generator.GenerateDeleteQuery();

			// Assert
			Assert.Equal($"DELETE FROM [dbo].[Users] OUTPUT [deleted].[Username], [deleted].[Password], [deleted].[DateCreated] WHERE [dbo].[Users].[Username] = @Username AND [dbo].[Users].[Password] = @Password;", deleteQuery);
		}
		#endregion

		#region GetAll
		[Fact]
		public void GenerateGetAllQuery_HasValueObjectAsId_Valid()
		{
			// Arrange
			var generator = CreateAggregateWithValueObjectIdQueryGenerator();

			// Act
			var query = generator.GenerateGetAllQuery();

			// Assert
			Assert.Equal($"SELECT [dbo].[Users].[Age], [dbo].[Users].[Id_Password], [dbo].[Users].[Id_Username] FROM [dbo].[Users];", query);
		}

		[Fact]
		public void GenerateGetAllQuery_HasValueObject_Valid()
		{
			var generator = CreateUserAggregateQueryGenerator();

			// Act
			var query = generator.GenerateGetAllQuery();

			// Assert
			Assert.Equal($"SELECT [dbo].[Users].[Id], [dbo].[Users].[DeliveryAddress_City], [dbo].[Users].[DeliveryAddress_Street], [dbo].[Users].[InvoiceAddress_City], [dbo].[Users].[InvoiceAddress_Street] FROM [dbo].[Users];", query);
		}

		[Fact]
		public void GenerateGetAllQuery_ProperTableName_Valid()
		{
			// Arrange
			var generator = CreateSinglePrimaryKeyAggregateQueryGenerator();

			// Act
			var selectQuery = generator.GenerateGetAllQuery();

			// Assert
			Assert.Equal($"SELECT [dbo].[Users].[Id], [dbo].[Users].[Username], [dbo].[Users].[Password] FROM [dbo].[Users];", selectQuery);
		}

		#endregion

		#region Get
		[Fact]
		public void GenerateGetQuery_HasValueObjectAsId_Valid()
		{
			// Arrange
			var generator = CreateAggregateWithValueObjectIdQueryGenerator();

			// Act
			var query = generator.GenerateGetQuery();

			// Assert
			Assert.Equal($"SELECT [dbo].[Users].[Age], [dbo].[Users].[Id_Password], [dbo].[Users].[Id_Username] FROM [dbo].[Users] WHERE [dbo].[Users].[Id_Password] = @Id_Password AND [dbo].[Users].[Id_Username] = @Id_Username;", query);
		}

		[Fact]
		public void GenerateGetQuery_HasValueObject_Valid()
		{
			var generator = CreateUserAggregateQueryGenerator();

			// Act
			var query = generator.GenerateGetQuery();

			// Assert
			Assert.Equal($"SELECT [dbo].[Users].[Id], [dbo].[Users].[DeliveryAddress_City], [dbo].[Users].[DeliveryAddress_Street], [dbo].[Users].[InvoiceAddress_City], [dbo].[Users].[InvoiceAddress_Street] FROM [dbo].[Users] WHERE [dbo].[Users].[Id] = @Id;", query);
		}

		[Fact]
		public void GenerateGetQuery_SinglePrimaryKey_Valid()
		{
			// Arrange
			var generator = CreateSinglePrimaryKeyAggregateQueryGenerator();

			// Act
			var selectQuery = generator.GenerateGetQuery();

			// Assert
			Assert.Equal($"SELECT [dbo].[Users].[Id], [dbo].[Users].[Username], [dbo].[Users].[Password] FROM [dbo].[Users] WHERE [dbo].[Users].[Id] = @Id;", selectQuery);
		}

		[Fact]
		public void GenerateGetQuery_CompositePrimaryKey_Valid()
		{
			// Arrange
			var generator = CreateCompositePrimaryKeyAggregateQueryGenerator();

			// Act
			var selectQuery = generator.GenerateGetQuery();

			// Assert
			Assert.Equal($"SELECT [dbo].[Users].[Username], [dbo].[Users].[Password], [dbo].[Users].[DateCreated] FROM [dbo].[Users] WHERE [dbo].[Users].[Username] = @Username AND [dbo].[Users].[Password] = @Password;", selectQuery);
		}
		#endregion

		#region Insert
		[Fact]
		public void GenerateInsertQuery_HasValueObjectAsId_Valid()
		{
			// Arrange
			var generator = CreateAggregateWithValueObjectIdQueryGenerator();

			// Act
			var query = generator.GenerateInsertQuery(new AggregateWithValueObjectId());

			// Assert
			Assert.Equal($"INSERT INTO [dbo].[Users] ([Age], [Id_Password], [Id_Username]) OUTPUT [inserted].[Age], [inserted].[Id_Password], [inserted].[Id_Username] VALUES (@Age, @Id_Password, @Id_Username);", query);
		}

		[Fact]
		public void GenerateInsertQuery_HasValueObject_Valid()
		{
			// Arrange
			var generator = CreateUserAggregateQueryGenerator();

			// Act
			var query = generator.GenerateInsertQuery(new UserAggregate());

			// Assert
			Assert.Equal($"INSERT INTO [dbo].[Users] ([Id], [DeliveryAddress_City], [DeliveryAddress_Street], [InvoiceAddress_City], [InvoiceAddress_Street]) OUTPUT [inserted].[Id], [inserted].[DeliveryAddress_City], [inserted].[DeliveryAddress_Street], [inserted].[InvoiceAddress_City], [inserted].[InvoiceAddress_Street] VALUES (@Id, @DeliveryAddress_City, @DeliveryAddress_Street, @InvoiceAddress_City, @InvoiceAddress_Street);", query);
		}

		[Fact]
		public void GenerateInsertQuery_CustomSchema_Valid()
		{
			// Arrange
			var generator = CreateSinglePrimaryKeyAggregateWithCustomSchemaQueryGenerator();

			// Act
			var query = generator.GenerateInsertQuery(new SinglePrimaryKeyAggregate());

			// Assert
			Assert.Equal("INSERT INTO [account].[Users] ([Username], [Password]) OUTPUT [inserted].[Id], [inserted].[Username], [inserted].[Password] VALUES (@Username, @Password);", query);
		}

		[Fact]
		public void GenerateInsertQuery_PropertyHasDefaultConstraintAndDefaultValue_Valid()
		{
			// Arrange
			var generator = CreateHasDefaultConstraintAggregateQueryGenerator();

			// Actj
			var query = generator.GenerateInsertQuery(new HasDefaultConstraintAggregate());

			// Assert
			Assert.Equal("INSERT INTO [dbo].[Users] ([Id]) OUTPUT [inserted].[Id], [inserted].[DateCreated] VALUES (@Id);", query);
		}

		[Fact]
		public void GenerateInsertQuery_PropertyHasDefaultConstraintAndNonDefaultValue_Valid()
		{
			// Arrange
			var generator = CreateHasDefaultConstraintAggregateQueryGenerator();
			var record = new HasDefaultConstraintAggregate
			{
				Id = 42,
				DateCreated = DateTime.Now
			};

			// Act
			var query = generator.GenerateInsertQuery(record);

			// Assert
			Assert.Equal("INSERT INTO [dbo].[Users] ([Id], [DateCreated]) OUTPUT [inserted].[Id], [inserted].[DateCreated] VALUES (@Id, @DateCreated);", query);
		}

		[Fact]
		public void GenerateInsertQuery_IdentityValuePrimaryKey_Valid()
		{
			// Arrange
			var generator = CreateSinglePrimaryKeyAggregateQueryGenerator();

			// Act
			var insertQuery = generator.GenerateInsertQuery(new SinglePrimaryKeyAggregate());

			// Assert
			Assert.Equal($"INSERT INTO [dbo].[Users] ([Username], [Password]) OUTPUT [inserted].[Id], [inserted].[Username], [inserted].[Password] VALUES (@Username, @Password);", insertQuery);
		}

		[Fact]
		public void GenerateInsertQuery_MissingPropertyValue_ContainsProperty()
		{
			// Arrange
			var generator = CreateCompositePrimaryKeyAggregateQueryGenerator();

			// Act
			var insertQuery = generator.GenerateInsertQuery(new CompositePrimaryKeyAggregate());

			// Assert
			Assert.Equal($"INSERT INTO [dbo].[Users] ([Username], [Password], [DateCreated]) OUTPUT [inserted].[Username], [inserted].[Password], [inserted].[DateCreated] VALUES (@Username, @Password, @DateCreated);", insertQuery);
		}

		[Fact]
		public void GenerateInsertQuery_CompositePrimaryKey_Valid()
		{
			// Arrange
			var generator = CreateCompositePrimaryKeyAggregateQueryGenerator();

			// Act
			var insertQuery = generator.GenerateInsertQuery(new CompositePrimaryKeyAggregate());

			// Assert
			Assert.Equal($"INSERT INTO [dbo].[Users] ([Username], [Password], [DateCreated]) OUTPUT [inserted].[Username], [inserted].[Password], [inserted].[DateCreated] VALUES (@Username, @Password, @DateCreated);", insertQuery);
		}
		#endregion

		#region Update
		[Fact]
		public void GenerateUpdateQuery_HasValueObjectAsId_Valid()
		{
			// Arrange
			var generator = CreateAggregateWithValueObjectIdQueryGenerator();

			// Act
			var query = generator.GenerateUpdateQuery();

			// Assert
			Assert.Equal($"UPDATE [dbo].[Users] SET [dbo].[Users].[Age] = @Age OUTPUT [inserted].[Age], [inserted].[Id_Password], [inserted].[Id_Username] WHERE [dbo].[Users].[Id_Password] = @Id_Password AND [dbo].[Users].[Id_Username] = @Id_Username;", query);
		}

		[Fact]
		public void GenerateUpdateQuery_HasValueObject_Valid()
		{
			var generator = CreateUserAggregateQueryGenerator();

			// Act
			var query = generator.GenerateUpdateQuery();

			// Assert
			Assert.Equal($"UPDATE [dbo].[Users] SET [dbo].[Users].[DeliveryAddress_City] = @DeliveryAddress_City, [dbo].[Users].[DeliveryAddress_Street] = @DeliveryAddress_Street, [dbo].[Users].[InvoiceAddress_City] = @InvoiceAddress_City, [dbo].[Users].[InvoiceAddress_Street] = @InvoiceAddress_Street OUTPUT [inserted].[Id], [inserted].[DeliveryAddress_City], [inserted].[DeliveryAddress_Street], [inserted].[InvoiceAddress_City], [inserted].[InvoiceAddress_Street] WHERE [dbo].[Users].[Id] = @Id;", query);
		}

		[Fact]
		public void GenerateUpdateQuery_SinglePrimaryKey_Valid()
		{
			// Arrange
			var generator = CreateSinglePrimaryKeyAggregateQueryGenerator();

			// Act 
			var updateQuery = generator.GenerateUpdateQuery();

			// Assert
			Assert.Equal($"UPDATE [dbo].[Users] SET [dbo].[Users].[Username] = @Username, [dbo].[Users].[Password] = @Password OUTPUT [inserted].[Id], [inserted].[Username], [inserted].[Password] WHERE [dbo].[Users].[Id] = @Id;", updateQuery);
		}

		[Fact]
		public void GenerateUpdateQuery_CompositePrimaryKey_Valid()
		{
			// Arrange
			var generator = CreateCompositePrimaryKeyAggregateQueryGenerator();

			// Act 
			var updateQuery = generator.GenerateUpdateQuery();

			// Assert
			Assert.Equal($"UPDATE [dbo].[Users] SET [dbo].[Users].[DateCreated] = @DateCreated OUTPUT [inserted].[Username], [inserted].[Password], [inserted].[DateCreated] WHERE [dbo].[Users].[Username] = @Username AND [dbo].[Users].[Password] = @Password;", updateQuery);
		}
		[Fact]
		public void GenerateUpdateQuery_AllPropertiesHasNoSetter_Throws()
		{
			// Arrange
			var configuration = new TableAggregateConfiguration<AllPropertiesHasMissingSetterAggregate>()
			{
				Schema = "dbo",
				TableName = "Users"
			};
			configuration.HasKey(aggregate => aggregate.Id);
			configuration.HasDefault(aggregate => aggregate.DateCreated);
			var generator = new SqlQueryGenerator<AllPropertiesHasMissingSetterAggregate>(configuration);

			// Act && Assert
			Assert.Throws<InvalidOperationException>(() => generator.GenerateUpdateQuery());
		}

		[Fact]
		public void GenerateUpdateQuery_PropertyHasNoSetter_PropertyIsExcluded()
		{
			// Arrange
			var configuration = new TableAggregateConfiguration<PropertyHasMissingSetterAggregate>()
			{
				Schema = "dbo",
				TableName = "Users"
			};
			configuration.HasKey(aggregate => aggregate.Id);
			configuration.HasDefault(aggregate => aggregate.DateCreated);
			var generator = new SqlQueryGenerator<PropertyHasMissingSetterAggregate>(configuration);

			// Act
			var query = generator.GenerateUpdateQuery();

			// Assert
			Assert.Equal("UPDATE [dbo].[Users] SET [dbo].[Users].[Age] = @Age OUTPUT [inserted].[Id], [inserted].[Age], [inserted].[DateCreated] WHERE [dbo].[Users].[Id] = @Id;", query);
		}
		#endregion

		#region Constructors
		private static SqlQueryGenerator<HasDefaultConstraintAggregate> CreateHasDefaultConstraintAggregateQueryGenerator()
		{
			var configuration = new TableAggregateConfiguration<HasDefaultConstraintAggregate>()
			{
				Schema = "dbo",
				TableName = "Users"
			};
			configuration.HasKey(aggregate => aggregate.Id);
			configuration.HasDefault(aggregate => aggregate.DateCreated);
			var generator = new SqlQueryGenerator<HasDefaultConstraintAggregate>(configuration);
			return generator;
		}

		private static SqlQueryGenerator<SinglePrimaryKeyAggregate> CreateSinglePrimaryKeyAggregateQueryGenerator()
		{
			var configuration = new TableAggregateConfiguration<SinglePrimaryKeyAggregate>()
			{
				Schema = "dbo",
				TableName = "Users"
			};
			configuration.HasKey(aggregate => aggregate.Id);
			configuration.HasIdentity(aggregate => aggregate.Id);
			var generator = new SqlQueryGenerator<SinglePrimaryKeyAggregate>(configuration);
			return generator;
		}

		private static SqlQueryGenerator<SinglePrimaryKeyAggregate> CreateSinglePrimaryKeyAggregateWithCustomSchemaQueryGenerator()
		{
			var configuration = new TableAggregateConfiguration<SinglePrimaryKeyAggregate>()
			{
				Schema = "account",
				TableName = "Users"
			};
			configuration.HasKey(aggregate => aggregate.Id);
			configuration.HasIdentity(aggregate => aggregate.Id);
			var generator = new SqlQueryGenerator<SinglePrimaryKeyAggregate>(configuration);
			return generator;
		}

		private static SqlQueryGenerator<CompositePrimaryKeyAggregate> CreateCompositePrimaryKeyAggregateQueryGenerator()
		{
			var configuration = new TableAggregateConfiguration<CompositePrimaryKeyAggregate>()
			{
				Schema = "dbo",
				TableName = "Users",

			};
			configuration.HasKey(aggregate => new { aggregate.Username, aggregate.Password });
			var generator = new SqlQueryGenerator<CompositePrimaryKeyAggregate>(configuration);
			return generator;
		}

		private static SqlQueryGenerator<UserAggregate> CreateUserAggregateQueryGenerator()
		{
			var config = new TableAggregateConfiguration<UserAggregate>()
			{
				Schema = "dbo",
				TableName = "Users"
			};
			config.HasKey(x => x.Id);
			var generator = new SqlQueryGenerator<UserAggregate>(config);
			return generator;
		}

		private static SqlQueryGenerator<AggregateWithValueObjectId> CreateAggregateWithValueObjectIdQueryGenerator()
		{
			var config = new TableAggregateConfiguration<AggregateWithValueObjectId>()
			{
				Schema = "dbo",
				TableName = "Users"
			};
			config.HasKey(x => x.Id);
			var generator = new SqlQueryGenerator<AggregateWithValueObjectId>(config);
			return generator;
		}
		#endregion
	}
}
