namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave Type 2</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit">Indicates the digit used.</param>
/// <param name="Cells">Indicates the cells used.</param>
internal sealed record BivalueUniversalGraveType2Step(ConclusionList Conclusions, ViewList Views, int Digit, scoped in CellMap Cells) :
	BivalueUniversalGraveStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BivalueUniversalGraveType2;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.ExtraDigit, A002024(Cells.Count) * .1M) };


	[ResourceTextFormatter]
	internal string ExtraDigitStr() => (Digit + 1).ToString();

	[ResourceTextFormatter]
	internal string CellsStr() => Cells.ToString();
}
