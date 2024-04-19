namespace System.Linq;

/// <summary>
/// Represent an enumerator type that reversely enumerates on each element in a <see cref="LinkedList{T}"/>.
/// </summary>
/// <typeparam name="T">The type of each element.</typeparam>
/// <param name="baseList">The base list.</param>
/// <seealso cref="LinkedList{T}"/>
public readonly ref partial struct LinkedListReversed<T>(LinkedList<T> baseList)
{
	/// <inheritdoc/>
	public int Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => baseList.Count;
	}


	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(baseList);

	/// <summary>
	/// Reverses the enumeration on each element. The method will directly return the base <see cref="LinkedList{T}"/> instance.
	/// </summary>
	/// <returns>The <see cref="LinkedList{T}"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public LinkedList<T> Reverse() => baseList;
}
