using System.Collections.Generic;

namespace System.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="IReadOnlyList{T}"/>.
	/// </summary>
	/// <seealso cref="IReadOnlyList{T}"/>
	public static class ReadOnlyListEx
	{
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
