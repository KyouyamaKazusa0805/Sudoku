using System.Numerics;
using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle Type 1</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="isAvoidable"><inheritdoc/></param>
/// <param name="absoluteOffset"><inheritdoc/></param>
/// <param name="cornerInterfererCandidatesCount">Indicates the interferer digits appeared in corner cell.</param>
/// <param name="emptyCellsCount">The number of empty cells.</param>
public sealed partial class UniqueRectangleType1Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Digit digit1,
	Digit digit2,
	scoped ref readonly CellMap cells,
	bool isAvoidable,
	int absoluteOffset,
	[Data(DataMemberKinds.Field, Accessibility = "private readonly")] int cornerInterfererCandidatesCount,
	[Data(DataMemberKinds.Field, Accessibility = "private readonly")] int emptyCellsCount
) : UniqueRectangleStep(
	conclusions,
	views,
	options,
	isAvoidable ? Technique.AvoidableRectangleType1 : Technique.UniqueRectangleType1,
	digit1,
	digit2,
	in cells,
	isAvoidable,
	absoluteOffset
)
{
	/// <inheritdoc/>
	public override decimal BaseLocatingDifficulty => 54;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [D1Str, D2Str, CellsStr]), new(ChineseLanguage, [D1Str, D2Str, CellsStr])];

	/// <inheritdoc/>
	public override LocatingDifficultyFactor[] LocatingDifficultyFactors
	{
		get
		{
			var blocks = Cells.BlockMask.GetAllSets();
			return [
				new(
					LocatingDifficultyFactorNames.HousePosition,
					(HotSpot.GetHotSpot(blocks[0]) + HotSpot.GetHotSpot(blocks[1])) * 9
				),
				new(LocatingDifficultyFactorNames.Interferer, _cornerInterfererCandidatesCount * 3),
				new(LocatingDifficultyFactorNames.Incompleteness, 60),
				new(LocatingDifficultyFactorNames.AvoidableRectangle, _emptyCellsCount * 60)
			];
		}
	}
}
