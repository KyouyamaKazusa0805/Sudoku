namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Defines a generate type.
/// </summary>
public enum GenerateType
{
	/// <summary>
	/// Indicates the generate type is to generate a puzzle with hard-pattern algorithm.
	/// </summary>
	[SupportedArguments(new[] { "hard-pattern", "hard", "h" })]
	HardPatternLike,

	/// <summary>
	/// Indicates the generate type is to generate a puzzle with pattern-based algorithm.
	/// </summary>
	[SupportedArguments(new[] { "pattern-based", "pattern", "p" })]
	PatternBased
}
