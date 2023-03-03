namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle 2D (or 3X)</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="TechniqueCode2"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="IsAvoidable"><inheritdoc/></param>
/// <param name="XDigit">Indicates the digit X.</param>
/// <param name="YDigit">Indicates the digit Y.</param>
/// <param name="XyCell">Indicates the cell XY.</param>
/// <param name="AbsoluteOffset"><inheritdoc/></param>
internal sealed record UniqueRectangle2DOr3XStep(
	ConclusionList Conclusions,
	ViewList Views,
	Technique TechniqueCode2,
	int Digit1,
	int Digit2,
	scoped in CellMap Cells,
	bool IsAvoidable,
	int XDigit,
	int YDigit,
	int XyCell,
	int AbsoluteOffset
) : UniqueRectangleStep(Conclusions, Views, TechniqueCode2, Digit1, Digit2, Cells, IsAvoidable, AbsoluteOffset)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .2M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.UniqueRectanglePlus;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.HardlyEver;

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
