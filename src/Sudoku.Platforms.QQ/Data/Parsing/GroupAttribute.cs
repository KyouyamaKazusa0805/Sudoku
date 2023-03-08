namespace Sudoku.Platforms.QQ.Data.Parsing;

/// <summary>
/// Defines a shopping item group attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class GroupAttribute : DataParsingAttribute
{
	/// <summary>
	/// Initializes a <see cref="GroupAttribute"/> instance.
	/// </summary>
	/// <param name="group">The group.</param>
	public GroupAttribute(ShoppingItemGroup group) => Group = group;


	/// <summary>
	/// Indicates the group of the shopping item.
	/// </summary>
	public ShoppingItemGroup Group { get; }
}
