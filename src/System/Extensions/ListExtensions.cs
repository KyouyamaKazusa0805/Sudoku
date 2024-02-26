namespace System.Collections.Generic;

/// <summary>
/// Provides extension methods on <see cref="List{T}"/>.
/// </summary>
/// <seealso cref="List{T}"/>
public static class ListExtensions
{
	/// <summary>
	/// Removes the last element stored in the current <see cref="List{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The list to be modified.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Remove<T>(this List<T> @this) => @this.RemoveAt(@this.Count - 1);

	/// <inheritdoc cref="List{T}.RemoveAt(int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemoveAt<T>(this List<T> @this, Index index) => @this.RemoveAt(index.GetOffset(@this.Count));

	/// <summary>
	/// Determines whether two sequences are equal by comparing the elements by using <see cref="IEquatable{T}.Equals(T)"/> for their type.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">A <see cref="List{T}"/> to compare to <paramref name="other"/>.</param>
	/// <param name="other">A <see cref="List{T}"/> to compare to <paramref name="this"/>.</param>
	/// <returns>
	/// <see langword="true"/> if the two source sequences are of equal length and their correpsonding elements are equal according
	/// to <see cref="IEquatable{T}.Equals(T)"/> for their type; otherwise, <see langword="false"/>.
	/// </returns>
	public static bool SequenceEqual<T>(this List<T> @this, List<T> other) where T : IEquatable<T>
	{
		if (@this.Count != other.Count)
		{
			return false;
		}

		scoped var leftSpan = @this.AsReadOnlySpan();
		scoped var rightSpan = other.AsReadOnlySpan();
		for (var i = 0; i < @this.Count; i++)
		{
			if (!leftSpan[i].Equals(rightSpan[i]))
			{
				return false;
			}
		}

		return true;
	}

	/// <inheritdoc cref="CollectionsMarshal.AsSpan{T}(List{T}?)"/>
	/// <param name="this">The instance to be transformed.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<T> AsSpan<T>(this List<T> @this) => CollectionsMarshal.AsSpan(@this);

	/// <summary>
	/// Gets a <see cref="ReadOnlySpan{T}"/> view over the data in a list. Items should not be added or removed from the <see cref="List{T}"/>
	/// while the <see cref="ReadOnlySpan{T}"/> is in use.
	/// </summary>
	/// <param name="this">The instance to be transformed.</param>
	/// <returns>A <see cref="ReadOnlySpan{T}"/> instance over the <see cref="List{T}"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<T> AsReadOnlySpan<T>(this List<T> @this) => CollectionsMarshal.AsSpan(@this);
}
