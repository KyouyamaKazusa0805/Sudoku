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
		foreach (var digit in digits)
		{
			result |= (Mask)(1 << digit);
		}

		return result;
	}

	/// <inheritdoc cref="Create(Digit[])"/>
	public static Mask Create(HashSet<Digit> digits)
	{
		var result = (Mask)0;
		foreach (var digit in digits)
		{
			result |= (Mask)(1 << digit);
		}

		return result;
	}
}
