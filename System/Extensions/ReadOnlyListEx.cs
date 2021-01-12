using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="IReadOnlyList{T}"/>.
	/// </summary>
	/// <seealso cref="IReadOnlyList{T}"/>
	public static class ReadOnlyListEx
	{
		/// <summary>
		/// Returns the list that is in the range specified as two parameters called
		/// <paramref name="start"/> and <paramref name="end"/>.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The list.</param>
		/// <param name="start">The start index.</param>
		/// <param name="end">The end index.</param>
		/// <returns>The list of the elements that is in the specified range.</returns>
		public static IEnumerable<T> Slice<T>(this IReadOnlyList<T> @this, int start, int end)
		{
			for (int i = start; i < end; i++)
			{
				yield return @this[i];
			}
		}

		/// <summary>
		/// Returns the list that is in the range specified as a <see cref="Range"/> instance.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The list.</param>
		/// <param name="range">(<see langword="in"/> parameter) The range.</param>
		/// <returns>The list of the elements that is in the specified range.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> Slice<T>(this IReadOnlyList<T> @this, in Range range) =>
			@this.Slice(range.Start.GetOffset(@this.Count), range.End.GetOffset(@this.Count));

		/// <summary>
		/// Find the index of an element that satisfy the specified condition.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The list.</param>
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

		/// <summary>
		/// Get all subsets from the specified number of the values to take.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The array.</param>
		/// <param name="count">The number of elements you want to take.</param>
		/// <returns>All subsets.</returns>
		public static IEnumerable<T[]> GetSubsets<T>(this IReadOnlyList<T> @this, int count)
		{
			if (count == 0)
			{
				return Array.Empty<T[]>();
			}

			var result = new List<T[]>();
			g(@this.Count, count, count, stackalloc int[count], @this, result);
			return result;

			static void g(
				int last, int count, int index, in Span<int> tempArray, IReadOnlyList<T> @this,
				in IList<T[]> resultList)
			{
				for (int i = last; i >= index; i--)
				{
					tempArray[index - 1] = i - 1;
					if (index > 1)
					{
						g(i - 1, count, index - 1, tempArray, @this, resultList);
					}
					else
					{
						var temp = new T[count];
						for (int j = 0; j < tempArray.Length; j++)
						{
							temp[j] = @this[tempArray[j]];
						}

						resultList.Add(temp);
					}
				}
			}
		}
	}
}
