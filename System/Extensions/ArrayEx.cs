using System.Collections.Generic;

namespace System.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Array"/>.
	/// </summary>
	/// <seealso cref="Array"/>
	public static class ArrayEx
	{
		/// <summary>
		/// Sort the specified array.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The array.</param>
		/// <param name="comparer">The method to compare two elements.</param>
		/// <remarks>
		/// If you want to use this method, please note that the <typeparamref name="T"/> may not be the built-in
		/// types such as <see cref="int"/>, <see cref="float"/> or so on, because they can use operators directly.
		/// </remarks>
		public static unsafe void Sort<T>(this T[] @this, delegate*<in T, in T, int> comparer)
		{
			q(0, @this.Length - 1, @this, comparer);

			static void q(int l, int r, T[] @this, delegate*<in T, in T, int> comparer)
			{
				if (l < r)
				{
					int i = l, j = r - 1;
					var middle = @this[(l + r) / 2];
					while (true)
					{
						while (i < r && comparer(@this[i], middle) < 0) i++;
						while (j > 0 && comparer(@this[j], middle) > 0) j--;
						if (i == j) break;

						var temp = @this[i];
						@this[i] = @this[j];
						@this[j] = temp;

						if (comparer(@this[i], @this[j]) == 0) j--;
					}

					q(l, i, @this, comparer);
					q(i + 1, r, @this, comparer);
				}
			}
		}

		/// <summary>
		/// Get all subsets from the specified number of the values to take.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The array.</param>
		/// <param name="count">The number of elements you want to take.</param>
		/// <returns>All subsets.</returns>
		public static IEnumerable<T[]> GetSubsets<T>(this T[] @this, int count)
		{
			if (count == 0)
			{
				return Array.Empty<T[]>();
			}

			var result = new List<T[]>();
			g(@this.Length, count, count, stackalloc int[count], @this, result);
			return result;

			static void g(
				int last, int count, int index, in Span<int> tempArray, in T[] @this, in IList<T[]> resultList)
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
