namespace Sudoku.Solving.Manual.Steps.Chains.Forcing;

/// <summary>
/// Provides with a step that is a <b>Binary Forcing Chains</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="SourceNode">Indicates the source node.</param>
/// <param name="FromOnNode">Indicates the node that is the destination (on side).</param>
/// <param name="FromOffNode">Indicates the node that is the destination (off side).</param>
/// <param name="IsAbsurd">Indicates whether the chain is absurd.</param>
/// <param name="IsMultiple"><inheritdoc/></param>
/// <param name="IsNishio"><inheritdoc/></param>
/// <param name="Level"><inheritdoc/></param>
public sealed record BinaryChainingStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	in ChainNode SourceNode,
	in ChainNode FromOnNode,
	in ChainNode FromOffNode,
	bool IsAbsurd,
	bool IsMultiple,
	bool IsNishio,
	byte Level
) : ChainStep(Conclusions, Views, XEnabled: true, YEnabled: true, IsNishio, IsMultiple, IsDynamic: true, Level)
{
	/// <inheritdoc/>
	public override int FlatComplexity => FromOnNode.AncestorsCount + FromOffNode.AncestorsCount;

	/// <inheritdoc/>
	public override decimal Difficulty => BaseDifficulty + LengthDifficulty;

	/// <summary>
	/// Indicates the anchor.
	/// </summary>
	public ChainNode Anchor =>
		IsNishio || IsAbsurd ? new(SourceNode.Cell, SourceNode.Digit, !SourceNode.IsOn) : FromOnNode;

	/// <inheritdoc/>
	public override ChainTypeCode SortKey =>
		IsAbsurd ? ChainTypeCode.DynamicContradictionFc : ChainTypeCode.DynamicDoubleFc;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => TechniqueTags.LongChaining | TechniqueTags.ForcingChains;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.ForcingChains;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;

	[FormatItem]
	private string AnchorCandidateStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Candidates { Anchor.Cell * 9 + Anchor.Digit }.ToString();
	}

	[FormatItem]
	private string AnchorIsTrueOrFalseStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ResourceDocumentManager.Shared[Anchor.IsOn ? "trueKeyword" : "falseKeyword"];
	}
}
