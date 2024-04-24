namespace Sudoku.MinLex;

/// <summary>
/// Represents a finder type.
/// </summary>
public sealed unsafe partial class MinLexFinder
{
	/// <summary>
	/// Indicates the total number of candidate list, which means the worst case.
	/// </summary>
	private const int CandidateListTotal = 15552;


	/// <summary>
	/// Indicates the internal mappers.
	/// </summary>
	private readonly List<Mapper> _mappers = [];


	/// <summary>
	/// Finds the minimal lexicographical form of the source grid code.
	/// </summary>
	/// <param name="source">Indicates the source grid code.</param>
	/// <returns>The corresponding minimal lexicographical form of the grid.</returns>
	public string FindMinLexForm(string source)
	{
		var span = (stackalloc char[81]);
		core(source, span);
		return span.ToString();


		void core(string source, Span<char> result)
		{
			var originalAndTransform = (stackalloc GridPattern[] { new(), new() });
			var givensCount = GridPattern.FromString(source, ref originalAndTransform[0], ref originalAndTransform[1]);

			_mappers.Clear();

			var minTopRowScores = (int[])[
				GridPattern.BestTopRowScore(in originalAndTransform[0]),
				GridPattern.BestTopRowScore(in originalAndTransform[1])
			];
			var minTopRowScore = minTopRowScores[0] <= minTopRowScores[1] ? minTopRowScores[0] : minTopRowScores[1];

			result[0] = (char)((minTopRowScore >> 8) & 1);
			result[1] = (char)((minTopRowScore >> 7) & 1);
			result[2] = (char)((minTopRowScore >> 6) & 1);
			result[3] = (char)((minTopRowScore >> 5) & 1);
			result[4] = (char)((minTopRowScore >> 4) & 1);
			result[5] = (char)((minTopRowScore >> 3) & 1);
			result[6] = (char)((minTopRowScore >> 2) & 1);
			result[7] = (char)((minTopRowScore >> 1) & 1);
			result[8] = (char)((minTopRowScore) & 1);

			// Step 1: Determine for top rows.
			var candidates = (stackalloc MinLexCandidate[CandidateListTotal]); // Rows 0, 2, 4, 6, 8.
			var candidates1 = (stackalloc MinLexCandidate[CandidateListTotal]); // Rows 1, 3, 5, 7.
			var nCurCandidates = 0;
			for (var nowTransposed = (sbyte)0; nowTransposed < 2; nowTransposed++)
			{
				if (minTopRowScores[nowTransposed] > minTopRowScore)
				{
					continue;
				}

				for (var topRow = 0; topRow < 9; topRow++)
				{
					if (GridPattern.MinCanNineBits[originalAndTransform[nowTransposed].Rows[topRow]] <= minTopRowScore)
					{
						// Here we have a top row candidate.
						// In the empty template fix only the transposition and row 0.
						var cand = new MinLexCandidate(nowTransposed, topRow);

						// To fix all minimal stack permutations and store for later row expansion
						cand.ExpandStacks(originalAndTransform, minTopRowScore, candidates, ref nCurCandidates);
					}
				}
			}
			// Here we have all top row candidates with fixed top row, stacks, and column permutation masks.

			// Step 2: Expand rows.
			fixed (MinLexCandidate* p = candidates) // Read from here.
			fixed (MinLexCandidate* q = candidates1) // Add row and write here.
			{
				var curCandidates = p;
				var nextCandidates = q;
				var nNextCandidates = 0;
				for (var toRow = 1; toRow < 9; toRow++)
				{
					var (bestTriplets0, bestTriplets1, bestTriplets2, rowInBand) = (7, 7, 7, toRow % 3);
					for (var curCandidateIndex = 0; curCandidateIndex < nCurCandidates; curCandidateIndex++)
					{
						var old = &curCandidates[curCandidateIndex];
						var (startRow, endRow) = rowInBand != 0 && old->MapRowsBackward[3 * (toRow / 3)] / 3 is var band
							? (band * 3, (band + 1) * 3) // Combine with unmapped rows from the same band.
							: (0, 9); // Try any unmapped row.

						for (var fromRow = startRow; fromRow < endRow; fromRow++)
						{
							if (old->MapRowsForward[fromRow] >= 0)
							{
								// Skip previously mapped rows.
								continue;
							}

							var toTriplets = new int[3];
							var rowGivens = originalAndTransform[old->IsTransposed].Rows[fromRow]; // Stacks unmapped.
							toTriplets[BestTripletPermutation.Perm[old->StacksPermutation, 0]] = rowGivens >> 6;
							toTriplets[BestTripletPermutation.Perm[old->StacksPermutation, 1]] = (rowGivens >> 3) & 7;
							toTriplets[BestTripletPermutation.Perm[old->StacksPermutation, 2]] = rowGivens & 7;
							ref readonly var bt0 = ref BestTripletPermutation.BestTripletPermutations[toTriplets[0], old->ColumnsPermutationMask[0]];
							if (bt0.BestResult > bestTriplets0)
							{
								continue;
							}

							if (bt0.BestResult < bestTriplets0)
							{
								nNextCandidates = 0;
								bestTriplets0 = bt0.BestResult;
								bestTriplets1 = 7;
								bestTriplets2 = 7;
							}
							ref readonly var bt1 = ref BestTripletPermutation.BestTripletPermutations[toTriplets[1], old->ColumnsPermutationMask[1]];
							if (bt1.BestResult > bestTriplets1)
							{
								continue;
							}

							if (bt1.BestResult < bestTriplets1)
							{
								nNextCandidates = 0;
								bestTriplets1 = bt1.BestResult;
								bestTriplets2 = 7;
							}
							ref readonly var bt2 = ref BestTripletPermutation.BestTripletPermutations[toTriplets[2], old->ColumnsPermutationMask[2]];
							if (bt2.BestResult > bestTriplets2)
							{
								continue;
							}

							if (bt2.BestResult < bestTriplets2)
							{
								nNextCandidates = 0;
								bestTriplets2 = bt2.BestResult;
							}

							// Tests passed, output the new candidate.
							var next = &nextCandidates[nNextCandidates++];
							*next = *old;
							next->MapRowsForward[fromRow] = (sbyte)toRow;
							next->MapRowsBackward[toRow] = (sbyte)fromRow;
							next->ColumnsPermutationMask[0] = (byte)bt0.ResultMask;
							next->ColumnsPermutationMask[1] = (byte)bt1.ResultMask;
							next->ColumnsPermutationMask[2] = (byte)bt2.ResultMask;
						}
					}

					// Flip-flop the old/new.
					var tmp = curCandidates;
					curCandidates = nextCandidates;
					nextCandidates = tmp;
					nCurCandidates = nNextCandidates;
					nNextCandidates = 0;

					// Store the best result.
					fixed (char* r = &result[9 * toRow])
					{
						r[0] = (char)((bestTriplets0 >> 2) & 1);
						r[1] = (char)((bestTriplets0 >> 1) & 1);
						r[2] = (char)((bestTriplets0) & 1);
						r[3] = (char)((bestTriplets1 >> 2) & 1);
						r[4] = (char)((bestTriplets1 >> 1) & 1);
						r[5] = (char)((bestTriplets1) & 1);
						r[6] = (char)((bestTriplets2 >> 2) & 1);
						r[7] = (char)((bestTriplets2 >> 1) & 1);
						r[8] = (char)((bestTriplets2) & 1);
					}
				} //toRow

				if (nCurCandidates == 0)
				{
					throw new InvalidOperationException("bad news: no candidates for minlex due to program errors");
				}

				// Step 3: Find the lexicographically minimal representative within the morphs.
				// This time taking into account the real values of the input givens.

				Unsafe.SkipInit<Mapper>(out var map);
				var minLex = (stackalloc int[81]); // The best result so far.
				for (var i = 0; i < 81; i++)
				{
					minLex[i] = result[i] << 5; // Initially set to large values.
				}
				for (var curCandidateIndex = 0; curCandidateIndex < nCurCandidates; curCandidateIndex++)
				{
					ref var target = ref curCandidates[curCandidateIndex];
					var toTriplets = new int[3];
					toTriplets[BestTripletPermutation.Perm[target.StacksPermutation, 0]] = 0;
					toTriplets[BestTripletPermutation.Perm[target.StacksPermutation, 1]] = 3;
					toTriplets[BestTripletPermutation.Perm[target.StacksPermutation, 2]] = 6;
					for (var colsPerm0 = 0; colsPerm0 < 6; colsPerm0++)
					{
						if (((target.ColumnsPermutationMask[0] >> colsPerm0) & 1) == 0)
						{
							continue;
						}

						var toColsInStack = new int[9];
						toColsInStack[BestTripletPermutation.Perm[colsPerm0, 0]] = toTriplets[0];
						toColsInStack[BestTripletPermutation.Perm[colsPerm0, 1]] = toTriplets[0] + 1;
						toColsInStack[BestTripletPermutation.Perm[colsPerm0, 2]] = toTriplets[0] + 2;
						for (var colsPerm1 = 0; colsPerm1 < 6; colsPerm1++)
						{
							if (((target.ColumnsPermutationMask[1] >> colsPerm1) & 1) == 0)
							{
								continue;
							}

							toColsInStack[3 + BestTripletPermutation.Perm[colsPerm1, 0]] = toTriplets[1];
							toColsInStack[3 + BestTripletPermutation.Perm[colsPerm1, 1]] = toTriplets[1] + 1;
							toColsInStack[3 + BestTripletPermutation.Perm[colsPerm1, 2]] = toTriplets[1] + 2;
							for (var colsPerm2 = 0; colsPerm2 < 6; colsPerm2++)
							{
								if (((target.ColumnsPermutationMask[2] >> colsPerm2) & 1) == 0)
								{
									continue;
								}

								toColsInStack[6 + BestTripletPermutation.Perm[colsPerm2, 0]] = toTriplets[2];
								toColsInStack[6 + BestTripletPermutation.Perm[colsPerm2, 1]] = toTriplets[2] + 1;
								toColsInStack[6 + BestTripletPermutation.Perm[colsPerm2, 2]] = toTriplets[2] + 2;
								var labelPerm = new int[10]; // Label mapping is unknown.
								var nextFreeLabel = 1;
								var nSet = 0; // The number of givens with positions set.
								for (var toRow = 0; toRow < 9; toRow++)
								{
									ref var rowGivens = ref originalAndTransform[target.IsTransposed].Digits[target.MapRowsBackward[toRow] * 9];
									for (var col = 0; col < 9; col++)
									{
										ref var fromDigit = ref Unsafe.Add(ref rowGivens, toColsInStack[col]);
										if (fromDigit == 0)
										{
											continue;
										}

										if (labelPerm[fromDigit] == 0)
										{
											labelPerm[fromDigit] = nextFreeLabel++;
										}
										if (labelPerm[fromDigit] > minLex[toRow * 9 + col])
										{
											goto nextColsPerm;
										}

										nSet++;
										if (labelPerm[fromDigit] < minLex[toRow * 9 + col])
										{
											for (var i = toRow * 9 + col + 1; i < 81; i++)
											{
												minLex[i] = result[i] << 5; // Invalidate the rest.
											}

											minLex[toRow * 9 + col] = labelPerm[fromDigit];

											// The buffered transformations become invalid at this point.
											_mappers.Clear();
										}
										if (nSet == givensCount)
										{
											// An isomorph of the currently best ordering
											// at this point we have the necessary information
											// for the transformation and can buffer it (if eventually it is one of the best ones).
											for (var r = 0; r < 9; r++)
											{
												for (var c = 0; c < 9; c++)
												{
													var src = target.IsTransposed != 0
														? target.MapRowsBackward[r] + 9 * toColsInStack[c]
														: target.MapRowsBackward[r] * 9 + toColsInStack[c];

													// Map all non-givens to 99, this masking irrelevant permutations.
													map.Cell[src] = (byte)(minLex[r * 9 + c] != 0 ? r * 9 + c : 99);
												}
											}
											for (var d = 0; d < 10; d++)
											{
												map.Label[d] = 0;
											}

											for (var d = 0; d < 10; d++)
											{
												map.Label[labelPerm[d]] = (byte)d;
											}

											map.Label[0] = 0;
											_mappers.Add(map); // Will automatically ignore duplicates.
										}
									}
								}
							nextColsPerm:;
							}
						}
					}
				}
				for (var i = 0; i < 81; i++)
				{
					result[i] = (char)(minLex[i] != 0 ? minLex[i] + '0' : '.');
				}
			}
		}
	}
}
