namespace Sudoku.MinLex;

using static BestTripletPermutation;

internal unsafe struct MinLexCandidate
{
	public static MinLexCandidate Empty
	{
		get
		{
			var result = new MinLexCandidate { isTransposed = 0, stacksPerm = 63 };
			for (var i = 0; i < 9; i++)
			{
				result.mapRowsForward[i] = -1;
			}
			for (var i = 0; i < 9; i++)
			{
				result.mapRowsBackward[i] = -1;
			}
			for (var i = 0; i < 3; i++)
			{
				result.colsPermMask[i] = 63;
			}

			return result;
		}
	}


	public sbyte isTransposed;

	public fixed sbyte mapRowsForward[9];

	public fixed sbyte mapRowsBackward[9];

	public byte stacksPerm;

	public fixed byte colsPermMask[3];


	public MinLexCandidate(sbyte transpose, int topRow)
	{
		this = Empty;
		isTransposed = transpose;
		mapRowsForward[topRow] = 0;
		mapRowsBackward[0] = (sbyte)topRow;
	}


	public readonly void ExpandStacks(Span<GridPattern> pair, int topKey, Span<MinLexCandidate> results, ref int nResults)
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
			if (bt0.bestResult > ((topKey >> 6) & 7))
			{
				continue;
			}

			var bt1 = BestTripletPermutations[toTriplets[1], 63];
			if (bt1.bestResult > ((topKey >> 3) & 7))
			{
				continue;
			}

			var bt2 = BestTripletPermutations[toTriplets[2], 63];
			if (bt2.bestResult > (topKey & 7))
			{
				continue;
			}

			//this stack permutation results in minimal top row. Store the expanded candidate.
			fixed (MinLexCandidate* pResults = results)
			{
				var res = &pResults[nResults++];
				*res = this;
				res->stacksPerm = (byte)stackPerm;
				res->colsPermMask[0] = (byte)bt0.resultMask;
				res->colsPermMask[1] = (byte)bt1.resultMask;
				res->colsPermMask[2] = (byte)bt2.resultMask;
			}
		}
	}

	/// <inheritdoc/>
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
