namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle External Almost Locked Sets XZ Rule</b> technique.
/// </summary>
public sealed class UniqueRectangleExternalAlmostLockedSetsXzStep(
	Conclusion[] conclusions,
	View[]? views,
	int digit1,
	int digit2,
	scoped in CellMap cells,
	scoped in CellMap guardianCells,
	AlmostLockedSet almostLockedSet,
	bool isIncomplete,
	bool isAvoidable,
	int absoluteOffset
) : UniqueRectangleStep(
	conclusions,
	views,
	isAvoidable ? Technique.AvoidableRectangleExternalAlmostLockedSetsXz : Technique.UniqueRectangleExternalAlmostLockedSetsXz,
	digit1,
	digit2,
	cells,
	isAvoidable,
	absoluteOffset
)
{
	/// <summary>
	/// Indicates whether the rectangle is incomplete.
	/// </summary>
	public bool IsIncomplete { get; } = isIncomplete;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <summary>
	/// Indicates the cells that the guardians lie in.
	/// </summary>
	public CellMap GuardianCells { get; } = guardianCells;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[]
		{
			new(ExtraDifficultyCaseNames.Guardian, A004526(GuardianCells.Count) * .1M),
			new(ExtraDifficultyCaseNames.Avoidable, IsAvoidable ? .1M : 0),
			new(ExtraDifficultyCaseNames.Incompleteness, IsIncomplete ? .1M : 0)
		};

	/// <summary>
	/// The almost locked sets used.
	/// </summary>
	public AlmostLockedSet AlmostLockedSet { get; } = almostLockedSet;

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
