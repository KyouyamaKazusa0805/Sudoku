namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Introduces the options that are used for formatting a sudoku grid.
/// </summary>
[Verb("format", HelpText = "To format a sudoku grid using string as the result representation.")]
public sealed class FormatOptions : IUsageProvider
{
	/// <summary>
	/// Indicates the grid as <see cref="string"/> representation.
	/// </summary>
	[Option('g', "grid", Required = true, HelpText = "Indicates the grid to be formatted.")]
	public string GridValue { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the format string.
	/// </summary>
	[Option('f', "format-string", Default = "", HelpText = "Indicates the format string of the grid to be formatted.")]
	public string FormatString { get; set; } = string.Empty;


#nullable disable
	/// <inheritdoc/>
	[Usage(ApplicationAlias = "Sudoku.CommandLine.exe")]
	public static IEnumerable<Example> Examples
	{
		get
		{
			yield return new(
				"Format the grid, without any candidates.",
				new FormatOptions
				{
					GridValue = "...892.....2...3..75.....69.359.814...........713.659.96.....21..4...6.....621...",
				}
			);
			yield return new(
				"Format the code, treating all values as given ones.",
				new FormatOptions
				{
					GridValue = "00+210090+310+403+95+7+209+30000+18+428+973+1+563+1+95+86+7+246+7+521+438+920+700+109+5001090007+906+70500+1:511 812 435 635 882 884 887",
					FormatString = "!"
				}
			);
		}
	}
#nullable restore
}
