namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Type 4</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Pattern"><inheritdoc/></param>
/// <param name="ConjugatePair">Indicates the conjugate pair used.</param>
internal sealed partial record QiuDeadlyPatternType4Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in QiuDeadlyPattern Pattern,
	scoped in Conjugate ConjugatePair
) : QiuDeadlyPatternStep(Conclusions, Views, Pattern), IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => base.Difficulty;

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[] { (PhasedDifficultyRatingKinds.ConjugatePair, .2M) };

	/// <inheritdoc/>
	public override int Type => 4;

	[ResourceTextFormatter]
	private partial string ConjStr() => ConjugatePair.ToString();
}
