using System;
using Dapper.Repository.Configuration;
using Dapper.Repository.MySql;
using Dapper.Repository.UnitTests.Aggregates;
using Xunit;

namespace Dapper.Repository.UnitTests.MySql
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
				TableName = null!
			};

			// Act && Assert
			Assert.Throws<ArgumentNullException>(() => new MySqlQueryGenerator<SinglePrimaryKeyAggregate>(configuration));
		}

		[Fact]
		public void Constructor_TableNameIsWhiteSpace_Throws()
		{
			// Arrange
			var configuration = new AggregateConfiguration<SinglePrimaryKeyAggregate>()
			{
				TableName = " "
			};
			// Act && Assert
			Assert.Throws<ArgumentException>(() => new MySqlQueryGenerator<SinglePrimaryKeyAggregate>(configuration));
		}
		#endregion

		#region Delete

		[Fact]
		public void GenerateDeleteQuery_OnePrimaryKey_Valid()
		{
			// Arrange
			var generator = CreateSinglePrimaryKeyAggregateQueryGenerator();

			// Act
			var deleteQuery = generator.GenerateDeleteQuery();

			// Assert
			Assert.Equal($@"SELECT Users.Id, Users.Username, Users.Password FROM Users WHERE Users.Id = @Id;
DELETE FROM Users WHERE Users.Id = @Id;", deleteQuery);
		}

		[Fact]
		public void GenerateDeleteQuery_CompositePrimaryKey_Valid()
		{
			// Arrange
			var generator = CreateCompositePrimaryKeyAggregateQueryGenerator();

			// Act
			var deleteQuery = generator.GenerateDeleteQuery();

			// Assert
			Assert.Equal($@"SELECT Users.Username, Users.Password, Users.DateCreated FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;
DELETE FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;", deleteQuery);
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
			Assert.Equal($"SELECT Users.Id, Users.Username, Users.Password FROM Users;", selectQuery);
		}

		#endregion

		#region Get
		[Fact]
		public void GenerateGetQuery_SinglePrimaryKey_Valid()
		{
			// Arrange
			var generator = CreateSinglePrimaryKeyAggregateQueryGenerator();

			// Act
			var selectQuery = generator.GenerateGetQuery();

			// Assert
			Assert.Equal($"SELECT Users.Id, Users.Username, Users.Password FROM Users WHERE Users.Id = @Id;", selectQuery);
		}

		[Fact]
		public void GenerateGetQuery_CompositePrimaryKey_Valid()
		{
			// Arrange
			var generator = CreateCompositePrimaryKeyAggregateQueryGenerator();

			// Act
			var selectQuery = generator.GenerateGetQuery();

			// Assert
			Assert.Equal($"SELECT Users.Username, Users.Password, Users.DateCreated FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;", selectQuery);
		}
		#endregion

		#region Insert
		[Fact]
		public void GenerateInsertQuery_AggregateHasMultipleIdentities_Invalid()
		{
			// Arrange
			var configuration = new AggregateConfiguration<HasMultipleIdentiesAggregate>()
			{
				TableName = "Users"
			};
			configuration.HasKey(x => x.Id);
			configuration.HasIdentity(x => x.Id);
			configuration.HasIdentity(x => x.Counter);
			var generator = new MySqlQueryGenerator<HasMultipleIdentiesAggregate>(configuration);

			var aggregate = new HasMultipleIdentiesAggregate();

			// Act && Assert
			var ex = Assert.Throws<InvalidOperationException>(() => generator.GenerateInsertQuery(aggregate));
			Assert.Equal("Cannot generate INSERT query for table with multiple identity properties", ex.Message);
		}

		[Fact]
		public void GenerateInsertQuery_PropertyHasDefaultConstraintAndDefaultValue_Valid()
		{
			// Arrange
			var generator = CreateHasDefaultConstraintAggregateQueryGenerator();

			// Actj
			var query = generator.GenerateInsertQuery(new HasDefaultConstraintAggregate());

			// Assert
			Assert.Equal(@"INSERT INTO Users (Id) VALUES (@Id);
SELECT Users.Id, Users.DateCreated FROM Users WHERE Users.Id = @Id;", query);
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
			Assert.Equal(@"INSERT INTO Users (Id, DateCreated) VALUES (@Id, @DateCreated);
SELECT Users.Id, Users.DateCreated FROM Users WHERE Users.Id = @Id;", query);
		}

		[Fact]
		public void GenerateInsertQuery_identityValuePrimaryKey_Valid()
		{
			// Arrange
			var generator = CreateSinglePrimaryKeyAggregateQueryGenerator();

			// Act
			var insertQuery = generator.GenerateInsertQuery(new SinglePrimaryKeyAggregate());

			// Assert
			Assert.Equal(@"INSERT INTO Users (Username, Password) VALUES (@Username, @Password);
SELECT Users.Id, Users.Username, Users.Password FROM Users WHERE Users.Id = LAST_INSERT_ID();", insertQuery);
		}

		[Fact]
		public void GenerateInsertQuery_MissingPropertyValue_ContainsProperty()
		{
			// Arrange
			var generator = CreateCompositePrimaryKeyAggregateQueryGenerator();

			// Act
			var insertQuery = generator.GenerateInsertQuery(new CompositePrimaryKeyAggregate());

			// Assert
			Assert.Equal(@"INSERT INTO Users (Username, Password, DateCreated) VALUES (@Username, @Password, @DateCreated);
SELECT Users.Username, Users.Password, Users.DateCreated FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;", insertQuery);
		}

		[Fact]
		public void GenerateInsertQuery_CompositePrimaryKey_Valid()
		{
			// Arrange
			var generator = CreateCompositePrimaryKeyAggregateQueryGenerator();

			// Act
			var insertQuery = generator.GenerateInsertQuery(new CompositePrimaryKeyAggregate());

			// Assert
			Assert.Equal(@"INSERT INTO Users (Username, Password, DateCreated) VALUES (@Username, @Password, @DateCreated);
SELECT Users.Username, Users.Password, Users.DateCreated FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;", insertQuery);
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
			Assert.Equal($@"UPDATE Users SET Username = @Username, Password = @Password WHERE Users.Id = @Id;
SELECT Users.Id, Users.Username, Users.Password FROM Users WHERE Users.Id = @Id;", updateQuery);
		}

		[Fact]
		public void GenerateUpdateQuery_CompositePrimaryKey_Valid()
		{
			// Arrange
			var generator = CreateCompositePrimaryKeyAggregateQueryGenerator();

			// Act 
			var updateQuery = generator.GenerateUpdateQuery();

			// Assert
			Assert.Equal($@"UPDATE Users SET DateCreated = @DateCreated WHERE Users.Username = @Username AND Users.Password = @Password;
SELECT Users.Username, Users.Password, Users.DateCreated FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;", updateQuery);
		}

		[Fact]
		public void GenerateUpdateQuery_AllPropertiesHasNoSetter_Throws()
		{
			// Arrange
			var configuration = new AggregateConfiguration<AllPropertiesHasMissingSetterAggregate>()
			{
				TableName = "Users"
			};
			configuration.HasKey(aggregate => aggregate.Id);
			configuration.HasDefault(aggregate => aggregate.DateCreated);
			var generator = new MySqlQueryGenerator<AllPropertiesHasMissingSetterAggregate>(configuration);

			// Act && Assert
			Assert.Throws<InvalidOperationException>(() => generator.GenerateUpdateQuery());
		}

		[Fact]
		public void GenerateUpdateQuery_PropertyHasNoSetter_PropertyIsExcluded()
		{
			// Arrange
			var configuration = new AggregateConfiguration<PropertyHasMissingSetterAggregate>()
			{
				TableName = "Users"
			};
			configuration.HasKey(aggregate => aggregate.Id);
			configuration.HasDefault(aggregate => aggregate.DateCreated);
			var generator = new MySqlQueryGenerator<PropertyHasMissingSetterAggregate>(configuration);

			// Act
			var query = generator.GenerateUpdateQuery();

			// Assert
			Assert.Equal(@"UPDATE Users SET Age = @Age WHERE Users.Id = @Id;
SELECT Users.Id, Users.Age, Users.DateCreated FROM Users WHERE Users.Id = @Id;", query);
		}
		#endregion


		#region Constructors
		private static MySqlQueryGenerator<HasDefaultConstraintAggregate> CreateHasDefaultConstraintAggregateQueryGenerator()
		{
			var configuration = new AggregateConfiguration<HasDefaultConstraintAggregate>()
			{
				TableName = "Users"
			};
			configuration.HasKey(aggregate => aggregate.Id);
			configuration.HasDefault(aggregate => aggregate.DateCreated);
			var generator = new MySqlQueryGenerator<HasDefaultConstraintAggregate>(configuration);
			return generator;
		}

		private static MySqlQueryGenerator<SinglePrimaryKeyAggregate> CreateSinglePrimaryKeyAggregateQueryGenerator()
		{
			var configuration = new AggregateConfiguration<SinglePrimaryKeyAggregate>()
			{
				TableName = "Users"
			};
			configuration.HasKey(aggregate => aggregate.Id);
			configuration.HasIdentity(aggregate => aggregate.Id);
			var generator = new MySqlQueryGenerator<SinglePrimaryKeyAggregate>(configuration);
			return generator;
		}

		private static MySqlQueryGenerator<CompositePrimaryKeyAggregate> CreateCompositePrimaryKeyAggregateQueryGenerator()
		{
			var configuration = new AggregateConfiguration<CompositePrimaryKeyAggregate>()
			{
				TableName = "Users",
			};
			configuration.HasKey(aggregate => new { aggregate.Username, aggregate.Password });
			var generator = new MySqlQueryGenerator<CompositePrimaryKeyAggregate>(configuration);
			return generator;
		}
		#endregion

	}
}
