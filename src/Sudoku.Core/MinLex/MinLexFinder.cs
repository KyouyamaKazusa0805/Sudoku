namespace Sudoku.MinLex;

using static BestTripletPermutation;

/// <summary>
/// Represents a finder type.
/// </summary>
public static unsafe class MinLexFinder
{
	private const int CandidateListTotal = 15552;


	private static readonly List<Mapper> Mappers = [];


	/// <summary>
	/// 
	/// </summary>
	/// <param name="source"></param>
	/// <param name="result"></param>
	public static void PatMinLex(string source, out string result)
	{
		var span = (stackalloc char[81]);
		PatMinLexCore(source, span);
		result = span.ToString();
	}

	private static void PatMinLexCore(string source, Span<char> result)
	{
		var candidates = (stackalloc MinLexCandidate[CandidateListTotal]); //rows 0,2,4,6,8
		var candidates1 = (stackalloc MinLexCandidate[CandidateListTotal]); //rows 1,3,5,7
		var minTopRowScores = new int[2];
		var pair = (stackalloc GridPattern[] { new(), new() }); //original and transposed patterns
		var nGivens = GridPattern.FromString(source, ref pair[0], ref pair[1]); //compose the pair of the patterns for the original and transposed morph

		Mappers.Clear();

		minTopRowScores[0] = GridPattern.BestTopRowScore(in pair[0]);
		minTopRowScores[1] = GridPattern.BestTopRowScore(in pair[1]);
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

		//step 1: determine top rows
		int nCurCandidates;
		int nNextCandidates;
		nCurCandidates = 0;
		for (var nowTransposed = (sbyte)0; nowTransposed < 2; nowTransposed++)
		{
			if (minTopRowScores[nowTransposed] > minTopRowScore)
			{
				continue;
			}

			for (var topRow = 0; topRow < 9; topRow++)
			{
				if (GridPattern.MinCanNineBits[pair[nowTransposed].Rows[topRow]] > minTopRowScore)
				{
					continue;
				}

				//here we have a top row candidate
				var cand = new MinLexCandidate(nowTransposed, topRow); //in the empty template fix only the transposition and row 0
				cand.ExpandStacks(pair, minTopRowScore, candidates, ref nCurCandidates); //fix all minimal stack permutations and store for later row expansion
			}
		}
		//here we have all top row candidates with fixed top row, stacks, and column permutation masks

		//step 2: expand rows
		fixed (MinLexCandidate* p = candidates)    //read from here
		fixed (MinLexCandidate* q = candidates1) //add row and write here
		{
			var curCandidates = p;
			var nextCandidates = q;

			nNextCandidates = 0;
			for (var toRow = 1; toRow < 9; toRow++)
			{
				var bestTriplets0 = 7;
				var bestTriplets1 = 7;
				var bestTriplets2 = 7;
				var rowInBand = toRow % 3;
				for (var curCandidateIndex = 0; curCandidateIndex < nCurCandidates; curCandidateIndex++)
				{
					var old = &curCandidates[curCandidateIndex];
					int startRow, endRow;
					if (rowInBand != 0)
					{
						//combine with unmapped rows from the same band
						var band = old->mapRowsBackward[3 * (toRow / 3)] / 3;
						startRow = band * 3;
						endRow = startRow + 3;
					}
					else
					{
						//try any unmapped row
						startRow = 0;
						endRow = 9;
					}
					for (var fromRow = startRow; fromRow < endRow; fromRow++)
					{
						if (old->mapRowsForward[fromRow] >= 0)
						{
							continue; //skip previously mapped rows
						}

						var toTriplets = new int[3];
						var rowGivens = pair[old->isTransposed].Rows[fromRow]; //stacks unmapped
						toTriplets[Perm[old->stacksPerm, 0]] = rowGivens >> 6;// & 7;
						toTriplets[Perm[old->stacksPerm, 1]] = (rowGivens >> 3) & 7;
						toTriplets[Perm[old->stacksPerm, 2]] = rowGivens & 7;
						ref readonly var bt0 = ref BestTripletPermutations[toTriplets[0], old->colsPermMask[0]];
						if (bt0.bestResult > bestTriplets0)
						{
							continue;
						}

						if (bt0.bestResult < bestTriplets0)
						{
							nNextCandidates = 0;
							bestTriplets0 = bt0.bestResult;
							bestTriplets1 = 7;
							bestTriplets2 = 7;
						}
						ref readonly var bt1 = ref BestTripletPermutations[toTriplets[1], old->colsPermMask[1]];
						if (bt1.bestResult > bestTriplets1)
						{
							continue;
						}

						if (bt1.bestResult < bestTriplets1)
						{
							nNextCandidates = 0;
							bestTriplets1 = bt1.bestResult;
							bestTriplets2 = 7;
						}
						ref readonly var bt2 = ref BestTripletPermutations[toTriplets[2], old->colsPermMask[2]];
						if (bt2.bestResult > bestTriplets2)
						{
							continue;
						}

						if (bt2.bestResult < bestTriplets2)
						{
							nNextCandidates = 0;
							bestTriplets2 = bt2.bestResult;
						}

						//tests passed, output the new candidate
						var next = &nextCandidates[nNextCandidates++];
						*next = *old; //copy
						next->mapRowsForward[fromRow] = (sbyte)toRow;
						next->mapRowsBackward[toRow] = (sbyte)fromRow;
						next->colsPermMask[0] = (byte)bt0.resultMask;
						next->colsPermMask[1] = (byte)bt1.resultMask;
						next->colsPermMask[2] = (byte)bt2.resultMask;
					} //fromRow
				} //oldCandidateIndex

				//flip-flop the old/new
				var tmp = curCandidates;
				curCandidates = nextCandidates;
				nextCandidates = tmp;
				nCurCandidates = nNextCandidates;
				nNextCandidates = 0;

				//store the best result
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

			//step 3: find the lexicographically minimal representative within the morphs,
			// this time taking into account the real values of the input givens

			Unsafe.SkipInit<Mapper>(out var map);
			var minLex = (stackalloc int[81]); //the best result so far
			for (var i = 0; i < 81; i++)
			{
				minLex[i] = result[i] << 5; //initially set to large values
			}
			for (var curCandidateIndex = 0; curCandidateIndex < nCurCandidates; curCandidateIndex++)
			{
				ref var target = ref curCandidates[curCandidateIndex];
				var toTriplets = new int[3];
				toTriplets[Perm[target.stacksPerm, 0]] = 0;
				toTriplets[Perm[target.stacksPerm, 1]] = 3;
				toTriplets[Perm[target.stacksPerm, 2]] = 6;
				for (var colsPerm0 = 0; colsPerm0 < 6; colsPerm0++)
				{
					if (((target.colsPermMask[0] >> colsPerm0) & 1) == 0)
					{
						continue; //forbidden permutation
					}

					var toColsInStack = new int[9];
					toColsInStack[Perm[colsPerm0, 0]] = toTriplets[0];
					toColsInStack[Perm[colsPerm0, 1]] = toTriplets[0] + 1;
					toColsInStack[Perm[colsPerm0, 2]] = toTriplets[0] + 2;
					for (var colsPerm1 = 0; colsPerm1 < 6; colsPerm1++)
					{
						if (((target.colsPermMask[1] >> colsPerm1) & 1) == 0)
						{
							continue; //forbidden permutation
						}
						toColsInStack[3 + Perm[colsPerm1, 0]] = toTriplets[1];
						toColsInStack[3 + Perm[colsPerm1, 1]] = toTriplets[1] + 1;
						toColsInStack[3 + Perm[colsPerm1, 2]] = toTriplets[1] + 2;
						for (var colsPerm2 = 0; colsPerm2 < 6; colsPerm2++)
						{
							if (((target.colsPermMask[2] >> colsPerm2) & 1) == 0)
							{
								continue; //forbidden permutation
							}

							toColsInStack[6 + Perm[colsPerm2, 0]] = toTriplets[2];
							toColsInStack[6 + Perm[colsPerm2, 1]] = toTriplets[2] + 1;
							toColsInStack[6 + Perm[colsPerm2, 2]] = toTriplets[2] + 2;
							var labelPerm = new int[10]; //label mapping is unknown
							var nextFreeLabel = 1;
							var nSet = 0; //the number of givens with positions set
							for (var toRow = 0; toRow < 9; toRow++)
							{
								ref var rowGivens = ref pair[target.isTransposed].Digits[target.mapRowsBackward[toRow] * 9];
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
										//if(minLex[toRow * 9 + col] >= (1 << 5)) { //invalidate the rest only if they are touched
										//the following puzzle demonstrates this is a bug
										//................12.....1345..6.74..1.7.8.6.348.4312.76.8..25.635..6.7.2.6.2138.57
										for (var i = toRow * 9 + col + 1; i < 81; i++)
										{
											minLex[i] = result[i] << 5; //invalidate the rest
										}
										//}
										minLex[toRow * 9 + col] = labelPerm[fromDigit]; //the best result so far
																						//the buffered transformations become invalid at this point
										Mappers.Clear();
									}
									if (nSet == nGivens)
									{
										//an isomorph of the currently best ordering
										//at this point we have the necessary information for the transformation and can buffer it (if eventually it is one of the best ones)
										for (var r = 0; r < 9; r++)
										{
											for (var c = 0; c < 9; c++)
											{
												var src = target.isTransposed != 0 ? target.mapRowsBackward[r] + 9 * toColsInStack[c] : target.mapRowsBackward[r] * 9 + toColsInStack[c]; //source cell index for target rc
												map.Cell[src] = (byte)(minLex[r * 9 + c] != 0 ? r * 9 + c : 99); //map all non-givens to 99, this masking irrelevant permutations
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

										map.Label[0] = 0; //don't map zero to non-zero
										Mappers.Add(map); //this will automatically ignore duplicates
									}
								} //col
							} //toRow
						nextColsPerm:
							;
						} //colsPerm2
					} //colsPerm1
				} //colsPerm0
			} //candidate
			for (var i = 0; i < 81; i++)
			{
				result[i] = (char)(minLex[i] != 0 ? minLex[i] + '0' : '.'); //copy the integers to chars
			}
		}
	}
}
