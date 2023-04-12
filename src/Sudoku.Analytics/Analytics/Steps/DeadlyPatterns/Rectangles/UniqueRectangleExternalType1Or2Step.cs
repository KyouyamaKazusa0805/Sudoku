namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle External Type 1/2</b>
/// or <b>Avoidable Rectangle External Type 1/2</b> technique.
/// </summary>
public sealed class UniqueRectangleExternalType1Or2Step(
	Conclusion[] conclusions,
	View[]? views,
	int digit1,
	int digit2,
	scoped in CellMap cells,
	scoped in CellMap guardianCells,
	int guardianDigit,
	bool IsIncomplete,
	bool isAvoidable,
	int absoluteOffset
) : UniqueRectangleStep(
	conclusions,
	views,
	(isAvoidable, guardianCells.Count == 1) switch
	{
		(true, true) => Technique.AvoidableRectangleExternalType1,
		(true, false) => Technique.AvoidableRectangleExternalType2,
		(false, true) => Technique.UniqueRectangleExternalType1,
		_ => Technique.UniqueRectangleExternalType2
	},
	digit1,
	digit2,
	cells,
	false,
	absoluteOffset
)
{
	/// <summary>
	/// Indicates whether the rectangle is incomplete.
	/// </summary>
	public bool IsIncomplete { get; } = IsIncomplete;

	/// <summary>
	/// Indicates the digit that the guardians are used.
	/// </summary>
	public int GuardianDigit { get; } = guardianDigit;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <summary>
	/// Indicates the cells that the guardians lie in.
	/// </summary>
	public CellMap GuardianCells { get; } = guardianCells;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new[]
		{
			(ExtraDifficultyCaseNames.Guardian, A004526(GuardianCells.Count) * .1M),
			(ExtraDifficultyCaseNames.Avoidable, IsAvoidable ? .1M : 0),
			(ExtraDifficultyCaseNames.Incompleteness, IsIncomplete ? .1M : 0)
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
