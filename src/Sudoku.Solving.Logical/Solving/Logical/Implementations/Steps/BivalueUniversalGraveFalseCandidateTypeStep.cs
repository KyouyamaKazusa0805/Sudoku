namespace Sudoku.Solving.Logical.Implementations.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave False Candidate Type</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="FalseCandidate">
/// Indicates the false candidate that will cause a BUG deadly pattern if it is true.
/// </param>
internal sealed record BivalueUniversalGraveFalseCandidateTypeStep(
	ConclusionList Conclusions,
	ViewList Views,
	int FalseCandidate
) : BivalueUniversalGraveStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => base.Difficulty + .1M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BivalueUniversalGraveFalseCandidateType;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	[ResourceTextFormatter]
	internal string FalseCandidateStr() => RxCyNotation.ToCandidateString(FalseCandidate);
}
