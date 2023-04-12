namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle with Unknown Covering</b> technique.
/// </summary>
public sealed class UniqueRectangleWithBabaGroupingStep(
	Conclusion[] conclusions,
	View[]? views,
	int digit1,
	int digit2,
	scoped in CellMap cells,
	int targetCell,
	int extraDigit,
	int absoluteOffset
) : UniqueRectangleStep(
	conclusions,
	views,
	Technique.UniqueRectangleBabaGrouping,
	digit1,
	digit2,
	cells,
	false,
	absoluteOffset
)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .4M;

	/// <summary>
	/// Indicates the target cell.
	/// </summary>
	public int TargetCell { get; } = targetCell;

	/// <summary>
	/// Indicates the extra digit used.
	/// </summary>
	public int ExtraDigit { get; } = extraDigit;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { D1Str, D2Str, CellsStr, TargetCellStr, DigitsStr, ExtraDigitStr } },
			{ "zh", new[] { D1Str, D2Str, CellsStr, TargetCellStr, DigitsStr, ExtraDigitStr } }
		};

	private string TargetCellStr => RxCyNotation.ToCellString(TargetCell);

	private string DigitsStr => DigitMaskFormatter.Format((Mask)(1 << Digit1 | 1 << Digit2), R["OrKeywordWithSpaces"]!);

	private string ExtraDigitStr => (ExtraDigit + 1).ToString();
}
