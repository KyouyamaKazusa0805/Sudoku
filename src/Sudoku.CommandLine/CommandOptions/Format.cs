namespace Sudoku.CommandLine.CommandOptions;

/// <summary>
/// Represents a format command.
/// </summary>
public sealed class Format : IRootCommand<ErrorCode>
{
	/// <summary>
	/// Indicates the format string.
	/// </summary>
	[Command('f', "format", "Indicates the format string used.")]
	public string FormatString { get; set; } = "0";

	/// <summary>
	/// Indicates the grid used.
	/// </summary>
	[Command('g', "grid", "Indicates the grid to be formatted.")]
	[CommandConverter(typeof(GridConverter))]
	public Grid Grid { get; set; }

	/// <inheritdoc/>
	public static string Name => "format";

	/// <inheritdoc/>
	public static string Description => "To format a sudoku grid using string as the result representation.";

	/// <inheritdoc/>
	public static string[] SupportedCommands => new[] { "format" };

	/// <inheritdoc/>
	public static IEnumerable<(string CommandLine, string Meaning)>? UsageCommands =>
		new[]
		{
			(
				"""
				format -g "...892.....2...3..75.....69.359.814...........713.659.96.....21..4...6.....621..." -f "0"
				""",
				"""Formats the specified grid, using the string "0" as the format one, which means the grid only displays the given cells, modifiables are treated as the empty ones, and all empty cells will be displayed as a zero character '0'."""
			)
		};


	/// <inheritdoc/>
	public ErrorCode Execute()
	{
		string format = FormatString;
		try
		{
			ConsoleExtensions.WriteLine(
				$"""
				Grid: '{Grid.ToString("0")}'
				Format: '{c(format)}'
				Result: {Grid.ToString(format)}
				"""
			);

			return ErrorCode.None;


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static string c(string f) => f is null ? "<null>" : string.IsNullOrWhiteSpace(f) ? "<empty>" : f;
		}
		catch (FormatException)
		{
			return ErrorCode.ArgFormatIsInvalid;
		}
	}
}
