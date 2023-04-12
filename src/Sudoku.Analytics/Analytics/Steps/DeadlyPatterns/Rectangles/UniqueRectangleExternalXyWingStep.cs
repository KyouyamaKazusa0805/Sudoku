namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle External XY-Wing</b> technique.
/// </summary>
public sealed class UniqueRectangleExternalXyWingStep(
	Conclusion[] conclusions,
	View[]? views,
	int digit1,
	int digit2,
	scoped in CellMap cells,
	scoped in CellMap guardianCells,
	scoped in CellMap cellPair,
	bool isIncomplete,
	bool isAvoidable,
	int absoluteOffset
) : UniqueRectangleStep(
	conclusions,
	views,
	isAvoidable ? Technique.AvoidableRectangleExternalXyWing : Technique.UniqueRectangleExternalXyWing,
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
	public override decimal BaseDifficulty => base.BaseDifficulty + .2M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <summary>
	/// Indicates the cells that the guardians lie in.
	/// </summary>
	public CellMap GuardianCells { get; } = guardianCells;

	/// <summary>
	/// Indicates the cell pair.
	/// </summary>
	public CellMap CellPair { get; } = cellPair;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new[]
		{
			(ExtraDifficultyCaseNames.Guardian, A004526(GuardianCells.Count) * .1M),
			(ExtraDifficultyCaseNames.Avoidable, IsAvoidable ? .1M : 0),
			(ExtraDifficultyCaseNames.Incompleteness, isIncomplete ? .1M : 0)
		};

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { D1Str, D2Str, CellsStr, GuardianCellsStr, CellPairStr } },
			{ "zh", new[] { D1Str, D2Str, CellsStr, GuardianCellsStr, CellPairStr } }
		};

	private string GuardianCellsStr => GuardianCells.ToString();

	private string CellPairStr => CellPair.ToString();
}
