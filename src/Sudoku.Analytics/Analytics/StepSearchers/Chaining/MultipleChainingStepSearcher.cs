using System.Numerics;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Metadata;
using Sudoku.Analytics.Steps;
using Sudoku.Analytics.StepSearcherModules;
using Sudoku.Concepts;
using Sudoku.Concepts.ObjectModel;
using static System.Numerics.BitOperations;
using static Sudoku.Analytics.CachedFields;
using static Sudoku.Analytics.ConclusionType;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Chain</b> step searcher using same algorithm with <b>Chaining</b> used by a program called Sudoku Explainer.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Multiple Forcing Chains:
/// <list type="bullet">
/// <item>Cell Forcing Chains</item>
/// <item>Region (House) Forcing Chains</item>
/// </list>
/// </item>
/// <item>
/// Dynamic Forcing Chains:
/// <list type="bullet">
/// <item>Dynamic Cell Forcing Chains</item>
/// <item>Dynamic Region (House) Forcing Chains</item>
/// <item>Dynamic Contradiction Forcing Chains</item>
/// <item>Dynamic Double Forcing Chains</item>
/// </list>
/// </item>
/// </list>
/// </summary>
[StepSearcher(
	Technique.CellForcingChains, Technique.RegionForcingChains, Technique.NishioForcingChains,
	Technique.DynamicCellForcingChains, Technique.DynamicRegionForcingChains,
	Technique.DynamicContradictionForcingChains, Technique.DynamicDoubleForcingChains)]
[StepSearcherSourceGeneration(CanDeriveTypes = true)]
[SplitStepSearcher(0, nameof(AllowNishio), true, nameof(AllowDynamic), true)]
[SplitStepSearcher(1, nameof(AllowMultiple), true)]
[SplitStepSearcher(2, nameof(AllowMultiple), true, nameof(AllowDynamic), true)]
public partial class MultipleChainingStepSearcher : StepSearcher
{
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


	/// <inheritdoc/>
	protected internal sealed override Step? Collect(scoped ref AnalysisContext context)
	{
		// TODO: Implement an implications cache.

		scoped ref readonly var grid = ref context.Grid;
		var result = Collect(in grid, ref context);
		if (result.Count == 0)
		{
			return null;
		}

		result.Order();

		if (context.OnlyFindOne)
		{
			return result[0];
		}

		context.Accumulator.AddRange(result);
		return null;
	}

