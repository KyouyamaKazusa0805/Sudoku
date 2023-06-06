namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave False Candidate Type</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="falseCandidate">Indicates the false candidate that will cause a BUG deadly pattern if it is true.</param>
public sealed partial class BivalueUniversalGraveFalseCandidateTypeStep(
	Conclusion[] conclusions,
	View[]? views,
	[PrimaryConstructorParameter] Candidate falseCandidate
) : BivalueUniversalGraveStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .1M;

	/// <inheritdoc/>
	public override Technique Code => Technique.BivalueUniversalGraveFalseCandidateType;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { EnglishLanguage, new[] { FalseCandidateStr } }, { ChineseLanguage, new[] { FalseCandidateStr } } };

	private string FalseCandidateStr => RxCyNotation.ToCandidateString(FalseCandidate);
}
