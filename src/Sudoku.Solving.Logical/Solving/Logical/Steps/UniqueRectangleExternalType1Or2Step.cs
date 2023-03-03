namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle External Type 1/2</b>
/// or <b>Avoidable Rectangle External Type 1/2</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="GuardianCells">Indicates the cells that the guardians lie in.</param>
/// <param name="GuardianDigit">Indicates the digit that the guardians are used.</param>
/// <param name="IsIncomplete">Indicates whether the rectangle is incomplete.</param>
/// <param name="IsAvoidable">Indicates whether the structure is based on avoidable rectangle.</param>
/// <param name="AbsoluteOffset"><inheritdoc/></param>
internal sealed record UniqueRectangleExternalType1Or2Step(
	ConclusionList Conclusions,
	ViewList Views,
	int Digit1,
	int Digit2,
	scoped in CellMap Cells,
	scoped in CellMap GuardianCells,
	int GuardianDigit,
	bool IsIncomplete,
	bool IsAvoidable,
	int AbsoluteOffset
) : UniqueRectangleStep(
	Conclusions,
	Views,
	(IsAvoidable, GuardianCells.Count == 1) switch
	{
		(true, true) => Technique.AvoidableRectangleExternalType1,
		(true, false) => Technique.AvoidableRectangleExternalType2,
		(false, true) => Technique.UniqueRectangleExternalType1,
		_ => Technique.UniqueRectangleExternalType2
	},
	Digit1,
	Digit2,
	Cells,
	false,
	AbsoluteOffset
)
{
	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.UniqueRectanglePlus;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Often;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[]
		{
			new(ExtraDifficultyCaseNames.Guardian, A004526(GuardianCells.Count) * .1M),
			new(ExtraDifficultyCaseNames.Avoidable, IsAvoidable ? .1M : 0),
			new(ExtraDifficultyCaseNames.Incompleteness, IsIncomplete ? .1M : 0)
		};

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { D1Str, D2Str, CellsStr, GuardianDigitStr, GuardianCellsStr } },
			{ "zh", new[] { D1Str, D2Str, CellsStr, GuardianDigitStr, GuardianCellsStr } }
		};

	private string GuardianDigitStr => (GuardianDigit + 1).ToString();

	private string GuardianCellsStr => GuardianCells.ToString();
}
