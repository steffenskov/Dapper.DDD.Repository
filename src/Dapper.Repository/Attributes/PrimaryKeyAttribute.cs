using System;

namespace Dapper.Repository.Attributes
{
	/// <summary>
	/// Marks a property as a primary key column.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class PrimaryKeyColumnAttribute : ColumnAttribute
	{
		/// <summary>
		/// Whether this primary key is an identity column (auto incrementing value)
		/// </summary>
		public bool IsIdentity { get; }

		/// <summary>
		/// </summary>
		/// <param name="isIdentity">Whether this primary key is an identity column (auto incrementing value)</param>
		public PrimaryKeyColumnAttribute(bool isIdentity = false, string? columnName = null, bool hasDefaultConstraint = false)
			: base(columnName, hasDefaultConstraint)
		{
			IsIdentity = isIdentity;
		}
	}
}
