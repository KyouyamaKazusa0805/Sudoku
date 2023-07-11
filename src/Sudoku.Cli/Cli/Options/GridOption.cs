namespace Sudoku.Cli.Options;

/// <summary>
/// Represents a grid option.
/// </summary>
public sealed class GridOption : IOption<GridOption, string>
{
	/// <inheritdoc/>
	public static string Description => "Indicates the target grid to be analyzed.";

	/// <inheritdoc/>
	public static string[] Aliases => new[] { "--grid", "-g" };

	/// <inheritdoc/>
	public static string DefaultValue => string.Empty;
}
