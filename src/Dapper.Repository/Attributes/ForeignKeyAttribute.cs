using System;

namespace Dapper.Repository.Attributes
{
	/// <summary>
	/// Marks a property as a foreign key column.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class ForeignKeyColumnAttribute : ColumnAttribute
	{
		/// <summary>
		/// Name of the referenced property in the referenced class.
		/// </summary>
		public string? ReferencedPropertyName { get; }

		/// <summary>
		/// </summary>
		/// <param name="referencedPropertyName">Optional name of the property in the referenced class, if it doesn't match the property name.</param>
		public ForeignKeyColumnAttribute(string? referencedPropertyName = null, string? columnName = null, bool hasDefaultConstraint = false)
			: base(columnName, hasDefaultConstraint)
		{
			this.ReferencedPropertyName = referencedPropertyName;
		}
	}
}
