namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Type 4</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Pattern"><inheritdoc/></param>
/// <param name="ConjugatePair">Indicates the conjugate pair used.</param>
[StepDisplayingFeature(StepDisplayingFeature.VeryRare)]
internal sealed record QiuDeadlyPatternType4Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in QiuDeadlyPattern Pattern,
	scoped in Conjugate ConjugatePair
) : QiuDeadlyPatternStep(Conclusions, Views, Pattern)
{
	/// <inheritdoc/>
	public override int Type => 4;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.ConjugatePair, .2M) };


	[ResourceTextFormatter]
	internal string ConjStr() => ConjugatePair.ToString();
}
