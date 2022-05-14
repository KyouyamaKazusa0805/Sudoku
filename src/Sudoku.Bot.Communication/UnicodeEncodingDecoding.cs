namespace Sudoku.Bot.Communication;

/// <summary>
/// Encodes or decodes the character values to/from a <c>\uxxxx</c> representation.
/// </summary>
internal static class UnicodeEncodingDecoding
{
	private static readonly Regex
		ReUnicode = new("""\\u([0-9a-fA-F]{4})""", RegexOptions.Compiled),
		ReUnicodeChar = new("""[^\u0000-\u00ff]""", RegexOptions.Compiled);


	/// <summary>
	/// Decodes a string from <c>\uXXXX</c> sequence.
	/// </summary>
	/// <param name="s">The string.</param>
	/// <returns>The decoded string.</returns>
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
	/// Encodes a string to <c>\uXXXX</c> sequence.
	/// </summary>
	/// <param name="s">The string.</param>
	/// <returns>The encoded string.</returns>
	public static string Encode(string s) => ReUnicodeChar.Replace(s, static m => $"""\u{(short)m.Value[0]:x4}""");
}
