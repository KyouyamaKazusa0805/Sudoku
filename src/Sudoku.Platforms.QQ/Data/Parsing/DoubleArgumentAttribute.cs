namespace Sudoku.Platforms.QQ.Modules.Group;

/// <summary>
/// Provides with an attribute type that describes this property is the target argument to be assigned when parsing.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed class DoubleArgumentAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="DoubleArgumentAttribute"/> instance its name.
	/// </summary>
	/// <param name="name">The name of the double argument.</param>
	public DoubleArgumentAttribute(string name) => Name = name;


	/// <summary>
	/// Indicates the name of the double argument.
	/// </summary>
	public string Name { get; }
}
