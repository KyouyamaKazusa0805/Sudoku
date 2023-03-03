namespace Sudoku.Solving.Logical.StepSearchers;

[StepSearcher]
[SeparatedStepSearcher(0, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(DynamicNestingLevel), 1)]
[SeparatedStepSearcher(1, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(DynamicNestingLevel), 2)]
[SeparatedStepSearcher(2, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(DynamicNestingLevel), 3)]
[SeparatedStepSearcher(3, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(DynamicNestingLevel), 4)]
[SeparatedStepSearcher(4, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(DynamicNestingLevel), 5)]
internal sealed partial class AdvancedMultipleChainingStepSearcher : MultipleChainingStepSearcher, IChainingStepSearcher
{
	/// <summary>
	/// Indicates the advanced step searchers.
	/// </summary>
	private Dictionary<int, IStepSearcher[]>? _otherStepSearchers;


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
	protected override void OnAdvanced(NodeList pendingOn, NodeList pendingOff, NodeSet toOff, scoped in Grid grid, scoped in Grid original)
	{
		if (pendingOn.Count == 0 && pendingOff.Count == 0 && DynamicNestingLevel > 0)
		{
			foreach (var pOff in GetAdvancedPotentials(grid, original, toOff))
			{
				if (!toOff.Contains(pOff))
				{
					// Not processed yet.
					toOff.Add(pOff);
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
	/// <inheritdoc cref="OnAdvanced(NodeList, NodeList, NodeSet, in Grid, in Grid)" path="/param[@name='toOff']"/>
	/// </param>
	/// <returns>Found <see cref="ChainNode"/> instances.</returns>
	[MemberNotNull(nameof(_otherStepSearchers))]
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
	private NodeList GetAdvancedPotentials(scoped in Grid grid, scoped in Grid original, NodeSet offPotentials)
	{
		_otherStepSearchers ??= new()
		{
			{ 1, new IStepSearcher[] { new LockedCandidatesStepSearcher(), new SubsetStepSearcher(), new NormalFishStepSearcher() } },
			{ 2, new IStepSearcher[] { new NonMultipleChainingStepSearcher() } },
			{ 3, new IStepSearcher[] { new MultipleChainingStepSearcher { AllowMultiple = true } } },
			{ 4, new IStepSearcher[] { new MultipleChainingStepSearcher { AllowDynamic = true, AllowMultiple = true } } },
			{ 5, new IStepSearcher[] { new AdvancedMultipleChainingStepSearcher { DynamicNestingLevel = DynamicNestingLevel - 3 } } }
		};

		var result = new NodeList();
		return result;
	}

	/// <summary>
	/// Try to create a binary forcing chain hint on "on" state.
	/// </summary>
	[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
	private BinaryForcingChainsStep CreateChainingOnStep(scoped in Grid grid, ChainNode dstOn, ChainNode dstOff, ChainNode src, ChainNode target, bool isAbsurd)
	{
		var conclusion = new[] { new Conclusion(Assignment, target.Candidate) };
		var result = new BinaryForcingChainsStep(conclusion, src, dstOn, dstOff, isAbsurd, AllowNishio, DynamicNestingLevel);
		return result with { Views = result.CreateViews(grid) };
	}

	/// <summary>
	/// Try to create a binary forcing chain hint on "off" state.
	/// </summary>
	[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
	private BinaryForcingChainsStep CreateChainingOffStep(scoped in Grid grid, ChainNode dstOn, ChainNode dstOff, ChainNode src, ChainNode target, bool isAbsurd)
	{
		var conclusion = new[] { new Conclusion(Elimination, target.Candidate) };
		var result = new BinaryForcingChainsStep(conclusion, src, dstOn, dstOff, isAbsurd, AllowNishio, DynamicNestingLevel);
		return result with { Views = result.CreateViews(grid) };
	}

	/// <summary>
	/// Try to create a cell forcing chain hint.
	/// </summary>
	[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
	private CellForcingChainsStep CreateCellForcingStep(scoped in Grid grid, byte srcCell, ChainNode target, ChainBranch outcomes)
	{
		var (targetCell, targetDigit, targetIsOn) = target;
		var conclusion = new[] { new Conclusion(targetIsOn ? Assignment : Elimination, targetCell, targetDigit) };

		// Build chains.
		var chains = new MultipleForcingChains();
		for (byte tempDigit = 0; tempDigit < 9; tempDigit++)
		{
			if (CandidatesMap[tempDigit].Contains(srcCell))
			{
				// Get corresponding value with the matching parents.
				chains.Add(tempDigit, outcomes[tempDigit].GetNullable(target) ?? default);
			}
		}

		var result = new CellForcingChainsStep(conclusion, srcCell, chains, AllowDynamic, DynamicNestingLevel);
		return result with { Views = result.CreateViews(grid) };
	}

	/// <summary>
	/// Try to create a region (house) forcing chain hint.
	/// </summary>
	[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
	private RegionForcingChainsStep CreateHouseForcingStep(scoped in Grid grid, int houseIndex, byte digit, ChainNode target, ChainBranch outcomes)
	{
		var (targetCell, targetDigit, targetIsOn) = target;
		var conclusions = new[] { new Conclusion(targetIsOn ? Assignment : Elimination, targetCell, targetDigit) };

		// Build chains.
		var chains = new MultipleForcingChains();
		foreach (byte tempCell in CandidatesMap[digit] & HousesMap[houseIndex])
		{
			// Get corresponding value with the matching parents.
			chains.Add(tempCell, outcomes[tempCell].GetNullable(target) ?? default);
		}

		var result = new RegionForcingChainsStep(conclusions, houseIndex, digit, chains, AllowDynamic, DynamicNestingLevel);
		return result with { Views = result.CreateViews(grid) };
	}
}
