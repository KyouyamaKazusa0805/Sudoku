namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle External Type 3</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="GuardianCells">Indicates the cells that the guardians lie in.</param>
/// <param name="SubsetCells">The extra cells that forms the subset.</param>
/// <param name="SubsetDigitsMask">Indicates the digits that the subset are used.</param>
/// <param name="IsIncomplete">Indicates whether the rectangle is incomplete.</param>
/// <param name="IsAvoidable">Indicates whether the structure is based on avoidable rectangle.</param>
/// <param name="AbsoluteOffset"><inheritdoc/></param>
internal sealed record UniqueRectangleExternalType3Step(
	ConclusionList Conclusions,
	ViewList Views,
	int Digit1,
	int Digit2,
	scoped in CellMap Cells,
	scoped in CellMap GuardianCells,
	scoped in CellMap SubsetCells,
	short SubsetDigitsMask,
	bool IsIncomplete,
	bool IsAvoidable,
	int AbsoluteOffset
) : UniqueRectangleStep(
	Conclusions,
	Views,
	IsAvoidable ? Technique.AvoidableRectangleExternalType3 : Technique.UniqueRectangleExternalType3,
	Digit1,
	Digit2,
	Cells,
	false,
	AbsoluteOffset
)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.6M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.UniqueRectanglePlus;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.HardlyEver;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[]
		{
			new(ExtraDifficultyCaseNames.ExtraDigit, PopCount((uint)SubsetDigitsMask) * .1M),
			new(ExtraDifficultyCaseNames.Avoidable, IsAvoidable ? .1M : 0),
			new(ExtraDifficultyCaseNames.Incompleteness, IsIncomplete ? .1M : 0)
		};

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { D1Str, D2Str, CellsStr, SubsetCellsStr, DigitsStr } },
			{ "zh", new[] { D1Str, D2Str, CellsStr, DigitsStr, SubsetCellsStr } }
		};

	private string DigitsStr => DigitMaskFormatter.Format(SubsetDigitsMask, FormattingMode.Normal);

	private string SubsetCellsStr => SubsetCells.ToString();
}
