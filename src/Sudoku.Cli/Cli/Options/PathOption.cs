namespace Sudoku.Cli.Options;

/// <summary>
/// Represents a path option.
/// </summary>
public sealed class PathOption : IOption<PathOption, string>
{
	/// <inheritdoc/>
	public static string Description => "Indicates the output path";

	/// <inheritdoc/>
	public static string[] Aliases => new[] { "--path", "-p" };

	/// <inheritdoc/>
	public static string DefaultValue => string.Empty;
}
