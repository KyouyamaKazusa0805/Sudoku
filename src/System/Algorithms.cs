namespace System;

/// <summary>
/// Provides all algorithm processing methods.
/// </summary>
public static class Algorithms
{
	/// <summary>
	/// Sort the specified array by quick sort.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The array.</param>
	/// <param name="comparer">The method to compare two elements.</param>
	/// <remarks>
	/// If you want to use this method, please note that the <typeparamref name="T"/>
	/// may not be the built-in types such as <see cref="int"/>, <see cref="float"/> or so on,
	/// because they can use operators directly.
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
		for (int size = 1, count = @this.Count; size <= count; size++)
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
					for (int j = 0, length = tempArray.Length; j < length; j++)
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
	public static unsafe IEnumerable<int[]> GetExtractedCombinations(this int[][] @this)
	{
		int length = @this.GetLength(0), resultCount = 1;
		int* tempArray = stackalloc int[length];
		for (int i = 0; i < length; i++)
		{
			tempArray[i] = -1;
			resultCount *= @this[i].Length;
		}

		int[][] result = new int[resultCount][];
		int m = -1, n = -1;
		do
		{
			if (m < length - 1)
			{
				m++;
			}

			int* value = tempArray + m;
			(*value)++;
			if (*value > @this[m].Length - 1)
			{
				*value = -1;
				m -= 2; // Backtrack.
			}

			if (m == length - 1)
			{
				n++;
				result[n] = new int[m + 1];
				for (int i = 0; i <= m; i++)
				{
					result[n][i] = @this[i][tempArray[i]];
				}
			}
		} while (m >= -1);

		return result;
	}
}
