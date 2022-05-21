using System;
using System.Collections.Generic;
using System.Linq;
using Dapper.Repository.Interfaces;
using Dapper.Repository.MetaInformation;
using Dapper.Repository.MetaInformation.PropertyInfos;

namespace Dapper.Repository.MySql
{
	public class MySqlQueryGenerator<TEntity> : IQueryGenerator<TEntity>
	where TEntity : DbEntity
	{
		private readonly string _table;

		public MySqlQueryGenerator(string tableName)
		{
			if (tableName == null)
			{
				throw new ArgumentNullException(nameof(tableName));
			}


			if (string.IsNullOrWhiteSpace(tableName))
			{
				throw new ArgumentException($"Invalid tableName: {tableName}", nameof(tableName));
			}

			_table = tableName;
		}

		public string GenerateDeleteQuery()
		{
			var info = EntityInformationCache.GetEntityInformation<TEntity>();

			var whereClause = info.PrimaryKeys.Count == 0
								? GenerateWhereClauseWithoutPrimaryKey(info)
								: GenerateWhereClauseWithPrimaryKeys(info);

			var outputColumns = GenerateColumnsList(_table, info.Columns);
			return $@"SELECT {outputColumns} FROM {_table} WHERE {whereClause};
DELETE FROM {_table} WHERE {whereClause};";
		}

		public string GenerateGetAllQuery()
		{
			var info = EntityInformationCache.GetEntityInformation<TEntity>();
			var columnsList = GenerateColumnsList(_table, info.Columns);
			return $"SELECT {columnsList} FROM {_table};";
		}

		public string GenerateGetQuery()
		{
			var info = EntityInformationCache.GetEntityInformation<TEntity>();
			var whereClause = info.PrimaryKeys.Count == 0
										? GenerateWhereClauseWithoutPrimaryKey(info)
										: GenerateWhereClauseWithPrimaryKeys(info);

			var columnsList = GenerateColumnsList(_table, info.Columns);

			return $"SELECT {columnsList} FROM {_table} WHERE {whereClause};";
		}

		public string GenerateInsertQuery(TEntity entity)
		{
			var info = EntityInformationCache.GetEntityInformation<TEntity>();
			var identityColumns = info.PrimaryKeys.Where(pk => pk.IsIdentity).ToList();
			var identityProperties = identityColumns.Select(pk => pk.Property).ToList();

			var columnsToInsert = info.Columns
										.Where(column => !identityProperties.Contains(column.Property) && (!column.HasDefaultConstraint || !column.HasDefaultValue(entity)))
										.ToList();

			string selectStatement = "";
			if (identityColumns.Any())
			{
				var column = identityColumns.SingleOrDefault();
				if (column == null)
				{
					throw new InvalidOperationException("Cannot generate INSERT query for table with multiple identity columns");
				}
				var columnsList = GenerateColumnsList(_table, info.Columns);
				selectStatement = $"SELECT {columnsList} FROM {_table} WHERE {_table}.{column.ColumnName} = LAST_INSERT_ID();";
			}
			else
			{
				selectStatement = GenerateGetQuery();
			}
			return $@"INSERT INTO {_table} ({string.Join(", ", columnsToInsert.Select(column => column.ColumnName))}) VALUES ({string.Join(", ", columnsToInsert.Select(column => $"@{column.Name}"))});
{selectStatement}";

		}

		public string GenerateUpdateQuery()
		{
			var info = EntityInformationCache.GetEntityInformation<TEntity>();
			if (!info.PrimaryKeys.Any())
			{
				throw new InvalidOperationException($"GenerateGetQuery for entity of type {typeof(TEntity).FullName} failed as the type has no properties marked with [PrimaryKeyColumn].");
			}

			var setClause = GenerateSetClause(info);

			if (string.IsNullOrEmpty(setClause))
			{
				throw new InvalidOperationException($"GenerateGetQuery for entity of type {typeof(TEntity).FullName} failed as the type has no columns with a setter.");
			}

			var outputColumns = GenerateColumnsList("inserted", info.Columns);
			var selectStatement = GenerateGetQuery();
			return $@"UPDATE {_table} SET {setClause} WHERE {GenerateWhereClauseWithPrimaryKeys(info)};
{selectStatement}";
		}

		#region Helpers

		private string GenerateSetClause(EntityInformation info)
		{
			var primaryKeys = info.PrimaryKeys.Select(pk => pk.Property).ToList();
			var columnsToSet = info.Columns.Where(column => !primaryKeys.Contains(column.Property) && column.HasSetter);
			return string.Join(", ", columnsToSet.Select(column => $"{column.ColumnName} = @{column.Name}"));
		}

		private string GenerateWhereClauseWithoutPrimaryKey(EntityInformation info)
		{
			return string.Join(" AND ", info.Columns.Select(column => $"{_table}.{column.ColumnName} = @{column.Name}"));
		}

		private string GenerateWhereClauseWithPrimaryKeys(EntityInformation info)
		{
			var primaryKeyProperties = info.PrimaryKeys.Select(pk => pk.Property).ToList();
			var primaryKeys = info.Columns
								.Where(column => primaryKeyProperties.Contains(column.Property));

			return string.Join(" AND ", primaryKeys.Select(column => $"{_table}.{column.ColumnName} = @{column.Name}"));
		}

		private string GenerateColumnsList(string tableName, IEnumerable<ColumnPropertyInfo> columns)
		{
			return string.Join(", ", columns.Select(column => GenerateColumnClause(tableName, column)));
		}

		private static string GenerateColumnClause(string tableName, ColumnPropertyInfo column)
		{
			if (column.IsCustomColumnName)
			{
				return $"{tableName}.{column.ColumnName} AS {column.Name}";
			}
			else
			{
				return $"{tableName}.{column.ColumnName}";
			}
		}
		#endregion
	}
}
