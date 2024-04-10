namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>(Grouped) H-Wing</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cell1">Indicates the cell 1.</param>
/// <param name="cell2">Indicates the cell 2.</param>
/// <param name="digitX">Indicates the digit X.</param>
/// <param name="digitY">Indicates the digit Y.</param>
/// <param name="digitZ">Indicates the digit Z.</param>
/// <param name="strongLink">Indicates the storng link used.</param>
public sealed partial class HWingStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] Cell cell1,
	[PrimaryConstructorParameter] Cell cell2,
	[PrimaryConstructorParameter] Digit digitX,
	[PrimaryConstructorParameter] Digit digitY,
	[PrimaryConstructorParameter] Digit digitZ,
	[PrimaryConstructorParameter] scoped ref readonly CellMap strongLink
) : IrregularWingStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override bool IsSymmetricPattern => false;

	/// <inheritdoc/>
	public override bool IsGrouped => StrongLink.Count > 2;

	/// <inheritdoc/>
	public override int BaseDifficulty => 47;

	/// <inheritdoc/>
	public override Technique Code => IsGrouped ? Technique.GroupedHWing : Technique.HWing;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [StrongLinkStr, CellsStr]), new(ChineseLanguage, [StrongLinkStr, CellsStr])];

	private string StrongLinkStr => $"{Options.Converter.CellConverter(StrongLink)}({DigitX + 1})";

	private string CellsStr => Options.Converter.CellConverter([Cell1, Cell2]);
}
