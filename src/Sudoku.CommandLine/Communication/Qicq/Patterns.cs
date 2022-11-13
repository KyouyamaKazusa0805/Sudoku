namespace Sudoku.Communication.Qicq;

/// <summary>
/// Extracts a type that stores regular expression patterns.
/// </summary>
internal static partial class Patterns
{
	[GeneratedRegex("""((\u54d4\u54e9)\2|[Bb]\s{0,3}\u7ad9|[Bb]i(li)bi\3)""", RegexOptions.Compiled, 5000, "zh-CN")]
	public static partial Regex BilibiliPattern();
}
