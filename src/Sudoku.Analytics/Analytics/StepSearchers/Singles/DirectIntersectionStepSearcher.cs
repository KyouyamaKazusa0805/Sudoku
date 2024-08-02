namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Direct Intersection</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Direct Pointing family:
/// <list type="bullet">
/// <item>Pointing Full House</item>
/// <item>Pointing Crosshatching in Block</item>
/// <item>Pointing Crosshatching in Row</item>
/// <item>Pointing Crosshatching in Column</item>
/// <item>Pointing Naked Single</item>
/// </list>
/// </item>
/// <item>
/// Direct Claiming family:
/// <list type="bullet">
/// <item>Claiming Full House</item>
/// <item>Claiming Crosshatching in Block</item>
/// <item>Claiming Crosshatching in Row</item>
/// <item>Claiming Crosshatching in Column</item>
/// <item>Claiming Naked Single</item>
/// </list>
/// </item>
/// </list>
/// </summary>
[StepSearcher(
	"StepSearcherName_DirectIntersectionStepSearcher",
	Technique.PointingFullHouse, Technique.PointingCrosshatchingBlock, Technique.PointingCrosshatchingRow,
	Technique.PointingCrosshatchingColumn, Technique.PointingNakedSingle,
	Technique.ClaimingFullHouse, Technique.ClaimingCrosshatchingBlock, Technique.ClaimingCrosshatchingRow,
	Technique.ClaimingCrosshatchingColumn, Technique.ClaimingNakedSingle,
	IsCachingSafe = true,
	IsAvailabilityReadOnly = true,
	IsOrderingFixed = true,
	RuntimeFlags = StepSearcherRuntimeFlags.DirectTechniquesOnly)]
