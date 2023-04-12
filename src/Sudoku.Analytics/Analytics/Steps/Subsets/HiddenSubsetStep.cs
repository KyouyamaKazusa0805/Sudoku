namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Hidden Subset</b> technique.
/// </summary>
public sealed class HiddenSubsetStep(Conclusion[] conclusions, View[]? views, int house, scoped in CellMap cells, Mask digitsMask) :
	SubsetStep(conclusions, views, house, cells, digitsMask)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .4M;

	/// <inheritdoc/>
	public override Technique Code
		=> Size switch { 2 => Technique.HiddenPair, 3 => Technique.HiddenTriple, 4 => Technique.HiddenQuadruple };

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new[] { (ExtraDifficultyCaseNames.Size, Size switch { 2 => 0, 3 => .6M, 4 => 2.0M }) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { DigitStr, HouseStr } }, { "zh", new[] { DigitStr, HouseStr } } };

	private string DigitStr => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	private string HouseStr => HouseFormatter.Format(1 << House);
}
