namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Locked Type</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="pattern"><inheritdoc/></param>
/// <param name="candidates">Indicates the candidates used as locked one.</param>
public sealed partial class QiuDeadlyPatternLockedTypeStep(
	Conclusion[] conclusions,
	View[]? views,
	scoped in QiuDeadlyPattern pattern,
	[PrimaryConstructorParameter(NamingRule = ">@Locked")] scoped in CandidateMap candidates
) : QiuDeadlyPatternStep(conclusions, views, pattern)
{
	/// <inheritdoc/>
	public override int Type => 5;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => [new(ExtraDifficultyCaseNames.LockedDigit, .2M)];

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ EnglishLanguage, [PatternStr, Quantifier, Number, SingularOrPlural, CandidateStr, BeVerb] },
			{ ChineseLanguage, [Number, PatternStr] }
		};

	private string CandidateStr => CandidatesLocked.ToString();

	private string Quantifier => CandidatesLocked.Count switch { 1 => string.Empty, 2 => " both", _ => " all" };

	private string Number => CandidatesLocked.Count == 1 ? " the" : $" {CandidatesLocked.Count}";

	private string SingularOrPlural => CandidatesLocked.Count == 1 ? "candidate" : "candidates";

	private string BeVerb => CandidatesLocked.Count == 1 ? "is" : "are";
}
