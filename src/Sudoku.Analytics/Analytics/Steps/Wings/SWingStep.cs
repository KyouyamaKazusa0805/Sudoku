namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>(Grouped) S-Wing</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="xNode1">Indicates the node 1 for digit X.</param>
/// <param name="xNode2">Indicates the node 2 for digit X.</param>
/// <param name="yNode1">Indicates the node 1 for digit Y.</param>
/// <param name="yNode2">Indicates the node 2 for digit Y.</param>
/// <param name="DigitX">Indicates the digit X.</param>
/// <param name="DigitY">Indicates the digit Y.</param>
/// <param name="midCell">Indicates the mid cell.</param>
public sealed partial class SWingStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] ref readonly CellMap xNode1,
	[PrimaryConstructorParameter] ref readonly CellMap xNode2,
	[PrimaryConstructorParameter] ref readonly CellMap yNode1,
	[PrimaryConstructorParameter] ref readonly CellMap yNode2,
	[PrimaryConstructorParameter] Digit DigitX,
	[PrimaryConstructorParameter] Digit DigitY,
	[PrimaryConstructorParameter] Cell midCell
) : IrregularWingStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override bool IsSymmetricPattern => true;

	/// <inheritdoc/>
	public override bool IsGrouped => XNode1.Count != 1 || XNode2.Count != 1 || YNode1.Count != 1 || YNode2.Count != 1;

	/// <inheritdoc/>
	public override int BaseDifficulty => 47;

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [new(SR.EnglishLanguage, [XLinkStr, YLinkStr, MidCellStr]), new(SR.ChineseLanguage, [XLinkStr, YLinkStr, MidCellStr])];

	/// <inheritdoc/>
	public override Technique Code => IsGrouped ? Technique.GroupedSWing : Technique.SWing;

	private string XLinkStr
	{
		get
		{
			var converter = Options.Converter.CellConverter;
			return $"{converter(XNode1)} == {converter(XNode2)}({Options.Converter.DigitConverter((Mask)(1 << DigitX))})";
		}
	}

	private string YLinkStr
	{
		get
		{
			var converter = Options.Converter.CellConverter;
			return $"{converter(YNode1)} == {converter(YNode2)}({Options.Converter.DigitConverter((Mask)(1 << DigitY))})";
		}
	}

	private string MidCellStr => Options.Converter.CellConverter(in MidCell.AsCellMap());
}
