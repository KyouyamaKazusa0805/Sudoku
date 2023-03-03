namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Type 1</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Pattern"><inheritdoc/></param>
/// <param name="Candidate">Indicates the extra candidate used.</param>
internal sealed record QiuDeadlyPatternType1Step(Conclusion[] Conclusions, View[]? Views, scoped in QiuDeadlyPattern Pattern, int Candidate) :
	QiuDeadlyPatternStep(Conclusions, Views, Pattern)
{
	/// <inheritdoc/>
	public override int Type => 1;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { PatternStr, CandidateStr } }, { "zh", new[] { CandidateStr, PatternStr } } };

	private string CandidateStr => (CandidateMap.Empty + Candidate).ToString();
}
