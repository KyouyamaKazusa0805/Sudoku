namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle External Type 1/2</b> or <b>Avoidable Rectangle External Type 1/2</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="guardianCells">Indicates the cells that the guardians lie in.</param>
/// <param name="guardianDigit">Indicates the digit that the guardians are used.</param>
/// <param name="isIncomplete">Indicates whether the rectangle is incomplete.</param>
/// <param name="isAvoidable"><inheritdoc/></param>
/// <param name="absoluteOffset"><inheritdoc/></param>
public sealed partial class UniqueRectangleExternalType1Or2Step(
	Conclusion[] conclusions,
	View[]? views,
	Digit digit1,
	Digit digit2,
	scoped in CellMap cells,
	[PrimaryConstructorParameter] scoped in CellMap guardianCells,
	[PrimaryConstructorParameter] Digit guardianDigit,
	[PrimaryConstructorParameter] bool isIncomplete,
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
			{ EnglishLanguage, new[] { D1Str, D2Str, CellsStr, GuardianDigitStr, GuardianCellsStr } },
			{ ChineseLanguage, new[] { D1Str, D2Str, CellsStr, GuardianDigitStr, GuardianCellsStr } }
		};

	private string GuardianDigitStr => (GuardianDigit + 1).ToString();

	private string GuardianCellsStr => GuardianCells.ToString();
}
