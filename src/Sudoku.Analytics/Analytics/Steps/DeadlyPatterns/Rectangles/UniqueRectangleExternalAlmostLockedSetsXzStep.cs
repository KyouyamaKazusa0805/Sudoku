namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle External Almost Locked Sets XZ Rule</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="guardianCells">Indicates the cells that the guardians lie in.</param>
/// <param name="almostLockedSet">Indicates the almost locked set pattern used.</param>
/// <param name="isIncomplete">Indicates whether the rectangle is incomplete.</param>
/// <param name="isAvoidable"><inheritdoc/></param>
/// <param name="absoluteOffset"><inheritdoc/></param>
public sealed partial class UniqueRectangleExternalAlmostLockedSetsXzStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Digit digit1,
	Digit digit2,
	scoped ref readonly CellMap cells,
	[Data] scoped ref readonly CellMap guardianCells,
	[Data] AlmostLockedSet almostLockedSet,
	[Data] bool isIncomplete,
	bool isAvoidable,
	int absoluteOffset
) : UniqueRectangleStep(
	conclusions,
	views,
	options,
	isAvoidable ? Technique.AvoidableRectangleExternalAlmostLockedSetsXz : Technique.UniqueRectangleExternalAlmostLockedSetsXz,
	digit1,
	digit2,
	in cells,
	isAvoidable,
	absoluteOffset
)
{
	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors
		=> [
			new(ExtraDifficultyFactorNames.Guardian, A004526(GuardianCells.Count) * .1M),
			new(ExtraDifficultyFactorNames.Avoidable, IsAvoidable ? .1M : 0),
			new(ExtraDifficultyFactorNames.Incompleteness, IsIncomplete ? .1M : 0)
		];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [D1Str, D2Str, CellsStr, GuardianCellsStr, AnotherAlsStr]),
			new(ChineseLanguage, [D1Str, D2Str, CellsStr, GuardianCellsStr, AnotherAlsStr])
		];

	private string GuardianCellsStr => Options.Converter.CellConverter(GuardianCells);

	private string AnotherAlsStr => AlmostLockedSet.ToString(Options.Converter);
}
