namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle Type 3</b> technique.
/// </summary>
public sealed class UniqueRectangleType3Step(
	Conclusion[] conclusions,
	View[]? views,
	int digit1,
	int digit2,
	scoped in CellMap cells,
	scoped in CellMap extraCells,
	Mask extraDigitsMask,
	int house,
	bool isAvoidable,
	int absoluteOffset,
	bool isNaked = true
) : UniqueRectangleStep(
	conclusions,
	views,
	isAvoidable ? Technique.AvoidableRectangleType3 : Technique.UniqueRectangleType3,
	digit1,
	digit2,
	cells,
	isAvoidable,
	absoluteOffset
)
{
	/// <summary>
	/// Indicates whether the subset is naked subset. If <see langword="true"/>, a naked subset; otherwise, a hidden subset.
	/// </summary>
	public bool IsNaked { get; } = isNaked;

	/// <summary>
	/// Indicates the house used.
	/// </summary>
	public int House { get; } = house;

	/// <summary>
	/// Indicates the mask that contains all extra digits used.
	/// </summary>
	public Mask ExtraDigitsMask { get; } = extraDigitsMask;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <summary>
	/// Indicates the extra cells used, forming the subset.
	/// </summary>
	public CellMap ExtraCells { get; } = extraCells;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new[]
		{
			(ExtraDifficultyCaseNames.Hidden, IsNaked ? 0 : .1M),
			(ExtraDifficultyCaseNames.Size, PopCount((uint)ExtraDigitsMask) * .1M)
		};

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { D1Str, D2Str, CellsStr, DigitsStr, OnlyKeyword, CellsStr, HouseStr } },
			{ "zh", new[] { D1Str, D2Str, CellsStr, DigitsStr, OnlyKeywordZhCn, HouseStr, CellsStr, AppearLimitKeyword } }
		};

	private string DigitsStr => DigitMaskFormatter.Format(ExtraDigitsMask, FormattingMode.Normal);

	private string OnlyKeyword => IsNaked ? string.Empty : "only ";

	private string OnlyKeywordZhCn => R["Only"]!;

	private string HouseStr => HouseFormatter.Format(1 << House);

	private string AppearLimitKeyword => R["Appear"]!;
}
