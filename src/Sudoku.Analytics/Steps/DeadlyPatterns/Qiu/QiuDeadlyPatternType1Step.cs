namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Type 1</b> technique.
/// </summary>
public sealed class QiuDeadlyPatternType1Step(Conclusion[] conclusions, View[]? views, scoped in QiuDeadlyPattern pattern, int candidate) :
	QiuDeadlyPatternStep(conclusions, views, pattern)
{
	/// <inheritdoc/>
	public override int Type => 1;

	/// <summary>
	/// Indicates the target candidate.
	/// </summary>
	public int Candidate { get; } = candidate;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { PatternStr, CandidateStr } }, { "zh", new[] { CandidateStr, PatternStr } } };

	private string CandidateStr => (CandidateMap.Empty + Candidate).ToString();
}
