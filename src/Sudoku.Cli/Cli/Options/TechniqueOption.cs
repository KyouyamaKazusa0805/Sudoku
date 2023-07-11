namespace Sudoku.Cli.Options;

/// <summary>
/// Represents a technique option.
/// </summary>
public sealed class TechniqueOption : IOption<TechniqueOption, Technique>
{
	/// <inheritdoc/>
	public static string Description => "Indicates the technique to be set.";

	/// <inheritdoc/>
	public static string[] Aliases => new[] { "--technique", "-t" };

	/// <inheritdoc/>
	public static Technique DefaultValue => 0;
}
