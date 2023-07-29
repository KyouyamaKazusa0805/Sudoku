namespace Sudoku.Concepts;

/// <summary>
/// Provides with <see cref="Regex"/> patterns used by <see cref="GridParser"/>.
/// </summary>
/// <seealso cref="Regex"/>
/// <seealso cref="GridParser"/>
internal static partial class GridParserPatterns
{
	[GeneratedRegex("""(\+?\d|\.)""", RegexOptions.Compiled, 5000)]
	public static partial Regex SusserDigitPattern();

	[GeneratedRegex("""\d(\|\d){242}""", RegexOptions.Compiled, 5000)]
	public static partial Regex OpenSudokuPattern();

	[GeneratedRegex("""(\<\d\>|\*\d\*|\d*[\+\-]?\d+)""", RegexOptions.Compiled, 5000)]
	public static partial Regex PencilmarkedPattern();

	[GeneratedRegex("""([\d\.\+]{9}(\r|\n|\r\n)){8}[\d\.\+]{9}""", RegexOptions.Compiled, 5000)]
	public static partial Regex SimpleMultilinePattern();

	[GeneratedRegex("""[\d\.\+]{80,}(\:(\d{3}\s+)*\d{3})?""", RegexOptions.Compiled, 5000)]
	public static partial Regex SusserPattern();

	[GeneratedRegex("""[\d\.\*]{1,9}(,[\d\.\*]{1,9}){8}""", RegexOptions.Compiled, 5000)]
	public static partial Regex ShortenedSusserPattern();

	[GeneratedRegex("""\d*[\-\+]?\d+""", RegexOptions.Compiled, 5000)]
	public static partial Regex SukakuSegmentPattern();
}
