#undef ALLOW_ADVANCED_CHAINING

namespace Sudoku.Solving.Logical.StepSearchers;

[StepSearcher]
[SeparatedStepSearcher(0)]
[SeparatedStepSearcher(1, nameof(AllowNishio), true, nameof(AllowDynamic), true)]
[SeparatedStepSearcher(2, nameof(AllowMultiple), true)]
[SeparatedStepSearcher(3, nameof(AllowMultiple), true, nameof(AllowDynamic), true)]
#if false
[SeparatedStepSearcher(4, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(DynamicNestingLevel), 1)]
[SeparatedStepSearcher(5, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(DynamicNestingLevel), 2)]
[SeparatedStepSearcher(6, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(DynamicNestingLevel), 3)]
[SeparatedStepSearcher(7, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(DynamicNestingLevel), 4)]
[SeparatedStepSearcher(8, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(DynamicNestingLevel), 5)]
#endif
internal sealed partial class ChainingStepSearcher : IChainingStepSearcher
{
	/// <summary>
	/// <para>Indicates the temporary saved grid.</para>
	/// <para><i>
	/// This field will be used when the step searcher uses advanced logic (dynamic forcing chains or dynamic forcing chains (+))
	/// to search for chains.
	/// </i></para>
	/// </summary>
	private Grid _savedGrid;


	/// <summary>
	/// Indicates whether the step searcher allows nishio forcing chains, which is equivalent to a dynamic forcing chains
	/// that only uses a single digit. It is a brute-force view of a fish.
	/// </summary>
	public bool AllowNishio { get; init; }

	/// <summary>
	/// Indicates whether the step searcher allows multiple forcing chains:
	/// <list type="bullet">
	/// <item>
	/// For non-dynamic forcing chains:
	/// <list type="bullet">
	/// <item>Cell forcing chains</item>
	/// <item>Region (House) forcing chains</item>
	/// </list>
	/// </item>
	/// <item>
	/// For dynamic forcing chains:
	/// <list type="bullet">
	/// <item>Dynamic cell forcing chains</item>
	/// <item>Dynamic region (house) forcing chains</item>
	/// </list>
	/// </item>
	/// </list>
	/// </summary>
	public bool AllowMultiple { get; init; }

	/// <summary>
	/// Indicates whether the step searcher allows dynamic forcing chains:
	/// <list type="bullet">
	/// <item>Dynamic contradiction forcing chains</item>
	/// <item>Dynamic double forcing chains</item>
	/// </list>
	/// </summary>
	/// <remarks>
	/// If step searcher enables for dynamic forcing chains, forcing chains will contain branches,
	/// or even branches over branches (recursively). It will be very useful on complex inferences.
	/// </remarks>
	public bool AllowDynamic { get; init; }

	/// <summary>
	/// Indicates the level of dynamic recursion. The value can be 0, 1, 2, 3, 4 and 5.
	/// </summary>
	/// <remarks>
	/// All possible values corresponds to their own cases respectively:
	/// <list type="table">
	/// <listheader>
	/// <term>Value</term>
	/// <description>Supported nesting rule</description>
	/// </listheader>
	/// <item>
	/// <term>0</term>
	/// <description>Non-dynamic forcing chains</description>
	/// </item>
	/// <item>
	/// <term>1</term>
	/// <description>Dynamic forcing chains (+ Structural techniques, e.g. <see cref="ILockedCandidatesStepSearcher"/>)</description>
	/// </item>
	/// <item>
	/// <term>2</term>
	/// <description>Dynamic forcing chains (+ AIC)</description>
	/// </item>
	/// <item>
	/// <term>3</term>
	/// <description>Dynamic forcing chains (+ Multiple forcing chains)</description>
	/// </item>
	/// <item>
	/// <term>4</term>
	/// <description>Dynamic forcing chains (+ Dynamic forcing chains)</description>
	/// </item>
	/// <item>
	/// <term>5</term>
	/// <description>Dynamic forcing chains (+ Dynamic forcing chains (+))</description>
	/// </item>
	/// </list>
	/// </remarks>
	/// <seealso cref="ILockedCandidatesStepSearcher"/>
	public int DynamicNestingLevel { get; init; }


	/// <inheritdoc/>
	public IStep? GetAll(scoped in LogicalAnalysisContext context)
	{
		// TODO: Implement an implications cache.

		scoped ref readonly var grid = ref context.Grid;
		var result = AllowMultiple || AllowDynamic ? GetMultipleChains(grid) : getNonMultipleChains(grid);
		if (result.Count == 0)
		{
			return null;
		}

		result.Sort(ChainingStep.Compare);

		if (context.OnlyFindOne)
		{
			return result[0];
		}

		context.Accumulator.AddRange(result);
		return null;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		List<ChainingStep> getNonMultipleChains(scoped in Grid grid)
		{
			var result = GetNonMultipleChains(grid, true, false);
			result.AddRange(GetNonMultipleChains(grid, false, true));
			result.AddRange(GetNonMultipleChains(grid, true, true));

			return result;
		}
	}

