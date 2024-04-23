namespace System;

/// <summary>
/// Extracts the type that includes the methods that operates with combinatorial values.
/// </summary>
public static class Combinatorial
{
	/// <summary>
	/// Indicates the <see href="https://en.wikipedia.org/wiki/Pascal%27s_triangle">Pascal's Triangle</see>
	/// (in Chinese: Yang Hui's Triangle), i.e. the combinatorial numbers from <c>C(1, 1)</c> to <c>C(30, 30)</c>.
	/// </summary>
	public static readonly int[][] PascalTriangle = [
		[1],
		[2, 1],
		[3, 3, 1],
		[4, 6, 4, 1],
		[5, 10, 10, 5, 1],
		[6, 15, 20, 15, 6, 1],
		[7, 21, 35, 35, 21, 7, 1],
		[8, 28, 56, 70, 56, 28, 8, 1],
		[9, 36, 84, 126, 126, 84, 36, 9, 1],
		[10, 45, 120, 210, 252, 210, 120, 45, 10, 1],
		[11, 55, 165, 330, 462, 462, 330, 165, 55, 11, 1],
		[12, 66, 220, 495, 792, 924, 792, 495, 220, 66, 12, 1],
		[13, 78, 286, 715, 1287, 1716, 1716, 1287, 715, 286, 78, 13, 1],
		[14, 91, 364, 1001, 2002, 3003, 3432, 3003, 2002, 1001, 364, 91, 14, 1],
		[15, 105, 455, 1365, 3003, 5005, 6435, 6435, 5005, 3003, 1365, 455, 105, 15, 1],
		[16, 120, 560, 1820, 4368, 8008, 11440, 12870, 11440, 8008, 4368, 1820, 560, 120, 16, 1],
		[17, 136, 680, 2380, 6188, 12376, 19448, 24310, 24310, 19448, 12376, 6188, 2380, 680, 136, 17, 1],
		[18, 153, 816, 3060, 8568, 18564, 31824, 43758, 48620, 43758, 31824, 18564, 8568, 3060, 816, 153, 18, 1],
		[19, 171, 969, 3876, 11628, 27132, 50388, 75582, 92378, 92378, 75582, 50388, 27132, 11628, 3876, 969, 171, 19, 1],
		[20, 190, 1140, 4845, 15504, 38760, 77520, 125970, 167960, 184756, 167960, 125970, 77520, 38760, 15504, 4845, 1140, 190, 20, 1],
		[21, 210, 1330, 5985, 20349, 54264, 116280, 203490, 293930, 352716, 352716, 293930, 203490, 116280, 54264, 20349, 5985, 1330, 210, 21, 1],
		[22, 231, 1540, 7315, 26334, 74613, 170544, 319770, 497420, 646646, 705432, 646646, 497420, 319770, 170544, 74613, 26334, 7315, 1540, 231, 22, 1],
		[23, 253, 1771, 8855, 33649, 100947, 245157, 490314, 817190, 1144066, 1352078, 1352078, 1144066, 817190, 490314, 245157, 100947, 33649, 8855, 1771, 253, 23, 1],
		[24, 276, 2024, 10626, 42504, 134596, 346104, 735471, 1307504, 1961256, 2496144, 2704156, 2496144, 1961256, 1307504, 735471, 346104, 134596, 42504, 10626, 2024, 276, 24, 1],
		[25, 300, 2300, 12650, 53130, 177100, 480700, 1081575, 2042975, 3268760, 4457400, 5200300, 5200300, 4457400, 3268760, 2042975, 1081575, 480700, 177100, 53130, 12650, 2300, 300, 25, 1],
		[26, 325, 2600, 14950, 65780, 230230, 657800, 1562275, 3124550, 5311735, 7726160, 9657700, 10400600, 9657700, 7726160, 5311735, 3124550, 1562275, 657800, 230230, 65780, 14950, 2600, 325, 26, 1],
		[27, 351, 2925, 17550, 80730, 296010, 888030, 2220075, 4686825, 8436285, 13037895, 17383860, 20058300, 20058300, 17383860, 13037895, 8436285, 4686825, 2220075, 888030, 296010, 80730, 17550, 2925, 351, 27, 1],
		[28, 378, 3276, 20475, 98280, 376740, 1184040, 3108105, 6906900, 13123110, 21474180, 30421755, 37442160, 40116600, 37442160, 30421755, 21474180, 13123110, 6906900, 3108105, 1184040, 376740, 98280, 20475, 3276, 378, 28, 1],
		[29, 406, 3654, 23751, 118755, 475020, 1560780, 4292145, 10015005, 20030010, 34597290, 51895935, 67863915, 77558760, 77558760, 67863915, 51895935, 34597290, 20030010, 10015005, 4292145, 1560780, 475020, 118755, 23751, 3654, 406, 29, 1],
		[30, 435, 4060, 27405, 142506, 593775, 2035800, 5852925, 14307150, 30045015, 54627300, 86493225, 119759850, 145422675, 155117520, 145422675, 119759850, 86493225, 54627300, 30045015, 14307150, 5852925, 2035800, 593775, 142506, 27405, 4060, 435, 30, 1]
	];


	/// <summary>
	/// Get all subsets from the specified number of the values to take.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The array.</param>
	/// <param name="count">The number of elements you want to take.</param>
	/// <returns>
	/// The subsets of the list.
	/// For example, if the input array is <c>[1, 2, 3]</c> and the argument <paramref name="count"/> is 2, the result will be
	/// <code>
	/// [[1, 2], [1, 3], [2, 3]]
	/// </code>
	/// 3 cases.
	/// </returns>
	public static T[][] GetSubsets<T>(this T[] @this, int count)
	{
		if (count == 0)
		{
			return [];
		}

		var result = new List<T[]>();
		g(@this.Length, count, count, stackalloc int[count], @this, result);
		return [.. result];


		static void g(int last, int count, int index, Span<int> tempArray, T[] @this, List<T[]> resultList)
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
	/// All combinations that each sub-array choose one.
	/// For example, if one array is <c>[[1, 2, 3], [1, 3], [1, 4, 7, 10]]</c>, the final combinations will be
	/// <code><![CDATA[
	/// [
	///     [1, 1, 1], [1, 1, 4], [1, 1, 7], [1, 1, 10],
	///     [1, 3, 1], [1, 3, 4], [1, 3, 7], [1, 3, 10],
	///     [2, 1, 1], [2, 1, 4], [2, 1, 7], [2, 1, 10],
	///     [2, 3, 1], [2, 3, 4], [2, 3, 7], [2, 3, 10],
	///     [3, 1, 1], [3, 1, 4], [3, 1, 7], [3, 1, 10],
	///     [3, 3, 1], [3, 3, 4], [3, 3, 7], [3, 3, 10]
	/// ]
	/// ]]></code>
	/// 24 cases.
	/// </returns>
	public static T[][] GetExtractedCombinations<T>(this T[][] @this)
	{
		var length = @this.Length;
		var resultCount = 1;
		var tempArray = (stackalloc int[length]);
		for (var i = 0; i < length; i++)
		{
			tempArray[i] = -1;
			resultCount *= @this[i].Length;
		}

		var (result, m, n) = (new T[resultCount][], -1, -1);
		do
		{
			if (m < length - 1)
			{
				m++;
			}

			ref var value = ref tempArray[m];
			value++;
			if (value > @this[m].Length - 1)
			{
				value = -1;
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
