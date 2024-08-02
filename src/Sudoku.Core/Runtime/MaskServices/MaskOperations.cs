namespace Sudoku.Runtime.MaskServices;

/// <summary>
/// Provides with a set of methods that operates with mask defined in basic sudoku concepts, as data structures.
/// </summary>
public static class MaskOperations
{
	/// <summary>
	/// Creates for a <see cref="Mask"/> instance via the specified digits.
	/// </summary>
	/// <param name="digits">
	/// <para>Indicates the digits to assign.</para>
	/// <include file="../../global-doc-comments.xml" path="//g/csharp12/feature[@name='params-collections']/target[@name='parameter']"/>
	/// </param>
	/// <returns>A <see cref="Mask"/> instance.</returns>
	public static Mask Create(params ReadOnlySpan<Digit> digits)
	{
		var result = (Mask)0;
		foreach (var digit in digits)
		{
			result |= (Mask)(1 << digit);
		}
		return result;
	}

	/// <inheritdoc cref="Create(ReadOnlySpan{Digit})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Mask Create(Digit[] digits) => Create(digits.AsReadOnlySpan());

	/// <typeparam name="TDigits">The type of the enumerable sequence.</typeparam>
	/// <inheritdoc cref="Create(ReadOnlySpan{Digit})"/>
	public static Mask Create<TDigits>(TDigits digits) where TDigits : IEnumerable<Digit>, allows ref struct
	{
		var result = (Mask)0;
		foreach (var digit in digits)
		{
			result |= (Mask)(1 << digit);
		}
		return result;
	}

	/// <summary>
	/// To get the digits that the current mask represents for. The mask must be between 0 and 512, and exclude 512.
	/// </summary>
	/// <param name="digitMask">The digit mask.</param>
	/// <returns>The digits returned.</returns>
	public static ReadOnlySpan<Digit> MaskToDigits(Mask digitMask)
	{
		var result = new Digit[Mask.PopCount(digitMask)];
		for (var (i, p) = (0, 0); i < 9; i++)
		{
			if ((digitMask >> i & 1) != 0)
			{
				result[p++] = i;
			}
		}
		return result;
	}

	/// <summary>
	/// To get the sudoku type for the specified cell mask inside a <see cref="Grid"/>.
	/// </summary>
	/// <param name="mask">The cell mask.</param>
	/// <returns>The sudoku type configured.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static SudokuType MaskToSudokuType(Mask mask)
		=> (mask >> Grid.HeaderShift << Grid.HeaderShift) switch { 0 => SudokuType.Standard, var resultMask => (SudokuType)resultMask };

	/// <summary>
	/// To get the cell state for a mask value. The mask is an inner representation to describe a cell's state.
	/// For more information please visit the details of the design for type <see cref="Grid"/>.
	/// </summary>
	/// <param name="mask">The mask.</param>
	/// <returns>The cell state.</returns>
	/// <seealso cref="Grid"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellState MaskToCellState(Mask mask) => (CellState)(mask >> 9 & 7);

	/// <summary>
	/// Try to split a mask into 3 parts, 3-bit as a unit.
	/// </summary>
	/// <param name="this">The mask instance to be split.</param>
	/// <returns>A triplet of values.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (int High, int Mid, int Low) SplitMask(this Mask @this) => (@this >> 6 & 7, @this >> 3 & 7, @this & 7);
}
