namespace Sudoku.Platforms.QQ.Data.Parsing;

/// <summary>
/// Represents a name attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class NameAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="NameAttribute"/> instance via the name.
	/// </summary>
	/// <param name="name">The name.</param>
	public NameAttribute(string name) => Name = name;

	/// <summary>
	/// Indicates the name.
	/// </summary>
	public string Name { get; }
}
