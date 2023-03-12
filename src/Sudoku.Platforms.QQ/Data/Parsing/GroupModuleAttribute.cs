namespace Sudoku.Platforms.QQ.Data.Parsing;

/// <summary>
/// Provides with an attribute type that describes this type is a <see cref="GroupModule"/> implementation type.
/// This attribute is necessary to be detected and invoked by <see cref="MiraiBot"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class GroupModuleAttribute : CommandLineParsingItemAttribute
{
	/// <summary>
	/// Initializes a <see cref="GroupModuleAttribute"/> instance via the specified name.
	/// </summary>
	/// <param name="name">The name.</param>
	public GroupModuleAttribute(string name) => Name = name;

	/// <summary>
	/// Initializes a <see cref="GroupModuleAttribute"/> instance.
	/// </summary>
	internal GroupModuleAttribute() : this(null!)
	{
	}


	/// <summary>
	/// Indicates the name of the module.
	/// </summary>
	public string Name { get; }
}
