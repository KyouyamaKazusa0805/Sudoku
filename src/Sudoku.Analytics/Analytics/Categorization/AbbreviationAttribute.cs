namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Represents an attribute type that describes its abbreviation of a technique group.
/// </summary>
/// <param name="abbr"><inheritdoc cref="Abbreviation" path="/summary"/></param>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class AbbreviationAttribute(string abbr) : Attribute
{
	/// <summary>
	/// Indicates the abbreviation.
	/// </summary>
	public string Abbreviation { get; } = abbr;
}
