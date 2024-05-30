namespace Sudoku.Concepts;

/// <summary>
/// Provides with extension methods on <see cref="Grid"/>.
/// </summary>
/// <seealso cref="Grid"/>
public static class GridTokenExtensions
{
	/// <inheritdoc/>
	/// <summary>
	/// The character span that indicates all possible characters appeared in a number with base 32.
	/// </summary>
	internal static ReadOnlySpan<char> Base32CharSpan => "0123456789abcdefghijklmnopqrstuv";


	/// <summary>
	/// Indicates the token of the grid at the initial state.
	/// </summary>
	/// <param name="this">Indicates the current instance.</param>
	/// <returns>The token.</returns>
	/// <remarks>
	/// A raw string example is:
	/// <code><![CDATA[
	/// 35i4ra00rlr4btf9a8s573tsk1ldni00ccfg094v02pk54ff1hc6e7
	/// ]]></code>
	/// We should cut them by 6 characters as a group:
	/// <code><![CDATA[
	/// 35i4ra 00rlr4 btf9a8 s573ts k1ldni 00ccfg 094v02 pk54ff 1hc6e7
	/// ]]></code>
	/// 9 groups in total.
	/// Then we should convert it into a valid 9-digit number by treating them as 32-based integers.
	/// Finally, combinate all groups, then we are done.
	/// The final text is
	/// <code><![CDATA[
	/// 106500970000907108400008520945000380672839410000406000009600002860000751051780039
	/// ]]></code>
	/// </remarks>
	/// <exception cref="NotSupportedException">Throws when the puzzle type is Sukaku.</exception>
	public static string GetToken(this scoped in Grid @this)
	{
		if (@this.PuzzleType == SudokuType.Sukaku)
		{
			throw new NotSupportedException(ResourceDictionary.ExceptionMessage("NotSupportedForSukakuPuzzles"));
		}

		var convertedString = @this.ToString("0");
		var values = from str in convertedString.Chunk(9) select int.Parse(str);
		var sb = new StringBuilder(54);
		for (var i = 8; i >= 0; i--)
		{
			var value = values[i];
			var stack = new Stack<int>();
			while (value != 0)
			{
				stack.Push(value & 31);
				value >>= 5;
			}

			foreach (var element in stack)
			{
				sb.Append(Base32CharSpan[element]);
			}
		}

		return sb.ToString();
	}

	/// <inheritdoc cref="CreateFromToken(string)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid CreateFromToken(ReadOnlySpan<char> token) => CreateFromToken(token.ToString());

	/// <summary>
	/// Creates a <see cref="Grid"/> instance using the specified token of length 54.
	/// </summary>
	/// <param name="token">Indicates the token.</param>
	/// <returns>A <see cref="Grid"/> result.</returns>
	/// <exception cref="FormatException">Throws when the length of the argument mismatched.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid CreateFromToken(string token)
		=> token.Length switch
		{
			54 => Grid.Parse(
				string.Concat(
					from i in Digits
					let segment = GetDigitViaToken(token[(i * 6)..((i + 1) * 6)]).ToString()
					select segment.PadLeft(9, '0')
				)
			),
			_ => throw new FormatException(string.Format(ResourceDictionary.ExceptionMessage("LengthMustBeMatched"), 54))
		};

	/// <summary>
	/// Get digit via token.
	/// </summary>
	/// <param name="s">The string.</param>
	/// <returns>The result digit.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static int GetDigitViaToken(string s)
		=> (Base32CharSpan.IndexOf(s[0]) << 25)
		+ (Base32CharSpan.IndexOf(s[1]) << 20)
		+ (Base32CharSpan.IndexOf(s[2]) << 15)
		+ (Base32CharSpan.IndexOf(s[3]) << 10)
		+ (Base32CharSpan.IndexOf(s[4]) << 5)
		+ Base32CharSpan.IndexOf(s[5]);
}
