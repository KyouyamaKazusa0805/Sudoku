namespace Sudoku.Algorithm.MinLex;

/// <summary>
/// Represents for a pattern for a grid.
/// </summary>
[LargeStructure]
public unsafe struct GridPattern
{
#pragma warning disable format
	internal static readonly int[] MinCanNineBits = [
		0, 1, 1, 3, 1, 3, 3, 7, 1, 9, 9, 11, 9, 11, 11, 15, 1, 9, 9, 11, 9, 11, 11, 15, 3, 11, 11, 27, 11,
		27, 27, 31, 1, 9, 9, 11, 9, 11, 11, 15, 3, 11, 11, 27, 11, 27, 27, 31, 3, 11, 11, 27, 11, 27, 27,
		31, 7, 15, 15, 31, 15, 31, 31, 63, 1, 9, 9, 11, 9, 11, 11, 15, 9, 73, 73, 75, 73, 75, 75, 79, 9,
		73, 73, 75, 73, 75, 75, 79, 11, 75, 75, 91, 75, 91, 91, 95, 9, 73, 73, 75, 73, 75, 75, 79, 11, 75,
		75, 91, 75, 91, 91, 95, 11, 75, 75, 91, 75, 91, 91, 95, 15, 79, 79, 95, 79, 95, 95, 127, 1, 9, 9,
		11, 9, 11, 11, 15, 9, 73, 73, 75, 73, 75, 75, 79, 9, 73, 73, 75, 73, 75, 75, 79, 11, 75, 75, 91,
		75, 91, 91, 95, 9, 73, 73, 75, 73, 75, 75, 79, 11, 75, 75, 91, 75, 91, 91, 95, 11, 75, 75, 91, 75,
		91, 91, 95, 15, 79, 79, 95, 79, 95, 95, 127, 3, 11, 11, 27, 11, 27, 27, 31, 11, 75, 75, 91, 75, 91,
		91, 95, 11, 75, 75, 91, 75, 91, 91, 95, 27, 91, 91, 219, 91, 219, 219, 223, 11, 75, 75, 91, 75, 91,
		91, 95, 27, 91, 91, 219, 91, 219, 219, 223, 27, 91, 91, 219, 91, 219, 219, 223, 31, 95, 95, 223, 95,
		223, 223, 255, 1, 9, 9, 11, 9, 11, 11, 15, 9, 73, 73, 75, 73, 75, 75, 79, 9, 73, 73, 75, 73, 75, 75,
		79, 11, 75, 75, 91, 75, 91, 91, 95, 9, 73, 73, 75, 73, 75, 75, 79, 11, 75, 75, 91, 75, 91, 91, 95,
		11, 75, 75, 91, 75, 91, 91, 95, 15, 79, 79, 95, 79, 95, 95, 127, 3, 11, 11, 27, 11, 27, 27, 31, 11,
		75, 75, 91, 75, 91, 91, 95, 11, 75, 75, 91, 75, 91, 91, 95, 27, 91, 91, 219, 91, 219, 219, 223, 11,
		75, 75, 91, 75, 91, 91, 95, 27, 91, 91, 219, 91, 219, 219, 223, 27, 91, 91, 219, 91, 219, 219, 223,
		31, 95, 95, 223, 95, 223, 223, 255, 3, 11, 11, 27, 11, 27, 27, 31, 11, 75, 75, 91, 75, 91, 91, 95,
		11, 75, 75, 91, 75, 91, 91, 95, 27, 91, 91, 219, 91, 219, 219, 223, 11, 75, 75, 91, 75, 91, 91, 95,
		27, 91, 91, 219, 91, 219, 219, 223, 27, 91, 91, 219, 91, 219, 219, 223, 31, 95, 95, 223, 95, 223,
		223, 255, 7, 15, 15, 31, 15, 31, 31, 63, 15, 79, 79, 95, 79, 95, 95, 127, 15, 79, 79, 95, 79, 95,
		95, 127, 31, 95, 95, 223, 95, 223, 223, 255, 15, 79, 79, 95, 79, 95, 95, 127, 31, 95, 95, 223, 95,
		223, 223, 255, 31, 95, 95, 223, 95, 223, 223, 255, 63, 127, 127, 255, 127, 255, 255, 511
	];
#pragma warning restore format


