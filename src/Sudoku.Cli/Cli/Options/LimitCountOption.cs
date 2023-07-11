namespace Sudoku.Cli.Options;

/// <summary>
/// Represents a limit count option.
/// </summary>
public sealed class LimitCountOption : IOption<LimitCountOption, int>
{
	/// <inheritdoc/>
	public static string Description
		=> """
		Indicates the maximum number of puzzles the backing generator will be generated. 
		If other options are too strict, this option will automatically break the generation to cancel the program.
		""".RemoveLineEndings();

	/// <inheritdoc/>
	public static string[] Aliases => new[] { "--limit-count", "-c" };

	/// <inheritdoc/>
	public static int DefaultValue => 0;
}
