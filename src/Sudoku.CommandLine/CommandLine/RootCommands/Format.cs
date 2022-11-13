namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Represents a format command.
/// </summary>
[RootCommand("format", "To format a sudoku grid using string as the result representation.")]
[SupportedArguments("format")]
[Usage("format -g <grid> -f <format>", IsPattern = true)]
[Usage($"""format -g "{SampleGrid}" -f 0""", @"Formats the specified grid, using the string ""0"" as the format one, which means the grid only displays the given cells, modifiables are treated as the empty ones, and all empty cells will be displayed as a zero character '0'.")]
public sealed class Format : IExecutable
{
	/// <summary>
	/// Indicates the format string.
	/// </summary>
	[DoubleArgumentsCommand('f', "format", "Indicates the format string used.")]
	public string FormatString { get; set; } = "0";

	/// <summary>
	/// Indicates the grid used.
	/// </summary>
	[DoubleArgumentsCommand('g', "grid", "Indicates the grid to be formatted.", IsRequired = true)]
	[CommandConverter<GridConverter>]
	public Grid Grid { get; set; }


	/// <inheritdoc/>
	public void Execute()
	{
		var format = FormatString;
		try
		{
			Terminal.WriteLine(
				$"""
				Grid: '{Grid:0}'
				Format: '{c(format)}'
				Result: {Grid.ToString(format)}
				"""
			);


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static string c(string f) => f is null ? "<null>" : string.IsNullOrWhiteSpace(f) ? "<empty>" : f;
		}
		catch (FormatException)
		{
			throw new CommandLineRuntimeException((int)ErrorCode.ArgFormatIsInvalid);
		}
	}
}
