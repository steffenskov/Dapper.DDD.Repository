using System;
using Dapper.Repository.MySql;
using Dapper.Repository.UnitTests.Entities;
using Xunit;

namespace Dapper.Repository.UnitTests.MySql
{
	public class QueryGeneratorTests
	{
		#region Constructor

		[Fact]
		public void Constructor_TableNameIsNull_Throws()
		{
			// Arrange, Act && Assert
			Assert.Throws<ArgumentNullException>(() => new MySqlQueryGenerator<HeapEntity>(null!));
		}


		[Fact]
		public void Constructor_TableNameIsWhiteSpace_Throws()
		{
			// Arrange, Act && Assert
			Assert.Throws<ArgumentException>(() => new MySqlQueryGenerator<HeapEntity>(" "));
		}
		#endregion

		#region Delete

		[Fact]
		public void GenerateDeleteQuery_OnePrimaryKey_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator<SinglePrimaryKeyEntity>("Users");

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
			var generator = new MySqlQueryGenerator<CompositePrimaryKeyEntity>("Users");

			// Act
			var deleteQuery = generator.GenerateDeleteQuery();

			// Assert
			Assert.Equal($@"SELECT Users.Username, Users.Password, Users.DateCreated FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;
DELETE FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;", deleteQuery);
		}

		[Fact]
		public void GenerateDeleteQuery_CustomColumnNames_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator<CustomColumnNamesEntity>("Orders");

			// Act
			var deleteQuery = generator.GenerateDeleteQuery();

			// Assert
			Assert.Equal($@"SELECT Orders.OrderId AS Id, Orders.DateCreated AS Date FROM Orders WHERE Orders.OrderId = @Id;
DELETE FROM Orders WHERE Orders.OrderId = @Id;", deleteQuery);
		}

		[Fact]
		public void GenerateDeleteQuery_NoPrimaryKey_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator<HeapEntity>("Users");

			// Act
			var deleteQuery = generator.GenerateDeleteQuery();

			// Assert
			Assert.Equal($@"SELECT Users.Username, Users.Password FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;
DELETE FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;", deleteQuery);
		}
		#endregion

		#region GetAll
		[Fact]
		public void GenerateGetAllQuery_ProperTableName_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator<HeapEntity>("Users");

			// Act
			var selectQuery = generator.GenerateGetAllQuery();

			// Assert
			Assert.Equal($"SELECT Users.Username, Users.Password FROM Users;", selectQuery);
		}

		[Fact]
		public void GenerateGetAllQuery_CustomColumnNames_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator<CustomColumnNamesEntity>("Orders");

			// Act
			var selectQuery = generator.GenerateGetAllQuery();

