using System.Numerics;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Metadata;
using Sudoku.Analytics.Steps;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Rendering.Nodes;
using Sudoku.Runtime.MaskServices;
using static System.Numerics.BitOperations;
using static Sudoku.Analytics.CachedFields;
using static Sudoku.Analytics.ConclusionType;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with an <b>Extended Rectangle</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Extended Rectangle Type 1</item>
/// <item>Extended Rectangle Type 2</item>
/// <item>Extended Rectangle Type 3</item>
/// <item>Extended Rectangle Type 4</item>
/// </list>
/// </summary>
[StepSearcher(
	Technique.ExtendedRectangleType1, Technique.ExtendedRectangleType2, Technique.ExtendedRectangleType3, Technique.ExtendedRectangleType4,
	Flags = ConditionalFlags.Standard)]
public sealed partial class ExtendedRectangleStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates all possible extended rectangle pattern combinations.
	/// </summary>
	/// <remarks>
	/// <para>The list contains two types of <b>Extended Rectangle</b>s:</para>
	/// <para>
	/// Fit type (2 blocks spanned):
	/// <code><![CDATA[
	/// ab | ab
	/// bc | bc
	/// ac | ac
	/// ]]></code>
	/// </para>
	/// <para>
	/// Fat type (3 blocks spanned):
	/// <code><![CDATA[
	/// ab | ac | bc
	/// ab | ac | bc
	/// ]]></code>
	/// </para>
	/// </remarks>
	private static readonly List<(CellMap Cells, List<(Cell Left, Cell Right)> PairCells, int Size)> RawPatternData;


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static ExtendedRectangleStepSearcher()
	{
#pragma warning disable format
		var houses = (int[][])[
			[9, 10], [9, 11], [10, 11],
			[12, 13], [12, 14], [13, 14],
			[15, 16], [15, 17], [16, 17],
			[18, 19], [18, 20], [19, 20],
			[21, 22], [21, 23], [22, 23],
			[24, 25], [24, 26], [25, 26]
		];
		var fitTableRow = (int[][])[
			[0, 3], [0, 4], [0, 5], [0, 6], [0, 7], [0, 8],
			[1, 3], [1, 4], [1, 5], [1, 6], [1, 7], [1, 8],
			[2, 3], [2, 4], [2, 5], [2, 6], [2, 7], [2, 8],
			[3, 6], [3, 7], [3, 8],
			[4, 6], [4, 7], [4, 8],
			[5, 6], [5, 7], [5, 8]
		];
		var fitTableColumn = (int[][])[
			[0, 27], [0, 36], [0, 45], [0, 54], [0, 63], [0, 72],
			[9, 27], [9, 36], [9, 45], [9, 54], [9, 63], [9, 72],
			[18, 27], [18, 36], [18, 45], [18, 54], [18, 63], [18, 72],
			[27, 54], [27, 63], [27, 72],
			[36, 54], [36, 63], [36, 72],
			[45, 54], [45, 63], [45, 72]
		];
#pragma warning restore format

		RawPatternData = [];

		// Initializes fit types.
		for (var j = 0; j < 3; j++)
		{
			for (var i = 0; i < fitTableRow.Length >> 1; i++)
			{
				var c11 = fitTableRow[i][0] + j * 27;
				var c21 = fitTableRow[i][1] + j * 27;
				var c12 = c11 + 9;
				var c22 = c21 + 9;
				var c13 = c11 + 18;
				var c23 = c21 + 18;
				RawPatternData.Add(([c11, c12, c13, c21, c22, c23], [(c11, c21), (c12, c22), (c13, c23)], 3));
			}
		}
		for (var j = 0; j < 3; j++)
		{
			for (var i = 0; i < fitTableColumn.Length >> 1; i++)
			{
				var c11 = fitTableColumn[i][0] + j * 3;
				var c21 = fitTableColumn[i][1] + j * 3;
				var c12 = c11 + 1;
				var c22 = c21 + 1;
				var c13 = c11 + 2;
				var c23 = c21 + 2;
				RawPatternData.Add(([c11, c12, c13, c21, c22, c23], [(c11, c21), (c12, c22), (c13, c23)], 3));
			}
		}

		// Initializes fat types.
		for (var size = 3; size <= 7; size++)
		{
			for (var i = 0; i < houses.Length >> 1; i++)
			{
				var (house1, house2) = (houses[i][0], houses[i][1]);
				foreach (Mask mask in new MaskCombinationsGenerator(9, size))
				{
					// Check whether all cells are in same house. If so, continue the loop immediately.
					if (size == 3 && mask.SplitMask() is not (not 7, not 7, not 7))
					{
						continue;
					}

					var (map, pairs) = (CellMap.Empty, (List<(Cell, Cell)>)[]);
					foreach (var pos in mask)
					{
						var (cell1, cell2) = (HouseCells[house1][pos], HouseCells[house2][pos]);
						map.Add(cell1);
						map.Add(cell2);
						pairs.Add((cell1, cell2));
					}

					RawPatternData.Add((map, pairs, size));
				}
			}
		}
	}


	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		var accumulator = context.Accumulator!;
		var onlyFindOne = context.OnlyFindOne;
		foreach (var (allCellsMap, pairs, size) in RawPatternData)
		{
			if ((EmptyCells & allCellsMap) != allCellsMap)
			{
				continue;
			}

			// Check each pair.
			// Ensures all pairs should contains same digits
			// and the kind of digits must be greater than 2.
			var checkKindsFlag = true;
			foreach (var (l, r) in pairs)
			{
				var tempMask = (Mask)(grid.GetCandidates(l) & grid.GetCandidates(r));
				if (tempMask == 0 || (tempMask & tempMask - 1) == 0)
				{
					checkKindsFlag = false;
					break;
				}
			}
			if (!checkKindsFlag)
			{
				// Failed to check.
				continue;
			}

			// Check the mask of cells from two houses.
			var m1 = (Mask)0;
			var m2 = (Mask)0;
			foreach (var (l, r) in pairs)
			{
				m1 |= grid.GetCandidates(l);
				m2 |= grid.GetCandidates(r);
			}

			var resultMask = (Mask)(m1 | m2);
			var normalDigits = (Mask)0;
			var extraDigits = (Mask)0;
			foreach (var digit in resultMask)
			{
				var count = 0;
				foreach (var (l, r) in pairs)
				{
					if (((grid.GetCandidates(l) & grid.GetCandidates(r)) >> digit & 1) != 0)
					{
						// Both two cells contain same digit.
						count++;
					}
				}

				(count >= 2 ? ref normalDigits : ref extraDigits) |= (Mask)(1 << digit);
			}

			if (PopCount((uint)normalDigits) != size)
			{
				// The number of normal digits are not enough.
				continue;
			}

			if (PopCount((uint)resultMask) == size + 1)
			{
				// Possible type 1 or 2 found.
				// Now check extra cells.
				var extraDigit = TrailingZeroCount(extraDigits);
				var extraCellsMap = allCellsMap & CandidatesMap[extraDigit];
				if (!extraCellsMap)
				{
					continue;
				}

				if (extraCellsMap.Count == 1)
				{
					if (CheckType1(accumulator, in grid, ref context, in allCellsMap, in extraCellsMap, normalDigits, extraDigit, onlyFindOne) is { } step1)
					{
						return step1;
					}
				}

				if (CheckType2(accumulator, in grid, ref context, in allCellsMap, in extraCellsMap, normalDigits, extraDigit, onlyFindOne) is { } step2)
				{
					return step2;
				}
			}
			else
			{
				var extraCellsMap = CellMap.Empty;
				foreach (var cell in allCellsMap)
				{
					foreach (var digit in extraDigits)
					{
						if (grid.GetCandidateIsOn(cell, digit))
						{
							extraCellsMap.Add(cell);
							break;
						}
					}
				}

				if (!extraCellsMap.InOneHouse(out _))
				{
					continue;
				}

				if (CheckType3Naked(accumulator, in grid, ref context, in allCellsMap, normalDigits, extraDigits, in extraCellsMap, onlyFindOne) is { } step3)
				{
					return step3;
				}

				if (CheckType14(accumulator, in grid, ref context, in allCellsMap, normalDigits, in extraCellsMap, onlyFindOne) is { } step14)
				{
					return step14;
				}
			}
		}

		return null;
	}

	/// <summary>
	/// Check type 1.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="allCellsMap">The map of all cells used.</param>
	/// <param name="extraCells">The extra cells map.</param>
	/// <param name="normalDigits">The normal digits mask.</param>
	/// <param name="extraDigit">The extra digit.</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searches for one step.</param>
	/// <returns>The first found step if worth.</returns>
	private ExtendedRectangleType1Step? CheckType1(
		List<Step> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		scoped ref readonly CellMap allCellsMap,
		scoped ref readonly CellMap extraCells,
		Mask normalDigits,
		Digit extraDigit,
		bool onlyFindOne
	)
	{
		var (conclusions, candidateOffsets) = (new List<Conclusion>(), new List<CandidateViewNode>());
		foreach (var cell in allCellsMap)
		{
			if (cell == extraCells[0])
			{
				foreach (var digit in grid.GetCandidates(cell))
				{
					if (digit != extraDigit)
					{
						conclusions.Add(new(Elimination, cell, digit));
					}
				}
			}
			else
			{
				foreach (var digit in grid.GetCandidates(cell))
				{
					candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
				}
			}
		}

		if (conclusions.Count == 0)
		{
			goto ReturnNull;
		}

		var step = new ExtendedRectangleType1Step(
			[.. conclusions],
			[[.. candidateOffsets]],
			context.PredefinedOptions,
			in allCellsMap,
			normalDigits
		);
		if (onlyFindOne)
		{
			return step;
		}

		accumulator.Add(step);

	ReturnNull:
		return null;
	}

	/// <summary>
	/// Check type 2.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="allCellsMap">The map of all cells used.</param>
	/// <param name="extraCells">The extra cells map.</param>
	/// <param name="normalDigits">The normal digits mask.</param>
	/// <param name="extraDigit">The extra digit.</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searches for one step.</param>
	/// <returns>The first found step if worth.</returns>
	private ExtendedRectangleType2Step? CheckType2(
		List<Step> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		scoped ref readonly CellMap allCellsMap,
		scoped ref readonly CellMap extraCells,
		Mask normalDigits,
		Digit extraDigit,
		bool onlyFindOne
	)
	{
		if ((extraCells.PeerIntersection & CandidatesMap[extraDigit]) is not (var elimMap and not []))
		{
			goto ReturnNull;
		}

		var candidateOffsets = new List<CandidateViewNode>();
		foreach (var cell in allCellsMap)
		{
			foreach (var digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add(new(digit == extraDigit ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal, cell * 9 + digit));
			}
		}

		var step = new ExtendedRectangleType2Step(
			[.. from cell in elimMap select new Conclusion(Elimination, cell, extraDigit)],
			[[.. candidateOffsets]],
			context.PredefinedOptions,
			in allCellsMap,
			normalDigits,
			extraDigit
		);
		if (onlyFindOne)
		{
			return step;
		}

		accumulator.Add(step);

	ReturnNull:
		return null;
	}

	/// <summary>
	/// Check type 3.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="allCellsMap">The map of all cells used.</param>
	/// <param name="normalDigits">The normal digits mask.</param>
	/// <param name="extraDigits">The extra digits mask.</param>
	/// <param name="extraCellsMap">The map of extra cells.</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searches for one step.</param>
	/// <returns>The first found step if worth.</returns>
	private ExtendedRectangleType3Step? CheckType3Naked(
		List<Step> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		scoped ref readonly CellMap allCellsMap,
		Mask normalDigits,
		Mask extraDigits,
		scoped ref readonly CellMap extraCellsMap,
		bool onlyFindOne
	)
	{
		foreach (var houseIndex in extraCellsMap.CoveredHouses)
		{
			var otherCells = (HousesMap[houseIndex] & EmptyCells) - allCellsMap;
			for (var size = 1; size < otherCells.Count; size++)
			{
				foreach (ref readonly var cells in otherCells.GetSubsets(size))
				{
					var mask = grid[in cells];
					if ((mask & extraDigits) != extraDigits || PopCount((uint)mask) != size + 1)
					{
						continue;
					}

					var elimMap = (HousesMap[houseIndex] & EmptyCells) - allCellsMap - cells;
					if (!elimMap)
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					foreach (var digit in mask)
					{
						foreach (var cell in elimMap & CandidatesMap[digit])
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<CandidateViewNode>();
					foreach (var cell in allCellsMap - extraCellsMap)
					{
						foreach (var digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
						}
					}
					foreach (var cell in extraCellsMap)
					{
						foreach (var digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(
								new(
									(mask >> digit & 1) != 0 ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal,
									cell * 9 + digit
								)
							);
						}
					}
					foreach (var cell in cells)
					{
						foreach (var digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, cell * 9 + digit));
						}
					}

					var step = new ExtendedRectangleType3Step(
						[.. conclusions],
						[[.. candidateOffsets, new HouseViewNode(0, houseIndex)]],
						context.PredefinedOptions,
						in allCellsMap,
						normalDigits,
						in cells,
						mask,
						houseIndex
					);

					if (onlyFindOne)
					{
						return step;
					}

					accumulator.Add(step);
				}
			}
		}

		return null;
	}

	/// <summary>
	/// Check type 4 and a part of type 1 that the method
	/// <see cref="CheckType1(List{Step}, ref readonly Grid, ref AnalysisContext, ref readonly CellMap, ref readonly CellMap, Mask, Digit, bool)"/>
	/// cannot be found.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="allCellsMap">The map of all cells used.</param>
	/// <param name="normalDigits">The normal digits mask.</param>
	/// <param name="extraCellsMap">The map of extra cells.</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searches for one step.</param>
	/// <returns>The first found step if worth.</returns>
	private Step? CheckType14(
		List<Step> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		scoped ref readonly CellMap allCellsMap,
		Mask normalDigits,
		scoped ref readonly CellMap extraCellsMap,
		bool onlyFindOne
	)
	{
		switch (extraCellsMap)
		{
			case [var extraCell]:
			{
				// Type 1 found.
				// Check eliminations.
				var conclusions = new List<Conclusion>();
				foreach (var digit in normalDigits)
				{
					if (CandidatesMap[digit].Contains(extraCell))
					{
						conclusions.Add(new(Elimination, extraCell, digit));
					}
				}

				if (conclusions.Count == 0)
				{
					goto ReturnNull;
				}

				// Gather all highlight candidates.
				var candidateOffsets = new List<CandidateViewNode>();
				foreach (var cell in allCellsMap)
				{
					if (cell == extraCell)
					{
						continue;
					}

					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
					}
				}

				var step = new ExtendedRectangleType1Step(
					[.. conclusions],
					[[.. candidateOffsets]],
					context.PredefinedOptions,
					in allCellsMap,
					normalDigits
				);
				if (onlyFindOne)
				{
					return step;
				}

				accumulator.Add(step);

				break;
			}
			case [var extraCell1, var extraCell2]:
			{
				// Type 4.
				var m1 = grid.GetCandidates(extraCell1);
				var m2 = grid.GetCandidates(extraCell2);
				var conjugateMask = (Mask)(m1 & m2 & normalDigits);
				if (conjugateMask == 0)
				{
					goto ReturnNull;
				}

				foreach (var conjugateDigit in conjugateMask)
				{
					foreach (var houseIndex in extraCellsMap.CoveredHouses)
					{
						var map = HousesMap[houseIndex] & extraCellsMap;
						if (map != extraCellsMap || map != (CandidatesMap[conjugateDigit] & HousesMap[houseIndex]))
						{
							continue;
						}

						var elimDigits = (Mask)(normalDigits & ~(1 << conjugateDigit));
						var conclusions = new List<Conclusion>();
						foreach (var digit in elimDigits)
						{
							foreach (var cell in extraCellsMap & CandidatesMap[digit])
							{
								conclusions.Add(new(Elimination, cell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<CandidateViewNode>();
						foreach (var cell in allCellsMap - extraCellsMap)
						{
							foreach (var digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
							}
						}
						foreach (var cell in extraCellsMap)
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, cell * 9 + conjugateDigit));
						}

						var step = new ExtendedRectangleType4Step(
							[.. conclusions],
							[[.. candidateOffsets, new HouseViewNode(0, houseIndex)]],
							context.PredefinedOptions,
							in allCellsMap,
							normalDigits,
							new(in extraCellsMap, conjugateDigit)
						);

						if (onlyFindOne)
						{
							return step;
						}

						accumulator.Add(step);
					}
				}

				break;
			}
		}

	ReturnNull:
		return null;
	}
}
