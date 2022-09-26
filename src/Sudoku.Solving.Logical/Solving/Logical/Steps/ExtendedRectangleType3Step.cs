namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is an <b>Extended Rectangle Type 3</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="ExtraCells">Indicates the extra cells used.</param>
/// <param name="ExtraDigitsMask">Indicates the mask that contains the extra digits.</param>
/// <param name="House">Indicates the house that extra subset formed.</param>
[StepDisplayingFeature(StepDisplayingFeature.DifficultyRatingNotStable)]
internal sealed record ExtendedRectangleType3Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in CellMap Cells,
	short DigitsMask,
	scoped in CellMap ExtraCells,
	short ExtraDigitsMask,
	int House
) : ExtendedRectangleStep(Conclusions, Views, Cells, DigitsMask), IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public override int Type => 3;

	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public override (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[]
		{
			base.ExtraDifficultyValues[0],
			(PhasedDifficultyRatingKinds.ExtraDigit, PopCount((uint)ExtraDigitsMask) * .1M)
		};

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	[ResourceTextFormatter]
	internal string ExtraDigitsStr() => DigitMaskFormatter.Format(ExtraDigitsMask, FormattingMode.Normal);

	[ResourceTextFormatter]
	internal string ExtraCellsStr() => ExtraCells.ToString();

	[ResourceTextFormatter]
	internal string HouseStr() => HouseFormatter.Format(1 << House);
}
