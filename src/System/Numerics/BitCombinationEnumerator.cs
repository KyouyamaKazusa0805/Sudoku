namespace System.Numerics;

/// <summary>
/// Indicates the enumerator of the current instance.
/// </summary>
/// <typeparam name="TInteger">The type of the target integer value.</typeparam>
/// <param name="bitCount">The number of bits.</param>
/// <param name="oneCount">The number of <see langword="true"/> bits.</param>
[TypeImpl(
	TypeImplFlags.AllObjectMethods | TypeImplFlags.Disposable,
	OtherModifiersOnDisposableDispose = "readonly",
	ExplicitlyImplsDisposable = true)]
public ref partial struct BitCombinationEnumerator<TInteger>(int bitCount, int oneCount) : IEnumerator<TInteger>
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
	/// The mask.
	/// </summary>
	private readonly TInteger _mask = (TInteger.MultiplicativeIdentity << bitCount - oneCount) - TInteger.MultiplicativeIdentity;

	/// <summary>
	/// Indicates whether that the value is the last one.
	/// </summary>
	private bool _isLast = bitCount == 0;


	/// <inheritdoc cref="IEnumerator.Current"/>
	public TInteger Current { get; private set; } = (TInteger.MultiplicativeIdentity << oneCount) - TInteger.MultiplicativeIdentity;

	/// <inheritdoc/>
	readonly object IEnumerator.Current => Current;


	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool MoveNext()
	{
		var result = HasNext();
		if (result && !_isLast)
		{
			var smallest = Current & -Current;
			var ripple = Current + smallest;
			var ones = Current ^ ripple;
			ones = (ones >> 2) / smallest;
			Current = ripple | ones;
		}
		return result;
	}

	/// <inheritdoc/>
	[DoesNotReturn]
	readonly void IEnumerator.Reset() => throw new NotImplementedException();

	/// <summary>
	/// Changes the state of the fields, and check whether the bit has another available possibility to be iterated.
	/// </summary>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool HasNext()
	{
		var result = !_isLast;
		_isLast = (Current & -Current & _mask) == TInteger.AdditiveIdentity;
		return result;
	}
}
