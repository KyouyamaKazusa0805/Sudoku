namespace Sudoku.Concepts;

/// <summary>
/// Represents some methods operating with <see cref="IBitStatusMap{TSelf, TElement, TEnumerator}"/> instances
/// (i.e. <see cref="CellMap"/> and <see cref="CandidateMap"/>) to get subsets.
/// </summary>
/// <seealso cref="IBitStatusMap{TSelf, TElement, TEnumerator}"/>
/// <seealso cref="CellMap"/>
/// <seealso cref="CandidateMap"/>
public static class MapCombinatorial
{
	/// <summary>
	/// Gets the subsets of the current collection via the specified size indicating the number of elements of the each subset.
	/// </summary>
	/// <param name="this">The instance to check for subsets.</param>
	/// <param name="subsetSize">The size to get.</param>
	/// <returns>
	/// All possible subsets. If:
	/// <list type="table">
	/// <listheader>
	/// <term>Condition</term>
	/// <description>Meaning</description>
	/// </listheader>
	/// <item>
	/// <term><c><paramref name="subsetSize"/> &gt; Count</c></term>
	/// <description>Will return an empty array</description>
	/// </item>
	/// <item>
	/// <term><c><paramref name="subsetSize"/> == Count</c></term>
	/// <description>
	/// Will return an array that contains only one element, same as the current instance.
	/// </description>
	/// </item>
	/// <item>
	/// <term>Other cases</term>
	/// <description>The valid combinations.</description>
	/// </item>
	/// </list>
	/// </returns>
	/// <exception cref="NotSupportedException">
	/// Throws when both the count of the current instance and <paramref name="subsetSize"/> are greater than 30.
	/// </exception>
	/// <remarks>
	/// For example, if the current instance is <c>r1c1</c>, <c>r1c2</c> and <c>r1c3</c>
	/// and the argument <paramref name="subsetSize"/> is 2,
	/// the method will return an array of 3 elements given below: <c>r1c12</c>, <c>r1c13</c> and <c>r1c23</c>.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe ReadOnlySpan<CellMap> GetSubsets(this scoped in CellMap @this, int subsetSize)
	{
		if (subsetSize == 0 || subsetSize > @this.Count)
		{
			return [];
		}

		if (subsetSize == @this.Count)
		{
			return (CellMap[])[@this];
		}

		var n = @this.Count;
		var buffer = stackalloc int[subsetSize];
		if (n <= 30 && subsetSize <= 30)
		{
			// Optimization: Use table to get the total number of result elements.
			var totalIndex = 0;
			var result = new CellMap[Combinatorial.PascalTriangle[n - 1][subsetSize - 1]];
			enumerateWithLimit(subsetSize, n, subsetSize, @this.Offsets);
			return result;


			void enumerateWithLimit(int size, int last, int index, Cell[] offsets)
			{
				for (var i = last; i >= index; i--)
				{
					buffer[index - 1] = i - 1;
					if (index > 1)
					{
						enumerateWithLimit(size, i - 1, index - 1, offsets);
					}
					else
					{
						var temp = new Cell[size];
						for (var j = 0; j < size; j++)
						{
							temp[j] = offsets[buffer[j]];
						}

						result[totalIndex++] = (CellMap)temp;
					}
				}
			}
		}
		else
		{
			if (n > 30 && subsetSize > 30)
			{
				throw new NotSupportedException(ResourceDictionary.ExceptionMessage("SubsetsExceeded"));
			}
			var result = new List<CellMap>();
			enumerateWithoutLimit(subsetSize, n, subsetSize, @this.Offsets);
			return result.AsReadOnlySpan();


			void enumerateWithoutLimit(int size, int last, int index, Cell[] offsets)
			{
				for (var i = last; i >= index; i--)
				{
					buffer[index - 1] = i - 1;
					if (index > 1)
					{
						enumerateWithoutLimit(size, i - 1, index - 1, offsets);
					}
					else
					{
						var temp = new Cell[size];
						for (var j = 0; j < size; j++)
						{
							temp[j] = offsets[buffer[j]];
						}

						result.AddRef((CellMap)temp);
					}
				}
			}
		}
	}

