namespace System.Algorithm;

/// <summary>
/// Provides operations on Combinatorics.
/// </summary>
public static class Combinatorics
{
	/// <summary>
	/// Get all subsets from the specified number of the values to take.
	/// </summary>
	/// <param name="this">The array.</param>
	/// <param name="count">The number of elements you want to take.</param>
	/// <returns>All subsets.</returns>
	public static IReadOnlyCollection<T[]> GetSubsets<T>(this scoped Span<T> @this, int count)
	{
		if (count == 0)
		{
			return Array.Empty<T[]>();
		}

		var result = new List<T[]>();
		g(@this.Length, count, count, stackalloc int[count], @this, result);
		return result;


		static void g(int last, int count, int index, scoped Span<int> tempArray, scoped Span<T> @this, in List<T[]> resultList)
		{
			for (var i = last; i >= index; i--)
			{
				tempArray[index - 1] = i - 1;
				if (index > 1)
				{
					g(i - 1, count, index - 1, tempArray, @this, resultList);
				}
				else
				{
					var temp = new T[count];
					for (var j = 0; j < tempArray.Length; j++)
					{
						temp[j] = @this[tempArray[j]];
					}

					resultList.Add(temp);
				}
			}
		}
	}

	/// <summary>
	/// Get all subsets from the specified number of the values to take.
	/// </summary>
	/// <param name="this">The array.</param>
	/// <param name="count">The number of elements you want to take.</param>
	/// <returns>All subsets.</returns>
	public static IReadOnlyCollection<T[]> GetSubsets<T>(this scoped ReadOnlySpan<T> @this, int count)
	{
		if (count == 0)
		{
			return Array.Empty<T[]>();
		}

		var result = new List<T[]>();
		g(@this.Length, count, count, stackalloc int[count], @this, result);
		return result;


		static void g(int last, int count, int index, scoped Span<int> tempArray, scoped ReadOnlySpan<T> @this, in List<T[]> resultList)
		{
			for (var i = last; i >= index; i--)
			{
				tempArray[index - 1] = i - 1;
				if (index > 1)
				{
					g(i - 1, count, index - 1, tempArray, @this, resultList);
				}
				else
				{
					var temp = new T[count];
					for (var j = 0; j < tempArray.Length; j++)
					{
						temp[j] = @this[tempArray[j]];
					}

					resultList.Add(temp);
				}
			}
		}
	}

	/// <summary>
	/// Get all subsets that each element is chosen at most once.
	/// Note that the null set <c>{ }</c> doesn't belong to the result.
	/// </summary>
	/// <param name="this">The array of elements.</param>
	/// <returns>
	/// The subsets of the list. For example, if the input array is <c>{ 1, 2, 3 }</c>, the output
	/// should be as follows:
	/// <list type="bullet">
	/// <item><c>{ 1 }</c></item>
	/// <item><c>{ 2 }</c></item>
	/// <item><c>{ 3 }</c></item>
	/// <item><c>{ 1, 2 }</c></item>
	/// <item><c>{ 1, 3 }</c></item>
	/// <item><c>{ 2, 3 }</c></item>
	/// <item><c>{ 1, 2, 3 }</c></item>
	/// </list>
	/// 7 cases (without null set) in total.
	/// </returns>
	public static IEnumerable<T[]> GetSubsets<T>(this IReadOnlyList<T> @this)
	{
		for (var size = 1; size <= @this.Count; size++)
		{
			foreach (var subset in @this.GetSubsets(size))
			{
				yield return subset;
			}
		}
	}

