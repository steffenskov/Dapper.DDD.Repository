using System;
using Dapper.Repository.Sql;
using Dapper.Repository.UnitTests.Entities;
using Xunit;

namespace Dapper.Repository.UnitTests.Sql
{
	public class QueryGeneratorTests
	{
		#region Constructor
		[Fact]
		public void Constructor_TableNameIsNull_Throws()
		{
			// Arrange, act && assert
			Assert.Throws<ArgumentNullException>(() => new SqlQueryGenerator<HeapEntity>("dbo", null!));
		}

		[Fact]
		public void Constructor_SchemaIsNull_Throws()
		{
			// Arrange, act && assert
			Assert.Throws<ArgumentNullException>(() => new SqlQueryGenerator<HeapEntity>(null!, "Users"));
		}

		[Fact]
		public void Constructor_TableNameIsWhitespace_Throws()
		{
			// Arrange, act && assert
			Assert.Throws<ArgumentException>(() => new SqlQueryGenerator<HeapEntity>("dbo", " "));
		}

		[Fact]
		public void Constructor_SchemaIsWhitespace_Throws()
		{
			// Arrange, act && assert
			Assert.Throws<ArgumentException>(() => new SqlQueryGenerator<HeapEntity>(" ", "Users"));
		}
		#endregion

		#region Delete


		[Fact]
		public void GenerateDeleteQuery_CustomSchema_Valid()
		{
			// Arrange
			var generator = new SqlQueryGenerator<SinglePrimaryKeyEntity>("account", "Users");

			// Act
			var query = generator.GenerateDeleteQuery();

			// Assert
			Assert.Equal("DELETE FROM [account].[Users] OUTPUT [deleted].[Id], [deleted].[Username], [deleted].[Password] WHERE [account].[Users].[Id] = @Id;", query);
		}

		[Fact]
		public void GenerateDeleteQuery_OnePrimaryKey_Valid()
		{
			// Arrange
			var generator = new SqlQueryGenerator<SinglePrimaryKeyEntity>("dbo", "Users");

			// Act
			var deleteQuery = generator.GenerateDeleteQuery();

			// Assert
			Assert.Equal($"DELETE FROM [dbo].[Users] OUTPUT [deleted].[Id], [deleted].[Username], [deleted].[Password] WHERE [dbo].[Users].[Id] = @Id;", deleteQuery);
		}

		[Fact]
		public void GenerateDeleteQuery_CompositePrimaryKey_Valid()
		{
			// Arrange
			var generator = new SqlQueryGenerator<CompositePrimaryKeyEntity>("dbo", "Users");

			// Act
			var deleteQuery = generator.GenerateDeleteQuery();

			// Assert
			Assert.Equal($"DELETE FROM [dbo].[Users] OUTPUT [deleted].[Username], [deleted].[Password], [deleted].[DateCreated] WHERE [dbo].[Users].[Username] = @Username AND [dbo].[Users].[Password] = @Password;", deleteQuery);
		}

		[Fact]
		public void GenerateDeleteQuery_CustomColumnNames_Valid()
		{
			// Arrange
			var generator = new SqlQueryGenerator<CustomColumnNamesEntity>("dbo", "Orders");

			// Act
			var deleteQuery = generator.GenerateDeleteQuery();

			// Assert
			Assert.Equal($"DELETE FROM [dbo].[Orders] OUTPUT [deleted].[OrderId] AS [Id], [deleted].[DateCreated] AS [Date] WHERE [dbo].[Orders].[OrderId] = @Id;", deleteQuery);
		}

		[Fact]
		public void GenerateDeleteQuery_NoPrimaryKey_Valid()
		{
			// Arrange
			var generator = new SqlQueryGenerator<HeapEntity>("dbo", "Users");

			// Act
			var deleteQuery = generator.GenerateDeleteQuery();

			// Assert
			Assert.Equal($"DELETE FROM [dbo].[Users] OUTPUT [deleted].[Username], [deleted].[Password] WHERE [dbo].[Users].[Username] = @Username AND [dbo].[Users].[Password] = @Password;", deleteQuery);
		}
		#endregion

		#region Insert
		[Fact]
		public void GenerateInsertQuery_CustomSchema_Valid()
		{
			// Arrange
			var generator = new SqlQueryGenerator<SinglePrimaryKeyEntity>("account", "Users");

			// Act
			var query = generator.GenerateInsertQuery(new SinglePrimaryKeyEntity());

			// Assert
			Assert.Equal("INSERT INTO [account].[Users] ([Username], [Password]) OUTPUT [inserted].[Id], [inserted].[Username], [inserted].[Password] VALUES (@Username, @Password);", query);
		}

		[Fact]
		public void GenerateInsertQuery_ColumnHasDefaultConstraintAndDefaultValue_Valid()
		{
			// Arrange
			var generator = new SqlQueryGenerator<HasDefaultConstraintEntity>("dbo", "Users");

			// Actj
			var query = generator.GenerateInsertQuery(new HasDefaultConstraintEntity());

			// Assert
			Assert.Equal("INSERT INTO [dbo].[Users] ([Id]) OUTPUT [inserted].[Id], [inserted].[DateCreated] VALUES (@Id);", query);
		}

