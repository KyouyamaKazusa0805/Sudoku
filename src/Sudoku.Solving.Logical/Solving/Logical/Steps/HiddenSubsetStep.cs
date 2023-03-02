namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Hidden Subset</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="House"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
internal sealed record HiddenSubsetStep(
	ConclusionList Conclusions,
	ViewList Views,
	int House,
	scoped in CellMap Cells,
	short DigitsMask
) : SubsetStep(Conclusions, Views, House, Cells, DigitsMask)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .4M;

	/// <inheritdoc/>
	public override Technique TechniqueCode
		=> Size switch { 2 => Technique.HiddenPair, 3 => Technique.HiddenTriple, 4 => Technique.HiddenQuadruple };

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.Size, Size switch { 2 => 0, 3 => .6M, 4 => 2.0M }) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { DigitStr, HouseStr } }, { "zh", new[] { DigitStr, HouseStr } } };

	private string DigitStr => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	private string HouseStr => HouseFormatter.Format(1 << House);
}
