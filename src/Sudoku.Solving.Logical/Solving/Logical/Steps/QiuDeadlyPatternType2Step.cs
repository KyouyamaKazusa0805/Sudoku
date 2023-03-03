namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Type 2</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Pattern"><inheritdoc/></param>
/// <param name="ExtraDigit">Indicates the extra digit used.</param>
internal sealed record QiuDeadlyPatternType2Step(Conclusion[] Conclusions, View[]? Views, scoped in QiuDeadlyPattern Pattern, int ExtraDigit) :
	QiuDeadlyPatternStep(Conclusions, Views, Pattern)
{
	/// <inheritdoc/>
	public override int Type => 2;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.ExtraDigit, .1M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { PatternStr, ExtraDigitStr } }, { "zh", new[] { PatternStr, ExtraDigitStr } } };

	private string ExtraDigitStr => (ExtraDigit + 1).ToString();
}
