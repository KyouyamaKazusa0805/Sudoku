namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle Type 3</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="extraCells">Indicates the extra cells used, forming the subset.</param>
/// <param name="extraDigitsMask">Indicates the mask that contains all extra digits used.</param>
/// <param name="house">Indicates the house used.</param>
/// <param name="isAvoidable"><inheritdoc/></param>
/// <param name="absoluteOffset"><inheritdoc/></param>
/// <param name="isNaked">
/// Indicates whether the subset is naked subset. If <see langword="true"/>, a naked subset; otherwise, a hidden subset.
/// </param>
public sealed partial class UniqueRectangleType3Step(
	Conclusion[] conclusions,
	View[]? views,
	Digit digit1,
	Digit digit2,
	scoped in CellMap cells,
	[PrimaryConstructorParameter] scoped in CellMap extraCells,
	[PrimaryConstructorParameter] Mask extraDigitsMask,
	[PrimaryConstructorParameter] House house,
	bool isAvoidable,
	int absoluteOffset,
	[PrimaryConstructorParameter] bool isNaked = true
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
	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> [
			(ExtraDifficultyCaseNames.Hidden, IsNaked ? 0 : .1M),
			(ExtraDifficultyCaseNames.Size, PopCount((uint)ExtraDigitsMask) * .1M)
		];

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ EnglishLanguage, [D1Str, D2Str, CellsStr, DigitsStr, OnlyKeyword, CellsStr, HouseStr] },
			{ ChineseLanguage, [D1Str, D2Str, CellsStr, DigitsStr, OnlyKeywordZhCn, HouseStr, CellsStr, AppearLimitKeyword] }
		};

	private string DigitsStr => DigitMaskFormatter.Format(ExtraDigitsMask, FormattingMode.Normal);

	private string OnlyKeyword => IsNaked ? string.Empty : "only ";

	private string OnlyKeywordZhCn => GetString("Only")!;

	private string HouseStr => HouseFormatter.Format(1 << House);

	private string AppearLimitKeyword => GetString("Appear")!;
}
