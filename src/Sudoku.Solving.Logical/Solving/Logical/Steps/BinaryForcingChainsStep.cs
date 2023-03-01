namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Binary Forcing Chains</b> technique that is compatible with program <b>Sudoku Explainer</b>.
/// The technique contains two kinds of forcing chains:
/// <list type="bullet">
/// <item>Dynamic Contradiction Forcing Chains</item>
/// <item>Dynamic Double Forcing Chains</item>
/// </list>
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="SourcePotential">Indicates the source potential of the chain.</param>
/// <param name="FromOnPotential">Indicates the "on" branch.</param>
/// <param name="FromOffPotential">Indicates the "off" branch.</param>
/// <param name="IsAbsurd">Indicates whether the forcing chains kind is contradiction.</param>
/// <param name="IsNishio"><inheritdoc/></param>
/// <param name="DynamicNestingLevel"><inheritdoc/></param>
internal sealed record BinaryForcingChainsStep(
	ConclusionList Conclusions,
	ChainNode SourcePotential,
	ChainNode FromOnPotential,
	ChainNode FromOffPotential,
	bool IsAbsurd,
	bool IsNishio,
	int DynamicNestingLevel = 0
) : ChainingStep(Conclusions, IsMultiple: true, IsDynamic: true, IsNishio: IsNishio, DynamicNestingLevel: DynamicNestingLevel)
{
	/// <inheritdoc/>
	public override string? Format
		=> IsAbsurd ? R["TechniqueFormat_ContradictionForcingChainsStep"] : R["TechniqueFormat_DoubleForcingChainsStep"];

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
	protected override CandidateMap GetGreenPotentials(int viewIndex)
		=> viewIndex >= FlatViewsCount ? GetNestedGreenPotentials(viewIndex) : GetColorCandidates(viewIndex, true);

	/// <inheritdoc/>
	protected override CandidateMap GetRedPotentials(int viewIndex)
		=> viewIndex >= FlatViewsCount ? GetNestedRedPotentials(viewIndex) : GetColorCandidates(viewIndex, false);

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
