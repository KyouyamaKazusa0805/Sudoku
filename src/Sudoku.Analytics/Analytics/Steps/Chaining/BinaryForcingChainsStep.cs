namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Binary Forcing Chains</b> technique that is compatible with program <b>Sudoku Explainer</b>.
/// The technique contains two kinds of forcing chains:
/// <list type="bullet">
/// <item>Dynamic Contradiction Forcing Chains</item>
/// <item>Dynamic Double Forcing Chains</item>
/// </list>
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="sourcePotential">Indicates the source potential of the chain.</param>
/// <param name="fromOnPotential">Indicates the "on" branch.</param>
/// <param name="fromOffPotential">Indicates the "off" branch.</param>
/// <param name="isAbsurd">Indicates whether the forcing chains kind is contradiction.</param>
/// <param name="isNishio"><inheritdoc/></param>
/// <param name="dynamicNestingLevel"><inheritdoc/></param>
public sealed partial class BinaryForcingChainsStep(
	Conclusion[] conclusions,
	View[]? views,
	[PrimaryConstructorParameter] ChainNode sourcePotential,
	[PrimaryConstructorParameter] ChainNode fromOnPotential,
	[PrimaryConstructorParameter] ChainNode fromOffPotential,
	[PrimaryConstructorParameter] bool isAbsurd,
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


	/// <inheritdoc/>
	public override string Format
		=> (IsAbsurd ? GetString("TechniqueFormat_ContradictionForcingChainsStep") : GetString("TechniqueFormat_DoubleForcingChainsStep"))!;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(
				EnglishLanguage,
				IsAbsurd ? [StartCandStr, StartCandOnOffStr, EndCandStr] : [StartCandStr, StartCandOnOffStr, EndCandStr]
			),
			new(
				ChineseLanguage,
				IsAbsurd ? [StartCandStr, StartCandOnOffStrZhCn, EndCandStr] : [EndCandStr, StartCandStr, StartCandOnOffStrZhCn]
			)
		];

	private string StartCandStr => RxCyNotation.ToCandidateString(SourcePotential.Candidate);

	private string StartCandOnOffStr => SourcePotential.IsOn.ToString().ToLower();

	private string StartCandOnOffStrZhCn => (SourcePotential.IsOn ? GetString("TrueKeyword") : GetString("FalseKeyword"))!;

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
