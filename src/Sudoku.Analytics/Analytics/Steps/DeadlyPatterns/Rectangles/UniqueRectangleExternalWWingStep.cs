using System.Algorithm;
using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Rating;
using Sudoku.Rendering;
using Sudoku.Text;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle External W-Wing</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="guardianCells">Indicates the cells that the guardians lie in.</param>
/// <param name="cellPair">Indicates the cell pair.</param>
/// <param name="isIncomplete">Indicates whether the rectangle is incomplete.</param>
/// <param name="isAvoidable"><inheritdoc/></param>
/// <param name="absoluteOffset"><inheritdoc/></param>
public sealed partial class UniqueRectangleExternalWWingStep(
	Conclusion[] conclusions,
	View[]? views,
	Digit digit1,
	Digit digit2,
	scoped in CellMap cells,
	[DataMember] scoped in CellMap guardianCells,
	[DataMember] scoped in CellMap cellPair,
	[DataMember] bool isIncomplete,
	bool isAvoidable,
	int absoluteOffset
) : UniqueRectangleStep(
	conclusions,
	views,
	isAvoidable ? Technique.AvoidableRectangleExternalWWing : Technique.UniqueRectangleExternalWWing,
	digit1,
	digit2,
	cells,
	isAvoidable,
	absoluteOffset
)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .3M;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> [
			new(ExtraDifficultyCaseNames.Guardian, Sequences.A004526(GuardianCells.Count) * .1M),
			new(ExtraDifficultyCaseNames.Avoidable, IsAvoidable ? .1M : 0),
			new(ExtraDifficultyCaseNames.Incompleteness, IsIncomplete ? .1M : 0)
		];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [D1Str, D2Str, CellsStr, GuardianCellsStr, CellPairStr]),
			new(ChineseLanguage, [D1Str, D2Str, CellsStr, GuardianCellsStr, CellPairStr])
		];

	private string GuardianCellsStr => GuardianCells.ToString();

	private string CellPairStr => CellPair.ToString();
}
