namespace Sudoku.Platforms.QQ.Data.Parsing;

/// <summary>
/// Provides with an attribute type that describes this property is the target argument to be assigned while parsing.
/// </summary>
public abstract class ArgumentAttribute : CommandLineParsingItemAttribute
{
	/// <summary>
	/// Indicates the number of required values.
	/// </summary>
	public abstract int RequiredValuesCount { get; }
}
