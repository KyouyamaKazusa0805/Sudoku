namespace System.Numerics;

/// <summary>
/// Indicates the enumerator of the current instance.
/// </summary>
/// <typeparam name="T">The type of the target value.</typeparam>
/// <param name="bitCount">The number of bits.</param>
/// <param name="oneCount">The number of <see langword="true"/> bits.</param>
public ref struct BitCombinationEnumerator<T>(int bitCount, int oneCount) : IEnumerator<T> where T : IBinaryInteger<T>
{
	/// <summary>
	/// The mask.
	/// </summary>
	private readonly T _mask = (T.One << bitCount - oneCount) - T.One;

	/// <summary>
	/// Indicates whether that the value is the last one.
	/// </summary>
	private bool _isLast = bitCount == 0;


	/// <inheritdoc cref="IEnumerator.Current"/>
	public T Current { get; private set; } = (T.One << oneCount) - T.One;

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

	/// <inheritdoc/>
	readonly void IDisposable.Dispose() { }

	/// <summary>
	/// Changes the state of the fields, and check whether the bit has another available possibility to be iterated.
	/// </summary>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool HasNext()
	{
		var result = !_isLast;
		_isLast = (Current & -Current & _mask) == T.Zero;
		return result;
	}
}