		[Fact]
		public void GenerateInsertQuery_ColumnHasDefaultConstraintAndNonDefaultValue_Valid()
		{
			// Arrange
			var generator = new SqlQueryGenerator<HasDefaultConstraintEntity>("dbo", "Users");
			var record = new HasDefaultConstraintEntity
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
			var generator = new SqlQueryGenerator<SinglePrimaryKeyEntity>("dbo", "Users");

			// Act
			var insertQuery = generator.GenerateInsertQuery(new SinglePrimaryKeyEntity());

			// Assert
			Assert.Equal($"INSERT INTO [dbo].[Users] ([Username], [Password]) OUTPUT [inserted].[Id], [inserted].[Username], [inserted].[Password] VALUES (@Username, @Password);", insertQuery);
		}

		[Fact]
		public void GenerateInsertQuery_MissingColumnValue_ContainsColumn()
		{
			// Arrange
			var generator = new SqlQueryGenerator<CompositePrimaryKeyEntity>("dbo", "Users");

			// Act
			var insertQuery = generator.GenerateInsertQuery(new CompositePrimaryKeyEntity());

			// Assert
			Assert.Equal($"INSERT INTO [dbo].[Users] ([Username], [Password], [DateCreated]) OUTPUT [inserted].[Username], [inserted].[Password], [inserted].[DateCreated] VALUES (@Username, @Password, @DateCreated);", insertQuery);
		}

		[Fact]
		public void GenerateInsertQuery_CompositePrimaryKey_Valid()
		{
			// Arrange
			var generator = new SqlQueryGenerator<CompositePrimaryKeyEntity>("dbo", "Users");

			// Act
			var insertQuery = generator.GenerateInsertQuery(new CompositePrimaryKeyEntity());

			// Assert
			Assert.Equal($"INSERT INTO [dbo].[Users] ([Username], [Password], [DateCreated]) OUTPUT [inserted].[Username], [inserted].[Password], [inserted].[DateCreated] VALUES (@Username, @Password, @DateCreated);", insertQuery);
		}

		[Fact]
		public void GenerateInsertQuery_CustomColumnNames_Valid()
		{
			// Arrange
			var generator = new SqlQueryGenerator<CustomColumnNamesEntity>("dbo", "Orders");

			// Act
			var insertQuery = generator.GenerateInsertQuery(new CustomColumnNamesEntity());

			// Assert
			Assert.Equal($"INSERT INTO [dbo].[Orders] ([DateCreated]) OUTPUT [inserted].[OrderId] AS [Id], [inserted].[DateCreated] AS [Date] VALUES (@Date);", insertQuery);
		}

		[Fact]
		public void GenerateInsertQuery_NoPrimaryKey_Valid()
		{
			// Arrange
			var generator = new SqlQueryGenerator<HeapEntity>("dbo", "Users");

			// Act
			var insertQuery = generator.GenerateInsertQuery(new HeapEntity());

			// Assert
			Assert.Equal($"INSERT INTO [dbo].[Users] ([Username], [Password]) OUTPUT [inserted].[Username], [inserted].[Password] VALUES (@Username, @Password);", insertQuery);
		}
		#endregion

		#region GetAll
		[Fact]
		public void GenerateGetAllQuery_ProperTableName_Valid()
		{
			// Arrange
			var generator = new SqlQueryGenerator<HeapEntity>("dbo", "Users");

			// Act
			var selectQuery = generator.GenerateGetAllQuery();

			// Assert
			Assert.Equal($"SELECT [dbo].[Users].[Username], [dbo].[Users].[Password] FROM [dbo].[Users];", selectQuery);
		}

		[Fact]
		public void GenerateGetAllQuery_CustomColumnNames_Valid()
		{
			// Arrange
			var generator = new SqlQueryGenerator<CustomColumnNamesEntity>("dbo", "Orders");

			// Act
			var selectQuery = generator.GenerateGetAllQuery();

			// Assert
			Assert.Equal($"SELECT [dbo].[Orders].[OrderId] AS [Id], [dbo].[Orders].[DateCreated] AS [Date] FROM [dbo].[Orders];", selectQuery);
		}
		#endregion

		#region Get
		[Fact]
		public void GenerateGetQuery_SinglePrimaryKey_Valid()
		{
			// Arrange
			var generator = new SqlQueryGenerator<SinglePrimaryKeyEntity>("dbo", "Users");

			// Act
			var selectQuery = generator.GenerateGetQuery();

			// Assert
			Assert.Equal($"SELECT [dbo].[Users].[Id], [dbo].[Users].[Username], [dbo].[Users].[Password] FROM [dbo].[Users] WHERE [dbo].[Users].[Id] = @Id;", selectQuery);
		}