	/// <summary>
	/// Checks whether hte specified <paramref name="child"/> is the real child node of <paramref name="parent"/>.
	/// </summary>
	/// <param name="child">The child node to be checked.</param>
	/// <param name="parent">The parent node to be checked.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	internal bool IsParent(Potential child, Potential parent)
	{
		var pTest = child;
		while (pTest.Parents is [var first, ..])
		{
			pTest = first;

			if (pTest == parent)
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Try to search for all AICs and continuous nice loops.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="isX">Indicates whether the chain allows X element (strong links in a house for a single digit).</param>
	/// <param name="isY">Indicates whether the chain allows Y element (strong links in a cell).</param>
	/// <returns>All possible found <see cref="ChainingStep"/>s.</returns>
	private List<ChainingStep> GetNonMultipleChains(scoped in Grid grid, bool isX, bool isY)
	{
		var result = new List<ChainingStep>();

		foreach (byte cell in EmptyCells)
		{
			for (byte digit = 0; digit < 9; digit++)
			{
				if (CandidatesMap[digit].Contains(cell))
				{
					var pOn = new Potential(cell, digit, true);
					DoUnaryChaining(grid, pOn, result, isX, isY);
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Search for hints on the given grid.
	/// </summary>
	/// <param name="grid">The grid on which to search for hints.</param>
	/// <returns>The hints found.</returns>
	private List<ChainingStep> GetMultipleChains(scoped in Grid grid)
	{
		var tempGrid = grid;
		var result = new List<ChainingStep>();

		// Iterate on all empty cells.
		foreach (byte cell in EmptyCells)
		{
			// The cell is empty.
			var cardinality = PopCount((uint)tempGrid.GetCandidates(cell));
			if (cardinality > 2 || cardinality > 1 && AllowDynamic)
			{
				// Prepare storage and accumulator for "Cell Reduction".
				var digitToOn = new ChainBranch();
				var digitToOff = new ChainBranch();
				var cellToOn = default(PotentialSet?);
				var cellToOff = default(PotentialSet?);

				// Iterate on all potential values that are not alone.
				for (byte digit = 0; digit < 9; digit++)
				{
					if (CandidatesMap[digit].Contains(cell))
					{
						// Do Binary chaining (same potential either on or off).
						var pOn = new Potential(cell, digit, true);
						var pOff = new Potential(cell, digit, false);
						var onToOn = new PotentialSet();
						var onToOff = new PotentialSet();
						var doDouble = cardinality >= 3 && !AllowNishio && AllowDynamic;
						var doContradiction = AllowDynamic || AllowNishio;
						DoBinaryChaining(ref tempGrid, pOn, pOff, result, onToOn, onToOff, doDouble, doContradiction);

						if (!AllowNishio)
						{
							// Do house chaining.
							DoHouseChaining(ref tempGrid, result, cell, digit, onToOn, onToOff);
						}

						// Collect results for cell chaining.
						digitToOn.Add(digit, onToOn);
						digitToOff.Add(digit, onToOff);
						if (cellToOn is null || cellToOff is null)
						{
							cellToOn = new(onToOn);
							cellToOff = new(onToOff);
						}
						else
						{
							cellToOn &= onToOn;
							cellToOff &= onToOff;
						}
					}
				}

				// Do cell reduction.
				if (!AllowNishio && (cardinality == 2 || AllowMultiple))
				{
					if (cellToOn is not null)
					{
						foreach (var p in cellToOn)
						{
							result.Add(CreateCellForcingStep(grid, cell, p, digitToOn));
						}
					}
					if (cellToOff is not null)
					{
						foreach (var p in cellToOff)
						{
							result.Add(CreateCellForcingStep(grid, cell, p, digitToOff));
						}
					}
				}
			}
		}

		return result;
	}

#if ALLOW_ADVANCED_CHAINING
	/// <summary>
	/// Get all non-trivial implications (involving fished, naked/hidden sets, etc).
	/// </summary>
	/// <returns>All found potentials off.</returns>
	private List<Potential> GetAdvancedPotentials(
		scoped in Grid grid,
		scoped in Grid source,
		PotentialSet offPotentials
	)
	{
		throw new NotImplementedException();
	}
#endif
}
