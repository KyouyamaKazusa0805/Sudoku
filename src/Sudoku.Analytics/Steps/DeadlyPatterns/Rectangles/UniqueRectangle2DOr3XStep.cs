namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle 2D (or 3X)</b> technique.
/// </summary>
public sealed class UniqueRectangle2DOr3XStep(
	Conclusion[] conclusions,
	View[]? views,
	Technique code,
	int digit1,
	int digit2,
	scoped in CellMap cells,
	bool isAvoidable,
	int xDigit,
	int yDigit,
	int xyCell,
	int absoluteOffset
) : UniqueRectangleStep(conclusions, views, code, digit1, digit2, cells, isAvoidable, absoluteOffset)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .2M;

	/// <summary>
	/// Indicates the digit X defined in this pattern.
	/// </summary>
	public int XDigit { get; } = xDigit;

	/// <summary>
	/// Indicates the digit Y defined in this pattern.
	/// </summary>
	public int YDigit { get; } = yDigit;

	/// <summary>
	/// Indicates a bi-value cell that only contains digit X and Y.
	/// </summary>
	public int XyCell { get; } = xyCell;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override TechniqueGroup Group => TechniqueGroup.UniqueRectanglePlus;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { D1Str, D2Str, CellsStr, XDigitStr, YDigitStr, XYCellsStr } },
			{ "zh", new[] { D1Str, D2Str, CellsStr, XDigitStr, YDigitStr, XYCellsStr } }
		};

	private string XDigitStr => (XDigit + 1).ToString();

	private string YDigitStr => (YDigit + 1).ToString();

	private string XYCellsStr => RxCyNotation.ToCellString(XyCell);
}
