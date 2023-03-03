namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle External Almost Locked Sets XZ Rule</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="GuardianCells">Indicates the cells that the guardians lie in.</param>
/// <param name="AlmostLockedSet">The almost locked sets used.</param>
/// <param name="IsIncomplete">Indicates whether the rectangle is incomplete.</param>
/// <param name="IsAvoidable">Indicates whether the structure is based on avoidable rectangle.</param>
/// <param name="AbsoluteOffset"><inheritdoc/></param>
internal sealed record UniqueRectangleExternalAlmostLockedSetsXzStep(
	ConclusionList Conclusions,
	ViewList Views,
	int Digit1,
	int Digit2,
	scoped in CellMap Cells,
	scoped in CellMap GuardianCells,
	AlmostLockedSet AlmostLockedSet,
	bool IsIncomplete,
	bool IsAvoidable,
	int AbsoluteOffset
) : UniqueRectangleStep(
	Conclusions,
	Views,
	IsAvoidable ? Technique.AvoidableRectangleExternalAlmostLockedSetsXz : Technique.UniqueRectangleExternalAlmostLockedSetsXz,
	Digit1,
	Digit2,
	Cells,
	IsAvoidable,
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
			{ "en", new[] { D1Str, D2Str, CellsStr, GuardianCellsStr, AnotherAlsStr } },
			{ "zh", new[] { D1Str, D2Str, CellsStr, GuardianCellsStr, AnotherAlsStr } }
		};

	private string GuardianCellsStr => GuardianCells.ToString();

	private string AnotherAlsStr => AlmostLockedSet.ToString();
}
