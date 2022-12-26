namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Represents a format command.
/// </summary>
[RootCommand("format", DescriptionResourceKey = "_Description_Format")]
[SupportedArguments("format")]
[Usage("format -g <grid> -f <format>", IsPattern = true)]
[Usage($"""format -g "{SampleGrid}" -f 0""", DescriptionResourceKey = "_Usage_Format_1")]
public sealed class Format : IExecutable
{
	/// <summary>
	/// Indicates the format string.
	/// </summary>
	[DoubleArgumentsCommand('f', "format", DescriptionResourceKey = "_Description_FormatString_Format")]
	public string FormatString { get; set; } = "0";

	/// <summary>
	/// Indicates the grid used.
	/// </summary>
	[DoubleArgumentsCommand('g', "grid", DescriptionResourceKey = "_Description_Grid_Format", IsRequired = true)]
	[CommandConverter<GridConverter>]
	public Grid Grid { get; set; }


	/// <inheritdoc/>
	public async Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		var format = FormatString;
		try
		{
			await Terminal.WriteLineAsync(string.Format(R["_MessageFormat_FormatResult"]!, Grid.ToString("0"), c(format), Grid.ToString(format)));


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static string c(string f) => f is null ? "<null>" : string.IsNullOrWhiteSpace(f) ? "<empty>" : f;
		}
		catch (FormatException)
		{
			throw new CommandLineRuntimeException((int)ErrorCode.ArgFormatIsInvalid);
		}
	}
}
