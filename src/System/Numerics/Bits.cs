namespace System.Numerics;

/// <summary>
/// Represents a list of methods that operates with bits.
/// </summary>
public static class Bits
{
	/// <summary>
	/// Creates a <see cref="MaskCombinationsGenerator"/> instance that can generate a list of <see cref="long"/>
	/// values that are all possibilities of combinations of integer values containing specified number of bits,
	/// and the specified number of bits set 1.
	/// </summary>
	/// <param name="bitCount">Indicates how many bits should be enumerated.</param>
	/// <param name="oneCount">Indicates how many bits set one contained in the value.</param>
	/// <returns>A <see cref="MaskCombinationsGenerator"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MaskCombinationsGenerator EnumerateOf(int bitCount, int oneCount) => new(bitCount, oneCount);
}
