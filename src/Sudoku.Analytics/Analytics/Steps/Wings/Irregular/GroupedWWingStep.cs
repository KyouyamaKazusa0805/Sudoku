namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Grouped W-Wing</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="startCell">Indicates the start cell.</param>
/// <param name="endCell">Indicates the end cell.</param>
/// <param name="bridge">Indicates the bridge cells connecting with cells <see cref="StartCell"/> and <see cref="EndCell"/>.</param>
public sealed partial class GroupedWWingStep(
	Conclusion[] conclusions,
	View[]? views,
	[PrimaryConstructorParameter] Cell startCell,
	[PrimaryConstructorParameter] Cell endCell,
	[PrimaryConstructorParameter] scoped in CellMap bridge
) : IrregularWingStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.5M;

	/// <inheritdoc/>
	public override Technique Code => Technique.GroupedWWing;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [StartCellStr, EndCellStr, BridgeStr]), new(ChineseLanguage, [StartCellStr, EndCellStr, BridgeStr])];

	private string StartCellStr => RxCyNotation.ToCellString(StartCell);

	private string EndCellStr => RxCyNotation.ToCellString(EndCell);

	private string BridgeStr => RxCyNotation.ToCellsString(Bridge);
}
