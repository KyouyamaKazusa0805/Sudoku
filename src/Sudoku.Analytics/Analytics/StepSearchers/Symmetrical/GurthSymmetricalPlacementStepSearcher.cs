using System.Numerics;
using System.Runtime.CompilerServices;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Metadata;
using Sudoku.Analytics.Steps;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Rendering.Nodes;
using Sudoku.Runtime.MaskServices;
using static Sudoku.Analytics.ConclusionType;

namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Gurth's Symmetrical Placement</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Gurth's Symmetrical Placement</item>
/// <!--<item>Gurth's Symmetrical Placement (Shuffling Type)</item>-->
/// <item>Anti- Gurth's Symmetrical Placement</item>
/// </list>
/// </summary>
[StepSearcher(
	Technique.GurthSymmetricalPlacement, Technique.ExtendedGurthSymmetricalPlacement, Technique.AntiGurthSymmetricalPlacement,
	IsPure = true)]
public sealed partial class GurthSymmetricalPlacementStepSearcher : StepSearcher
{
	/// <summary>
	/// Normal types.
	/// </summary>
	private static readonly unsafe delegate*<ref readonly Grid, ref AnalysisContext, GurthSymmetricalPlacementStep?>[] NormalTypeCheckers = [
		&CheckDiagonal,
		&CheckAntiDiagonal,
		&CheckCentral
	];

	/// <summary>
	/// Anti types.
	/// </summary>
	private static readonly unsafe delegate*<ref readonly Grid, ref AnalysisContext, AntiGurthSymmetricalPlacementStep?>[] AntiTypeCheckers = [
		&CheckDiagonal_Anti,
		&CheckAntiDiagonal_Anti,
		&CheckXAxis_Anti,
		&CheckYAxis_Anti,
		&CheckCentral_Anti
	];


	/// <inheritdoc/>
	protected internal override unsafe Step? Collect(scoped ref AnalysisContext context)
	{
		// Normal types.
		foreach (var checker in NormalTypeCheckers)
		{
			if (checker(in context.Grid, ref context) is not { } normalTypeStep)
			{
				continue;
			}

			if (context.OnlyFindOne)
			{
				return normalTypeStep;
			}

			context.Accumulator.Add(normalTypeStep);
		}

		// Anti types.
		foreach (var checker in AntiTypeCheckers)
		{
			if (checker(in context.Grid, ref context) is not { } antiTypeStep)
			{
				continue;
			}

			if (context.OnlyFindOne)
			{
				return antiTypeStep;
			}

			context.Accumulator.Add(antiTypeStep);
		}

		return null;
	}


	/// <summary>
	/// Records all possible highlight cells.
	/// </summary>
	/// <param name="grid">The grid as reference.</param>
	/// <param name="cellOffsets">The target collection.</param>
	/// <param name="mapping">The mapping relation.</param>
	private static void RecordHighlightCells(scoped ref readonly Grid grid, List<CellViewNode> cellOffsets, scoped Span<Digit?> mapping)
	{
		scoped var colorIndices = (stackalloc Digit[9]);
		for (var (digit, colorIndexCurrent, digitsMaskBucket) = (0, 0, (Mask)0); digit < 9; digit++)
		{
			if ((digitsMaskBucket >> digit & 1) != 0)
			{
				continue;
			}

			var currentMappingRelationDigit = mapping[digit];

			colorIndices[digit] = colorIndexCurrent;
			digitsMaskBucket |= (Mask)(1 << digit);
			if (currentMappingRelationDigit is { } relatedDigit && relatedDigit != digit)
			{
				colorIndices[relatedDigit] = colorIndexCurrent;
				digitsMaskBucket |= (Mask)(1 << relatedDigit);
			}

			colorIndexCurrent++;
		}

		foreach (var cell in ~grid.EmptyCells)
		{
			cellOffsets.Add(new(colorIndices[grid.GetDigit(cell)], cell));
		}
	}