	/// <summary>
	/// Indicates the rows.
	/// </summary>
	public fixed int Rows[9];

	/// <summary>
	/// Indicates the digits.
	/// </summary>
	public fixed int Digits[81];


	/// <summary>
	/// Indicates the best top-row score.
	/// </summary>
	public readonly int BestTopRowScore
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var amin = MinCanNineBits[Rows[0]];
			if (amin > MinCanNineBits[Rows[1]]) { amin = MinCanNineBits[Rows[1]]; }
			if (amin > MinCanNineBits[Rows[2]]) { amin = MinCanNineBits[Rows[2]]; }
			if (amin > MinCanNineBits[Rows[3]]) { amin = MinCanNineBits[Rows[3]]; }
			if (amin > MinCanNineBits[Rows[4]]) { amin = MinCanNineBits[Rows[4]]; }
			if (amin > MinCanNineBits[Rows[5]]) { amin = MinCanNineBits[Rows[5]]; }
			if (amin > MinCanNineBits[Rows[6]]) { amin = MinCanNineBits[Rows[6]]; }
			if (amin > MinCanNineBits[Rows[7]]) { amin = MinCanNineBits[Rows[7]]; }
			if (amin > MinCanNineBits[Rows[8]]) { amin = MinCanNineBits[Rows[8]]; }

			return amin;
		}
	}


	/// <inheritdoc cref="FromStringUnsafe(string, GridPattern*, GridPattern*)"/>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="text"/> does not have 81 characters.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void FromString(string text, out GridPattern normal, out GridPattern transposed)
	{
		ArgumentOutOfRangeException.ThrowIfNotEqual(text.Length, 81);

		Unsafe.SkipInit(out normal);
		Unsafe.SkipInit(out transposed);
		fixed (GridPattern* pNormal = &normal, pTransposed = &transposed)
		{
			FromStringUnsafe(text, pNormal, pTransposed);
		}
	}

	/// <summary>
	/// Loads a string text, parsing the data and returns two <see cref="GridPattern"/> results
	/// indicating the data equivalent to the grid.
	/// </summary>
	/// <param name="text">The text.</param>
	/// <param name="pair">The pair of pointers indicating the normal and transposed cases. <i><b>The length must be 2.</b></i></param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void FromStringUnsafe(string text, GridPattern* pair) => FromStringUnsafe(text, pair, pair + 1);

	/// <summary>
	/// Loads a string text, parsing the data and returns two <see cref="GridPattern"/> results
	/// indicating the data equivalent to the grid.
	/// </summary>
	/// <param name="text">The text.</param>
	/// <param name="normal">The normal converted data.</param>
	/// <param name="transposed">The transposed converted data.</param>
	public static void FromStringUnsafe(string text, GridPattern* normal, GridPattern* transposed)
	{
		normal->Rows[0] = 0; normal->Rows[1] = 0; normal->Rows[2] = 0;
		normal->Rows[3] = 0; normal->Rows[4] = 0; normal->Rows[5] = 0;
		normal->Rows[6] = 0; normal->Rows[7] = 0; normal->Rows[8] = 0;
		transposed->Rows[0] = 0; transposed->Rows[1] = 0; transposed->Rows[2] = 0;
		transposed->Rows[3] = 0; transposed->Rows[4] = 0; transposed->Rows[5] = 0;
		transposed->Rows[6] = 0; transposed->Rows[7] = 0; transposed->Rows[8] = 0;

		for (var cell = 0; cell < 81; cell++)
		{
			var (row, column) = (cell / 9, cell % 9);
			if (text[cell] is var c and >= '1' and <= '9')
			{
				normal->Rows[row] |= 1 << column;
				transposed->Rows[column] |= 1 << row;
				normal->Digits[row * 9 + column] = c - '0';
				transposed->Digits[column * 9 + row] = c - '0';
			}
			else
			{
				normal->Digits[row * 9 + column] = 0;
				transposed->Digits[column * 9 + row] = 0;
			}
		}
	}
}
