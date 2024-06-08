namespace Sudoku.Concepts;

/// <summary>
/// Provides with extension methods on <see cref="CellMap"/>.
/// </summary>
/// <seealso cref="CellMap"/>
public static class CellMapToken
{
	/// <summary>
	/// Indicates the token to the current instance.
	/// </summary>
	/// <param name="this">The instance.</param>
	/// <returns>The string token.</returns>
	public static string GetToken(this scoped in CellMap @this)
	{
		var convertedString = @this.ToString(new BitmapCellMapFormatInfo());
		var bits = convertedString.Chunk(27);
		var sb = new StringBuilder(18);
		foreach (var z in (sextuple(getInteger(bits[2])), sextuple(getInteger(bits[1])), sextuple(getInteger(bits[0]))))
		{
			foreach (var element in z)
			{
				sb.Append(GridToken.Base32CharSpan[element]);
			}
		}
		return sb.ToString();


		static int getInteger(string bits)
		{
			var result = 0;
			for (var i = 0; i < 27; i++)
			{
				if (bits[i] == '1')
				{
					result |= 1 << i;
				}
			}

			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static int[] sextuple(int value)
			=> [value >> 25 & 3, value >> 20 & 31, value >> 15 & 31, value >> 10 & 31, value >> 5 & 31, value & 31];
	}


	/// <inheritdoc cref="CreateFromToken(string)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap CreateFromToken(ReadOnlySpan<char> token) => CreateFromToken(token.ToString());

	/// <summary>
	/// Creates a <see cref="CellMap"/> instance using the specified token of length 18.
	/// </summary>
	/// <param name="token">Indicates the token.</param>
	/// <returns>A <see cref="CellMap"/> result.</returns>
	/// <exception cref="FormatException">Throws when the length of the argument mismatched.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap CreateFromToken(string token)
		=> token.Length switch
		{
			18 => CellMap.Parse(
				string.Concat(
					from i in Digits[..3]
					let segment = GridToken.GetDigitViaToken(token[(i * 6)..((i + 1) * 6)]).ToString()
					let binary = Convert.ToString(int.Parse(segment), 2)
					select binary.PadLeft(27, '0')
				),
				new BitmapCellMapFormatInfo()
			),
			_ => throw new FormatException(string.Format(ResourceDictionary.ExceptionMessage("LengthMustBeMatched"), 18))
		};
}
