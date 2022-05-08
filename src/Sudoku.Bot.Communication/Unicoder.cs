namespace Sudoku.Bot.Communication;

/// <summary>
/// Unicode编解码器
/// </summary>
public static class Unicoder
{
	/// <summary>
	/// Indicates the inner unicode regular expression pattern.
	/// </summary>
	private static readonly Regex ReUnicode = new("""\\u([0-9a-fA-F]{4})""", RegexOptions.Compiled);

	private static readonly Regex ReUnicodeChar = new("""[^\u0000-\u00ff]""", RegexOptions.Compiled);


	/// <summary>
	/// Unicode编码(\uxxxx)序列转字符串
	/// </summary>
	/// <param name="s"></param>
	/// <returns></returns>
	public static string Decode(string s)
	{
		return ReUnicode.Replace(
			s,
			static m => m is { Groups: [_, { Value: var rawValue }, ..] } && shortTryParse(rawValue, out short c)
				? ((char)c).ToString()
				: m.Value
		);


		static bool shortTryParse(string rawValue, out short result)
			=> short.TryParse(rawValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
	}

	/// <summary>
	/// 字符串转Unicode编码(\uxxxx)序列
	/// </summary>
	/// <param name="s"></param>
	/// <returns></returns>
	public static string Encode(string s) => ReUnicodeChar.Replace(s, static m => $"""\u{(short)m.Value[0]:x4}""");
}
