namespace Sudoku.Analytics.StepSearchers;

using unsafe SingleModuleSearcherFuncPtr = delegate*<SingleStepSearcher, ref AnalysisContext, ref readonly Grid, Step?>;

/// <summary>
/// Provides with a <b>Single</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Common techniques:
/// <list type="bullet">
/// <item>Full House (If the property <see cref="EnableFullHouse"/> is <see langword="true"/>)</item>
/// <item>Last Digit (If the property <see cref="EnableLastDigit"/> is <see langword="true"/>)</item>
/// <item>Naked Single</item>
/// </list>
/// </item>
/// <item>
/// Direct techniques:
/// <list type="bullet">
/// <item>Crosshatching</item>
/// </list>
/// </item>
/// <item>
/// Indirect techniques:
/// <list type="bullet">
/// <item>Hidden Single</item>
/// </list>
/// </item>
/// </list>
/// </summary>
[StepSearcher(
	"StepSearcherName_SingleStepSearcher",
	Technique.CrosshatchingBlock, Technique.CrosshatchingRow, Technique.CrosshatchingColumn, Technique.LastDigit,
	Technique.FullHouse, Technique.HiddenSingleBlock, Technique.HiddenSingleRow, Technique.HiddenSingleColumn, Technique.NakedSingle,
	IsCachingSafe = true,
	IsOrderingFixed = true,
	IsAvailabilityReadOnly = true)]
