using System.SourceGeneration;
using Sudoku.Analytics.Configuration;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Text;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Loop Type 2</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="loop"><inheritdoc/></param>
/// <param name="extraDigit">Indicates the extra digit used.</param>
public sealed partial class UniqueLoopType2Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Digit digit1,
	Digit digit2,
	scoped ref readonly CellMap loop,
	[DataMember] Digit extraDigit
) : UniqueLoopStep(conclusions, views, options, digit1, digit2, in loop)
{
	/// <inheritdoc/>
	public override int Type => 2;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .1M;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [Digit1Str, Digit2Str, LoopStr, ExtraDigitStr]),
			new(ChineseLanguage, [Digit1Str, Digit2Str, LoopStr, ExtraDigitStr])
		];

	private string ExtraDigitStr => Options.CoordinateConverter.DigitConverter((Mask)(1 << ExtraDigit));
}
