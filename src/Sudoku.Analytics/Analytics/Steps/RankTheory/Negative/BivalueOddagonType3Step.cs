namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bivalue Oddagon Type 3</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="loopCells"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="extraCells">Indicates the extra cells used.</param>
/// <param name="extraDigitsMask">Indicates the mask that contains all extra digits used.</param>
public sealed partial class BivalueOddagonType3Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped in CellMap loopCells,
	Digit digit1,
	Digit digit2,
	[PrimaryConstructorParameter] scoped in CellMap extraCells,
	[PrimaryConstructorParameter] Mask extraDigitsMask
) : BivalueOddagonStep(conclusions, views, loopCells, digit1, digit2)
{
	/// <inheritdoc/>
	public override int Type => 3;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => [new(ExtraDifficultyCaseNames.Size, (ExtraCells.Count >> 1) * .1M)];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [LoopStr, Digit1Str, Digit2Str, DigitsStr, ExtraCellsStr]),
			new(ChineseLanguage, [Digit1Str, Digit2Str, LoopStr, ExtraCellsStr, DigitsStr])
		];

	private string Digit1Str => (Digit1 + 1).ToString();

	private string Digit2Str => (Digit2 + 1).ToString();

	private string DigitsStr => DigitNotation.ToString(ExtraDigitsMask);

	private string ExtraCellsStr => ExtraCells.ToString();
}