	/// <summary>
	/// Checks for diagonal symmetry steps.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <returns>A correct step if found; otherwise, <see langword="null"/>.</returns>
	private static GurthSymmetricalPlacementStep? CheckDiagonal(scoped ref readonly Grid grid, scoped ref AnalysisContext context)
	{
		var diagonalHasEmptyCell = false;
		for (var i = 0; i < 9; i++)
		{
			if (grid.GetState(i * 9 + i) == CellState.Empty)
			{
				diagonalHasEmptyCell = true;
				break;
			}
		}
		if (!diagonalHasEmptyCell)
		{
			// No conclusion.
			return null;
		}

		var mapping = new Digit?[9];
		for (var i = 0; i < 9; i++)
		{
			for (var j = 0; j < i; j++)
			{
				var c1 = i * 9 + j;
				var c2 = j * 9 + i;
				var condition = grid.GetState(c1) == CellState.Empty;
				if (condition ^ grid.GetState(c2) == CellState.Empty)
				{
					// One of two cells is empty. Not this symmetry.
					return null;
				}

				if (condition)
				{
					continue;
				}

				var d1 = grid.GetDigit(c1);
				var d2 = grid.GetDigit(c2);
				if (d1 == d2)
				{
					var o1 = mapping[d1];
					if (o1 is null)
					{
						mapping[d1] = d1;
						continue;
					}

					if (o1 != d1)
					{
						return null;
					}
				}
				else
				{
					var o1 = mapping[d1];
					var o2 = mapping[d2];
					if (o1.HasValue ^ o2.HasValue)
					{
						return null;
					}

					if (o1 is null && o2 is null)
					{
						mapping[d1] = d2;
						mapping[d2] = d1;
						continue;
					}

					// 'o1' and 'o2' are both not null.
					if (o1 != d2 || o2 != d1)
					{
						return null;
					}
				}
			}
		}

		var singleDigitList = new List<Digit>();
		for (var digit = 0; digit < 9; digit++)
		{
			var mappingDigit = mapping[digit];
			if (!mappingDigit.HasValue || mappingDigit == digit)
			{
				singleDigitList.Add(digit);
			}
		}

		var cellOffsets = new List<CellViewNode>();
		var candidateOffsets = new List<CandidateViewNode>();
		var conclusions = new List<Conclusion>();
		for (var i = 0; i < 9; i++)
		{
			var cell = i * 9 + i;
			if (grid.GetState(cell) != CellState.Empty)
			{
				continue;
			}

			foreach (var digit in grid.GetCandidates(cell))
			{
				if (singleDigitList.Contains(digit))
				{
					candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
					continue;
				}

				conclusions.Add(new(Elimination, cell, digit));
			}
		}
		RecordHighlightCells(in grid, cellOffsets, mapping);

		return conclusions.Count == 0
			? null
			: new(
				[.. conclusions],
				[[.. cellOffsets, .. candidateOffsets]],
				context.PredefinedOptions,
				SymmetricType.Diagonal,
				[.. mapping]
			);
	}

