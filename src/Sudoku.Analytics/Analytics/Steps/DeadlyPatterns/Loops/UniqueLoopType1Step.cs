namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Loop Type 1</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="loop"><inheritdoc/></param>
public sealed class UniqueLoopType1Step(Conclusion[] conclusions, View[]? views, Digit digit1, Digit digit2, scoped in CellMap loop) :
	UniqueLoopStep(conclusions, views, digit1, digit2, loop)
{
	/// <inheritdoc/>
	public override int Type => 1;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [Digit1Str, Digit2Str, LoopStr]), new(ChineseLanguage, [Digit1Str, Digit2Str, LoopStr])];
}