	/// <inheritdoc cref="GetSubsets(in CellMap, int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe ReadOnlySpan<CandidateMap> GetSubsets(this scoped in CandidateMap @this, int subsetSize)
	{
		if (subsetSize == 0 || subsetSize > @this.Count)
		{
			return [];
		}

		if (subsetSize == @this.Count)
		{
			return (CandidateMap[])[@this];
		}

		var n = @this.Count;
		var buffer = stackalloc int[subsetSize];
		if (n <= 30 && subsetSize <= 30)
		{
			// Optimization: Use table to get the total number of result elements.
			var totalIndex = 0;
			var result = new CandidateMap[Combinatorial.PascalTriangle[n - 1][subsetSize - 1]];
			enumerateWithLimit(subsetSize, n, subsetSize, @this.Offsets);
			return result;


			void enumerateWithLimit(int size, int last, int index, Candidate[] offsets)
			{
				for (var i = last; i >= index; i--)
				{
					buffer[index - 1] = i - 1;
					if (index > 1)
					{
						enumerateWithLimit(size, i - 1, index - 1, offsets);
					}
					else
					{
						var temp = new Candidate[size];
						for (var j = 0; j < size; j++)
						{
							temp[j] = offsets[buffer[j]];
						}

						result[totalIndex++] = [.. temp];
					}
				}
			}
		}
		else
		{
			if (n > 30 && subsetSize > 30)
			{
				throw new NotSupportedException(ResourceDictionary.ExceptionMessage("SubsetsExceeded"));
			}
			var result = new List<CandidateMap>();
			enumerateWithoutLimit(subsetSize, n, subsetSize, @this.Offsets);
			return result.AsReadOnlySpan();


			void enumerateWithoutLimit(int size, int last, int index, Candidate[] offsets)
			{
				for (var i = last; i >= index; i--)
				{
					buffer[index - 1] = i - 1;
					if (index > 1)
					{
						enumerateWithoutLimit(size, i - 1, index - 1, offsets);
					}
					else
					{
						var temp = new Candidate[size];
						for (var j = 0; j < size; j++)
						{
							temp[j] = offsets[buffer[j]];
						}

						result.AddRef([.. temp]);
					}
				}
			}
		}
	}

	/// <summary>
	/// Gets all subsets of the current collection via the specified size
	/// indicating the <b>maximum</b> number of elements of the each subset.
	/// </summary>
	/// <param name="this">The instance to check subsets.</param>
	/// <param name="limitSubsetSize">The size to get.</param>
	/// <returns>
	/// All possible subsets. If:
	/// <list type="table">
	/// <listheader>
	/// <term>Condition</term>
	/// <description>Meaning</description>
	/// </listheader>
	/// <item>
	/// <term><c><paramref name="limitSubsetSize"/> &gt; Count</c></term>
	/// <description>Will return an empty array</description>
	/// </item>
	/// <item>
	/// <term>Other cases</term>
	/// <description>The valid combinations.</description>
	/// </item>
	/// </list>
	/// </returns>
	public static ReadOnlySpan<CellMap> GetSubsetsBelow(this scoped in CellMap @this, int limitSubsetSize)
	{
		if (limitSubsetSize == 0 || !@this)
		{
			return [];
		}

		var (n, desiredSize) = (@this.Count, 0);
		var length = Math.Min(n, limitSubsetSize);
		for (var i = 1; i <= length; i++)
		{
			desiredSize += Combinatorial.PascalTriangle[n - 1][i - 1];
		}

		var result = new List<CellMap>(desiredSize);
		for (var i = 1; i <= length; i++)
		{
			result.AddRangeRef(@this >> i);
		}
		return result.AsReadOnlySpan();
	}

	/// <inheritdoc cref="GetSubsetsBelow(in CellMap, int)"/>
	public static ReadOnlySpan<CandidateMap> GetSubsetsBelow(this scoped in CandidateMap @this, int limitSubsetSize)
	{
		if (limitSubsetSize == 0 || !@this)
		{
			return [];
		}

		var (n, desiredSize) = (@this.Count, 0);
		var length = Math.Min(n, limitSubsetSize);
		for (var i = 1; i <= length; i++)
		{
			desiredSize += Combinatorial.PascalTriangle[n - 1][i - 1];
		}

		var result = new List<CandidateMap>(desiredSize);
		for (var i = 1; i <= length; i++)
		{
			result.AddRangeRef(@this >> i);
		}
		return result.AsReadOnlySpan();
	}
}
