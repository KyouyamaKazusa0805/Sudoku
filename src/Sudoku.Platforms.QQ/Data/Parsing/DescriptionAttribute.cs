namespace Sudoku.Platforms.QQ.Data.Parsing;

/// <summary>
/// Represents a description attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class DescriptionAttribute : DataParsingAttribute
{
	/// <summary>
	/// Initializes a <see cref="DescriptionAttribute"/> instance via the name.
	/// </summary>
	/// <param name="description">The description.</param>
	public DescriptionAttribute(string description) => Description = description;

	/// <summary>
	/// Indicates the description.
	/// </summary>
	public string Description { get; }
}
