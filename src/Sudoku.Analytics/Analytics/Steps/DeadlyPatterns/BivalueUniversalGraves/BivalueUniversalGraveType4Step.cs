using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Text;
using Sudoku.Text.Notation;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave Type 4</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="digitsMask">Indicates the mask of digits used.</param>
/// <param name="cells">Indicates the cells used.</param>
/// <param name="conjugatePair">Indicates the conjugate pair used.</param>
public sealed partial class BivalueUniversalGraveType4Step(
	Conclusion[] conclusions,
	View[]? views,
	[DataMember] Mask digitsMask,
	[DataMember] scoped in CellMap cells,
	[DataMember] scoped in Conjugate conjugatePair
) : BivalueUniversalGraveStep(conclusions, views)
{
	/// <inheritdoc/>
	public override Technique Code => Technique.BivalueUniversalGraveType4;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => [new(ExtraDifficultyCaseNames.ConjugatePair, .1M)];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [DigitsStr, CellsStr, ConjStr]), new(ChineseLanguage, [CellsStr, DigitsStr, ConjStr])];

	private string DigitsStr => DigitNotation.ToString(DigitsMask);

	private string CellsStr => Cells.ToString();

	private string ConjStr => ConjugatePair.ToString();
}
