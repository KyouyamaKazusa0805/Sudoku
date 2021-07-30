using System.Collections.Generic;

namespace System.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Span{T}"/>.
	/// </summary>
	/// <seealso cref="Span{T}"/>
	public static class SpanEx
	{
		/// <summary>
		/// The select method used in <see langword="from"/>-<see langword="in"/>-<see langword="select"/>
		/// clause.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <typeparam name="TResult">The result type.</typeparam>
		/// <param name="this">The list.</param>
		/// <param name="selector">The selector that is used for conversion.</param>
		/// <returns>The array of target result elements.</returns>
		/// <example>
		/// For example:
		/// <code>
		/// <see langword="int"/>[] selection = <see langword="from"/> digit <see langword="in"/> 17.GetAllSets() <see langword="select"/> digit + 1;
		/// </code>
		/// </example>
		public static TResult[] Select<T, TResult>(this in Span<T> @this, Func<T, TResult> selector)
		{
			var result = new TResult[@this.Length];
			int i = 0;
			foreach (var element in @this)
			{
				result[i++] = selector(element);
			}

			return result;
		}

		/// <summary>
		/// Get all subsets from the specified number of the values to take.
		/// </summary>
		/// <param name="this">The array.</param>
		/// <param name="count">The number of elements you want to take.</param>
		/// <returns>All subsets.</returns>
		public static IEnumerable<T[]> GetSubsets<T>(this in Span<T> @this, int count)
		{
			if (count == 0)
			{
				return Array.Empty<T[]>();
			}

			var result = new List<T[]>();
			g(@this.Length, count, count, stackalloc int[count], @this, result);
			return result;


			static void g(
				int last, int count, int index, in Span<int> tempArray,
				in Span<T> @this, in IList<T[]> resultList)
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
						for (int j = 0, length = tempArray.Length; j < length; j++)
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
