namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is an <b>Extended Rectangle Type 2</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="ExtraDigit">Indicates the extra digit used.</param>
internal sealed record ExtendedRectangleType2Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in CellMap Cells,
	short DigitsMask,
	int ExtraDigit
) : ExtendedRectangleStep(Conclusions, Views, Cells, DigitsMask)
{
	/// <inheritdoc/>
	public override int Type => 2;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new[] { base.ExtraDifficultyCases[0], new(ExtraDifficultyCaseNames.ExtraDigit, .1M) };


	[ResourceTextFormatter]
	internal string ExtraDigitStr() => (ExtraDigit + 1).ToString();
}
