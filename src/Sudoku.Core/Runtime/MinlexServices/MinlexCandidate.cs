namespace Sudoku.Runtime.MinlexServices;

using static BestTripletPermutation;

/// <summary>
/// Indicates a node representing the min-lexicographical candidate.
/// </summary>
public unsafe struct MinlexCandidate
{
	/// <summary>
	/// A flag field indicating whether the candidate is handled as transposed.
	/// </summary>
	public sbyte IsTransposed;

	/// <summary>
	/// Indicates the map bits on forward rows.
	/// </summary>
	public fixed sbyte MapRowsForward[9];

	/// <summary>
	/// Indicates the map bits on backward rows.
	/// </summary>
	public fixed sbyte MapRowsBackward[9];

	/// <summary>
	/// Indicates the stack permutation.
	/// </summary>
	public byte StacksPermutation;

	/// <summary>
	/// Indicates mask bit values for column permutation.
	/// </summary>
	public fixed byte ColumnsPermutationMask[3];


	/// <summary>
	/// Initializes a <see cref="MinlexCandidate"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public MinlexCandidate() => this = Empty;

	/// <summary>
	/// Initializes a <see cref="MinlexCandidate"/> instance via the specified flag for transpose case and top row.
	/// </summary>
	/// <param name="transpose">The value for transpose flag. The value can only be 0 or 1.</param>
	/// <param name="topRow">The top row used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public MinlexCandidate(sbyte transpose, int topRow) : this()
	{
		IsTransposed = transpose;
		MapRowsForward[topRow] = 0;
		MapRowsBackward[0] = (sbyte)topRow;
	}


	/// <summary>
	/// Represents an empty <see cref="MinlexCandidate"/> instance.
	/// </summary>
	public static MinlexCandidate Empty
	{
		get
		{
			var result = default(MinlexCandidate) with { IsTransposed = 0, StacksPermutation = 63 };
			for (var i = 0; i < 9; i++)
			{
				result.MapRowsForward[i] = -1;
				result.MapRowsBackward[i] = -1;

				if (i < 3)
				{
					result.ColumnsPermutationMask[i] = 63;
				}
			}

			return result;
		}
	}


	/// <summary>
	/// To expand stacks.
	/// </summary>
	/// <param name="pair">A pair of <see cref="GridPattern"/> instance.</param>
	/// <param name="topKey">Indicates the top key.</param>
	/// <param name="results">Indicates the results.</param>
	/// <param name="resultCount">The result count.</param>
	public readonly void ExpandStacks(ReadOnlySpan<GridPattern> pair, int topKey, Span<MinlexCandidate> results, ref int resultCount)
	{
		// For a top row, obtain stack and columns permutations.
		ref readonly var gr = ref pair[IsTransposed];
		var rowGivens = gr.Rows[MapRowsBackward[0]];
		var toTriplets = (stackalloc int[3]);
		for (var stackPerm = 0; stackPerm < 6; stackPerm++)
		{
			toTriplets.Clear();
			toTriplets[Perm[stackPerm][0]] = (rowGivens >> 6) & 7;
			toTriplets[Perm[stackPerm][1]] = (rowGivens >> 3) & 7;
			toTriplets[Perm[stackPerm][2]] = rowGivens & 7;
			var bt0 = BestTripletPermutations[toTriplets[0]][63];
			if (bt0.BestResult > ((topKey >> 6) & 7))
			{
				continue;
			}

			var bt1 = BestTripletPermutations[toTriplets[1]][63];
			if (bt1.BestResult > ((topKey >> 3) & 7))
			{
				continue;
			}

			var bt2 = BestTripletPermutations[toTriplets[2]][63];
			if (bt2.BestResult > (topKey & 7))
			{
				continue;
			}

			// This stack permutation results in minimal top row. Store the expanded candidate.
			ref var result = ref results[resultCount++];
			result = this;
			result.StacksPermutation = (byte)stackPerm;
			result.ColumnsPermutationMask[0] = (byte)bt0.ResultMask;
			result.ColumnsPermutationMask[1] = (byte)bt1.ResultMask;
			result.ColumnsPermutationMask[2] = (byte)bt2.ResultMask;
		}
	}

	/// <inheritdoc cref="object.ToString"/>
	public override readonly string ToString()
	{
		var sb = new StringBuilder();
		sb.Append(IsTransposed);
		sb.Append(',');
		sb.Append('[');
		for (var i = 0; i < 9; i++)
		{
			sb.Append(MapRowsForward[i]);
			sb.Append(',');
		}
		sb.Append(']');
		sb.Append(',');
		sb.Append('[');
		for (var i = 0; i < 9; i++)
		{
			sb.Append(MapRowsBackward[i]);
			sb.Append(',');
		}
		sb.Append(']');
		sb.Append(',');
		sb.Append(StacksPermutation);
		sb.Append(',');
		sb.Append('[');
		for (var i = 0; i < 3; i++)
		{
			sb.Append(ColumnsPermutationMask[i]);
			sb.Append(',');
		}
		sb.Append(']');
		return sb.ToString();
	}
}
