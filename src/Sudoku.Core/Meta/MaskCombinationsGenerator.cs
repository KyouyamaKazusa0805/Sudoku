namespace Sudoku.Meta;

/// <summary>
/// Represents a combination generator that iterations each combination of bits for the specified number of bits, and how many 1's in it.
/// </summary>
/// <param name="bitCount">The number of bits.</param>
/// <param name="oneCount">The number of <see langword="true"/> bits.</param>
public readonly ref partial struct MaskCombinationsGenerator(int bitCount, int oneCount)
{
	/// <summary>
	/// Gets the enumerator of the current instance in order to use <see langword="foreach"/> loop.
	/// </summary>
	/// <returns>The enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(bitCount, oneCount);
}
