namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Empty Rectangle Intersection Pair</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="startCell">Indicates the start cell to be calculated.</param>
/// <param name="endCell">Indicates the end cell to be calculated.</param>
/// <param name="house">Indicates the house index that the empty rectangle forms.</param>
/// <param name="digit1">Indicates the first digit used.</param>
/// <param name="digit2">Indicates the second digit used.</param>
public sealed partial class EmptyRectangleIntersectionPairStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] Cell startCell,
	[PrimaryConstructorParameter] Cell endCell,
	[PrimaryConstructorParameter] House house,
	[PrimaryConstructorParameter] Digit digit1,
	[PrimaryConstructorParameter] Digit digit2
) : AlmostLockedSetsStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => 60;

	/// <inheritdoc/>
	public override Technique Code => Technique.EmptyRectangleIntersectionPair;

	/// <inheritdoc/>
	public override Interpolation[] Interpolations
		=> [
			new(EnglishLanguage, [Digit1Str, Digit2Str, StartCellStr, EndCellStr, HouseStr]),
			new(ChineseLanguage, [Digit1Str, Digit2Str, StartCellStr, EndCellStr, HouseStr])
		];

	private string Digit1Str => Options.Converter.DigitConverter((Mask)(1 << Digit1));

	private string Digit2Str => Options.Converter.DigitConverter((Mask)(1 << Digit2));

	private string StartCellStr => Options.Converter.CellConverter(in StartCell.AsCellMap());

	private string EndCellStr => Options.Converter.CellConverter(in EndCell.AsCellMap());

	private string HouseStr => Options.Converter.HouseConverter(1 << House);
}
