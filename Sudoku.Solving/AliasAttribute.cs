using System;

namespace Sudoku.Solving
{
	/// <summary>
	/// To mark on a enum field to give an alias.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, Inherited = false)]
	public sealed class AliasAttribute : Attribute
	{
		/// <summary>
		/// Initializes an instance with the enum type and the field.
		/// </summary>
		/// <param name="enumType">The type.</param>
		/// <param name="fieldName">The field name.</param>
		public AliasAttribute(Type enumType, string fieldName) => (EnumType, FieldName) = (enumType, fieldName);


		/// <summary>
		/// The type.
		/// </summary>
		public Type EnumType { get; }

		/// <summary>
		/// The field name.
		/// </summary>
		public string FieldName { get; }
	}
}
