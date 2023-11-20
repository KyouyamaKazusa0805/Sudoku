using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Concepts;
using Sudoku.Rendering;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Firework Pair Type 2</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digitsMask">Indicates the mask of digits used.</param>
/// <param name="pattern1Map">Indicates the map for pattern 1.</param>
/// <param name="pattern1Pivot">Indicates the pivot cell for pattern 1. <see langword="null"/> when the pattern is a quadruple.</param>
/// <param name="pattern2Map">Indicates the map for pattern 2.</param>
/// <param name="pattern2Pivot">Indicates the pivot cell for pattern 2. <see langword="null"/> when the pattern is a quadruple.</param>
/// <param name="extraCell">Indicates the extra cell used.</param>
public sealed partial class FireworkPairType2Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[Data] Mask digitsMask,
	[Data] scoped ref readonly CellMap pattern1Map,
	[Data] Cell? pattern1Pivot,
	[Data] scoped ref readonly CellMap pattern2Map,
	[Data] Cell? pattern2Pivot,
	[Data] Cell extraCell
) : FireworkStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .3M;

	/// <inheritdoc/>
	public override Technique Code => Technique.FireworkPairType2;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [Firework1Str, Firework2Str, DigitsStr, ExtraCellStr]),
			new(ChineseLanguage, [Firework1Str, Firework2Str, DigitsStr, ExtraCellStr])
		];

	private string ExtraCellStr => Options.Converter.CellConverter([ExtraCell]);

	private string DigitsStr => Options.Converter.DigitConverter(DigitsMask);

	private string Firework1Str => $"{Options.Converter.CellConverter(Pattern1Map)}({DigitsStr})";

	private string Firework2Str => $"{Options.Converter.CellConverter(Pattern2Map)}({DigitsStr})";
}
