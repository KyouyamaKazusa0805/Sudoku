namespace Sudoku.Cli.Options;

/// <summary>
/// Represents a difficulty level option.
/// </summary>
public sealed class DifficultyLevelOption : IOption<DifficultyLevelOption, DifficultyLevel>
{
	/// <inheritdoc/>
	public static string Description => "Indicates the difficulty level to be set.";

	/// <inheritdoc/>
	public static string[] Aliases => new[] { "--difficulty-level", "-d" };

	/// <inheritdoc/>
	public static DifficultyLevel DefaultValue => 0;
}
