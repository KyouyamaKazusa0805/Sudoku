namespace System.Collections.Generic
{
	/// <summary>
	/// Provides extension methods on <see cref="IReadOnlyList{T}"/>.
	/// </summary>
	/// <seealso cref="IReadOnlyList{T}"/>
	public static class ReadOnlyListExtensions
	{
		/// <summary>
		/// Returns the list that is in the range specified as two parameters called
		/// <paramref name="start"/> and <paramref name="length"/>.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">The list.</param>
		/// <param name="start">The start index.</param>
		/// <param name="length">The end index.</param>
		/// <returns>The list of the elements that is in the specified range.</returns>
		public static IReadOnlyList<T> Slice<T>(this IReadOnlyList<T> @this, int start, int length)
		{
			var array = new T[length - start];
			for (int i = start, end = start + length; i < end; i++)
			{
				array[i - start] = @this[i];
			}

			return array;
		}

		/// <summary>
		/// Find the index of an element that satisfy the specified condition.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">The list.</param>
		/// <param name="predicate">The condition.</param>
		/// <returns>
		/// The result index of that element. If the list doesn't contain any element to satisfy the condition,
		/// the method will return -1 as the result.
		/// </returns>
		public static int FindIndexOf<T>(this IReadOnlyList<T> @this, Predicate<T> predicate)
		{
			switch (@this)
			{
				case T[] array:
				{
					return Array.FindIndex(array, predicate);
				}
				case List<T> list:
				{
					return list.FindIndex(predicate);
				}
				default:
				{
					for (int index = 0, count = @this.Count; index < count; index++)
					{
						if (predicate(@this[index]))
						{
							return index;
						}
					}

					return -1;
				}
			}
		}
	}
}
