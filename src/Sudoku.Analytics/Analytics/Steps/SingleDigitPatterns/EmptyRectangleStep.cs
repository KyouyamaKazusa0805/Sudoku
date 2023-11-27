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
/// Provides with a step that is an <b>Empty Rectangle</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
/// <param name="block">Indicates the block that the real empty rectangle pattern lis in.</param>
/// <param name="conjugatePair">Indicates the conjugate pair used.</param>
/// <param name="emptyRectangleCellsCount">Indicates the number of empty rectangle cells.</param>
public sealed partial class EmptyRectangleStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Digit digit,
	[Data] House block,
	[Data] scoped ref readonly Conjugate conjugatePair,
	[Data(DataMemberKinds.Field, Accessibility = "private readonly")] int emptyRectangleCellsCount
) : SingleDigitPatternStep(conclusions, views, options, digit)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.6M;

	/// <inheritdoc/>
	public override decimal BaseLocatingDifficulty => 460;

	/// <inheritdoc/>
	public override Technique Code => Technique.EmptyRectangle;

	/// <inheritdoc/>
	public override LocatingDifficultyFactor[] LocatingDifficultyFactors
		=> [
			new(LocatingDifficultyFactorNames.EmptyRectangleCellsCount, (5 - _emptyRectangleCellsCount) * 27),
			new(LocatingDifficultyFactorNames.Digit, Digit * 3),
			new(LocatingDifficultyFactorNames.ConjugatePair, HotSpot.GetHotSpot(ConjugatePair.Houses.SetAt(0)) * 9)
		];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [DigitStr, HouseStr, ConjStr]), new(ChineseLanguage, [DigitStr, HouseStr, ConjStr])];

	private string DigitStr => Options.Converter.DigitConverter((Mask)(1 << Digit));

	private string HouseStr => Options.Converter.HouseConverter(1 << Block);

	private string ConjStr => Options.Converter.ConjugateConverter([ConjugatePair]);
}
