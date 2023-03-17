namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle External Turbot Fish</b> technique.
/// </summary>
public sealed class UniqueRectangleExternalTurbotFishStep(
	Conclusion[] conclusions,
	View[]? views,
	int digit1,
	int digit2,
	scoped in CellMap cells,
	scoped in CellMap guardianCells,
	scoped in CellMap cellPair,
	int turbotFishDigit,
	int baseHouse,
	int targetHouse,
	bool isIncomplete,
	bool isAvoidable,
	int absoluteOffset
) : UniqueRectangleStep(
	conclusions,
	views,
	(isAvoidable, baseHouse / 9, targetHouse / 9) switch
	{
		(true, 0, _) or (true, _, 0) => Technique.AvoidableRectangleExternalTurbotFish,
		(true, 1, 1) or (true, 2, 2) => Technique.AvoidableRectangleExternalSkyscraper,
		(true, 1, 2) or (true, 2, 1) => Technique.AvoidableRectangleExternalTwoStringKite,
		(false, 0, _) or (false, _, 0) => Technique.UniqueRectangleExternalTurbotFish,
		(false, 1, 1) or (false, 2, 2) => Technique.UniqueRectangleExternalSkyscraper,
		(false, 1, 2) or (false, 2, 1) => Technique.UniqueRectangleExternalTwoStringKite
	},
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
	public override decimal BaseDifficulty => base.BaseDifficulty + .1M;

	/// <summary>
	/// Indicates the digit that the turbot fish used.
	/// </summary>
	public int TurbotFishDigit { get; } = turbotFishDigit;

	/// <summary>
	/// Indicates the base house used.
	/// </summary>
	public int BaseHouse { get; } = baseHouse;

	/// <summary>
	/// Indicates the target house used.
	/// </summary>
	public int TargetHouse { get; } = targetHouse;

	/// <inheritdoc/>
	public override string Format => R["TechniqueFormat_UniqueRectangleStep"]!;

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
		=> new ExtraDifficultyCase[]
		{
			new(ExtraDifficultyCaseNames.Guardian, A004526(GuardianCells.Count) * .1M),
			new(ExtraDifficultyCaseNames.Avoidable, IsAvoidable ? .1M : 0),
			new(ExtraDifficultyCaseNames.Incompleteness, isIncomplete ? .1M : 0)
		};

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { D1Str, D2Str, CellsStr, GuardianCellsStr, CellPairStr, TurbotFishDigitStr } },
			{ "zh", new[] { D1Str, D2Str, CellsStr, GuardianCellsStr, CellPairStr, TurbotFishDigitStr } }
		};

	private string TurbotFishDigitStr => (TurbotFishDigit + 1).ToString();

	private string GuardianCellsStr => GuardianCells.ToString();

	private string CellPairStr => CellPair.ToString();
}
