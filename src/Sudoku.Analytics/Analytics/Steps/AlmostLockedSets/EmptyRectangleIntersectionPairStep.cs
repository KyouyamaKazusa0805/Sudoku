namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Empty Rectangle Intersection Pair</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="startCell">Indicates the start cell to be calculated.</param>
/// <param name="endCell">Indicates the end cell to be calculated.</param>
/// <param name="house">Indicates the house index that the empty rectangle forms.</param>
/// <param name="digit1">Indicates the first digit used.</param>
/// <param name="digit2">Indicates the second digit used.</param>
public sealed partial class EmptyRectangleIntersectionPairStep(
	Conclusion[] conclusions,
	View[]? views,
	[DataMember] Cell startCell,
	[DataMember] Cell endCell,
	[DataMember] House house,
	[DataMember] Digit digit1,
	[DataMember] Digit digit2
) : AlmostLockedSetsStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 6.0M;

	/// <inheritdoc/>
	public override Technique Code => Technique.EmptyRectangleIntersectionPair;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [Digit1Str, Digit2Str, StartCellStr, EndCellStr, HouseStr]),
			new(ChineseLanguage, [Digit1Str, Digit2Str, StartCellStr, EndCellStr, HouseStr])
		];

	private string Digit1Str => (Digit1 + 1).ToString();

	private string Digit2Str => (Digit2 + 1).ToString();

	private string StartCellStr => CellNotation.ToString(StartCell);

	private string EndCellStr => CellNotation.ToString(EndCell);

	private string HouseStr => HouseNotation.ToString(House);
}
