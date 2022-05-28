using System;
using Dapper.Repository.Configuration;
using Dapper.Repository.Sql;
using Dapper.Repository.UnitTests.Aggregates;
using Xunit;

namespace Dapper.Repository.UnitTests.Sql
{
	public class QueryGeneratorTests
	{
		#region Constructor
		[Fact]
		public void Constructor_TableNameIsNull_Throws()
		{
			// Arrange
			var configuration = new AggregateConfiguration<SinglePrimaryKeyAggregate>()
			{
				Schema = "dbo",
				TableName = null
			};

			// Act && assert
			Assert.Throws<ArgumentNullException>(() => new SqlQueryGenerator<SinglePrimaryKeyAggregate>(configuration));
		}

		[Fact]
		public void Constructor_SchemaIsNull_Throws()
		{
			// Arrange
			var configuration = new AggregateConfiguration<SinglePrimaryKeyAggregate>()
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
			var configuration = new AggregateConfiguration<SinglePrimaryKeyAggregate>()
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
			var configuration = new AggregateConfiguration<SinglePrimaryKeyAggregate>()
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

		#region Insert
		[Fact]
		public void GenerateInsertQuery_CustomSchema_Valid()
		{
			// Arrange
			SqlQueryGenerator<SinglePrimaryKeyAggregate> generator = CreateSinglePrimaryKeyAggregateWithCustomSchemaQueryGenerator();

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
		public void GenerateInsertQuery_identityValuePrimaryKey_Valid()
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

		#region GetAll
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
		public void GenerateGetQuery_HasValueObject_Valid()
		{
			var config = new AggregateConfiguration<UserAggregate>()
			{
				Schema = "dbo",
				TableName = "Users"
			};
			config.HasKey(x => x.Id);
			config.HasValueObject(x => x.Address);
			var generator = new SqlQueryGenerator<UserAggregate>(config);

			// Act
			var query = generator.GenerateGetQuery();

			// TODO: Create GetAll, Delete, Insert and Update query tests as well
			// Assert
			Assert.Equal($"SELECT [dbo].[Users].[Id], [dbo].[Users].[Address_City], [dbo].[Users].[Address_Street] FROM [dbo].[Users] WHERE [dbo].[Users].[Id] = @Id;", query);
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

		#region Update

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
			var configuration = new AggregateConfiguration<AllPropertiesHasMissingSetterAggregate>()
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
			var configuration = new AggregateConfiguration<PropertyHasMissingSetterAggregate>()
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
			var configuration = new AggregateConfiguration<HasDefaultConstraintAggregate>()
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
			var configuration = new AggregateConfiguration<SinglePrimaryKeyAggregate>()
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
			var configuration = new AggregateConfiguration<SinglePrimaryKeyAggregate>()
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
			var configuration = new AggregateConfiguration<CompositePrimaryKeyAggregate>()
			{
				Schema = "dbo",
				TableName = "Users",

			};
			configuration.HasKey(aggregate => new { aggregate.Username, aggregate.Password });
			var generator = new SqlQueryGenerator<CompositePrimaryKeyAggregate>(configuration);
			return generator;
		}
		#endregion
	}
}
