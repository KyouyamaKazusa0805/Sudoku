namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Introduces the options that are used for checking attributes for a sudoku grid.
/// </summary>
[Verb("check", HelpText = "To check the attributes for a sudoku grid.")]
public sealed class CheckGridOptions : IUsageProvider
{
	/// <summary>
	/// Indicates the grid value.
	/// </summary>
	[Value(0, HelpText = "Indicates the sudoku grid as string representation.", Required = true)]
	public string GridValue { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the operation is for checking the validity of the grid.
	/// </summary>
	[Option('v', "validity", HelpText = "Indicates to check the validity for a sudoku grid.", SetName = "check-validity")]
	public bool ChecksForValidity { get; set; }


#nullable disable
	/// <inheritdoc/>
	[Usage(ApplicationAlias = "Sudoku.CommandLine.exe")]
	public static IEnumerable<Example> Examples
	{
		get
		{
			yield return new(
				"Checks whether the grid is valid.",
				UnParserSettings.WithGroupSwitchesOnly(),
				new CheckGridOptions
				{
					GridValue = "...892.....2...3..75.....69.359.814...........713.659.96.....21..4...6.....621...",
					ChecksForValidity = true
				}
			);
		}
	}
#nullable restore
}
