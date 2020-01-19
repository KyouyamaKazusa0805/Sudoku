using System;

namespace Sudoku.Diagnostics.CodeAnalysis.Nullability
{
	/// <summary>
	/// To mark on a property, means the value is not null when
	/// another <see cref="bool"/>-type property holds the specified
	/// value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class PropertyNotNullWhenAttribute : Attribute
	{
		/// <summary>
		/// Initializes an instance with property name and its value.
		/// </summary>
		/// <param name="propertyName">The property name.</param>
		/// <param name="value">The value.</param>
		public PropertyNotNullWhenAttribute(string propertyName, bool value) =>
			(PropertyName, Value) = (propertyName, value);


		/// <summary>
		/// Indicates the property name.
		/// </summary>
		public string PropertyName { get; }

		/// <summary>
		/// Indicates the value.
		/// </summary>
		public bool Value { get; }
	}
}
