namespace System.Numerics;

/// <summary>
/// Indicates the enumerator of the current instance.
/// </summary>
/// <param name="bitCount">The number of bits.</param>
/// <param name="oneCount">The number of <see langword="true"/> bits.</param>
public ref struct MaskCombinationEnumerator(int bitCount, int oneCount) : IEnumerator<long>
{
	/// <summary>
	/// The mask.
	/// </summary>
	private readonly long _mask = (1 << bitCount - oneCount) - 1;

	/// <summary>
	/// Indicates whether that the value is the last one.
	/// </summary>
	private bool _isLast = bitCount == 0;


	/// <inheritdoc cref="IEnumerator.Current"/>
	public long Current { get; private set; } = (1 << oneCount) - 1;

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
		_isLast = (Current & -Current & _mask) == 0;
		return result;
	}
}
