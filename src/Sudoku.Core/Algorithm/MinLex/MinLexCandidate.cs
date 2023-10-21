using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Sudoku.Algorithm.MinLex;

/// <summary>
/// Represents for the basic data for candidates used by MinLex operations.
/// </summary>
[DebuggerDisplay($$"""{{{nameof(ToString)}}(),nq}""")]
public unsafe struct MinLexCandidate
{
	private const Count CandListSize = 15552;


	/// <summary>
	/// The permutation cases.
	/// </summary>
	private static readonly Offset[][] Perm = [[0, 1, 2], [0, 2, 1], [1, 0, 2], [1, 2, 0], [2, 0, 1], [2, 1, 0]];


	private bool _isTransposed; // false

	private fixed sbyte _mapRowsForward[9]; // [-1, -1, -1, -1, -1, -1, -1, -1, -1]

	private fixed sbyte _mapRowsBackward[9]; // [-1, -1, -1, -1, -1, -1, -1, -1, -1]

	private sbyte _stacksPerm; // 63

	private fixed byte _colsPermMask[3]; // [63, 63, 63]


	/// <summary>
	/// Skips for initialization for <see langword="this"/>
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public MinLexCandidate() => Unsafe.SkipInit(out this);


	/// <inheritdoc cref="object.ToString"/>
	public override readonly string ToString()
	{
		const string separator = ", ";

		var mapRowsForwardSb = new StringHandler();
		for (var i = 0; i < 9; i++)
		{
			mapRowsForwardSb.Append(_mapRowsForward[i]);
			mapRowsForwardSb.Append(separator);
		}
		mapRowsForwardSb.RemoveFromEnd(separator.Length);

		var mapRowsBackwardSb = new StringHandler();
		for (var i = 0; i < 9; i++)
		{
			mapRowsBackwardSb.Append(_mapRowsBackward[i]);
			mapRowsBackwardSb.Append(separator);
		}
		mapRowsBackwardSb.RemoveFromEnd(separator.Length);

		var colsPermMaskSb = new StringHandler();
		for (var i = 0; i < 3; i++)
		{
			colsPermMaskSb.Append(_colsPermMask[i]);
			colsPermMaskSb.Append(separator);
		}
		colsPermMaskSb.RemoveFromEnd(separator.Length);

		return $$"""
			{{nameof(MinLexCandidate)}} 
			{ 
			{{nameof(_isTransposed)}} = {{_isTransposed}}, 
			{{nameof(_mapRowsForward)}} = [{{mapRowsForwardSb.ToStringAndClear()}}], 
			{{nameof(_mapRowsBackward)}} = [{{mapRowsBackwardSb.ToStringAndClear()}}], 
			{{nameof(_stacksPerm)}} = {{_stacksPerm}}, 
			{{nameof(_colsPermMask)}} = [{{colsPermMaskSb.ToStringAndClear()}}]
			}
			""".RemoveLineEndings();
	}

	/// <summary>
	/// Try to expand stacks for the specified pair of <see cref="GridPattern"/> instances.
	/// </summary>
	private void ExpandStacks(GridPattern* pair, int topKey, MinLexCandidate* candidates, Count* nResults)
	{
		*nResults = 0;

		var gr = pair + (_isTransposed ? 1 : 0);
		var rowGivens = gr->Rows[_mapRowsBackward[0]];
		var toTriplets = stackalloc int[3];
		MinLexCandidate* res;
		for (var stackPerm = 0; stackPerm < 6; stackPerm++)
		{
			toTriplets[Perm[stackPerm][0]] = rowGivens >> 6 & 7;
			toTriplets[Perm[stackPerm][1]] = rowGivens >> 3 & 7;
			toTriplets[Perm[stackPerm][2]] = rowGivens & 7;

			fixed (BestTriplet* bt0 = &BestTriplet.BestTripletPermutations[toTriplets[0]][63])
			fixed (BestTriplet* bt1 = &BestTriplet.BestTripletPermutations[toTriplets[1]][63])
			fixed (BestTriplet* bt2 = &BestTriplet.BestTripletPermutations[toTriplets[2]][63])
			{
				if (bt0->BestResult > (topKey >> 6 & 7))
				{
					continue;
				}
				if (bt1->BestResult > (topKey >> 3 & 7))
				{
					continue;
				}
				if (bt2->BestResult > (topKey & 7))
				{
					continue;
				}

				res = candidates + (*nResults)++;
				*res = this;
				res->_stacksPerm = (sbyte)stackPerm;
				res->_colsPermMask[0] = (byte)bt0->ResultMask;
				res->_colsPermMask[1] = (byte)bt1->ResultMask;
				res->_colsPermMask[2] = (byte)bt2->ResultMask;
			}
		}
	}

