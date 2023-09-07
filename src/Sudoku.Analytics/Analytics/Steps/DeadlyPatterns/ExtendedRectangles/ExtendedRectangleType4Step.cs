using System.SourceGeneration;
using Sudoku.Analytics.Rating;
using Sudoku.Rendering;
using Sudoku.Text;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Extended Rectangle Type 4</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="conjugatePair">Indicates the conjugate pair used.</param>
public sealed partial class ExtendedRectangleType4Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped in CellMap cells,
	Mask digitsMask,
	[DataMember] scoped in Conjugate conjugatePair
) : ExtendedRectangleStep(conclusions, views, cells, digitsMask)
{
	/// <inheritdoc/>
	public override int Type => 4;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => [.. base.ExtraDifficultyCases, new(ExtraDifficultyCaseNames.ConjugatePair, .1M)];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [DigitsStr, CellsStr, ConjStr]), new(ChineseLanguage, [DigitsStr, CellsStr, ConjStr])];

	private string ConjStr => ConjugatePair.ToString();
}
