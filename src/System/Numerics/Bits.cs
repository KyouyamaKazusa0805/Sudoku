namespace System.Numerics;

/// <summary>
/// Represents a list of methods that operates with bits.
/// </summary>
public static class Bits
{
	/// <summary>
	/// Creates a <see cref="BitCombinationGenerator{T}"/> instance that can generate a list of <see cref="long"/>
	/// values that are all possibilities of combinations of integer values containing specified number of bits,
	/// and the specified number of bits set 1.
	/// </summary>
	/// <typeparam name="TInteger">The type of target integer value.</typeparam>
	/// <param name="bitCount">Indicates how many bits should be enumerated.</param>
	/// <param name="oneCount">Indicates how many bits set one contained in the value.</param>
	/// <returns>A <see cref="BitCombinationGenerator{T}"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BitCombinationGenerator<TInteger> EnumerateOf<TInteger>(int bitCount, int oneCount)
#if NUMERIC_GENERIC_TYPE
		where TInteger : IBinaryInteger<TInteger>
#else
		where TInteger :
			IAdditionOperators<TInteger, TInteger, TInteger>,
			IAdditiveIdentity<TInteger, TInteger>,
			IBitwiseOperators<TInteger, TInteger, TInteger>,
			IDivisionOperators<TInteger, TInteger, TInteger>,
			IEqualityOperators<TInteger, TInteger, bool>,
			IMultiplicativeIdentity<TInteger, TInteger>,
			IUnaryNegationOperators<TInteger, TInteger>,
			IShiftOperators<TInteger, int, TInteger>,
			ISubtractionOperators<TInteger, TInteger, TInteger>
#endif
		=> new(bitCount, oneCount);
}
