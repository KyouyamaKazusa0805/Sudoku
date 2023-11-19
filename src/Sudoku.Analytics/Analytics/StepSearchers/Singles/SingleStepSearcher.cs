using System.Numerics;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Metadata;
using Sudoku.Analytics.Steps;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Rendering.Nodes;
using static System.Numerics.BitOperations;
using static Sudoku.Analytics.ConclusionType;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Single</b> step searcher. The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Full House (If the property <see cref="EnableFullHouse"/> is <see langword="true"/>)</item>
/// <item>Last Digit (If the property <see cref="EnableLastDigit"/> is <see langword="true"/>)</item>
/// <item>Hidden Single</item>
/// <item>Naked Single</item>
/// </list>
/// </summary>
[StepSearcher(
	Technique.LastDigit, Technique.FullHouse, Technique.HiddenSingleBlock, Technique.HiddenSingleRow,
	Technique.HiddenSingleColumn, Technique.NakedSingle,
	IsPure = true, IsFixed = true)]
public sealed partial class SingleStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates whether the solver enables the technique full house.
	/// </summary>
	[RuntimeIdentifier(RuntimeIdentifier.EnableFullHouse)]
	public bool EnableFullHouse { get; set; }

	/// <summary>
	/// Indicates whether the solver enables the technique last digit.
	/// </summary>
	[RuntimeIdentifier(RuntimeIdentifier.EnableLastDigit)]
	public bool EnableLastDigit { get; set; }

	/// <summary>
	/// Indicates whether the solver checks for hidden single in block firstly.
	/// </summary>
	[RuntimeIdentifier(RuntimeIdentifier.HiddenSinglesInBlockFirst)]
	public bool HiddenSinglesInBlockFirst { get; set; }

	/// <summary>
	/// Indicates whether the solver uses ittoryu mode to solve a puzzle.
	/// </summary>
	[RuntimeIdentifier(RuntimeIdentifier.AnalyzerUseIttoryuMode)]
	public bool UseIttoryuMode { get; set; }


	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
		=> UseIttoryuMode ? Collect_IttoryuMode(ref context) : Collect_NonIttoryuMode(ref context);

	/// <summary>
	/// Checks for single steps using ittoryu mode.
	/// </summary>
	/// <param name="context"><inheritdoc cref="Collect(ref AnalysisContext)" path="/param[@name='context']"/></param>
	/// <returns><inheritdoc cref="Collect(ref AnalysisContext)" path="/returns"/></returns>
	private Step? Collect_IttoryuMode(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		for (var (i, digit) = (0, context.PreviousSetDigit); i < 9; i++, digit = (digit + 1) % 9)
		{
			if (!EnableFullHouse)
			{
				goto CheckForHiddenSingle;
			}

			for (var house = 0; house < 27; house++)
			{
				var (count, resultCell, flag) = (0, -1, true);
				foreach (var cell in HousesMap[house])
				{
					if (grid.GetState(cell) == CellState.Empty && (resultCell = cell) is var _ && ++count > 1)
					{
						flag = false;
						break;
					}
				}
				if (!flag || count == 0)
				{
					continue;
				}

				if (TrailingZeroCount(grid.GetCandidates(resultCell)) != digit)
				{
					continue;
				}

				var step = new FullHouseStep(
					[new(Assignment, resultCell, digit)],
					[[new HouseViewNode(WellKnownColorIdentifier.Normal, house)]],
					context.PredefinedOptions,
					house,
					resultCell,
					digit
				);

				if (context.OnlyFindOne)
				{
					context.PreviousSetDigit = digit;
					return step;
				}

				context.Accumulator.Add(step);
			}

		CheckForHiddenSingle:
			for (var house = 0; house < 27; house++)
			{
				if (CheckForHiddenSingleAndLastDigit(in grid, ref context, digit, house) is not { } step)
				{
					continue;
				}

				if (context.OnlyFindOne)
				{
					context.PreviousSetDigit = digit;
					return step;
				}

				context.Accumulator.Add(step);
			}

			for (var cell = 0; cell < 81; cell++)
			{
				if (grid.GetState(cell) != CellState.Empty)
				{
					continue;
				}

				var mask = grid.GetCandidates(cell);
				if (!IsPow2(mask))
				{
					continue;
				}

				var tempDigit = TrailingZeroCount(mask);
				if (tempDigit != digit)
				{
					continue;
				}

				var step = new NakedSingleStep(
					[new(Assignment, cell, digit)],
					[[.. GetNakedSingleExcluders(in grid, cell, digit)]],
					context.PredefinedOptions,
					cell,
					digit
				);
				if (context.OnlyFindOne)
				{
					context.PreviousSetDigit = digit;
					return step;
				}

				context.Accumulator.Add(step);
			}
		}

		return null;
	}

	/// <summary>
	/// Checks for single steps using non-ittoryu mode.
	/// </summary>
	/// <param name="context"><inheritdoc cref="Collect(ref AnalysisContext)" path="/param[@name='context']"/></param>
	/// <returns><inheritdoc cref="Collect(ref AnalysisContext)" path="/returns"/></returns>
	private Step? Collect_NonIttoryuMode(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;

		if (!EnableFullHouse)
		{
			goto CheckForHiddenSingle;
		}

		for (var house = 0; house < 27; house++)
		{
			var (count, resultCell, flag) = (0, -1, true);
			foreach (var cell in HousesMap[house])
			{
				if (grid.GetState(cell) == CellState.Empty)
				{
					resultCell = cell;
					if (++count > 1)
					{
						flag = false;
						break;
					}
				}
			}
			if (!flag || count == 0)
			{
				continue;
			}

			var digit = TrailingZeroCount(grid.GetCandidates(resultCell));
			var step = new FullHouseStep(
				[new(Assignment, resultCell, digit)],
				[[new HouseViewNode(WellKnownColorIdentifier.Normal, house)]],
				context.PredefinedOptions,
				house,
				resultCell,
				digit
			);
			if (context.OnlyFindOne)
			{
				return step;
			}

			context.Accumulator.Add(step);
		}

	CheckForHiddenSingle:
		if (HiddenSinglesInBlockFirst)
		{
			// If block first, we'll extract all blocks and iterate on them firstly.
			for (var house = 0; house < 9; house++)
			{
				for (var digit = 0; digit < 9; digit++)
				{
					if (CheckForHiddenSingleAndLastDigit(in grid, ref context, digit, house) is not { } step)
					{
						continue;
					}

					if (context.OnlyFindOne)
					{
						return step;
					}

					context.Accumulator.Add(step);
				}
			}

			// Then secondly rows and columns.
			for (var house = 9; house < 27; house++)
			{
				for (var digit = 0; digit < 9; digit++)
				{
					if (CheckForHiddenSingleAndLastDigit(in grid, ref context, digit, house) is not { } step)
					{
						continue;
					}

					if (context.OnlyFindOne)
					{
						return step;
					}

					context.Accumulator.Add(step);
				}
			}
		}
		else
		{
			// We'll directly iterate on each house.
			// Theoretically, this iteration should be faster than above one, but in practice,
			// we may found hidden singles in block much more times than in row or column.
			for (var digit = 0; digit < 9; digit++)
			{
				for (var house = 0; house < 27; house++)
				{
					if (CheckForHiddenSingleAndLastDigit(in grid, ref context, digit, house) is not { } step)
					{
						continue;
					}

					if (context.OnlyFindOne)
					{
						return step;
					}

					context.Accumulator.Add(step);
				}
			}
		}

		for (var cell = 0; cell < 81; cell++)
		{
			if (grid.GetState(cell) != CellState.Empty)
			{
				continue;
			}

			var mask = grid.GetCandidates(cell);
			if (!IsPow2(mask))
			{
				continue;
			}

			var digit = TrailingZeroCount(mask);
			var step = new NakedSingleStep(
				[new(Assignment, cell, digit)],
				[[.. GetNakedSingleExcluders(in grid, cell, digit)]],
				context.PredefinedOptions,
				cell,
				digit
			);
			if (context.OnlyFindOne)
			{
				return step;
			}

			context.Accumulator.Add(step);
		}

		return null;
	}

	/// <summary>
	/// Checks for existence of hidden single and last digit conclusion in the specified house.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="digit">The digit used.</param>
	/// <param name="house">The house used.</param>
	/// <returns>Not <see langword="null"/> if conclusion can be found.</returns>
	/// <remarks>
	/// <para><include file="../../global-doc-comments.xml" path="/g/developer-notes"/></para>
	/// <para>
	/// The main idea of hidden single is to search for a digit can only appear once in a house,
	/// so we should check all possibilities in a house to found whether the house exists a digit
	/// that only appears once indeed.
	/// </para>
	/// </remarks>
	private HiddenSingleStep? CheckForHiddenSingleAndLastDigit(
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Digit digit,
		House house
	)
	{
		var (count, resultCell, flag) = (0, -1, true);
		foreach (var cell in HousesMap[house])
		{
			if (grid.Exists(cell, digit) is true)
			{
				resultCell = cell;
				if (++count > 1)
				{
					flag = false;
					break;
				}
			}
		}
		if (!flag || count == 0)
		{
			// The digit has been filled into the house, or the digit appears more than once,
			// it will be invalid for a hidden single. Just skip it.
			return null;
		}

		// The digit is a hidden single.
		// Now collect information (especially for rendering & text) from the current found step.
		var (enableAndIsLastDigit, cellOffsets) = (false, new List<CellViewNode>());
		if (EnableLastDigit)
		{
			// Sum up the number of appearing in the grid of 'digit'.
			var digitCount = 0;
			for (var i = 0; i < 81; i++)
			{
				if (grid.GetDigit(i) == digit)
				{
					digitCount++;
					cellOffsets.Add(new(WellKnownColorIdentifier.Normal, i) { RenderingMode = RenderingMode.BothDirectAndPencilmark });
				}
			}

			enableAndIsLastDigit = digitCount == 8;
		}

		return (enableAndIsLastDigit, house) switch
		{
			(true, >= 9) => null,
			_ => new(
				[new(Assignment, resultCell, digit)],
				[
					[
						.. enableAndIsLastDigit ? cellOffsets : [],
						.. enableAndIsLastDigit ? [] : GetHiddenSingleExcluders(in grid, digit, house, resultCell),
						.. enableAndIsLastDigit ? [] : (ViewNode[])[new HouseViewNode(WellKnownColorIdentifier.Normal, house)]
					]
				],
				context.PredefinedOptions,
				resultCell,
				digit,
				house,
				enableAndIsLastDigit
			)
		};
	}

	/// <summary>
	/// Try to create a list of <see cref="CellViewNode"/>s indicating the crosshatching base cells.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="digit">The digit.</param>
	/// <param name="house">The house.</param>
	/// <param name="cell">The cell.</param>
	/// <returns>A list of <see cref="CellViewNode"/> instances.</returns>
	private CellViewNode[] GetHiddenSingleExcluders(scoped ref readonly Grid grid, Digit digit, House house, Cell cell)
		=> Crosshatching.GetCrosshatchingInfo(in grid, digit, house, in CellsMap[cell]) switch
		{
			var (combination, emptyCellsShouldBeCovered, emptyCellsNotNeedToBeCovered) => [
				..
				from c in combination
				select new CellViewNode(WellKnownColorIdentifier.Normal, c) { RenderingMode = RenderingMode.DirectModeOnly },
				..
				from c in emptyCellsShouldBeCovered
				let p = emptyCellsNotNeedToBeCovered.Contains(c) ? WellKnownColorIdentifier.Auxiliary2 : WellKnownColorIdentifier.Auxiliary1
				select new CellViewNode(p, c) { RenderingMode = RenderingMode.DirectModeOnly }
			],
			_ => []
		};

	/// <summary>
	/// Get all <see cref="CellViewNode"/>s that represents as excluders.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>A list of <see cref="CellViewNode"/> instances.</returns>
	private CellViewNode[] GetNakedSingleExcluders(scoped ref readonly Grid grid, Cell cell, Digit digit)
	{
		var (result, i) = (new CellViewNode[8], 0);
		foreach (var otherDigit in (Mask)(Grid.MaxCandidatesMask & ~(1 << digit)))
		{
			foreach (var otherCell in Peers[cell])
			{
				if (grid.GetDigit(otherCell) == otherDigit)
				{
					result[i++] = new(WellKnownColorIdentifier.Normal, otherCell) { RenderingMode = RenderingMode.DirectModeOnly };
					break;
				}
			}
		}

		return result;
	}
}
