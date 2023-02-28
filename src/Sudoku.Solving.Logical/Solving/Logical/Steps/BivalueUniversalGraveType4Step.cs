namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave Type 4</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="DigitsMask">Indicates the digits used.</param>
/// <param name="Cells">Indicates the cells used.</param>
/// <param name="ConjugatePair">Indicates the conjugate pair used.</param>
internal sealed record BivalueUniversalGraveType4Step(
	ConclusionList Conclusions,
	ViewList Views,
	short DigitsMask,
	scoped in CellMap Cells,
	scoped in Conjugate ConjugatePair
) : BivalueUniversalGraveStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BivalueUniversalGraveType4;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.ConjugatePair, .1M) };


	[ResourceTextFormatter]
	internal string DigitsStr() => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	[ResourceTextFormatter]
	internal string CellsStr() => Cells.ToString();

	[ResourceTextFormatter]
	internal string ConjStr() => ConjugatePair.ToString();
}
