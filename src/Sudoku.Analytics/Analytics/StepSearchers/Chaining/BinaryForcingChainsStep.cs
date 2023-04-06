namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a step that is a <b>Binary Forcing Chains</b> technique that is compatible with program <b>Sudoku Explainer</b>.
/// The technique contains two kinds of forcing chains:
/// <list type="bullet">
/// <item>Dynamic Contradiction Forcing Chains</item>
/// <item>Dynamic Double Forcing Chains</item>
/// </list>
/// </summary>
public sealed class BinaryForcingChainsStep(
	Conclusion[] conclusions,
	View[]? views,
	ChainNode sourcePotential,
	ChainNode fromOnPotential,
	ChainNode fromOffPotential,
	bool isAbsurd,
	bool isNishio,
	int dynamicNestingLevel = 0
) : ChainingStep(conclusions, views, isMultiple: true, isDynamic: true, isNishio: isNishio, dynamicNestingLevel: dynamicNestingLevel)
{
	internal BinaryForcingChainsStep(
		Conclusion[] conclusions,
		ChainNode sourcePotential,
		ChainNode fromOnPotential,
		ChainNode fromOffPotential,
		bool isAbsurd,
		bool isNishio,
		int dynamicNestingLevel = 0
	) : this(conclusions, null!, sourcePotential, fromOnPotential, fromOffPotential, isAbsurd, isNishio, dynamicNestingLevel)
	{
	}

	internal BinaryForcingChainsStep(BinaryForcingChainsStep @base, View[]? views) :
		this(
			@base.Conclusions,
			views,
			@base.SourcePotential,
			@base.FromOnPotential,
			@base.FromOffPotential,
			@base.IsAbsurd,
			@base.IsNishio,
			@base.DynamicNestingLevel
		)
	{
	}


	/// <summary>
	/// Indicates whether the forcing chains kind is contradiction.
	/// </summary>
	public bool IsAbsurd { get; } = isAbsurd;

	/// <inheritdoc/>
	public override string Format
		=> (IsAbsurd ? R["TechniqueFormat_ContradictionForcingChainsStep"] : R["TechniqueFormat_DoubleForcingChainsStep"])!;

	/// <summary>
	/// Indicates the source potential of the chain.
	/// </summary>
	public ChainNode SourcePotential { get; } = sourcePotential;

	/// <summary>
	/// Indicates the "on" branch.
	/// </summary>
	public ChainNode FromOnPotential { get; } = fromOnPotential;

	/// <summary>
	/// Indicates the "off" branch.
	/// </summary>
	public ChainNode FromOffPotential { get; } = fromOffPotential;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{
				"en",
				IsAbsurd ? new[] { StartCandStr, StartCandOnOffStr, EndCandStr } : new[] { StartCandStr, StartCandOnOffStr, EndCandStr }
			},
			{
				"zh",
				IsAbsurd ? new[] { StartCandStr, StartCandOnOffStrZhCn, EndCandStr } : new[] { EndCandStr, StartCandStr, StartCandOnOffStrZhCn }
			}
		};

	private string StartCandStr => RxCyNotation.ToCandidateString(SourcePotential.Candidate);

	private string StartCandOnOffStr => SourcePotential.IsOn.ToString().ToLower();

	private string StartCandOnOffStrZhCn => (SourcePotential.IsOn ? R["TrueKeyword"] : R["FalseKeyword"])!;

	private string EndCandStr => RxCyNotation.ToCandidateString(FromOnPotential.Candidate);


	/// <inheritdoc/>
	protected override CandidateMap GetOnPotentials(int viewIndex)
		=> viewIndex >= FlatViewsCount ? GetNestedOnPotentials(viewIndex) : GetColorCandidates(viewIndex, true);

	/// <inheritdoc/>
	protected override CandidateMap GetOffPotentials(int viewIndex)
		=> viewIndex >= FlatViewsCount ? GetNestedOffPotentials(viewIndex) : GetColorCandidates(viewIndex, false);

	/// <inheritdoc/>
	protected override List<LinkViewNode> GetLinks(int viewIndex)
		=> viewIndex >= FlatViewsCount ? GetNestedLinks(viewIndex) : GetLinks(viewIndex == 0 ? FromOnPotential : FromOffPotential);

	/// <summary>
	/// Gets all colored candidates with the specified state.
	/// </summary>
	/// <param name="viewIndex">The view index.</param>
	/// <param name="state">The state of the candidate you want to color.</param>
	/// <returns>All colored candidates with a same state.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private CandidateMap GetColorCandidates(int viewIndex, bool state)
		=> GetColorCandidates(viewIndex == 0 ? FromOnPotential : FromOffPotential, state, state);
}
