namespace System.Numerics;

/// <summary>
/// Represents a combination generator that iterations each combination of bits for the specified number of bits, and how many 1's in it.
/// </summary>
/// <typeparam name="TInteger">The type of the target integer value.</typeparam>
/// <param name="bitCount">Indicates the number of bits.</param>
/// <param name="oneCount">Indicates the number of bits set <see langword="true"/>.</param>
[DebuggerStepThrough]
[TypeImpl(TypeImplFlag.AllObjectMethods)]
public readonly ref partial struct BitCombinationGenerator<TInteger>(
	[PrimaryConstructorParameter(MemberKinds.Field)] int bitCount,
	[PrimaryConstructorParameter(MemberKinds.Field)] int oneCount
)
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
{
	/// <summary>
	/// Gets the enumerator of the current instance in order to use <see langword="foreach"/> loop.
	/// </summary>
	/// <returns>The enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BitCombinationEnumerator<TInteger> GetEnumerator() => new(_bitCount, _oneCount);
}
