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
/// Provides with a step that is a <b>Bi-value Universal Grave Type 4</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digitsMask">Indicates the mask of digits used.</param>
/// <param name="cells">Indicates the cells used.</param>
/// <param name="emptyCellsCount">The number of empty cells.</param>
/// <param name="conjugatePair">Indicates the conjugate pair used.</param>
public sealed partial class BivalueUniversalGraveType4Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[Data] Mask digitsMask,
	[Data] scoped ref readonly CellMap cells,
	[Data(DataMemberKinds.Field, Accessibility = "private readonly")] int emptyCellsCount,
	[Data] scoped ref readonly Conjugate conjugatePair
) : BivalueUniversalGraveStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override Technique Code => Technique.BivalueUniversalGraveType4;

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors => [new(ExtraDifficultyFactorNames.ConjugatePair, .1M)];

	/// <inheritdoc/>
	public override LocatingDifficultyFactor[] LocatingDifficultyFactors
		=> [
			new(LocatingDifficultyFactorNames.EmptyCell, 560 * Math.Round(_emptyCellsCount / 11M, 2)),
			new(LocatingDifficultyFactorNames.TrueCandidate, Cells.Count * 60),
			new(
				LocatingDifficultyFactorNames.ConjugatePair,
				9 * ConjugatePair.Houses.SetAt(0).ToHouseType() switch { HouseType.Block => 1, HouseType.Row => 3, HouseType.Column => 6 }
			)
		];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [DigitsStr, CellsStr, ConjStr]), new(ChineseLanguage, [CellsStr, DigitsStr, ConjStr])];

	private string DigitsStr => Options.Converter.DigitConverter(DigitsMask);

	private string CellsStr => Options.Converter.CellConverter(Cells);

	private string ConjStr => Options.Converter.ConjugateConverter([ConjugatePair]);
}
