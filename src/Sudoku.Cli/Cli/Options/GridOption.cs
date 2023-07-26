namespace Sudoku.Cli.Options;

/// <summary>
/// Represents a grid option.
/// </summary>
public sealed class GridOption : IOption<GridOption, Grid, GridArgumentConverter>
{
	/// <inheritdoc/>
	public static bool IsDefault => true;

	/// <inheritdoc/>
	public static bool IsRequired => true;

	/// <inheritdoc/>
	public static string Description => "Indicates the target grid to be analyzed.";

	/// <inheritdoc/>
	public static string[] Aliases => ["--grid", "-g"];

	/// <inheritdoc/>
	public static Grid DefaultValue => Grid.Undefined;
}
