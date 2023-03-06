namespace Sudoku.Platforms.QQ;

/// <summary>
/// Extracts a type that stores regular expression patterns.
/// </summary>
internal static partial class Patterns
{
	[GeneratedRegex("""((哔哩)\2|[Bb]\s{0,3}\u7ad9|[Bb]i(li)bi\3)""", RegexOptions.Compiled, 5000, "zh-CN")]
	public static partial Regex BilibiliPattern();

	[GeneratedRegex("""#([1-9]|1[0-5])""", RegexOptions.Compiled, 5000)]
	public static partial Regex ColorIdPattern();

	[GeneratedRegex("""#[\dA-Fa-f]{6}([\dA-Fa-f]{2})?""", RegexOptions.Compiled, 5000)]
	public static partial Regex ColorHexValuePattern();

	[GeneratedRegex("""[行列宫]\s*[1-9]""", RegexOptions.Compiled, 5000, "zh-CN")]
	public static partial Regex ChineseHouseIndexPattern();
}
