namespace System.Linq;

/// <summary>
/// Represents LINQ methods used by <see cref="HashSet{T}"/> instances.
/// </summary>
/// <seealso cref="HashSet{T}"/>
public static class HashSetEnumerable
{
	/// <summary>
	/// Indicates the first element of the collection.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The collection.</param>
	/// <returns>The first element of the collection.</returns>
	/// <exception cref="InvalidOperationException">Throws when the collection contains no elements.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T First<T>(this HashSet<T> @this)
	{
		var enumerator = @this.GetEnumerator();
		return enumerator.MoveNext()
			? enumerator.Current
			: throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("NoElementsFoundInCollection"));
	}
}
