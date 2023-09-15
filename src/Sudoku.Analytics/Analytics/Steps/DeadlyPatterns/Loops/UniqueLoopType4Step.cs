using System.SourceGeneration;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Text;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Loop Type 4</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="loop"><inheritdoc/></param>
/// <param name="conjugatePair">Indicates the conjugate pair used.</param>
public sealed partial class UniqueLoopType4Step(
	Conclusion[] conclusions,
	View[]? views,
	Digit digit1,
	Digit digit2,
	scoped ref readonly CellMap loop,
	[DataMember] scoped ref readonly Conjugate conjugatePair
) : UniqueLoopStep(conclusions, views, digit1, digit2, loop)
{
	/// <inheritdoc/>
	public override int Type => 4;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .1M;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [Digit1Str, Digit2Str, LoopStr, ConjStr]), new(ChineseLanguage, [Digit1Str, Digit2Str, LoopStr, ConjStr])];

	private string ConjStr => ConjugatePair.ToString();
}
