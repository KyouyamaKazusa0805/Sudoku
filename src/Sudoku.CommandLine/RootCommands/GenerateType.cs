namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Defines a generate type.
/// </summary>
public enum GenerateType
{
	/// <summary>
	/// Indicates the generate type is to generate a puzzle with the hard-pattern algorithm.
	/// </summary>
	[SupportedNames(new[] { "hard-pattern", "hard", "h" })]
	HardPatternLike,
}
