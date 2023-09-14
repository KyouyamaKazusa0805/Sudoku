using Sudoku.Analytics.Categorization;
using Sudoku.Text;
using Sudoku.Text.Notation;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Pattern Overlay</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
public sealed class PatternOverlayStep(Conclusion[] conclusions) : LastResortStep(conclusions, null)
{
	/// <summary>
	/// Indicates the digit.
	/// </summary>
	public Digit Digit => Conclusions[0].Digit;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => 8.5M;

	/// <inheritdoc/>
	public override Technique Code => Technique.PatternOverlay;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts => [new(EnglishLanguage, [DigitStr]), new(ChineseLanguage, [DigitStr])];

	private string DigitStr => DigitNotation.ToString(Digit);
}