			// Assert
			Assert.Equal($"SELECT Orders.OrderId AS Id, Orders.DateCreated AS Date FROM Orders;", selectQuery);
		}
		#endregion

		#region Get
		[Fact]
		public void GenerateGetQuery_SinglePrimaryKey_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator<SinglePrimaryKeyEntity>("Users");

			// Act
			var selectQuery = generator.GenerateGetQuery();

			// Assert
			Assert.Equal($"SELECT Users.Id, Users.Username, Users.Password FROM Users WHERE Users.Id = @Id;", selectQuery);
		}

		[Fact]
		public void GenerateGetQuery_CompositePrimaryKey_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator<CompositePrimaryKeyEntity>("Users");

			// Act
			var selectQuery = generator.GenerateGetQuery();

			// Assert
			Assert.Equal($"SELECT Users.Username, Users.Password, Users.DateCreated FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;", selectQuery);
		}

		[Fact]
		public void GenerateGetQuery_CustomColumnNames_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator<CustomColumnNamesEntity>("Orders");

			// Act
			var selectQuery = generator.GenerateGetQuery();

			// Assert
			Assert.Equal($"SELECT Orders.OrderId AS Id, Orders.DateCreated AS Date FROM Orders WHERE Orders.OrderId = @Id;", selectQuery);
		}

		[Fact]
		public void GenerateGetQuery_NoPrimaryKey_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator<HeapEntity>("Users");

			// Act
			var query = generator.GenerateGetQuery();

			// Assert
			Assert.Equal("SELECT Users.Username, Users.Password FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;", query);
		}
		#endregion

		#region Insert
		[Fact]
		public void GenerateInsertQuery_ColumnHasDefaultConstraintAndDefaultValue_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator<HasDefaultConstraintEntity>("Users");

			// Actj
			var query = generator.GenerateInsertQuery(new HasDefaultConstraintEntity());

			// Assert
			Assert.Equal(@"INSERT INTO Users (Id) VALUES (@Id);
SELECT Users.Id, Users.DateCreated FROM Users WHERE Users.Id = @Id;", query);
		}

		[Fact]
		public void GenerateInsertQuery_ColumnHasDefaultConstraintAndNonDefaultValue_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator<HasDefaultConstraintEntity>("Users");
			var record = new HasDefaultConstraintEntity
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
		public void GenerateInsertQuery_IdentityValuePrimaryKey_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator<SinglePrimaryKeyEntity>("Users");

			// Act
			var insertQuery = generator.GenerateInsertQuery(new SinglePrimaryKeyEntity());

			// Assert
			Assert.Equal(@"INSERT INTO Users (Username, Password) VALUES (@Username, @Password);
SELECT Users.Id, Users.Username, Users.Password FROM Users WHERE Users.Id = LAST_INSERT_ID();", insertQuery);
		}

		[Fact]
		public void GenerateInsertQuery_MissingColumnValue_ContainsColumn()
		{
			// Arrange
			var generator = new MySqlQueryGenerator<CompositePrimaryKeyEntity>("Users");

			// Act
			var insertQuery = generator.GenerateInsertQuery(new CompositePrimaryKeyEntity());

			// Assert
			Assert.Equal(@"INSERT INTO Users (Username, Password, DateCreated) VALUES (@Username, @Password, @DateCreated);
SELECT Users.Username, Users.Password, Users.DateCreated FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;", insertQuery);
		}

		[Fact]
		public void GenerateInsertQuery_CompositePrimaryKey_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator<CompositePrimaryKeyEntity>("Users");

			// Act
			var insertQuery = generator.GenerateInsertQuery(new CompositePrimaryKeyEntity());

			// Assert
			Assert.Equal(@"INSERT INTO Users (Username, Password, DateCreated) VALUES (@Username, @Password, @DateCreated);
SELECT Users.Username, Users.Password, Users.DateCreated FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;", insertQuery);
		}

		[Fact]
		public void GenerateInsertQuery_CustomColumnNames_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator<CustomColumnNamesEntity>("Orders");

			// Act
			var insertQuery = generator.GenerateInsertQuery(new CustomColumnNamesEntity());

			// Assert
			Assert.Equal(@"INSERT INTO Orders (DateCreated) VALUES (@Date);
SELECT Orders.OrderId AS Id, Orders.DateCreated AS Date FROM Orders WHERE Orders.OrderId = LAST_INSERT_ID();", insertQuery);
		}

		[Fact]
		public void GenerateInsertQuery_NoPrimaryKey_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator<HeapEntity>("Users");

			// Act
			var insertQuery = generator.GenerateInsertQuery(new HeapEntity());

			// Assert
			Assert.Equal(@"INSERT INTO Users (Username, Password) VALUES (@Username, @Password);
SELECT Users.Username, Users.Password FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;", insertQuery);
		}
		#endregion

		#region Update

		[Fact]
		public void GenerateUpdateQuery_SinglePrimaryKey_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator<SinglePrimaryKeyEntity>("Users");

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
			var generator = new MySqlQueryGenerator<CompositePrimaryKeyEntity>("Users");

			// Act 
			var updateQuery = generator.GenerateUpdateQuery();

			// Assert
			Assert.Equal($@"UPDATE Users SET DateCreated = @DateCreated WHERE Users.Username = @Username AND Users.Password = @Password;
SELECT Users.Username, Users.Password, Users.DateCreated FROM Users WHERE Users.Username = @Username AND Users.Password = @Password;", updateQuery);
		}

		[Fact]
		public void GenerateUpdateQuery_CustomColumnNames_Valid()
		{
			// Arrange
			var generator = new MySqlQueryGenerator<CustomColumnNamesEntity>("Orders");

			// Act 
			var updateQuery = generator.GenerateUpdateQuery();

			// Assert
			Assert.Equal($@"UPDATE Orders SET DateCreated = @Date WHERE Orders.OrderId = @Id;
SELECT Orders.OrderId AS Id, Orders.DateCreated AS Date FROM Orders WHERE Orders.OrderId = @Id;", updateQuery);
		}

		[Fact]
		public void GenerateUpdateQuery_NoPrimaryKey_Throws()
		{
			// Arrange
			var generator = new MySqlQueryGenerator<HeapEntity>("Users");

			// Act && Assert
			Assert.Throws<InvalidOperationException>(() => generator.GenerateUpdateQuery());
		}

		[Fact]
		public void GenerateUpdateQuery_AllColumnsHasNoSetter_Throws()
		{
			// Arrange
			var generator = new MySqlQueryGenerator<AllColumnsHasMissingSetterEntity>("Users");

			// Act && Assert
			Assert.Throws<InvalidOperationException>(() => generator.GenerateUpdateQuery());
		}

		[Fact]
		public void GenerateUpdateQuery_ColumnHasNoSetter_ColumnIsExcluded()
		{
			// Arrange
			var generator = new MySqlQueryGenerator<ColumnHasMissingSetterEntity>("Users");

			// Act
			var query = generator.GenerateUpdateQuery();

			// Assert
			Assert.Equal(@"UPDATE Users SET Age = @Age WHERE Users.Id = @Id;
SELECT Users.Id, Users.Age, Users.DateCreated FROM Users WHERE Users.Id = @Id;", query);
		}
		#endregion
	}
}
