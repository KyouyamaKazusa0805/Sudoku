namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bivalue Universal Grave Type 2</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="digit">Indicates the extra digit.</param>
/// <param name="cells">Indicates the cells used.</param>
public sealed partial class BivalueUniversalGraveType2Step(
	Conclusion[] conclusions,
	View[]? views,
	[PrimaryConstructorParameter(GeneratedMemberName = "ExtraDigit")] Digit digit,
	[PrimaryConstructorParameter] scoped in CellMap cells
) : BivalueUniversalGraveStep(conclusions, views)
{
	/// <inheritdoc/>
	public override Technique Code => Technique.BivalueUniversalGraveType2;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => new[] { (ExtraDifficultyCaseNames.ExtraDigit, A002024(Cells.Count) * .1M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { EnglishLanguage, new[] { ExtraDigitStr, CellsStr } }, { ChineseLanguage, new[] { CellsStr, ExtraDigitStr } } };

	private string ExtraDigitStr => (digit + 1).ToString();

	private string CellsStr => Cells.ToString();
}
