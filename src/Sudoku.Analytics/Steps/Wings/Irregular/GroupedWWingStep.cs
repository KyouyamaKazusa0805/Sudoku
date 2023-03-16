namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Grouped W-Wing</b> technique.
/// </summary>
public sealed class GroupedWWingStep(Conclusion[] Conclusions, View[]? Views, int startCell, int endCell, scoped in CellMap bridge) :
	IrregularWingStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.5M;

	/// <summary>
	/// Indicates the start cell.
	/// </summary>
	public int StartCell { get; } = startCell;

	/// <summary>
	/// Indicates the end cell.
	/// </summary>
	public int EndCell { get; } = endCell;

	/// <inheritdoc/>
	public override Technique Code => Technique.GroupedWWing;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <summary>
	/// Indicates the bridge cells connecting with cells <see cref="StartCell"/> and <see cref="EndCell"/>.
	/// </summary>
	public CellMap Bridge { get; } = bridge;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { StartCellStr, EndCellStr, BridgeStr } },
			{ "zh", new[] { StartCellStr, EndCellStr, BridgeStr } }
		};

	private string StartCellStr => RxCyNotation.ToCellString(StartCell);

	private string EndCellStr => RxCyNotation.ToCellString(EndCell);

	private string BridgeStr => RxCyNotation.ToCellsString(Bridge);
}
