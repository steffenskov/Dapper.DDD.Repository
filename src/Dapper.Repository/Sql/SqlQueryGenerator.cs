using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Dapper.Repository.Interfaces;
using Dapper.Repository.MetaInformation;
using Dapper.Repository.MetaInformation.PropertyInfos;

[assembly: InternalsVisibleTo("Dapper.Repository.UnitTests")]
namespace Dapper.Repository.Sql
{
	internal class SqlQueryGenerator<TEntity> : IQueryGenerator<TEntity>
	where TEntity : DbEntity
	{
		private readonly string _schemaAndTable;

		public SqlQueryGenerator(string schema, string tableName)
		{
			if (tableName == null)
			{
				throw new ArgumentNullException(nameof(tableName));
			}

			if (schema == null)
			{
				throw new ArgumentNullException(nameof(schema));
			}

			if (string.IsNullOrWhiteSpace(tableName))
			{
				throw new ArgumentException($"Invalid tableName: {tableName}", nameof(tableName));
			}

			if (string.IsNullOrWhiteSpace(schema))
			{
				throw new ArgumentException($"Invalid schema: {schema}", nameof(schema));
			}
			_schemaAndTable = $"{EnsureSquareBrackets(schema)}.{EnsureSquareBrackets(tableName)}";
		}

		public string GenerateDeleteQuery()
		{
			var info = EntityInformationCache.GetEntityInformation<TEntity>();

			var whereClause = info.PrimaryKeys.Count == 0
								? GenerateWhereClauseWithoutPrimaryKey(info)
								: GenerateWhereClauseWithPrimaryKeys(info);

			var outputColumns = GenerateColumnsList("deleted", info.Columns);
			return $"DELETE FROM {_schemaAndTable} OUTPUT {outputColumns} WHERE {whereClause};";
		}

		public string GenerateInsertQuery(TEntity entity)
		{
			var info = EntityInformationCache.GetEntityInformation<TEntity>();
			var identityColumns = info.PrimaryKeys.Where(pk => pk.IsIdentity).Select(pk => pk.Property).ToList();

			var columnsToInsert = info.Columns
										.Where(column => !identityColumns.Contains(column.Property) && (!column.HasDefaultConstraint || !column.HasDefaultValue(entity)))
										.ToList();

			var outputColumns = GenerateColumnsList("inserted", info.Columns);
			return $"INSERT INTO {_schemaAndTable} ({string.Join(", ", columnsToInsert.Select(column => AddSquareBrackets(column.ColumnName)))}) OUTPUT {outputColumns} VALUES ({string.Join(", ", columnsToInsert.Select(column => $"@{column.Name}"))});";
		}

		public string GenerateGetAllQuery()
		{
			var info = EntityInformationCache.GetEntityInformation<TEntity>();
			var columnsList = GenerateColumnsList(_schemaAndTable, info.Columns);
			return $"SELECT {columnsList} FROM {_schemaAndTable};";
		}

		public string GenerateGetQuery()
		{
			var info = EntityInformationCache.GetEntityInformation<TEntity>();
			var whereClause = info.PrimaryKeys.Count == 0
										? GenerateWhereClauseWithoutPrimaryKey(info)
										: GenerateWhereClauseWithPrimaryKeys(info);

			var columnsList = GenerateColumnsList(_schemaAndTable, info.Columns);

			return $"SELECT {columnsList} FROM {_schemaAndTable} WHERE {whereClause};";
		}

		public string GenerateUpdateQuery()
		{
			var info = EntityInformationCache.GetEntityInformation<TEntity>();
			if (!info.PrimaryKeys.Any())
			{
				throw new InvalidOperationException($"GenerateUpdateQuery for entity of type {typeof(TEntity).FullName} failed as the type has no properties marked with [PrimaryKeyColumn].");
			}

			var setClause = GenerateSetClause(info);

			if (string.IsNullOrEmpty(setClause))
			{
				throw new InvalidOperationException($"GenerateUpdateQuery for entity of type {typeof(TEntity).FullName} failed as the type has no columns with a setter.");
			}

			var outputColumns = GenerateColumnsList("inserted", info.Columns);

			return $"UPDATE {_schemaAndTable} SET {setClause} OUTPUT {outputColumns} WHERE {GenerateWhereClauseWithPrimaryKeys(info)};";
		}

		private string GenerateSetClause(EntityInformation info)
		{
			var primaryKeys = info.PrimaryKeys.Select(pk => pk.Property).ToList();
			var columnsToSet = info.Columns.Where(column => !primaryKeys.Contains(column.Property) && column.HasSetter);
			return string.Join(", ", columnsToSet.Select(column => $"{_schemaAndTable}.{AddSquareBrackets(column.ColumnName)} = @{column.Name}"));
		}

		private string GenerateWhereClauseWithoutPrimaryKey(EntityInformation info)
		{
			return string.Join(" AND ", info.Columns.Select(column => $"{_schemaAndTable}.{AddSquareBrackets(column.ColumnName)} = @{column.Name}"));
		}

		private string GenerateWhereClauseWithPrimaryKeys(EntityInformation info)
		{
			var primaryKeyProperties = info.PrimaryKeys.Select(pk => pk.Property).ToList();
			var primaryKeys = info.Columns
								.Where(column => primaryKeyProperties.Contains(column.Property));

			return string.Join(" AND ", primaryKeys.Select(column => $"{_schemaAndTable}.{AddSquareBrackets(column.ColumnName)} = @{column.Name}"));
		}


		private string GenerateColumnsList(string tableName, IEnumerable<ColumnPropertyInfo> columns)
		{
			tableName = EnsureSquareBrackets(tableName);

			return string.Join(", ", columns.Select(column => GenerateColumnClause(tableName, column)));
		}

		private static string GenerateColumnClause(string tableName, ColumnPropertyInfo column)
		{
			if (column.IsCustomColumnName)
			{
				return $"{tableName}.{AddSquareBrackets(column.ColumnName)} AS {AddSquareBrackets(column.Name)}";
			}
			else
			{
				return $"{tableName}.{AddSquareBrackets(column.ColumnName)}";
			}
		}

		private static string EnsureSquareBrackets(string name)
		{
			if (!name.StartsWith('['))
				return AddSquareBrackets(name);
			else
				return name;
		}

		private static string AddSquareBrackets(string name)
		{
			return $"[{name}]";
		}
	}
}