public sealed partial class DirectIntersectionStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates whether the step searcher allows for searching for direct pointing.
	/// </summary>
	[SettingItemName(SettingItemNames.AllowDirectPointing)]
	public bool AllowDirectPointing { get; set; }

	/// <summary>
	/// Indicates whether the step searcher allows for searching for direct claiming.
	/// </summary>
	[SettingItemName(SettingItemNames.AllowDirectClaiming)]
	public bool AllowDirectClaiming { get; set; }


	/// <inheritdoc/>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="/g/developer-notes" />
	/// Please check documentation comments for <see cref="LockedCandidatesStepSearcher.Collect(ref AnalysisContext)"/>
	/// to learn more information about this technique.
	/// </remarks>
	protected internal override Step? Collect(ref AnalysisContext context)
	{
		ref readonly var grid = ref context.Grid;
		var emptyCells = grid.EmptyCells;
		var candidatesMap = grid.CandidatesMap;
		foreach (var ((bs, cs), (a, b, c, _)) in Miniline.Map)
		{
			if (!IntersectionModule.IsLockedCandidates(in grid, in a, in b, in c, in emptyCells, out var m))
			{
				continue;
			}

			foreach (var digit in m)
			{
				ref readonly var map = ref candidatesMap[digit];

				// Check whether the digit contains any eliminations.
				var (housesMask, elimMap) = a & map ? ((Mask)(cs << 8 | bs), a & map) : ((Mask)(bs << 8 | cs), b & map);
				if (!elimMap)
				{
					continue;
				}

				var (realBaseSet, realCoverSet, intersection) = (housesMask >> 8 & 127, housesMask & 127, c & map);
				if (intersection.Count < 2)
				{
					continue;
				}

				if (!AllowDirectPointing && realBaseSet < 9 || !AllowDirectClaiming && realBaseSet >= 9)
				{
					continue;
				}

				// Different with normal locked candidates searcher, this searcher is used as direct views.
				// We should check any possible assignments after such eliminations applied -
				// such assignments are the real conclusions of this technique.
				if (CheckFullHouse(
					ref context, in grid, realBaseSet, realCoverSet, in intersection,
					in emptyCells, in elimMap, digit) is { } fullHouse)
				{
					return fullHouse;
				}
				if (CheckHiddenSingle(
					ref context, in grid, realBaseSet, realCoverSet, in intersection,
					in elimMap, candidatesMap, in emptyCells, digit) is { } hiddenSingle)
				{
					return hiddenSingle;
				}
				if (CheckNakedSingle(
					ref context, in grid, realBaseSet, realCoverSet, in intersection,
					in elimMap, in emptyCells, digit) is { } nakedSingle)
				{
					return nakedSingle;
				}
			}
		}

		return null;
	}

	/// <summary>
	/// Check full house.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="baseSet">The base set.</param>
	/// <param name="coverSet">The cover set.</param>
	/// <param name="intersection">The intersection cells.</param>
	/// <param name="emptyCells">Indicates the empty cells of grid.</param>
	/// <param name="elimMap">The eliminated cells of digit <paramref name="digit"/>.</param>
	/// <param name="digit">The digit that can be eliminated in locked candidates.</param>
	/// <returns>The result step found.</returns>
	private DirectIntersectionStep? CheckFullHouse(
		ref AnalysisContext context,
		ref readonly Grid grid,
		House baseSet,
		House coverSet,
		ref readonly CellMap intersection,
		ref readonly CellMap emptyCells,
		ref readonly CellMap elimMap,
		Digit digit
	)
	{
		foreach (var house in elimMap.Houses)
		{
			var emptyCellsInHouse = HousesMap[house] & emptyCells;
			if (emptyCellsInHouse.Count != 2)
			{
				continue;
			}

			var lastDigitMask = (Mask)(grid[in emptyCellsInHouse] & ~(1 << digit));
			if (!Mask.IsPow2(lastDigitMask))
			{
				continue;
			}

			var lastDigit = Mask.Log2(lastDigitMask);
			var lastCell = (emptyCellsInHouse & elimMap)[0];
			var step = new DirectIntersectionStep(
				[new(Assignment, lastCell, lastDigit)],
				[
					[
						.. from cell in intersection select new CandidateViewNode(ColorIdentifier.Normal, cell * 9 + digit),
						.. Excluder.GetLockedCandidatesExcluders(in grid, digit, baseSet, in intersection),
						new CandidateViewNode(ColorIdentifier.Elimination, lastCell * 9 + digit),
						new HouseViewNode(ColorIdentifier.Normal, baseSet),
						new HouseViewNode(ColorIdentifier.Auxiliary1, coverSet),
						new HouseViewNode(ColorIdentifier.Auxiliary3, house)
					]
				],
				context.Options,
				lastCell,
				lastDigit,
				HousesMap[baseSet] & HousesMap[coverSet] & emptyCells,
				baseSet,
				[lastCell],
				digit,
				house switch
				{
					< 9 => SingleSubtype.FullHouseBlock,
					< 18 => SingleSubtype.FullHouseRow,
					_ => SingleSubtype.FullHouseColumn
				},
				Technique.FullHouse,
				baseSet < 9
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
	/// Check hidden single.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="baseSet">The base set.</param>
	/// <param name="coverSet">The cover set.</param>
	/// <param name="intersection">The intersection cells.</param>
	/// <param name="elimMap">The eliminated cells of digit <paramref name="digit"/>.</param>
	/// <param name="candidatesMap">Indicates the candidates map.</param>
	/// <param name="emptyCells">Indicates the empty cells.</param>
	/// <param name="digit">The digit that can be eliminated in locked candidates.</param>
	/// <returns>The result step found.</returns>
	private DirectIntersectionStep? CheckHiddenSingle(
		ref AnalysisContext context,
		ref readonly Grid grid,
		House baseSet,
		House coverSet,
		ref readonly CellMap intersection,
		ref readonly CellMap elimMap,
		ReadOnlySpan<CellMap> candidatesMap,
		ref readonly CellMap emptyCells,
		Digit digit
	)
	{
		foreach (var house in elimMap.Houses)
		{
			var emptyCellsInHouse = HousesMap[house] & candidatesMap[digit] & ~elimMap;
			if (emptyCellsInHouse is not [var lastCell])
			{
				continue;
			}

			var step = new DirectIntersectionStep(
				[new(Assignment, lastCell, digit)],
				[
					[
						.. Excluder.GetHiddenSingleExcluders(in grid, digit, house, lastCell, out var chosenCells, out _),
						.. from cell in intersection select new CandidateViewNode(ColorIdentifier.Normal, cell * 9 + digit),
						.. Excluder.GetLockedCandidatesExcluders(in grid, digit, baseSet, in intersection),
						..
						from cell in HousesMap[house] & elimMap
						select new CandidateViewNode(ColorIdentifier.Elimination, cell * 9 + digit),
						new DiamondViewNode(ColorIdentifier.Auxiliary3, lastCell),
						new CandidateViewNode(ColorIdentifier.Elimination, lastCell * 9 + digit),
						new HouseViewNode(ColorIdentifier.Normal, baseSet),
						new HouseViewNode(ColorIdentifier.Auxiliary1, coverSet),
						new HouseViewNode(ColorIdentifier.Auxiliary3, house)
					]
				],
				context.Options,
				lastCell,
				digit,
				HousesMap[baseSet] & HousesMap[coverSet] & emptyCells,
				baseSet,
				(HousesMap[house] & candidatesMap[digit]) - lastCell,
				digit,
				SingleModule.GetHiddenSingleSubtype(in grid, lastCell, house, in chosenCells),
				house switch
				{
					< 9 => Technique.CrosshatchingBlock,
					< 18 => Technique.CrosshatchingRow,
					_ => Technique.CrosshatchingColumn
				},
				baseSet < 9
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
	/// Check naked single.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="baseSet">The base set.</param>
	/// <param name="coverSet">The cover set.</param>
	/// <param name="intersection">The intersection cells.</param>
	/// <param name="elimMap">The eliminated cells of digit <paramref name="digit"/>.</param>
	/// <param name="digit">The digit that can be eliminated in locked candidates.</param>
	/// <param name="emptyCells">Indicates the empty cells.</param>
	/// <returns>The result step found.</returns>
	private DirectIntersectionStep? CheckNakedSingle(
		ref AnalysisContext context,
		ref readonly Grid grid,
		House baseSet,
		House coverSet,
		ref readonly CellMap intersection,
		ref readonly CellMap elimMap,
		ref readonly CellMap emptyCells,
		Digit digit
	)
	{
		foreach (var lastCell in elimMap)
		{
			if (grid.GetCandidates(lastCell) is var digitsMask && Mask.PopCount(digitsMask) != 2)
			{
				continue;
			}

			var lastDigit = Mask.TrailingZeroCount((Mask)(digitsMask & ~(1 << digit)));
			var step = new DirectIntersectionStep(
				[new(Assignment, lastCell, lastDigit)],
				[
					[
						.. from cell in intersection select new CandidateViewNode(ColorIdentifier.Normal, cell * 9 + digit),
						.. Excluder.GetNakedSingleExcluders(in grid, lastCell, lastDigit, out _),
						.. Excluder.GetLockedCandidatesExcluders(in grid, digit, baseSet, in intersection),
						new DiamondViewNode(ColorIdentifier.Auxiliary3, lastCell),
						new CandidateViewNode(ColorIdentifier.Elimination, lastCell * 9 + digit),
						new HouseViewNode(ColorIdentifier.Normal, baseSet),
						new HouseViewNode(ColorIdentifier.Auxiliary1, coverSet)
					]
				],
				context.Options,
				lastCell,
				lastDigit,
				HousesMap[baseSet] & HousesMap[coverSet] & emptyCells,
				baseSet,
				in lastCell.AsCellMap(),
				digit,
				SingleModule.GetNakedSingleSubtype(in grid, lastCell),
				Technique.NakedSingle,
				baseSet < 9
			);
			if (context.OnlyFindOne)
			{
				return step;
			}

			context.Accumulator.Add(step);
		}

		return null;
	}
}
