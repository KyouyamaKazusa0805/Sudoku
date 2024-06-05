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
[StepSearcher(
	"StepSearcherName_AdvancedMultipleChainingStepSearcher",
	Technique.DynamicCellForcingChains, Technique.DynamicRegionForcingChains,
	Technique.DynamicContradictionForcingChains, Technique.DynamicDoubleForcingChains)]
[SplitStepSearcher(0, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(DynamicNestingLevel), 1)]
[SplitStepSearcher(1, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(DynamicNestingLevel), 2)]
[SplitStepSearcher(2, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(DynamicNestingLevel), 3)]
[SplitStepSearcher(3, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(DynamicNestingLevel), 4)]
[SplitStepSearcher(4, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(DynamicNestingLevel), 5)]
[Obsolete]
public sealed partial class AdvancedMultipleChainingStepSearcher : MultipleChainingStepSearcher
{
	/// <summary>
	/// Indicates the advanced step searchers.
	/// </summary>
	internal List<(int Priority, StepSearcher[] StepSearchersInThisLevel)>? _otherStepSearchers;


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


	/// <summary>
	/// Try to create a binary forcing chain hint on "on" state.
	/// </summary>
	[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
	private BinaryForcingChainsStep CreateChainingOnStep(
		ref readonly Grid grid,
		ref AnalysisContext context,
		ChainNode dstOn,
		ChainNode dstOff,
		ChainNode src,
		ChainNode target,
		bool isAbsurd
	)
	{
		var conclusion = (Conclusion[])[new(Assignment, target.Candidate)];
		var result = new BinaryForcingChainsStep(conclusion, context.Options, src, dstOn, dstOff, isAbsurd, AllowNishio, DynamicNestingLevel);
		return new(result, result.CreateViews(in grid));
	}

	/// <summary>
	/// Try to create a binary forcing chain hint on "off" state.
	/// </summary>
	[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
	private BinaryForcingChainsStep CreateChainingOffStep(
		ref readonly Grid grid,
		ref AnalysisContext context,
		ChainNode dstOn,
		ChainNode dstOff,
		ChainNode src,
		ChainNode target,
		bool isAbsurd
	)
	{
		var conclusion = (Conclusion[])[new(Elimination, target.Candidate)];
		var result = new BinaryForcingChainsStep(conclusion, context.Options, src, dstOn, dstOff, isAbsurd, AllowNishio, DynamicNestingLevel);
		return new(result, result.CreateViews(in grid));
	}

	/// <summary>
	/// Try to create a cell forcing chain hint.
	/// </summary>
	[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
	private CellForcingChainsStep CreateCellForcingStep(
		ref readonly Grid grid,
		ref AnalysisContext context,
		byte srcCell,
		ChainNode target,
		ChainBranchCollection outcomes
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

		var result = new CellForcingChainsStep(conclusion, context.Options, srcCell, chains, AllowDynamic, DynamicNestingLevel);
		return new(result, result.CreateViews(in grid));
	}

	/// <summary>
	/// Try to create a region (house) forcing chain hint.
	/// </summary>
	[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
	private RegionForcingChainsStep CreateHouseForcingStep(
		ref readonly Grid grid,
		ref AnalysisContext context,
		House houseIndex,
		byte digit,
		ChainNode target,
		ChainBranchCollection outcomes
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

		var result = new RegionForcingChainsStep(conclusions, context.Options, houseIndex, digit, chains, AllowDynamic, DynamicNestingLevel);
		return new(result, result.CreateViews(in grid));
	}
}
