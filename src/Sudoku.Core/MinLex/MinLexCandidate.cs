namespace Sudoku.MinLex;

using static BestTripletPermutation;

/// <summary>
/// Indicates a node representing the min-lexicographical candidate.
/// </summary>
internal unsafe struct MinLexCandidate
{
	/// <summary>
	/// Represents an empty <see cref="MinLexCandidate"/> instance.
	/// </summary>
	public static MinLexCandidate Empty
	{
		get
		{
			var result = new MinLexCandidate { isTransposed = 0, stacksPerm = 63 };
			for (var i = 0; i < 9; i++)
			{
				result.mapRowsForward[i] = -1;
				result.mapRowsBackward[i] = -1;

				if (i < 3)
				{
					result.colsPermMask[i] = 63;
				}
			}

			return result;
		}
	}


	/// <summary>
	/// A flag field indicating whether the candidate is handled as transposed.
	/// </summary>
	public sbyte isTransposed;

	/// <summary>
	/// Indicates the map bits on forward rows.
	/// </summary>
	public fixed sbyte mapRowsForward[9];

	/// <summary>
	/// Indicates the map bits on backward rows.
	/// </summary>
	public fixed sbyte mapRowsBackward[9];

	/// <summary>
	/// Indicates the stack permutation.
	/// </summary>
	public byte stacksPerm;

	/// <summary>
	/// Indicates mask bit values for column permutation.
	/// </summary>
	public fixed byte colsPermMask[3];


	/// <summary>
	/// Initializes a <see cref="MinLexCandidate"/> instance via the specified flag for tranpose case and top row.
	/// </summary>
	/// <param name="transpose">The value for transpose flag. The value can only be 0 or 1.</param>
	/// <param name="topRow">The top row used.</param>
	public MinLexCandidate(sbyte transpose, int topRow)
	{
		this = Empty;
		isTransposed = transpose;
		mapRowsForward[topRow] = 0;
		mapRowsBackward[0] = (sbyte)topRow;
	}


	/// <summary>
	/// To expand stacks.
	/// </summary>
	/// <param name="pair">A pair of <see cref="GridPattern"/> instance.</param>
	/// <param name="topKey">Indicates the top key.</param>
	/// <param name="results">Indicates the results.</param>
	/// <param name="resultCount">The result count.</param>
	public readonly void ExpandStacks(Span<GridPattern> pair, int topKey, Span<MinLexCandidate> results, ref int resultCount)
	{
		//for a top row, obtain stack and cols permutations
		ref readonly var gr = ref pair[isTransposed];
		var rowGivens = gr.Rows[mapRowsBackward[0]];
		for (var stackPerm = 0; stackPerm < 6; stackPerm++)
		{
			var toTriplets = new int[3];
			toTriplets[Perm[stackPerm, 0]] = (rowGivens >> 6) & 7;
			toTriplets[Perm[stackPerm, 1]] = (rowGivens >> 3) & 7;
			toTriplets[Perm[stackPerm, 2]] = rowGivens & 7;
			var bt0 = BestTripletPermutations[toTriplets[0], 63];
			if (bt0.BestResult > ((topKey >> 6) & 7))
			{
				continue;
			}

			var bt1 = BestTripletPermutations[toTriplets[1], 63];
			if (bt1.BestResult > ((topKey >> 3) & 7))
			{
				continue;
			}

			var bt2 = BestTripletPermutations[toTriplets[2], 63];
			if (bt2.BestResult > (topKey & 7))
			{
				continue;
			}

			//this stack permutation results in minimal top row. Store the expanded candidate.
			fixed (MinLexCandidate* pResults = results)
			{
				var res = &pResults[resultCount++];
				*res = this;
				res->stacksPerm = (byte)stackPerm;
				res->colsPermMask[0] = (byte)bt0.ResultMask;
				res->colsPermMask[1] = (byte)bt1.ResultMask;
				res->colsPermMask[2] = (byte)bt2.ResultMask;
			}
		}
	}

	/// <inheritdoc cref="object.ToString"/>
	public override readonly string ToString()
	{
		var sb = new StringBuilder();
		sb.Append(isTransposed);
		sb.Append(',');
		sb.Append('[');
		for (var i = 0; i < 9; i++)
		{
			sb.Append(mapRowsForward[i]);
			sb.Append(',');
		}
		sb.Append(']');
		sb.Append(',');
		sb.Append('[');
		for (var i = 0; i < 9; i++)
		{
			sb.Append(mapRowsBackward[i]);
			sb.Append(',');
		}
		sb.Append(']');
		sb.Append(',');
		sb.Append(stacksPerm);
		sb.Append(',');
		sb.Append('[');
		for (var i = 0; i < 3; i++)
		{
			sb.Append(colsPermMask[i]);
			sb.Append(',');
		}
		sb.Append(']');
		return sb.ToString();
	}
}