	/// <summary>
	/// Search for hints on the given grid.
	/// </summary>
	/// <param name="grid">The grid on which to search for hints.</param>
	/// <param name="context">The context.</param>
	/// <returns>The hints found.</returns>
	private List<ChainingStep> Collect(scoped ref readonly Grid grid, scoped ref AnalysisContext context)
	{
		var result = new List<ChainingStep>();

		// Iterate on all empty cells.
		foreach (byte cell in EmptyCells)
		{
			// The cell is empty.
			var mask = grid.GetCandidates(cell);
			var count = PopCount((uint)mask);
			if (count > 2 || count > 1 && AllowDynamic)
			{
				// Prepare storage and accumulator for "Cell Reduction".
				var digitToOn = new ChainBranch();
				var digitToOff = new ChainBranch();
				var cellToOn = default(NodeSet);
				var cellToOff = default(NodeSet);

				// Iterate on all potential values that are not alone.
				foreach (byte digit in mask)
				{
					// Do Binary chaining (same potential either on or off).
					var pOn = new ChainNode(cell, digit, true);
					var pOff = new ChainNode(cell, digit, false);
					var onToOn = new NodeSet();
					var onToOff = new NodeSet();
					var doDouble = count >= 3 && !AllowNishio && AllowDynamic;
					var doContradiction = AllowDynamic || AllowNishio;
					DoBinaryChaining(in grid, ref context, pOn, pOff, result, onToOn, onToOff, doDouble, doContradiction);

					if (!AllowNishio)
					{
						// Do house chaining.
						DoHouseChaining(in grid, ref context, result, cell, digit, onToOn, onToOff);
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

				// Do cell reduction.
				if (!AllowNishio && (count == 2 || AllowMultiple))
				{
					if (cellToOn is not null)
					{
						foreach (var p in cellToOn)
						{
							result.Add(CreateCellForcingStep(in grid, ref context, cell, p, digitToOn));
						}
					}
					if (cellToOff is not null)
					{
						foreach (var p in cellToOff)
						{
							result.Add(CreateCellForcingStep(in grid, ref context, cell, p, digitToOff));
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
	private List<ChainNode> GetAdvancedPotentials(
		scoped ref readonly Grid grid,
		scoped ref readonly Grid source,
		PotentialSet offPotentials
	)
	{
		throw new NotImplementedException();
	}
#endif

	/// <summary>
	/// <para>From the potential <c>p</c>, compute the consequences from both states.</para>
	/// <para>
	/// More precisely, <c>p</c> is first assumed to be correct ("on"), and then to be incorrect ("off");
	/// and the following sets are created:
	/// <list type="bullet">
	/// <item><c><paramref name="onToOn"/></c> the set of potentials that must be "on" when <c>p</c> is "on".</item>
	/// <item><c><paramref name="onToOff"/></c> the set of potentials that must be "off" when <c>p</c> is "on".</item>
	/// <item><c>offToOn</c> the set of potentials that must be "on" when <c>p</c> is "off".</item>
	/// <item><c>offToOff</c> the set of potentials that must be "off" when <c>p</c> is "off".</item>
	/// </list>
	/// Then the following rules are applied:
	/// <list type="bullet">
	/// <item>
	/// If a potential belongs to both <c><paramref name="onToOn"/></c> and <c><paramref name="onToOff"/></c>,
	/// the potential <c>p</c> cannot be "on" because it would imply a potential
	/// to be both "on" and "off", which is an absurd.
	/// </item>
	/// <item>
	/// If a potential belongs to both <c>offToOn</c> and <c>offToOff</c>,
	/// the potential <c>p</c> cannot be "off" because it would imply a potential
	/// to be both "on" and "off", which is an absurd.
	/// </item>
	/// <item>
	/// If a potential belongs to both <c><paramref name="onToOn"/></c> and <c>offToOn</c>,
	/// this potential must be "on", because it is implied to be "on" by the two possible
	/// states of <c>p</c>.
	/// </item>
	/// <item>
	/// If a potential belongs to both <c><paramref name="onToOff"/></c> and <c>offToOff</c>,
	/// this potential must be "off", because it is implied to be "off" by the two possible
	/// states of <c>p</c>.
	/// </item>
	/// </list>
	/// Note that if a potential belongs to all the four sets, the sudoku has no solution. This is not checked.
	/// </para>
	/// </summary>
	/// <param name="grid">
	/// <inheritdoc cref="NonMultipleChainingStepSearcher.Collect(ref readonly Grid, ref AnalysisContext, bool, bool)" path="/param[@name='grid']"/>
	/// </param>
	/// <param name="context">The context.</param>
	/// <param name="pOn"></param>
	/// <param name="pOff"></param>
	/// <param name="result">
	/// <inheritdoc
	///     cref="NonMultipleChainingStepSearcher.DoUnaryChaining(ref readonly Grid, ref AnalysisContext, ChainNode, List{ChainingStep}, bool, bool)"
	///     path="/param[@name='result']"/>
	/// </param>
	/// <param name="onToOn">An empty set, filled with potentials that get on if the given potential is on.</param>
	/// <param name="onToOff">An empty set, filled with potentials that get off if the given potential is on.</param>
	/// <param name="doReduction"></param>
	/// <param name="doContradiction"></param>
	private void DoBinaryChaining(
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		ChainNode pOn,
		ChainNode pOff,
		List<ChainingStep> result,
		NodeSet onToOn,
		NodeSet onToOff,
		bool doReduction,
		bool doContradiction
	)
	{
		var offToOn = new NodeSet();
		var offToOff = new NodeSet();

		// Circular Forcing Chains (hypothesis implying its negation) are already covered by Cell Forcing Chains,
		// and are therefore not checked for.

		// Test p = "on".
		onToOn.Add(pOn);
		var pair = ChainingModule.DoChaining(this, grid, onToOn, onToOff, AllowNishio, AllowDynamic);
		if (doContradiction && pair is var (absurdOn1, absurdOff1))
		{
			// p cannot hold its value, because else it would lead to a contradiction.
			result.Add(CreateChainingOffStep(in grid, ref context, absurdOn1, absurdOff1, pOn, pOn, true));
		}

		// Test p = "off".
		offToOff.Add(pOff);
		pair = ChainingModule.DoChaining(this, grid, offToOn, offToOff, AllowNishio, AllowDynamic);
		if (doContradiction && pair is var (absurdOn2, absurdOff2))
		{
			// p must hold its value, because else it would lead to a contradiction.
			result.Add(CreateChainingOnStep(in grid, ref context, absurdOn2, absurdOff2, pOff, pOff, true));
		}

		if (doReduction)
		{
			// Check potentials that must be on in both case.
			foreach (var pFromOn in onToOn)
			{
				if (offToOn.GetNullable(pFromOn) is { } pFromOff)
				{
					result.Add(CreateChainingOnStep(in grid, ref context, pFromOn, pFromOff, pOn, pFromOn, false));
				}
			}

			// Check potentials that must be off in both case.
			foreach (var pFromOn in onToOff)
			{
				if (offToOff.GetNullable(pFromOn) is { } pFromOff)
				{
					result.Add(CreateChainingOffStep(in grid, ref context, pFromOn, pFromOff, pOff, pFromOff, false));
				}
			}
		}
	}

	/// <summary>
	/// Search for region (house) forcing chains.
	/// </summary>
	/// <param name="grid">
	/// <inheritdoc cref="NonMultipleChainingStepSearcher.Collect(ref readonly Grid, ref AnalysisContext, bool, bool)" path="/param[@name='grid']"/>
	/// </param>
	/// <param name="context">The context.</param>
	/// <param name="result">
	/// <inheritdoc
	///     cref="NonMultipleChainingStepSearcher.DoUnaryChaining(ref readonly Grid, ref AnalysisContext, ChainNode, List{ChainingStep}, bool, bool)"
	///     path="/param[@name='result']"/>
	/// </param>
	/// <param name="cell"></param>
	/// <param name="digit"></param>
	/// <param name="onToOn">
	/// <inheritdoc
	///     cref="DoBinaryChaining(ref readonly Grid, ref AnalysisContext, ChainNode, ChainNode, List{ChainingStep}, NodeSet, NodeSet, bool, bool)"
	///     path="/param[@name='onToOn']"/>
	/// </param>
	/// <param name="onToOff">
	/// <inheritdoc
	///     cref="DoBinaryChaining(ref readonly Grid, ref AnalysisContext, ChainNode, ChainNode, List{ChainingStep}, NodeSet, NodeSet, bool, bool)"
	///     path="/param[@name='onToOff']"/>
	/// </param>
	private void DoHouseChaining(
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		List<ChainingStep> result,
		byte cell,
		byte digit,
		NodeSet onToOn,
		NodeSet onToOff
	)
	{
		foreach (var houseType in HouseTypes)
		{
			var houseIndex = cell.ToHouseIndex(houseType);
			var potentialPositions = HousesMap[houseIndex] & CandidatesMap[digit];
			if (potentialPositions.Count == 2 || AllowMultiple && potentialPositions.Count > 2)
			{
				// Do we meet region for the first time?
				if (potentialPositions[0] == cell)
				{
					var posToOn = new ChainBranch();
					var posToOff = new ChainBranch();
					var houseToOn = new NodeSet();
					var houseToOff = new NodeSet();

					// Iterate on potential positions within the house.
					foreach (byte otherCell in potentialPositions)
					{
						if (otherCell == cell)
						{
							posToOn.Add(otherCell, onToOn);
							posToOff.Add(otherCell, onToOff);
							houseToOn |= onToOn;
							houseToOff |= onToOff;
						}
						else
						{
							var otherToOn = new NodeSet { new(otherCell, digit, true) };
							var otherToOff = new NodeSet();

							ChainingModule.DoChaining(this, grid, otherToOn, otherToOff, AllowNishio, AllowDynamic);

							posToOn.Add(otherCell, otherToOn);
							posToOff.Add(otherCell, otherToOff);
							houseToOn &= otherToOn;
							houseToOff &= otherToOff;
						}
					}

					// Gather results.
					foreach (var p in houseToOn)
					{
						result.Add(CreateHouseForcingStep(in grid, ref context, houseIndex, digit, p, posToOn));
					}
					foreach (var p in houseToOff)
					{
						result.Add(CreateHouseForcingStep(in grid, ref context, houseIndex, digit, p, posToOff));
					}
				}
			}
		}
	}

	/// <summary>
	/// Try to create a binary forcing chain hint on "on" state.
	/// </summary>
	private BinaryForcingChainsStep CreateChainingOnStep(
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		ChainNode dstOn,
		ChainNode dstOff,
		ChainNode src,
		ChainNode target,
		bool isAbsurd
	)
	{
		var conclusion = (Conclusion[])[new(Assignment, target.Candidate)];
		var result = new BinaryForcingChainsStep(conclusion, context.PredefinedOptions, src, dstOn, dstOff, isAbsurd, AllowNishio);
		return new(result, result.CreateViews(in grid));
	}

	/// <summary>
	/// Try to create a binary forcing chain hint on "off" state.
	/// </summary>
	private BinaryForcingChainsStep CreateChainingOffStep(
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		ChainNode dstOn,
		ChainNode dstOff,
		ChainNode src,
		ChainNode target,
		bool isAbsurd
	)
	{
		var conclusion = (Conclusion[])[new(Elimination, target.Candidate)];
		var result = new BinaryForcingChainsStep(conclusion, context.PredefinedOptions, src, dstOn, dstOff, isAbsurd, AllowNishio);
		return new(result, result.CreateViews(in grid));
	}

	/// <summary>
	/// Try to create a cell forcing chain hint.
	/// </summary>
	private CellForcingChainsStep CreateCellForcingStep(
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		byte srcCell,
		ChainNode target,
		ChainBranch outcomes
	)
	{
		var (targetCell, targetDigit, targetIsOn) = target;
		var conclusion = (Conclusion[])[new(targetIsOn ? Assignment : Elimination, targetCell, targetDigit)];

		// Build chains.
		var chains = new MultipleForcingChains();
		for (byte tempDigit = 0; tempDigit < 9; tempDigit++)
		{
			if (CandidatesMap[tempDigit].Contains(srcCell))
			{
				// Get corresponding value with the matching parents.
				chains.Add(tempDigit, outcomes[tempDigit][target]);
			}
		}

		var result = new CellForcingChainsStep(conclusion, context.PredefinedOptions, srcCell, chains, AllowDynamic);
		return new(result, result.CreateViews(in grid));
	}

	/// <summary>
	/// Try to create a region (house) forcing chain hint.
	/// </summary>
	private RegionForcingChainsStep CreateHouseForcingStep(
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		House houseIndex,
		byte digit,
		ChainNode target,
		ChainBranch outcomes
	)
	{
		var (targetCell, targetDigit, targetIsOn) = target;
		var conclusions = (Conclusion[])[new(targetIsOn ? Assignment : Elimination, targetCell, targetDigit)];

		// Build chains.
		var chains = new MultipleForcingChains();
		foreach (byte tempCell in CandidatesMap[digit] & HousesMap[houseIndex])
		{
			// Get corresponding value with the matching parents.
			chains.Add(tempCell, outcomes[tempCell][target]);
		}

		var result = new RegionForcingChainsStep(conclusions, context.PredefinedOptions, houseIndex, digit, chains, AllowDynamic);
		return new(result, result.CreateViews(in grid));
	}
}