	/// <summary>
	/// Checks for anti-diagonal symmetry steps.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <returns>A correct step if found; otherwise, <see langword="null"/>.</returns>
	private static GurthSymmetricalPlacementStep? CheckAntiDiagonal(scoped ref readonly Grid grid, scoped ref AnalysisContext context)
	{
		var antiDiagonalHasEmptyCell = false;
		for (var i = 0; i < 9; i++)
		{
			if (grid.GetState(i * 9 + (8 - i)) == CellState.Empty)
			{
				antiDiagonalHasEmptyCell = true;
				break;
			}
		}
		if (!antiDiagonalHasEmptyCell)
		{
			// No conclusion.
			return null;
		}

		var mapping = new Digit?[9];
		for (var i = 0; i < 9; i++)
		{
			for (var j = 0; j < 8 - i; j++)
			{
				var c1 = i * 9 + j;
				var c2 = (8 - j) * 9 + (8 - i);
				var condition = grid.GetState(c1) == CellState.Empty;
				if (condition ^ grid.GetState(c2) == CellState.Empty)
				{
					// One of two cells is empty. Not this symmetry.
					return null;
				}

				if (condition)
				{
					continue;
				}

				var d1 = grid.GetDigit(c1);
				var d2 = grid.GetDigit(c2);
				if (d1 == d2)
				{
					var o1 = mapping[d1];
					if (o1 is null)
					{
						mapping[d1] = d1;
						continue;
					}

					if (o1 != d1)
					{
						return null;
					}
				}
				else
				{
					var o1 = mapping[d1];
					var o2 = mapping[d2];
					if (o1.HasValue ^ o2.HasValue)
					{
						return null;
					}

					if (o1 is null || o2 is null)
					{
						mapping[d1] = d2;
						mapping[d2] = d1;
						continue;
					}

					// 'o1' and 'o2' are both not null.
					if (o1 != d2 || o2 != d1)
					{
						return null;
					}
				}
			}
		}

		var singleDigitList = new List<Digit>();
		for (var digit = 0; digit < 9; digit++)
		{
			var mappingDigit = mapping[digit];
			if (!mappingDigit.HasValue || mappingDigit == digit)
			{
				singleDigitList.Add(digit);
			}
		}

		var cellOffsets = new List<CellViewNode>();
		var candidateOffsets = new List<CandidateViewNode>();
		var conclusions = new List<Conclusion>();
		for (var i = 0; i < 9; i++)
		{
			var cell = i * 9 + (8 - i);
			if (grid.GetState(cell) != CellState.Empty)
			{
				continue;
			}

			foreach (var digit in grid.GetCandidates(cell))
			{
				if (singleDigitList.Contains(digit))
				{
					candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
					continue;
				}

				conclusions.Add(new(Elimination, cell, digit));
			}
		}
		RecordHighlightCells(in grid, cellOffsets, mapping);

		return conclusions.Count == 0
			? null
			: new([.. conclusions], [[.. cellOffsets, .. candidateOffsets]], context.PredefinedOptions, SymmetricType.AntiDiagonal, [.. mapping]);
	}

	/// <summary>
	/// Checks for central symmetry steps.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <returns>A correct step if found; otherwise, <see langword="null"/>.</returns>
	private static GurthSymmetricalPlacementStep? CheckCentral(scoped ref readonly Grid grid, scoped ref AnalysisContext context)
	{
		if (grid.GetState(40) != CellState.Empty)
		{
			// Has no conclusion even though the grid may be symmetrical.
			return null;
		}

		var mapping = new Digit?[9];
		for (var cell = 0; cell < 40; cell++)
		{
			var anotherCell = 80 - cell;
			var condition = grid.GetState(cell) == CellState.Empty;
			if (condition ^ grid.GetState(anotherCell) == CellState.Empty)
			{
				// One of two cell is empty, not central symmetry type.
				return null;
			}

			if (condition)
			{
				continue;
			}

			var d1 = grid.GetDigit(cell);
			var d2 = grid.GetDigit(anotherCell);
			if (d1 == d2)
			{
				var o1 = mapping[d1];
				if (o1 is null)
				{
					mapping[d1] = d1;
					continue;
				}

				if (o1 != d1)
				{
					return null;
				}
			}
			else
			{
				var o1 = mapping[d1];
				var o2 = mapping[d2];
				if (o1 is not null ^ o2 is not null)
				{
					return null;
				}

				if (o1 is null || o2 is null)
				{
					mapping[d1] = d2;
					mapping[d2] = d1;
					continue;
				}

				// 'o1' and 'o2' are both not null.
				if (o1 != d2 || o2 != d1)
				{
					return null;
				}
			}
		}

		for (var digit = 0; digit < 9; digit++)
		{
			if (mapping[digit] is not null && mapping[digit] != digit)
			{
				continue;
			}

			var cellOffsets = new List<CellViewNode>();
			RecordHighlightCells(in grid, cellOffsets, mapping);

			return new(
				[new(Assignment, 40, digit)],
				[[.. cellOffsets, new CandidateViewNode(WellKnownColorIdentifier.Normal, 360 + digit)]],
				context.PredefinedOptions,
				SymmetricType.Central,
				[.. mapping]
			);
		}

		return null;
	}

