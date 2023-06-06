namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Extended Rectangle Type 2</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="extraDigit">Indicates the extra digit used.</param>
public sealed partial class ExtendedRectangleType2Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped in CellMap cells,
	Mask digitsMask,
	[PrimaryConstructorParameter] Digit extraDigit
) : ExtendedRectangleStep(conclusions, views, cells, digitsMask)
{
	/// <inheritdoc/>
	public override int Type => 2;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new[] { base.ExtraDifficultyCases[0], (ExtraDifficultyCaseNames.ExtraDigit, .1M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ EnglishLanguage, new[] { DigitsStr, CellsStr, ExtraDigitStr } },
			{ ChineseLanguage, new[] { DigitsStr, CellsStr, ExtraDigitStr } }
		};

	private string ExtraDigitStr => (ExtraDigit + 1).ToString();
}
