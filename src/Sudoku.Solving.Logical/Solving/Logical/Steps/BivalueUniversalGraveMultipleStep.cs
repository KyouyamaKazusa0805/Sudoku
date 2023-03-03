namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave + n</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Candidates">Indicates the true candidates.</param>
internal sealed record BivalueUniversalGraveMultipleStep(ConclusionList Conclusions, ViewList Views, IReadOnlyList<int> Candidates) :
	BivalueUniversalGraveStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override string Name => $"{base.Name} + {Candidates.Count}";

	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .1M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BivalueUniversalGravePlusN;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.Size, A002024(Candidates.Count) * .1M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?>? FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { CandidatesStr } }, { "zh", new[] { CandidatesStr } } };

	private string CandidatesStr => (CandidateMap.Empty + Candidates).ToString();
}