public sealed partial class SingleStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates whether the solver enables the technique full house.
	/// </summary>
	[SettingItemName(SettingItemNames.EnableFullHouse)]
	public bool EnableFullHouse { get; set; }

	/// <summary>
	/// Indicates whether the solver enables the technique last digit.
	/// </summary>
	[SettingItemName(SettingItemNames.EnableLastDigit)]
	public bool EnableLastDigit { get; set; }

	/// <summary>
	/// Indicates whether the solver checks for hidden single in block firstly.
	/// </summary>
	[SettingItemName(SettingItemNames.HiddenSinglesInBlockFirst)]
	public bool HiddenSinglesInBlockFirst { get; set; }

	/// <summary>
	/// Indicates whether the solver uses ittoryu mode to solve a puzzle.
	/// </summary>
	[SettingItemName(SettingItemNames.AnalyzerUseIttoryuMode)]
	public bool UseIttoryuMode { get; set; }


	/// <inheritdoc/>
	protected internal override Step? Collect(ref AnalysisContext context)
		=> UseIttoryuMode ? Collect_IttoryuMode(ref context) : Collect_NonIttoryuMode(ref context);

	/// <summary>
	/// Checks for single steps using ittoryu mode.
	/// </summary>
	/// <param name="context"><inheritdoc cref="Collect(ref AnalysisContext)" path="/param[@name='context']"/></param>
	/// <returns><inheritdoc cref="Collect(ref AnalysisContext)" path="/returns"/></returns>
	private Step? Collect_IttoryuMode(ref AnalysisContext context)
	{
		ref readonly var grid = ref context.Grid;
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

				var emptyCellsCountFromAllPeerHouses = 0;
				foreach (var houseType in HouseTypes)
				{
					var peerHouse = resultCell.ToHouseIndex(houseType);
					foreach (var cell in HousesCells[peerHouse])
					{
						if (grid.GetState(cell) == CellState.Empty)
						{
							emptyCellsCountFromAllPeerHouses++;
						}
					}
				}

				var step = new FullHouseStep(
					[new(Assignment, resultCell, digit)],
					[[new HouseViewNode(ColorIdentifier.Normal, house)]],
					context.Options,
					house,
					resultCell,
					digit,
					SingleModule.GetLasting(in grid, resultCell, house)
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
				if (CheckForHiddenSingleAndLastDigit(this, in grid, ref context, digit, house) is not { } step)
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

				if (grid.GetCandidates(cell) is var mask && !IsPow2(mask))
				{
					continue;
				}

				if (TrailingZeroCount(mask) != digit)
				{
					continue;
				}

				if (SingleModule.GetNakedSingleSubtype(in grid, cell) is var subtype && subtype.IsUnnecessary() && !context.IsSukaku)
				{
					continue;
				}

				var step = new NakedSingleStep(
					[new(Assignment, cell, digit)],
					[[.. SingleModule.GetNakedSingleExcluders(in grid, cell, digit, out _)]],
					context.Options,
					cell,
					digit,
					subtype,
					SingleModule.GetLastingAllHouses(in grid, cell, out _)
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
	private unsafe Step? Collect_NonIttoryuMode(ref AnalysisContext context)
	{
		ref readonly var grid = ref context.Grid;
		var isFullyMarkedMode = !context.Options.DistinctDirectMode || !context.Options.IsDirectMode;

		// Please note that, by default we should start with hidden singles. However, if a user has set the option
		// that a step searcher should distinct with direct mode and in-direct mode (i.e. all candidates are displayed),
		// we should start with a naked single because they are "direct" in such mode.
		var p = stackalloc SingleModuleSearcherFuncPtr[] { &CheckFullHouse, &CheckNakedSingle, &CheckHiddenSingle };
		var q = stackalloc SingleModuleSearcherFuncPtr[] { &CheckFullHouse, &CheckHiddenSingle, &CheckNakedSingle };
		var r = stackalloc SingleModuleSearcherFuncPtr[] { &CheckNakedSingle, &CheckHiddenSingle };
		var s = stackalloc SingleModuleSearcherFuncPtr[] { &CheckHiddenSingle, &CheckNakedSingle };
		var searchers = (EnableFullHouse, isFullyMarkedMode) switch { (true, true) => p, (true, _) => q, (_, true) => r, _ => s };
		for (var i = 0; i < (searchers == p || searchers == q ? 3 : 2); i++)
		{
			if (searchers[i](this, ref context, in grid) is { } step)
			{
				return step;
			}
		}

		return null;
	}


	/// <summary>
	/// Check for full houses.
	/// </summary>
	private static FullHouseStep? CheckFullHouse(SingleStepSearcher @this, ref AnalysisContext context, ref readonly Grid grid)
	{
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
				[[new HouseViewNode(ColorIdentifier.Normal, house)]],
				context.Options,
				house,
				resultCell,
				digit,
				SingleModule.GetLasting(in grid, resultCell, house)
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
	/// Check for hidden singles.
	/// </summary>
	private static HiddenSingleStep? CheckHiddenSingle(SingleStepSearcher @this, ref AnalysisContext context, ref readonly Grid grid)
	{
		if (@this.HiddenSinglesInBlockFirst)
		{
			// If block first, we'll extract all blocks and iterate on them firstly.
			for (var house = 0; house < 9; house++)
			{
				for (var digit = 0; digit < 9; digit++)
				{
					if (CheckForHiddenSingleAndLastDigit(@this, in grid, ref context, digit, house) is not { } step)
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
					if (CheckForHiddenSingleAndLastDigit(@this, in grid, ref context, digit, house) is not { } step)
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
					if (CheckForHiddenSingleAndLastDigit(@this, in grid, ref context, digit, house) is not { } step)
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

		return null;
	}

	/// <summary>
	/// Check for naked singles.
	/// </summary>
	private static NakedSingleStep? CheckNakedSingle(SingleStepSearcher @this, ref AnalysisContext context, ref readonly Grid grid)
	{
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
				[[.. SingleModule.GetNakedSingleExcluders(in grid, cell, digit, out _)]],
				context.Options,
				cell,
				digit,
				SingleModule.GetNakedSingleSubtype(in grid, cell),
				SingleModule.GetLastingAllHouses(in grid, cell, out _)
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
	/// <param name="this">The current instance.</param>
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
	private static HiddenSingleStep? CheckForHiddenSingleAndLastDigit(
		SingleStepSearcher @this,
		ref readonly Grid grid,
		ref AnalysisContext context,
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
		if (@this.EnableLastDigit)
		{
			// Sum up the number of appearing in the grid of 'digit'.
			var digitCount = 0;
			for (var cell = 0; cell < 81; cell++)
			{
				if (grid.GetDigit(cell) == digit)
				{
					digitCount++;
					cellOffsets.Add(new(ColorIdentifier.Normal, cell) { RenderingMode = BothDirectAndPencilmark });
				}
			}

			enableAndIsLastDigit = digitCount == 8;
		}

		return (enableAndIsLastDigit, house) switch
		{
			(true, >= 9) => null,
			(true, _) => new LastDigitStep(
				[new(Assignment, resultCell, digit)],
				[[.. cellOffsets]],
				context.Options,
				resultCell,
				digit,
				house,
				SingleModule.GetLastingAllHouses(in grid, resultCell, out _)
			),
			_ => SingleModule.GetHiddenSingleExcluders(in grid, digit, house, resultCell, out var chosenCells) switch
			{
				var cellOffsets2 => SingleModule.GetHiddenSingleSubtype(in grid, resultCell, house, in chosenCells) switch
				{
					var subtype when subtype.IsUnnecessary() && !context.IsSukaku => null,
					var subtype => new HiddenSingleStep(
						[new(Assignment, resultCell, digit)],
						[[.. cellOffsets2, new HouseViewNode(ColorIdentifier.Normal, house)]],
						context.Options,
						resultCell,
						digit,
						house,
						enableAndIsLastDigit,
						SingleModule.GetLasting(in grid, resultCell, house),
						subtype
					)
				}
			}
		};
	}
}
