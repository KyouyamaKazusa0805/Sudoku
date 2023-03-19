namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave False Candidate Type</b> technique.
/// </summary>
public sealed class BivalueUniversalGraveFalseCandidateTypeStep(Conclusion[] conclusions, View[]? views, int falseCandidate) :
	BivalueUniversalGraveStep(conclusions, views)
{
	/// <summary>
	/// Indicates the false candidate that will cause a BUG deadly pattern if it is true.
	/// </summary>
	public int FalseCandidate { get; } = falseCandidate;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .1M;

	/// <inheritdoc/>
	public override Technique Code => Technique.BivalueUniversalGraveFalseCandidateType;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?>? FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { FalseCandidateStr } }, { "zh", new[] { FalseCandidateStr } } };

	private string FalseCandidateStr => RxCyNotation.ToCandidateString(FalseCandidate);
}
