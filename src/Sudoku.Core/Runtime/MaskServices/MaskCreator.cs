namespace Sudoku.Runtime.MaskServices;

/// <summary>
/// Represents a type that creates for a <see cref="Mask"/> value.
/// </summary>
public static class MaskCreator
{
	/// <inheritdoc cref="Create(Digit[])"/>
	public static Mask Create(scoped ReadOnlySpan<Digit> digits)
	{
		var result = (Mask)0;

#if SIMD
		if (Vector<Digit>.IsSupported)
		{
			var vResult = Vector<Digit>.Zero;
			var i = 0;
			var lastBlockIndex = digits.Length - digits.Length % Vector<Digit>.Count;
			while (i < lastBlockIndex)
			{
				vResult |= new Vector<Digit>(digits[i..]);
				i += Vector<Digit>.Count;
			}

			for (var n = 0; n < Vector<Digit>.Count; n++)
			{
				result |= (Mask)vResult[n];
			}

			while (i < digits.Length)
			{
				result |= (Mask)digits[i];
				i += 1;
			}

			return result;
		}
#endif

		foreach (var digit in digits)
		{
			result |= (Mask)(1 << digit);
		}

		return result;
	}

	/// <summary>
	/// Creates for a <see cref="Mask"/> instance via the specified digits.
	/// </summary>
	/// <param name="digits">The digits.</param>
	/// <returns>A <see cref="Mask"/> instance.</returns>
	public static Mask Create(Digit[] digits)
	{
		var result = (Mask)0;

#if SIMD
		if (Vector<Digit>.IsSupported)
		{
			var vResult = Vector<Digit>.Zero;
			var i = 0;
			var lastBlockIndex = digits.Length - digits.Length % Vector<Digit>.Count;
			while (i < lastBlockIndex)
			{
				vResult |= new Vector<Digit>(digits[i..]);
				i += Vector<Digit>.Count;
			}

			for (var n = 0; n < Vector<Digit>.Count; n++)
			{
				result |= (Mask)vResult[n];
			}

			while (i < digits.Length)
			{
				result |= (Mask)digits[i];
				i += 1;
			}

			return result;
		}
#endif

		foreach (var digit in digits)
		{
			result |= (Mask)(1 << digit);
		}

		return result;
	}

	/// <inheritdoc cref="Create(Digit[])"/>
	public static Mask Create(HashSet<Digit> digits)
	{
#if SIMD
		// Hash sets does not contain indexers and to array method. The compiler cannot optimize them.
#endif
		var result = (Mask)0;
		foreach (var digit in digits)
		{
			result |= (Mask)(1 << digit);
		}

		return result;
	}
}
