namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Locked Type</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Pattern"><inheritdoc/></param>
/// <param name="Candidates">Indicates the candidates used.</param>
[StepDisplayingFeature(StepDisplayingFeature.VeryRare)]
internal sealed record QiuDeadlyPatternLockedTypeStep(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in QiuDeadlyPattern Pattern,
	IReadOnlyList<int> Candidates
) : QiuDeadlyPatternStep(Conclusions, Views, Pattern)
{
	/// <inheritdoc/>
	public override int Type => 5;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.LockedDigit, .2M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { PatternStr, Quantifier, Number, SingularOrPlural, CandidateStr, BeVerb } },
			{ "zh", new[] { Number, PatternStr } }
		};

	private string CandidateStr => (CandidateMap.Empty + Candidates).ToString();

	private string Quantifier => Candidates.Count switch { 1 => string.Empty, 2 => " both", _ => " all" };

	private string Number => Candidates.Count == 1 ? " the" : $" {Candidates.Count}";

	private string SingularOrPlural => Candidates.Count == 1 ? "candidate" : "candidates";

	private string BeVerb => Candidates.Count == 1 ? "is" : "are";
}
