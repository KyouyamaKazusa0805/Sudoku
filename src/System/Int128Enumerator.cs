namespace System.Numerics;

/// <summary>
/// Represents an enumerator that iterates a <see cref="llong"/> or <see cref="ullong"/> value.
/// </summary>
/// <param name="value">The value to be iterated.</param>
[StructLayout(LayoutKind.Auto)]
public ref struct Int128Enumerator(ullong value) : IEnumerator<int>
{
	/// <summary>
	/// Indicates the population count of the value.
	/// </summary>
	public readonly int PopulationCount
	{
		get
		{
			var (upper, lower) = ((ulong)(value >>> 64), (ulong)(value & ulong.MaxValue));
			return PopCount(upper) + PopCount(lower);
		}
	}

	/// <summary>
	/// Indicates the bits set.
	/// </summary>
	public readonly ReadOnlySpan<int> Bits => value.GetAllSets();

	/// <inheritdoc cref="IEnumerator{T}.Current"/>
	public int Current { get; private set; } = -1;

	/// <inheritdoc/>
	readonly object IEnumerator.Current => Current;


	/// <inheritdoc cref="BitOperationsExtensions.SetAt(uint, int)"/>
	public readonly int this[int index] => value.SetAt(index);


	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	public bool MoveNext()
	{
		while (++Current < 64)
		{
			if ((value >> Current & 1) != 0)
			{
				return true;
			}
		}
		return false;
	}

	/// <inheritdoc/>
	readonly void IDisposable.Dispose() { }

	/// <inheritdoc/>
	[DoesNotReturn]
	readonly void IEnumerator.Reset() => throw new NotImplementedException();
}