	/// <summary>
	/// Checks for diagonal symmetry steps, anti-GSP type.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <returns>A correct step if found; otherwise, <see langword="null"/>.</returns>
	private static AntiGurthSymmetricalPlacementStep? CheckDiagonal_Anti(scoped ref readonly Grid grid, scoped ref AnalysisContext context)
	{
		var mapping = new Digit?[9];
		var cellsNotSymmetrical = CellMap.Empty;
		for (var i = 0; i < 9; i++)
		{
			for (var j = 0; j < i; j++)
			{
				var c1 = i * 9 + j;
				var c2 = j * 9 + i;
				var condition = grid.GetState(c1) == CellState.Empty;
				if (condition ^ grid.GetState(c2) == CellState.Empty)
				{
					// One of two cells is empty. Not this symmetry.
					if (cellsNotSymmetrical)
					{
						return null;
					}

					cellsNotSymmetrical.Add(c1);
					cellsNotSymmetrical.Add(c2);
					continue;
				}

				if (condition)
				{
					continue;
				}

				var d1 = grid.GetDigit(c1);
				var d2 = grid.GetDigit(c2);
				if (d1 == d2)
				{
					var o1 = mapping[d1];
					if (o1 is null)
					{
						mapping[d1] = d1;
						continue;
					}

					if (o1 != d1)
					{
						return null;
					}
				}
				else
				{
					var o1 = mapping[d1];
					var o2 = mapping[d2];
					if (o1.HasValue ^ o2.HasValue)
					{
						return null;
					}

					if (o1 is null && o2 is null)
					{
						mapping[d1] = d2;
						mapping[d2] = d1;
						continue;
					}

					// 'o1' and 'o2' are both not null.
					if (o1 != d2 || o2 != d1)
					{
						return null;
					}
				}
			}
		}

		if (!cellsNotSymmetrical)
		{
			// All cells are symmetrical placements. This will have been checked in normal types.
			return null;
		}

		var singleDigitList = new List<Digit>();
		for (var digit = 0; digit < 9; digit++)
		{
			var mappingDigit = mapping[digit];
			if (!mappingDigit.HasValue || mappingDigit == digit)
			{
				singleDigitList.Add(digit);
			}
		}

		var singleDigitsMask = MaskOperations.Create([.. singleDigitList]);

		// Now check for diagonal line cells, determining whether the solution grid may not be a symmetrical placement.
		var isSolutionAsymmetry = singleDigitList.Count > 3;
		foreach (var cell in SymmetricType.Diagonal.GetCellsInSymmetryAxis())
		{
			if ((Mask)(grid.GetCandidates(cell) & singleDigitsMask) == 0)
			{
				// We can conclude that the solution should be an asymmetrical placement if one of two following conditions is true:
				// 1) If the cell is empty, and no possible candidates in the single-digit set can be filled into the cell.
				// 2) If the cell is not empty, and it is filled with other digits not appearing in the single-digit set.
				// Two conditions can be expressed with one boolean expression.
				isSolutionAsymmetry = true;
				break;
			}
		}
		if (!isSolutionAsymmetry)
		{
			// We cannot determine whether the solution is an asymmetrical placement.
			return null;
		}

		var gridCopied = grid;
		var elimCell = cellsNotSymmetrical.First(cell => gridCopied.GetState(cell) == CellState.Empty);
		var elimDigit = mapping[grid.GetDigit((cellsNotSymmetrical - elimCell)[0])]!.Value;
		if ((grid.GetCandidates(elimCell) >> elimDigit & 1) == 0)
		{
			// No elimination.
			return null;
		}

		var cellOffsets = new List<CellViewNode>();
		var candidateOffsets = new List<CandidateViewNode>();
		RecordHighlightCells(in grid, cellOffsets, mapping);

		return new(
			[new(Elimination, elimCell, elimDigit)],
			[[.. cellOffsets, .. candidateOffsets]],
			context.PredefinedOptions,
			SymmetricType.Diagonal,
			[.. mapping]
		);
	}

