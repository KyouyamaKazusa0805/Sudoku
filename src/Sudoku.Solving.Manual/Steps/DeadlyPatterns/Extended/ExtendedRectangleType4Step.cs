namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is an <b>Extended Rectangle Type 4</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="ConjugatePair">Indicates the conjugate pair used.</param>
internal sealed partial record ExtendedRectangleType4Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in CellMap Cells,
	short DigitsMask,
	scoped in Conjugate ConjugatePair
) : ExtendedRectangleStep(Conclusions, Views, Cells, DigitsMask), IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public override int Type => 4;

	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public override (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[]
		{
			base.ExtraDifficultyValues[0],
			(PhasedDifficultyRatingKinds.ConjugatePair, .1M)
		};

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;

	[ResourceTextFormatter]
	private partial string ConjStr() => ConjugatePair.ToString();
}