	/// <summary>
	/// To initialize data manually.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void Init(bool transpose, int topRow)
	{
		_isTransposed = transpose;
		_mapRowsForward[0] = _mapRowsBackward[0] = -1;
		_mapRowsForward[1] = _mapRowsBackward[1] = -1;
		_mapRowsForward[2] = _mapRowsBackward[2] = -1;
		_mapRowsForward[3] = _mapRowsBackward[3] = -1;
		_mapRowsForward[4] = _mapRowsBackward[4] = -1;
		_mapRowsForward[5] = _mapRowsBackward[5] = -1;
		_mapRowsForward[6] = _mapRowsBackward[6] = -1;
		_mapRowsForward[7] = _mapRowsBackward[7] = -1;
		_mapRowsForward[8] = _mapRowsBackward[8] = -1;

		_mapRowsForward[topRow] = 0;
		_mapRowsBackward[0] = (sbyte)topRow;

		_stacksPerm = 63;
		_colsPermMask[0] = 63;
		_colsPermMask[1] = 63;
		_colsPermMask[2] = 63;
	}


	/// <summary>
	/// Find minimum lexicographical-ordered string for the specified grid as a string value.
	/// </summary>
	/// <param name="source">The source grid.</param>
	/// <param name="result">The result grid.</param>
	/// <param name="patternOnly">Indicates whether the method only studies with pattern.</param>
	public static void PatCanon(string source, out string result, bool patternOnly = false)
	{
		Unsafe.SkipInit(out int nCurCandidates);
		var candidates = stackalloc MinLexCandidate[CandListSize];
		var candidates1 = stackalloc MinLexCandidate[CandListSize];
		var pPair = stackalloc GridPattern[2];
		GridPattern.FromStringUnsafe(source, pPair);
		scoped var minTopRowScores = (ReadOnlySpan<int>)[pPair[0].BestTopRowScore, pPair[1].BestTopRowScore];
		var minTopRowScore = Math.Min(minTopRowScores[0], minTopRowScores[1]);
		var resultBuffer = stackalloc byte[82];
		resultBuffer[0] = (byte)(minTopRowScore >> 8 & 1);
		resultBuffer[1] = (byte)(minTopRowScore >> 7 & 1);
		resultBuffer[2] = (byte)(minTopRowScore >> 6 & 1);
		resultBuffer[3] = (byte)(minTopRowScore >> 5 & 1);
		resultBuffer[4] = (byte)(minTopRowScore >> 4 & 1);
		resultBuffer[5] = (byte)(minTopRowScore >> 3 & 1);
		resultBuffer[6] = (byte)(minTopRowScore >> 2 & 1);
		resultBuffer[7] = (byte)(minTopRowScore >> 1 & 1);
		resultBuffer[8] = (byte)(minTopRowScore & 1);
		resultBuffer[81] = 0;
		foreach (var nowTransposed in (false, true))
		{
			if (minTopRowScores[nowTransposed ? 1 : 0] > minTopRowScore)
			{
				continue;
			}

			for (var topRow = 0; topRow < 9; topRow++)
			{
				if (GridPattern.MinCanNineBits[pPair[nowTransposed ? 1 : 0].Rows[topRow]] > minTopRowScore)
				{
					continue;
				}

				var cand = new MinLexCandidate();
				cand.Init(nowTransposed, topRow);
				cand.ExpandStacks(pPair, minTopRowScore, candidates, &nCurCandidates);
			}
		}

		var curCandidates = &candidates[0];
		var nextCandidates = &candidates1[0];
		var nNextCandidates = 0;
		var toTriplets = stackalloc int[3];
		for (var toRow = (sbyte)1; toRow < 9; toRow++)
		{
			var (bestTriplets0, bestTriplets1, bestTriplets2) = (7, 7, 7);
			var rowInBand = toRow % 3;
			for (var curCandidateIndex = 0; curCandidateIndex < nCurCandidates; curCandidateIndex++)
			{
				var old = curCandidates + curCandidateIndex;
				var startRow = (sbyte)0;
				var endRow = (sbyte)9;
				if (rowInBand != 0)
				{
					var band = old->_mapRowsBackward[3 * toRow / 3] / 3;
					startRow = (sbyte)(band * 3);
					endRow = (sbyte)(startRow + 3);
				}

				for (var fromRow = startRow; fromRow < endRow; fromRow++)
				{
					if (old->_mapRowsForward[fromRow] >= 0)
					{
						continue;
					}

					var rowGivens = pPair[old->_isTransposed ? 1 : 0].Rows[fromRow];
					toTriplets[Perm[old->_stacksPerm][0]] = rowGivens >> 6 & 7;
					toTriplets[Perm[old->_stacksPerm][1]] = rowGivens >> 3 & 7;
					toTriplets[Perm[old->_stacksPerm][2]] = rowGivens & 7;

					fixed (BestTriplet* bt0 = &BestTriplet.BestTripletPermutations[toTriplets[0]][old->_colsPermMask[0]])
					fixed (BestTriplet* bt1 = &BestTriplet.BestTripletPermutations[toTriplets[1]][old->_colsPermMask[1]])
					fixed (BestTriplet* bt2 = &BestTriplet.BestTripletPermutations[toTriplets[2]][old->_colsPermMask[2]])
					{
						if (bt0->BestResult > bestTriplets0) { continue; }
						if (bt0->BestResult < bestTriplets0)
						{
							nNextCandidates = 0;
							bestTriplets0 = bt0->BestResult;
							bestTriplets1 = 7;
							bestTriplets2 = 7;
						}

						if (bt1->BestResult > bestTriplets1) { continue; }
						if (bt1->BestResult < bestTriplets1)
						{
							nNextCandidates = 0;
							bestTriplets1 = bt1->BestResult;
							bestTriplets2 = 7;
						}

						if (bt2->BestResult > bestTriplets2) { continue; }
						if (bt2->BestResult < bestTriplets2)
						{
							nNextCandidates = 0;
							bestTriplets2 = bt2->BestResult;
						}

						var nNext = nextCandidates + nNextCandidates++;
						*nNext = *old;
						nNext->_mapRowsForward[fromRow] = toRow;
						nNext->_mapRowsBackward[toRow] = fromRow;
						nNext->_colsPermMask[0] = (byte)bt0->ResultMask;
						nNext->_colsPermMask[1] = (byte)bt1->ResultMask;
						nNext->_colsPermMask[2] = (byte)bt2->ResultMask;
					}
				}
			}

			var tmp = curCandidates;
			curCandidates = nextCandidates;
			nextCandidates = tmp;

			nCurCandidates = nNextCandidates;
			nNextCandidates = 0;

			resultBuffer[9 * toRow] = (byte)(bestTriplets0 >> 2 & 1);
			resultBuffer[9 * toRow + 1] = (byte)(bestTriplets0 >> 1 & 1);
			resultBuffer[9 * toRow + 2] = (byte)(bestTriplets0 & 1);
			resultBuffer[9 * toRow + 3] = (byte)(bestTriplets1 >> 2 & 1);
			resultBuffer[9 * toRow + 4] = (byte)(bestTriplets1 >> 1 & 1);
			resultBuffer[9 * toRow + 5] = (byte)(bestTriplets1 & 1);
			resultBuffer[9 * toRow + 6] = (byte)(bestTriplets2 >> 2 & 1);
			resultBuffer[9 * toRow + 7] = (byte)(bestTriplets2 >> 1 & 1);
			resultBuffer[9 * toRow + 8] = (byte)(bestTriplets2 & 1);
		}

		var minLex = stackalloc int[81];
		if (patternOnly)
		{
			for (var i = 0; i < 81; i++)
			{
				if (resultBuffer[i] == 0)
				{
					resultBuffer[i] = (byte)'.';
				}
				else
				{
					resultBuffer[i] += (byte)'0';
				}
			}

			result = ((Utf8String)new ReadOnlySpan<byte>(resultBuffer, 81)).ToString();
			return;
		}

		for (var i = 0; i < 81; i++)
		{
			minLex[i] = resultBuffer[i] << 5;
		}

		var toColsInStack = stackalloc int[9];
		var labelPerm = stackalloc int[10];
		for (var curCandidateIndex = 0; curCandidateIndex < nCurCandidates; curCandidateIndex++)
		{
			var target = curCandidates + curCandidateIndex;
			toTriplets[Perm[target->_stacksPerm][0]] = 0;
			toTriplets[Perm[target->_stacksPerm][1]] = 3;
			toTriplets[Perm[target->_stacksPerm][2]] = 6;
			for (var colsPerm0 = 0; colsPerm0 < 6; colsPerm0++)
			{
				if ((target->_colsPermMask[0] >> colsPerm0 & 1) == 0) { continue; }

				toColsInStack[Perm[colsPerm0][0]] = toTriplets[0];
				toColsInStack[Perm[colsPerm0][1]] = toTriplets[0] + 1;
				toColsInStack[Perm[colsPerm0][2]] = toTriplets[0] + 2;
				for (var colsPerm1 = 0; colsPerm1 < 6; colsPerm1++)
				{
					if ((target->_colsPermMask[1] >> colsPerm1 & 1) == 0) { continue; }

					toColsInStack[Perm[colsPerm1][0] + 3] = toTriplets[1];
					toColsInStack[Perm[colsPerm1][1] + 3] = toTriplets[1] + 1;
					toColsInStack[Perm[colsPerm1][2] + 3] = toTriplets[1] + 2;
					for (var colsPerm2 = 0; colsPerm2 < 6; colsPerm2++)
					{
						if ((target->_colsPermMask[2] >> colsPerm2 & 1) == 0) { continue; }

						toColsInStack[Perm[colsPerm2][0] + 6] = toTriplets[2];
						toColsInStack[Perm[colsPerm2][1] + 6] = toTriplets[2] + 1;
						toColsInStack[Perm[colsPerm2][2] + 6] = toTriplets[2] + 2;

						Unsafe.InitBlock(labelPerm, 0, sizeof(int) * 10);
						var nextFreeLabel = 1;
						for (var toRow = 0; toRow < 9; toRow++)
						{
							var rgs = &pPair[target->_isTransposed ? 1 : 0].Digits[target->_mapRowsBackward[toRow] * 9];
							for (var col = 0; col < 9; col++)
							{
								var fromDigit = rgs[toColsInStack[col]];
								if (fromDigit == 0) { continue; }

								if (labelPerm[fromDigit] == 0)
								{
									labelPerm[fromDigit] = nextFreeLabel++;
								}
								if (labelPerm[fromDigit] > minLex[toRow * 9 + col])
								{
									goto NextLoop;
								}

								if (labelPerm[fromDigit] < minLex[toRow * 9 + col])
								{
									for (var i = toRow * 9 + col + 1; i < 81; i++)
									{
										minLex[i] = resultBuffer[i] << 5;
									}

									minLex[toRow * 9 + col] = labelPerm[fromDigit];
								}
							}
						}

					NextLoop:;
					}
				}
			}
		}

		for (var i = 0; i < 81; i++)
		{
			resultBuffer[i] = minLex[i] == 0 ? (byte)'.' : (byte)(minLex[i] + '0');
		}

		result = ((Utf8String)new ReadOnlySpan<byte>(resultBuffer, 81)).ToString();
	}
}
