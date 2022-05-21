using System;

namespace Dapper.Repository.Attributes
{
	/// <summary>
	/// Marks a property for mapping with a DB column
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class ColumnAttribute : Attribute
	{
		public string? ColumnName { get; }

		public bool HasDefaultConstraint { get; }

		/// <summary>
		/// </summary>
		/// <param name="columnName">Optional name of the DB column, if it doesn't match the property name.</param>
		/// <param name="hasDefaultConstraint">Optional: whether the column has a default constraint in the database.</param>
		public ColumnAttribute(string? columnName = null, bool hasDefaultConstraint = false)
		{
			ColumnName = columnName;
			HasDefaultConstraint = hasDefaultConstraint;
		}
	}
}
