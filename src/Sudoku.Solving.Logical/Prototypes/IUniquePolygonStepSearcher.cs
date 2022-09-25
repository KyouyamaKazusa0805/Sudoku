namespace Sudoku.Solving.Logical.Prototypes;

/// <summary>
/// Provides with a <b>Unique Polygon</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Unique Polygon Type 1</item>
/// <item>Unique Polygon Type 2</item>
/// <item>Unique Polygon Type 3</item>
/// <item>Unique Polygon Type 4</item>
/// </list>
/// </summary>
public interface IUniquePolygonStepSearcher : IDeadlyPatternStepSearcher
{
	/// <summary>
	/// Indicates all possible patterns to iterate.
	/// </summary>
	/// <remarks>
	/// Please note that all possible heptagons and octagons are in here.
	/// </remarks>
	protected static readonly UniquePolygonPattern[] Patterns = new UniquePolygonPattern[BdpTemplatesSize3Count];


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static IUniquePolygonStepSearcher()
	{
		int[][] quads =
		{
			new[] { 0, 1, 3, 4 }, new[] { 1, 2, 4, 5 }, new[] { 3, 4, 6, 7 }, new[] { 4, 5, 7, 8 },
			new[] { 0, 2, 3, 5 }, new[] { 3, 5, 6, 8 }, new[] { 0, 1, 6, 7 }, new[] { 1, 2, 7, 8 },
			new[] { 0, 2, 6, 8 }
		};

		var count = 0;
		for (var block = 0; block < 9; block++)
		{
			for (var i = 0; i < 9; i++) // 9 cases.
			{
				var quad = quads[i];
				var tempQuad = new int[4];
				for (var j = 0; j < 4; j++)
				{
					// Set all indices to cell offsets.
					tempQuad[j] = (block / 3 * 3 + quad[j] / 3) * 9 + block % 3 * 3 + quad[j] % 3;
				}

				gatherHeptagons(block, i, tempQuad, ref count);
				gatherOctagons(block, i, tempQuad, ref count);
			}
		}


		static void gatherHeptagons(int block, int i, int[] quad, scoped ref int count)
		{
			if (quad is not [var q1, var q2, var q3, var q4])
			{
				return;
			}

			var triplets = new int[4][]
			{
				new[] { q1, q2, q3 }, // (0, 1) and (0, 2) is same house.
				new[] { q2, q1, q4 }, // (0, 1) and (1, 3) is same house.
				new[] { q3, q1, q4 }, // (0, 2) and (2, 3) is same house.
				new[] { q4, q2, q3 }, // (1, 3) and (2, 3) is same house.
			};

			for (var j = 0; j < 4; j++)
			{
				if (triplets[j] is not [var t1, var t2, var t3] triplet)
				{
					continue;
				}

				var house1 = (CellsMap[t1] + t2).CoveredLine;
				var house2 = (CellsMap[t1] + t3).CoveredLine;
				int[,] pair1 = new int[6, 2], pair2 = new int[6, 2];
				var (incre1, incre2) = i switch
				{
					0 or 1 or 2 or 3 => (9, 1),
					4 or 5 => (9, 2),
					6 or 7 => (18, 1),
					8 => (18, 2)
				};
				if (house1 is >= 9 and < 18)
				{
					// 'house1' is a row and 'house2' is a column.
					r(block, house1, pair1, incre1, j);
					r(block, house2, pair2, incre2, j);
				}
				else
				{
					// 'house1' is a column and 'house2' is a row.
					r(block, house1, pair1, incre2, j);
					r(block, house2, pair2, incre1, j);
				}

				for (var i1 = 0; i1 < 6; i1++)
				{
					for (var i2 = 0; i2 < 6; i2++)
					{
						// Now check extra digits.
						var allCells = new List<int>(triplet)
						{
							pair1[i1, 0], pair1[i1, 1], pair2[i2, 0], pair2[i2, 1]
						};

						long v = 0;
						for (int z = 0, iterationCount = allCells.Count; z < iterationCount; z++)
						{
							v |= (long)allCells[z];

							if (z != allCells.Count - 1)
							{
								v <<= 7;
							}

							if (z == 2)
							{
								v |= 127;
								v <<= 7;
							}
						}

						Patterns[count++] = new(v);
					}
				}
			}
		}

		static void gatherOctagons(int block, int i, int[] quad, scoped ref int count)
		{
			if (quad is not [var t1, var t2, var t3, _])
			{
				return;
			}

			var house1 = (CellsMap[t1] + t2).CoveredLine;
			var house2 = (CellsMap[t1] + t3).CoveredLine;
			int[,] pair1 = new int[6, 2], pair2 = new int[6, 2];
			var (incre1, incre2) = i switch
			{
				0 or 1 or 2 or 3 => (9, 1),
				4 or 5 => (9, 2),
				6 or 7 => (18, 1),
				8 => (18, 2)
			};
			if (house1 is >= 9 and < 18)
			{
				// 'house1' is a row and 'house2' is a column.
				r(block, house1, pair1, incre1, 0);
				r(block, house2, pair2, incre2, 0);
			}
			else
			{
				// 'house1' is a column and 'house2' is a row.
				r(block, house1, pair1, incre2, 0);
				r(block, house2, pair2, incre1, 0);
			}

			for (var i1 = 0; i1 < 6; i1++)
			{
				for (var i2 = 0; i2 < 6; i2++)
				{
					// Now check extra digits.
					var allCells = new List<int>(quad)
					{
						pair1[i1, 0],
						pair1[i1, 1],
						pair2[i2, 0],
						pair2[i2, 1]
					};

					long v = 0;
					for (int z = 0, iterationCount = allCells.Count; z < iterationCount; z++)
					{
						var cell = allCells[z];
						v |= (long)cell;
						if (z != allCells.Count - 1)
						{
							v <<= 7;
						}
					}

					Patterns[count++] = new(v);
				}
			}
		}

		static void r(int block, int houseIndex, int[,] pair, int increment, int index)
		{
			for (int i = 0, cur = 0; i < 9; i++)
			{
				var cell = HouseCells[houseIndex][i];
				if (block == cell.ToHouseIndex(HouseType.Block))
				{
					continue;
				}

				(pair[cur, 0], pair[cur, 1]) = index switch
				{
					0 => (cell, cell + increment),
					1 => houseIndex is >= 18 and < 27 ? (cell - increment, cell) : (cell, cell + increment),
					2 => houseIndex is >= 9 and < 18 ? (cell - increment, cell) : (cell, cell + increment),
					3 => (cell - increment, cell)
				};
				cur++;
			}
		}
	}
}
