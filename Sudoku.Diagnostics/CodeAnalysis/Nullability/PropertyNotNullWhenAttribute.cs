using System;

namespace Sudoku.Diagnostics.CodeAnalysis.Nullability
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class PropertyNotNullWhenAttribute : Attribute
	{
		public PropertyNotNullWhenAttribute(string propertyName, bool value) =>
			(PropertyName, Value) = (propertyName, value);


		public string PropertyName { get; }

		public bool Value { get; }
	}
}
