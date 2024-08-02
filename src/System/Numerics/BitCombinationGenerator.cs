namespace System.Numerics;

/// <summary>
/// Represents a combination generator that iterations each combination of bits for the specified number of bits, and how many 1's in it.
/// </summary>
/// <typeparam name="T">The type of the target value.</typeparam>
/// <param name="bitCount">Indicates the number of bits.</param>
/// <param name="oneCount">Indicates the number of bits set <see langword="true"/>.</param>
[DebuggerStepThrough]
[TypeImpl(TypeImplFlag.AllObjectMethods)]
public readonly ref partial struct BitCombinationGenerator<T>(
	[PrimaryConstructorParameter(MemberKinds.Field)] int bitCount,
	[PrimaryConstructorParameter(MemberKinds.Field)] int oneCount
)
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
{
	/// <summary>
	/// Gets the enumerator of the current instance in order to use <see langword="foreach"/> loop.
	/// </summary>
	/// <returns>The enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BitCombinationEnumerator<T> GetEnumerator() => new(_bitCount, _oneCount);
}
