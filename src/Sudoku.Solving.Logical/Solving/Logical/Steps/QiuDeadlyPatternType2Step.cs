namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Type 2</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Pattern"><inheritdoc/></param>
/// <param name="ExtraDigit">Indicates the extra digit used.</param>
[StepDisplayingFeature(StepDisplayingFeature.VeryRare)]
internal sealed record QiuDeadlyPatternType2Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in QiuDeadlyPattern Pattern,
	int ExtraDigit
) : QiuDeadlyPatternStep(Conclusions, Views, Pattern)
{
	/// <inheritdoc/>
	public override int Type => 2;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.ExtraDigit, .1M) };


	[ResourceTextFormatter]
	internal string ExtraDigitStr() => (ExtraDigit + 1).ToString();
}
