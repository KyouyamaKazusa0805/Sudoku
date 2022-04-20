#nullable disable

namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Introduces the options that are used for checking attributes for a sudoku grid.
/// </summary>
[Verb("check", HelpText = "To check the attributes for a sudoku grid.")]
public sealed class CheckGridOptions
{
	/// <summary>
	/// Indicates the grid value.
	/// </summary>
	[Value(0, HelpText = "Indicates the sudoku grid as string representation.", Required = true)]
	public string GridValue { get; set; }

	/// <summary>
	/// Indicates the operation is for checking the validity of the grid.
	/// </summary>
	[Option('v', "validity", HelpText = "Indicates to check the validity for a sudoku grid.", SetName = "check-validity")]
	public bool ChecksForValidity { get; set; }


	/// <summary>
	/// Introduces the usages of the current command.
	/// </summary>
	/// <remarks><b><i>
	/// Due to the bug of the command line nuget package, we should disable the
	/// implicitly-generated nullable attribute and then use this property; otherwise
	/// the <see cref="InvalidCastException"/>-typed exception instance will be thrown.
	/// For more details on this bug, please visit
	/// <see href="https://github.com/commandlineparser/commandline/issues/714">this link</see>.
	/// </i></b></remarks>
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
}
