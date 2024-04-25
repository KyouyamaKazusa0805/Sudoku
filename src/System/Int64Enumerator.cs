namespace System.Numerics;

/// <summary>
/// Represents an enumerator that iterates a <see cref="long"/> or <see cref="ulong"/> value.
/// </summary>
/// <param name="value">The value to be iterated.</param>
[StructLayout(LayoutKind.Auto)]
public ref struct Int64Enumerator(ulong value)
{
	/// <summary>
	/// Indicates the population count of the value.
	/// </summary>
	public readonly int PopulationCount => PopCount(value);

	/// <summary>
	/// Indicates the bits set.
	/// </summary>
	public readonly ReadOnlySpan<int> Bits => value.GetAllSets();

	/// <inheritdoc cref="IEnumerator{T}.Current"/>
	public int Current { get; private set; } = -1;


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
}
