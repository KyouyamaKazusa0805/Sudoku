namespace System.Collections.Generic;

/// <summary>
/// Provides with extension methods on <see cref="List{T}"/>.
/// </summary>
/// <seealso cref="List{T}"/>
public static class ListExtensions
{
	/// <summary>
	/// Removes the last element of the list, and return that removed element.
	/// </summary>
	/// <typeparam name="T">The type of the element.</typeparam>
	/// <param name="this">The removed list.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Remove<T>(this List<T> @this)
	{
		var result = @this[^1];
		@this.RemoveAt(@this.Count - 1);
		return result;
	}

	/// <summary>
	/// Slices the list.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The list.</param>
	/// <param name="startIndex">The desired start index.</param>
	/// <param name="count">The desired number of elements.</param>
	/// <returns>The sliced list.</returns>
	public static List<T> Slice<T>(this List<T> @this, int startIndex, int count)
	{
		var result = new List<T>(count);
		for (int i = startIndex, j = 0; j < count; i++, j++)
		{
			result.Add(@this[i]);
		}

		return result;
	}

	/// <summary>
	/// Creates a new <see cref="List{T}"/> instance, with all elements in the current instance, except the element at the specified index.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The list.</param>
	/// <param name="index">The desired index.</param>
	/// <returns>The target list.</returns>
	public static List<T> CopyExcept<T>(this List<T> @this, int index)
	{
		var result = new List<T>(@this.Count - 1);
		for (var i = 0; i < index; i++)
		{
			result.Add(@this[i]);
		}
		for (var i = index + 1; i < @this.Count; i++)
		{
			result.Add(@this[i]);
		}

		return result;
	}

	/// <inheritdoc cref="ImmutableArray.ToImmutableArray{TSource}(IEnumerable{TSource})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ImmutableArray<T> ToImmutableArray<T>(this List<T> @this) => ImmutableArray.Create(@this.ToArray());
}
