namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle External Turbot Fish</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="guardianCells">Indicates the cells that the guardians lie in.</param>
/// <param name="cellPair">Indicates the cell pair.</param>
/// <param name="turbotFishDigit">Indicates the digit that the turbot fish used.</param>
/// <param name="baseHouse">Indicates the base house used.</param>
/// <param name="targetHouse">Indicates the target house used.</param>
/// <param name="isIncomplete">Indicates whether the rectangle is incomplete.</param>
/// <param name="isAvoidable"><inheritdoc/></param>
/// <param name="absoluteOffset"><inheritdoc/></param>
public sealed partial class UniqueRectangleExternalTurbotFishStep(
	Conclusion[] conclusions,
	View[]? views,
	Digit digit1,
	Digit digit2,
	scoped in CellMap cells,
	[PrimaryConstructorParameter] scoped in CellMap guardianCells,
	[PrimaryConstructorParameter] scoped in CellMap cellPair,
	[PrimaryConstructorParameter] Digit turbotFishDigit,
	[PrimaryConstructorParameter] House baseHouse,
	[PrimaryConstructorParameter] House targetHouse,
	[PrimaryConstructorParameter] bool isIncomplete,
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
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .1M;

	/// <inheritdoc/>
	public override string Format => GetString("TechniqueFormat_UniqueRectangleStep")!;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

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
			{ EnglishLanguage, new[] { D1Str, D2Str, CellsStr, GuardianCellsStr, CellPairStr, TurbotFishDigitStr } },
			{ ChineseLanguage, new[] { D1Str, D2Str, CellsStr, GuardianCellsStr, CellPairStr, TurbotFishDigitStr } }
		};

	private string TurbotFishDigitStr => (TurbotFishDigit + 1).ToString();

	private string GuardianCellsStr => GuardianCells.ToString();

	private string CellPairStr => CellPair.ToString();
}
