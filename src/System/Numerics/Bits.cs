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
	/// <typeparam name="T">The type of target value.</typeparam>
	/// <param name="bitCount">Indicates how many bits should be enumerated.</param>
	/// <param name="oneCount">Indicates how many bits set one contained in the value.</param>
	/// <returns>A <see cref="BitCombinationGenerator{T}"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BitCombinationGenerator<T> EnumerateOf<T>(int bitCount, int oneCount)
#if NUMERIC_GENERIC_TYPE
		where T : IBinaryInteger<T>
#else
		where T :
			IAdditionOperators<T, T, T>,
			IAdditiveIdentity<T, T>,
			IBitwiseOperators<T, T, T>,
			IDivisionOperators<T, T, T>,
			IEqualityOperators<T, T, bool>,
			IMultiplicativeIdentity<T, T>,
			IUnaryNegationOperators<T, T>,
			IShiftOperators<T, int, T>,
			ISubtractionOperators<T, T, T>
#endif
		=> new(bitCount, oneCount);
}
