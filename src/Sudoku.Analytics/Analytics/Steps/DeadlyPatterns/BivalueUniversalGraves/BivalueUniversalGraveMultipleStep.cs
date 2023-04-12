namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave + n</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="trueCandidates">Indicates the true candidates.</param>
public sealed class BivalueUniversalGraveMultipleStep(Conclusion[] conclusions, View[]? views, IReadOnlyList<int> trueCandidates) :
	BivalueUniversalGraveStep(conclusions, views)
{
	/// <inheritdoc/>
	public override string Name => $"{base.Name} + {TrueCandidates.Count}";

	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .1M;

	/// <inheritdoc/>
	public override Technique Code => Technique.BivalueUniversalGravePlusN;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => new[] { (ExtraDifficultyCaseNames.Size, A002024(TrueCandidates.Count) * .1M) };

	/// <summary>
	/// Indicates the true candidates.
	/// </summary>
	public IReadOnlyList<int> TrueCandidates { get; } = trueCandidates;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { CandidatesStr } }, { "zh", new[] { CandidatesStr } } };

	private string CandidatesStr => (CandidateMap.Empty + TrueCandidates).ToString();
}