		[Fact]
		public void GenerateGetQuery_CompositePrimaryKey_Valid()
		{
			// Arrange
			var generator = new SqlQueryGenerator<CompositePrimaryKeyEntity>("dbo", "Users");

			// Act
			var selectQuery = generator.GenerateGetQuery();

			// Assert
			Assert.Equal($"SELECT [dbo].[Users].[Username], [dbo].[Users].[Password], [dbo].[Users].[DateCreated] FROM [dbo].[Users] WHERE [dbo].[Users].[Username] = @Username AND [dbo].[Users].[Password] = @Password;", selectQuery);
		}

		[Fact]
		public void GenerateGetQuery_CustomColumnNames_Valid()
		{
			// Arrange
			var generator = new SqlQueryGenerator<CustomColumnNamesEntity>("dbo", "Orders");

			// Act
			var selectQuery = generator.GenerateGetQuery();

			// Assert
			Assert.Equal($"SELECT [dbo].[Orders].[OrderId] AS [Id], [dbo].[Orders].[DateCreated] AS [Date] FROM [dbo].[Orders] WHERE [dbo].[Orders].[OrderId] = @Id;", selectQuery);
		}

		[Fact]
		public void GenerateGetQuery_NoPrimaryKey_Valid()
		{
			// Arrange
			var generator = new SqlQueryGenerator<HeapEntity>("dbo", "Users");

			// Act
			var query = generator.GenerateGetQuery();

			// Assert
			Assert.Equal("SELECT [dbo].[Users].[Username], [dbo].[Users].[Password] FROM [dbo].[Users] WHERE [dbo].[Users].[Username] = @Username AND [dbo].[Users].[Password] = @Password;", query);
		}
		#endregion

		#region Update

		[Fact]
		public void GenerateUpdateQuery_SinglePrimaryKey_Valid()
		{
			// Arrange
			var generator = new SqlQueryGenerator<SinglePrimaryKeyEntity>("dbo", "Users");

			// Act 
			var updateQuery = generator.GenerateUpdateQuery();

			// Assert
			Assert.Equal($"UPDATE [dbo].[Users] SET [dbo].[Users].[Username] = @Username, [dbo].[Users].[Password] = @Password OUTPUT [inserted].[Id], [inserted].[Username], [inserted].[Password] WHERE [dbo].[Users].[Id] = @Id;", updateQuery);
		}

		[Fact]
		public void GenerateUpdateQuery_CompositePrimaryKey_Valid()
		{
			// Arrange
			var generator = new SqlQueryGenerator<CompositePrimaryKeyEntity>("dbo", "Users");

			// Act 
			var updateQuery = generator.GenerateUpdateQuery();

			// Assert
			Assert.Equal($"UPDATE [dbo].[Users] SET [dbo].[Users].[DateCreated] = @DateCreated OUTPUT [inserted].[Username], [inserted].[Password], [inserted].[DateCreated] WHERE [dbo].[Users].[Username] = @Username AND [dbo].[Users].[Password] = @Password;", updateQuery);
		}

		[Fact]
		public void GenerateUpdateQuery_CustomColumnNames_Valid()
		{
			// Arrange
			var generator = new SqlQueryGenerator<CustomColumnNamesEntity>("dbo", "Orders");

			// Act 
			var updateQuery = generator.GenerateUpdateQuery();

			// Assert
			Assert.Equal($"UPDATE [dbo].[Orders] SET [dbo].[Orders].[DateCreated] = @Date OUTPUT [inserted].[OrderId] AS [Id], [inserted].[DateCreated] AS [Date] WHERE [dbo].[Orders].[OrderId] = @Id;", updateQuery);
		}

		[Fact]
		public void GenerateUpdateQuery_NoPrimaryKey_Throws()
		{
			// Arrange
			var generator = new SqlQueryGenerator<HeapEntity>("dbo", "Users");

			// Act && Assert
			Assert.Throws<InvalidOperationException>(() => generator.GenerateUpdateQuery());
		}

		[Fact]
		public void GenerateUpdateQuery_AllColumnsHasNoSetter_Throws()
		{
			// Arrange
			var generator = new SqlQueryGenerator<AllColumnsHasMissingSetterEntity>("dbo", "Users");

			// Act && Assert
			Assert.Throws<InvalidOperationException>(() => generator.GenerateUpdateQuery());
		}

		[Fact]
		public void GenerateUpdateQuery_ColumnHasNoSetter_ColumnIsExcluded()
		{
			// Arrange
			var generator = new SqlQueryGenerator<ColumnHasMissingSetterEntity>("dbo", "Users");

			// Act
			var query = generator.GenerateUpdateQuery();

			// Assert
			Assert.Equal("UPDATE [dbo].[Users] SET [dbo].[Users].[Age] = @Age OUTPUT [inserted].[Id], [inserted].[Age], [inserted].[DateCreated] WHERE [dbo].[Users].[Id] = @Id;", query);
		}
		#endregion
	}
}
