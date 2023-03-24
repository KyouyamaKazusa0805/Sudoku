namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Locked Type</b> technique.
/// </summary>
public sealed class QiuDeadlyPatternLockedTypeStep(
	Conclusion[] conclusions,
	View[]? views,
	scoped in QiuDeadlyPattern pattern,
	scoped in CandidateMap candidates
) : QiuDeadlyPatternStep(conclusions, views, pattern)
{
	/// <inheritdoc/>
	public override int Type => 5;

	/// <summary>
	/// Indicates the candidates used.
	/// </summary>
	public CandidateMap Candidates { get; } = candidates;

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