	/// <summary>
	/// Checks for diagonal symmetry steps, anti-GSP type.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <returns>A correct step if found; otherwise, <see langword="null"/>.</returns>
	private static AntiGurthSymmetricalPlacementStep? CheckAntiDiagonal_Anti(scoped ref readonly Grid grid, scoped ref AnalysisContext context)
	{
		var mapping = new Digit?[9];
		var cellsNotSymmetrical = CellMap.Empty;
		for (var i = 0; i < 9; i++)
		{
			for (var j = 0; j < 8 - i; j++)
			{
				var c1 = i * 9 + j;
				var c2 = (8 - j) * 9 + (8 - i);
				var condition = grid.GetState(c1) == CellState.Empty;
				if (condition ^ grid.GetState(c2) == CellState.Empty)
				{
					// One of two cells is empty. Not this symmetry.
					if (cellsNotSymmetrical)
					{
						return null;
					}

					cellsNotSymmetrical.Add(c1);
					cellsNotSymmetrical.Add(c2);
					continue;
				}

				if (condition)
				{
					continue;
				}

				var d1 = grid.GetDigit(c1);
				var d2 = grid.GetDigit(c2);
				if (d1 == d2)
				{
					var o1 = mapping[d1];
					if (o1 is null)
					{
						mapping[d1] = d1;
						continue;
					}

					if (o1 != d1)
					{
						return null;
					}
				}
				else
				{
					var o1 = mapping[d1];
					var o2 = mapping[d2];
					if (o1.HasValue ^ o2.HasValue)
					{
						return null;
					}

					if (o1 is null || o2 is null)
					{
						mapping[d1] = d2;
						mapping[d2] = d1;
						continue;
					}

					// 'o1' and 'o2' are both not null.
					if (o1 != d2 || o2 != d1)
					{
						return null;
					}
				}
			}
		}

		if (!cellsNotSymmetrical)
		{
			// All cells are symmetrical placements. This will have been checked in normal types.
			return null;
		}

		var singleDigitList = new List<Digit>();
		for (var digit = 0; digit < 9; digit++)
		{
			var mappingDigit = mapping[digit];
			if (!mappingDigit.HasValue || mappingDigit == digit)
			{
				singleDigitList.Add(digit);
			}
		}

		var singleDigitsMask = MaskOperations.Create([.. singleDigitList]);

		// Now check for diagonal line cells, determining whether the solution grid may not be a symmetrical placement.
		var isSolutionAsymmetry = singleDigitList.Count > 3;
		foreach (var cell in SymmetricType.AntiDiagonal.GetCellsInSymmetryAxis())
		{
			if ((Mask)(grid.GetCandidates(cell) & singleDigitsMask) == 0)
			{
				// We can conclude that the solution should be an asymmetrical placement if one of two following conditions is true:
				// 1) If the cell is empty, and no possible candidates in the single-digit set can be filled into the cell.
				// 2) If the cell is not empty, and it is filled with other digits not appearing in the single-digit set.
				// Two conditions can be expressed with one boolean expression.
				isSolutionAsymmetry = true;
				break;
			}
		}
		if (!isSolutionAsymmetry)
		{
			// We cannot determine whether the solution is an asymmetrical placement.
			return null;
		}

		var gridCopied = grid;
		var elimCell = cellsNotSymmetrical.First(cell => gridCopied.GetState(cell) == CellState.Empty);
		var elimDigit = mapping[grid.GetDigit((cellsNotSymmetrical - elimCell)[0])]!.Value;
		if ((grid.GetCandidates(elimCell) >> elimDigit & 1) == 0)
		{
			// No elimination.
			return null;
		}

		var cellOffsets = new List<CellViewNode>();
		var candidateOffsets = new List<CandidateViewNode>();
		RecordHighlightCells(in grid, cellOffsets, mapping);

		return new(
			[new(Elimination, elimCell, elimDigit)],
			[[.. cellOffsets, .. candidateOffsets]],
			context.PredefinedOptions,
			SymmetricType.AntiDiagonal,
			[.. mapping]
		);
	}