	/// <summary>
	/// Get all subsets from the specified number of the values to take.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The array.</param>
	/// <param name="count">The number of elements you want to take.</param>
	/// <returns>
	/// The subsets of the list. For example, if the input array is <c>{ 1, 2, 3 }</c> and
	/// the argument <paramref name="count"/> is 2, the output should be as follows:
	/// <list type="table">
	/// <item><c>{ 1, 2 }</c>,</item>
	/// <item><c>{ 1, 3 }</c>,</item>
	/// <item><c>{ 2, 3 }</c></item>
	/// </list>
	/// 3 cases in total.
	/// </returns>
	public static IEnumerable<T[]> GetSubsets<T>(this IReadOnlyList<T> @this, int count)
	{
		if (count == 0)
		{
			return Array.Empty<T[]>();
		}

		var result = new List<T[]>();
		g(@this.Count, count, count, stackalloc int[count], @this, result);
		return result;


		static void g(int last, int count, int index, scoped Span<int> tempArray, IReadOnlyList<T> @this, in List<T[]> resultList)
		{
			for (var i = last; i >= index; i--)
			{
				tempArray[index - 1] = i - 1;
				if (index > 1)
				{
					g(i - 1, count, index - 1, tempArray, @this, resultList);
				}
				else
				{
					var temp = new T[count];
					for (var j = 0; j < tempArray.Length; j++)
					{
						temp[j] = @this[tempArray[j]];
					}

					resultList.Add(temp);
				}
			}
		}
	}

	/// <summary>
	/// Get all combinations that each sub-array only choose one.
	/// </summary>
	/// <param name="this">The jigsaw array.</param>
	/// <returns>
	/// All combinations that each sub-array choose one. For example, if the array is
	/// <c>{ { 1, 2, 3 }, { 1, 3 }, { 1, 4, 7, 10 } }</c>, all combinations are:
	/// <list type="table">
	/// <item><c>{ 1, 1, 1 }</c>, <c>{ 1, 1, 4 }</c>, <c>{ 1, 1, 7 }</c>, <c>{ 1, 1, 10 }</c>,</item>
	/// <item><c>{ 1, 3, 1 }</c>, <c>{ 1, 3, 4 }</c>, <c>{ 1, 3, 7 }</c>, <c>{ 1, 3, 10 }</c>,</item>
	/// <item><c>{ 2, 1, 1 }</c>, <c>{ 2, 1, 4 }</c>, <c>{ 2, 1, 7 }</c>, <c>{ 2, 1, 10 }</c>,</item>
	/// <item><c>{ 2, 3, 1 }</c>, <c>{ 2, 3, 4 }</c>, <c>{ 2, 3, 7 }</c>, <c>{ 2, 3, 10 }</c>,</item>
	/// <item><c>{ 3, 1, 1 }</c>, <c>{ 3, 1, 4 }</c>, <c>{ 3, 1, 7 }</c>, <c>{ 3, 1, 10 }</c>,</item>
	/// <item><c>{ 3, 3, 1 }</c>, <c>{ 3, 3, 4 }</c>, <c>{ 3, 3, 7 }</c>, <c>{ 3, 3, 10 }</c></item>
	/// </list>
	/// 24 cases in total.
	/// </returns>
	/// <remarks>
	/// Please note that each return values unit (an array) contains the same number of elements
	/// with the whole array.
	/// </remarks>
	[SkipLocalsInit]
	public static unsafe IEnumerable<T[]> GetExtractedCombinations<T>(this T[][] @this)
	{
		int length = @this.Length, resultCount = 1;
		var tempArray = stackalloc int[length];
		for (var i = 0; i < length; i++)
		{
			tempArray[i] = -1;
			resultCount *= @this[i].Length;
		}

		var result = new T[resultCount][];
		int m = -1, n = -1;
		do
		{
			if (m < length - 1)
			{
				m++;
			}

			var value = tempArray + m;
			(*value)++;
			if (*value > @this[m].Length - 1)
			{
				*value = -1;
				m -= 2; // Backtrack.
			}

			if (m == length - 1)
			{
				n++;
				result[n] = new T[m + 1];
				for (var i = 0; i <= m; i++)
				{
					result[n][i] = @this[i][tempArray[i]];
				}
			}
		} while (m >= -1);

		return result;
	}
}
