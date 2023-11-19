using System.Diagnostics.CodeAnalysis;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Metadata;
using Sudoku.Analytics.Steps;
using Sudoku.Concepts;
using Sudoku.Concepts.ObjectModel;
using static Sudoku.Analytics.CachedFields;
using static Sudoku.Analytics.ConclusionType;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Chain</b> step searcher using same algorithm with <b>Chaining</b> used by a program called Sudoku Explainer.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Alternating Inference Chains (Cycles)</item>
/// <item>
/// Forcing Chains:
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
/// </item>
/// </list>
/// </summary>
/// <remarks>
/// The type is special: it uses source code from another project called Sudoku Explainer.
/// However unfortunately, I cannot find any sites available of the project.
/// One of the original website is <see href="https://diuf.unifr.ch/pai/people/juillera/Sudoku/Sudoku.html">this link</see> (A broken link).
/// </remarks>
[StepSearcher(
	Technique.DynamicCellForcingChains, Technique.DynamicRegionForcingChains,
	Technique.DynamicContradictionForcingChains, Technique.DynamicDoubleForcingChains)]
[SplitStepSearcher(0, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(DynamicNestingLevel), 1)]
[SplitStepSearcher(1, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(DynamicNestingLevel), 2)]
[SplitStepSearcher(2, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(DynamicNestingLevel), 3)]
[SplitStepSearcher(3, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(DynamicNestingLevel), 4)]
[SplitStepSearcher(4, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(DynamicNestingLevel), 5)]
public sealed partial class AdvancedMultipleChainingStepSearcher : MultipleChainingStepSearcher
{
	/// <summary>
	/// Indicates the advanced step searchers.
	/// </summary>
	private List<(int Priority, StepSearcher[] StepSearchersInThisLevel)>? _otherStepSearchers;


	/// <summary>
	/// Indicates the level of dynamic recursion. The value can be 1, 2, 3, 4 and 5.
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
	/// <description>Dynamic forcing chains (+ Structural techniques, e.g. <see cref="LockedCandidatesStepSearcher"/>)</description>
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
	/// <seealso cref="LockedCandidatesStepSearcher"/>
	public int DynamicNestingLevel { get; init; }


	/// <inheritdoc/>
	protected override void OnAdvanced(NodeList pendingOn, NodeList pendingOff, NodeSet toOff, scoped ref readonly Grid grid, scoped ref readonly Grid original)
	{
		if (pendingOn.Count == 0 && pendingOff.Count == 0 && DynamicNestingLevel > 0)
		{
			foreach (var pOff in GetAdvancedPotentials(in grid, in original, toOff))
			{
				if (toOff.Add(pOff))
				{
					// Not processed yet.
					pendingOff.AddLast(pOff);
				}
			}
		}
	}

	/// <summary>
	/// Get all non-trivial implications (involving fished, naked/hidden sets, etc).
	/// </summary>
	/// <param name="grid">Indicates the current grid state.</param>
	/// <param name="original">Indicates the original grid state.</param>
	/// <param name="offPotentials">
	/// <inheritdoc
	///     cref="ChainingStepSearcher.OnAdvanced(NodeList, NodeList, NodeSet, ref readonly Grid, ref readonly Grid)"
	///     path="/param[@name='toOff']"/>
	/// </param>
	/// <returns>Found <see cref="ChainNode"/> instances.</returns>
	[MemberNotNull(nameof(_otherStepSearchers))]
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
	[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
	private NodeList GetAdvancedPotentials(scoped ref readonly Grid grid, scoped ref readonly Grid original, NodeSet offPotentials)
	{
		_otherStepSearchers ??= [
			(1, [new LockedCandidatesStepSearcher(), new LockedSubsetStepSearcher(), new NormalFishStepSearcher(), new NormalSubsetStepSearcher()]),
			(2, [new NonMultipleChainingStepSearcher()]),
			(3, [new MultipleChainingStepSearcher { AllowMultiple = true }]),
			(4, [new MultipleChainingStepSearcher { AllowDynamic = true, AllowMultiple = true }]),
			(5, [new AdvancedMultipleChainingStepSearcher { DynamicNestingLevel = DynamicNestingLevel - 3 }])
		];

		var result = new NodeList();
		return result;
	}

	/// <summary>
	/// Try to create a binary forcing chain hint on "on" state.
	/// </summary>
	[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
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
		var result = new BinaryForcingChainsStep(conclusion, context.PredefinedOptions, src, dstOn, dstOff, isAbsurd, AllowNishio, DynamicNestingLevel);
		return new(result, result.CreateViews(in grid));
	}

	/// <summary>
	/// Try to create a binary forcing chain hint on "off" state.
	/// </summary>
	[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
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
		var result = new BinaryForcingChainsStep(conclusion, context.PredefinedOptions, src, dstOn, dstOff, isAbsurd, AllowNishio, DynamicNestingLevel);
		return new(result, result.CreateViews(in grid));
	}

	/// <summary>
	/// Try to create a cell forcing chain hint.
	/// </summary>
	[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
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

		var result = new CellForcingChainsStep(conclusion, context.PredefinedOptions, srcCell, chains, AllowDynamic, DynamicNestingLevel);
		return new(result, result.CreateViews(in grid));
	}

	/// <summary>
	/// Try to create a region (house) forcing chain hint.
	/// </summary>
	[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
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

		var result = new RegionForcingChainsStep(conclusions, context.PredefinedOptions, houseIndex, digit, chains, AllowDynamic, DynamicNestingLevel);
		return new(result, result.CreateViews(in grid));
	}
}
