namespace Sudoku.Diagnostics.CodeGen;

/// <summary>
/// The name attribute that applies to a field in enumeration field, indicating its name.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
internal sealed class NameAttribute : Attribute
{
	/// <summary>
	/// Initializes an <see cref="NameAttribute"/> via the name.
	/// </summary>
	/// <param name="name">The name of the field.</param>
	public NameAttribute(string name) => Name = name;


	/// <summary>
	/// Indicates the name of the attribute.
	/// </summary>
	public string Name { get; }
}
