using System;
using System.Collections.Generic;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Array"/>.
	/// </summary>
	/// <seealso cref="Array"/>
	public static class ArrayEx
	{
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
			g(@this.Length, count, count, new int[count], in @this, in result);
			return result;

			static void g(int last, int count, int index, int[] tempArray, in T[] @this, in List<T[]> resultList)
			{
				for (int i = last; i >= index; i--)
				{
					tempArray[index - 1] = i - 1;
					if (index > 1)
					{
						g(i - 1, count, index - 1, tempArray, in @this, in resultList);
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
