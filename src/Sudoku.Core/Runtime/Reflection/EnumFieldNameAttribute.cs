namespace Sudoku.Runtime.Reflection;

/// <summary>
/// Defines the name of a field in an enumeration type.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
internal sealed class EnumFieldNameAttribute : Attribute
{
	/// <summary>
	/// Initializes an <see cref="EnumFieldNameAttribute"/> instance via the specified name.
	/// </summary>
	/// <param name="name">The name.</param>
	public EnumFieldNameAttribute(string name) => Name = name;


	/// <summary>
	/// Indicates the name of the field.
	/// </summary>
	public string Name { get; }
}
