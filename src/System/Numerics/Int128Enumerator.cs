namespace System.Numerics;

/// <summary>
/// Represents an enumerator that iterates a <see cref="Int128"/> or <see cref="UInt128"/> value.
/// </summary>
/// <param name="_value">The value to be iterated.</param>
[TypeImpl(
	TypeImplFlags.AllObjectMethods | TypeImplFlags.Disposable,
	OtherModifiersOnDisposableDispose = "readonly",
	ExplicitlyImplsDisposable = true)]
public ref partial struct Int128Enumerator(UInt128 _value) : IEnumerator<int>
{
	/// <summary>
	/// Indicates the population count of the value.
	/// </summary>
	public readonly int PopulationCount
	{
		get
		{
			var (upper, lower) = ((ulong)(_value >>> 64), (ulong)(_value & ulong.MaxValue));
			return BitOperations.PopCount(upper) + BitOperations.PopCount(lower);
		}
	}

	/// <summary>
	/// Indicates the bits set.
	/// </summary>
	public readonly ReadOnlySpan<int> Bits => _value.GetAllSets();

	/// <inheritdoc cref="IEnumerator{T}.Current"/>
	public int Current { get; private set; } = -1;

	/// <inheritdoc/>
	readonly object IEnumerator.Current => Current;


	/// <inheritdoc cref="BitOperationsExtensions.SetAt(uint, int)"/>
	public readonly int this[int index] => _value.SetAt(index);


	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	public bool MoveNext()
	{
		while (++Current < 64)
		{
			if ((_value >> Current & 1) != 0)
			{
				return true;
			}
		}
		return false;
	}

	/// <inheritdoc/>
	[DoesNotReturn]
	readonly void IEnumerator.Reset() => throw new NotImplementedException();
}