	/// <summary>
	/// Checks for X-axis symmetry steps, anti-GSP type.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <returns>A correct step if found; otherwise, <see langword="null"/>.</returns>
	private static AntiGurthSymmetricalPlacementStep? CheckXAxis_Anti(scoped ref readonly Grid grid, scoped ref AnalysisContext context)
	{
		var mapping = new Digit?[9];
		var cellsNotSymmetrical = CellMap.Empty;
		for (var i = 0; i < 4; i++)
		{
			for (var j = 0; j < 9; j++)
			{
				var c1 = i * 9 + j;
				var c2 = (SymmetricType.XAxis.GetCells(c1) - c1)[0];
				var condition = grid.GetState(c1) == CellState.Empty;
				if (condition ^ grid.GetState(c2) == CellState.Empty)
				{
					// One of two cells is empty. Not this symmetry.
					if (cellsNotSymmetrical)
					{
						return null;
					}

					cellsNotSymmetrical.Add(c1);
					cellsNotSymmetrical.Add(c2);
					continue;
				}

				if (condition)
				{
					continue;
				}

				var d1 = grid.GetDigit(c1);
				var d2 = grid.GetDigit(c2);
				if (d1 == d2)
				{
					var o1 = mapping[d1];
					if (o1 is null)
					{
						mapping[d1] = d1;
						continue;
					}

					if (o1 != d1)
					{
						return null;
					}
				}
				else
				{
					var o1 = mapping[d1];
					var o2 = mapping[d2];
					if (o1.HasValue ^ o2.HasValue)
					{
						return null;
					}

					if (o1 is null || o2 is null)
					{
						mapping[d1] = d2;
						mapping[d2] = d1;
						continue;
					}

					// 'o1' and 'o2' are both not null.
					if (o1 != d2 || o2 != d1)
					{
						return null;
					}
				}
			}
		}

		if (!cellsNotSymmetrical)
		{
			// All cells are symmetrical placements. This will have been checked in normal types.
			return null;
		}

		var gridCopied = grid;
		var elimCell = cellsNotSymmetrical.First(cell => gridCopied.GetState(cell) == CellState.Empty);
		var elimDigit = mapping[grid.GetDigit((cellsNotSymmetrical - elimCell)[0])]!.Value;
		if ((grid.GetCandidates(elimCell) >> elimDigit & 1) == 0)
		{
			// No elimination.
			return null;
		}

		var cellOffsets = new List<CellViewNode>();
		var candidateOffsets = new List<CandidateViewNode>();
		RecordHighlightCells(in grid, cellOffsets, mapping);

		return new(
			[new(Elimination, elimCell, elimDigit)],
			[[.. cellOffsets, .. candidateOffsets]],
			context.PredefinedOptions,
			SymmetricType.XAxis,
			[.. mapping]
		);
	}

	/// <summary>
	/// Checks for Y-axis symmetry steps, anti-GSP type.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <returns>A correct step if found; otherwise, <see langword="null"/>.</returns>
	private static AntiGurthSymmetricalPlacementStep? CheckYAxis_Anti(scoped ref readonly Grid grid, scoped ref AnalysisContext context)
	{
		var mapping = new Digit?[9];
		var cellsNotSymmetrical = CellMap.Empty;
		for (var i = 0; i < 9; i++)
		{
			for (var j = 0; j < 4; j++)
			{
				var c1 = i * 9 + j;
				var c2 = (SymmetricType.YAxis.GetCells(c1) - c1)[0];
				var condition = grid.GetState(c1) == CellState.Empty;
				if (condition ^ grid.GetState(c2) == CellState.Empty)
				{
					// One of two cells is empty. Not this symmetry.
					if (cellsNotSymmetrical)
					{
						return null;
					}

					cellsNotSymmetrical.Add(c1);
					cellsNotSymmetrical.Add(c2);
					continue;
				}

				if (condition)
				{
					continue;
				}

				var d1 = grid.GetDigit(c1);
				var d2 = grid.GetDigit(c2);
				if (d1 == d2)
				{
					var o1 = mapping[d1];
					if (o1 is null)
					{
						mapping[d1] = d1;
						continue;
					}

					if (o1 != d1)
					{
						return null;
					}
				}
				else
				{
					var o1 = mapping[d1];
					var o2 = mapping[d2];
					if (o1.HasValue ^ o2.HasValue)
					{
						return null;
					}

					if (o1 is null || o2 is null)
					{
						mapping[d1] = d2;
						mapping[d2] = d1;
						continue;
					}

					// 'o1' and 'o2' are both not null.
					if (o1 != d2 || o2 != d1)
					{
						return null;
					}
				}
			}
		}

		if (!cellsNotSymmetrical)
		{
			// All cells are symmetrical placements. This will have been checked in normal types.
			return null;
		}

		var gridCopied = grid;
		var elimCell = cellsNotSymmetrical.First(cell => gridCopied.GetState(cell) == CellState.Empty);
		var elimDigit = mapping[grid.GetDigit((cellsNotSymmetrical - elimCell)[0])]!.Value;
		if ((grid.GetCandidates(elimCell) >> elimDigit & 1) == 0)
		{
			// No elimination.
			return null;
		}

		var cellOffsets = new List<CellViewNode>();
		var candidateOffsets = new List<CandidateViewNode>();
		RecordHighlightCells(in grid, cellOffsets, mapping);

		return new(
			[new(Elimination, elimCell, elimDigit)],
			[[.. cellOffsets, .. candidateOffsets]],
			context.PredefinedOptions,
			SymmetricType.YAxis,
			[.. mapping]
		);
	}

