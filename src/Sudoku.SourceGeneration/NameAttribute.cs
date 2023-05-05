namespace Sudoku.SourceGeneration;

/// <summary>
/// The name attribute that applies to a field in enumeration field, indicating its name.
/// </summary>
/// <param name="name">The name of the field.</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
internal sealed class NameAttribute(string name) : Attribute
{
	/// <summary>
	/// Indicates the name of the attribute.
	/// </summary>
	public string Name { get; } = name;
}