	/// <summary>
	/// Checks for central symmetry steps, anti-GSP type.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <returns>A correct step if found; otherwise, <see langword="null"/>.</returns>
	private static AntiGurthSymmetricalPlacementStep? CheckCentral_Anti(scoped ref readonly Grid grid, scoped ref AnalysisContext context)
	{
		var mapping = new Digit?[9];
		var cellsNotSymmetrical = CellMap.Empty;
		for (var cell = 0; cell < 40; cell++)
		{
			var anotherCell = 80 - cell;
			var condition = grid.GetState(cell) == CellState.Empty;
			if (condition ^ grid.GetState(anotherCell) == CellState.Empty)
			{
				// One of two cell is empty, not central symmetry type.
				if (cellsNotSymmetrical)
				{
					return null;
				}

				cellsNotSymmetrical.Add(cell);
				cellsNotSymmetrical.Add(anotherCell);
				continue;
			}

			if (condition)
			{
				continue;
			}

			var d1 = grid.GetDigit(cell);
			var d2 = grid.GetDigit(anotherCell);
			if (d1 == d2)
			{
				var o1 = mapping[d1];
				if (o1 is null)
				{
					mapping[d1] = d1;
					continue;
				}

				if (o1 != d1)
				{
					return null;
				}
			}
			else
			{
				var o1 = mapping[d1];
				var o2 = mapping[d2];
				if (o1 is not null ^ o2 is not null)
				{
					return null;
				}

				if (o1 is null || o2 is null)
				{
					mapping[d1] = d2;
					mapping[d2] = d1;
					continue;
				}

				// 'o1' and 'o2' are both not null.
				if (o1 != d2 || o2 != d1)
				{
					return null;
				}
			}
		}

		if (!cellsNotSymmetrical)
		{
			// All cells are symmetrical placements. This will have been checked in normal types.
			return null;
		}

		var singleDigitList = new List<Digit>();
		for (var digit = 0; digit < 9; digit++)
		{
			var mappingDigit = mapping[digit];
			if (!mappingDigit.HasValue || mappingDigit == digit)
			{
				singleDigitList.Add(digit);
			}
		}

		var singleDigitsMask = MaskOperations.Create([.. singleDigitList]);

		// Now check for diagonal line cells, determining whether the solution grid may not be a symmetrical placement.
		var isSolutionAsymmetry = singleDigitList.Count > 1 || (Mask)(grid.GetCandidates(40) & singleDigitsMask) == 0;
		if (!isSolutionAsymmetry)
		{
			// We cannot determine whether the solution is an asymmetrical placement.
			return null;
		}

		var gridCopied = grid;
		var elimCell = cellsNotSymmetrical.First(cell => gridCopied.GetState(cell) == CellState.Empty);
		var elimDigit = mapping[grid.GetDigit((cellsNotSymmetrical - elimCell)[0])]!.Value;
		if ((grid.GetCandidates(elimCell) >> elimDigit & 1) == 0)
		{
			// No elimination.
			return null;
		}

		var cellOffsets = new List<CellViewNode>();
		var candidateOffsets = new List<CandidateViewNode>();
		RecordHighlightCells(in grid, cellOffsets, mapping);

		return new(
			[new(Elimination, elimCell, elimDigit)],
			[[.. cellOffsets, .. candidateOffsets]],
			context.PredefinedOptions,
			SymmetricType.Central,
			[.. mapping]
		);
	}
}
