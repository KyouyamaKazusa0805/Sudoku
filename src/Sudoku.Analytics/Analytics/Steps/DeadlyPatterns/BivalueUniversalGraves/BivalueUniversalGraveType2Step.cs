using System.Algorithm;
using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Rating;
using Sudoku.Rendering;
using Sudoku.Text;
using Sudoku.Text.Notation;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bivalue Universal Grave Type 2</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="digit">Indicates the extra digit.</param>
/// <param name="cells">Indicates the cells used.</param>
public sealed partial class BivalueUniversalGraveType2Step(
	Conclusion[] conclusions,
	View[]? views,
	[DataMember(GeneratedMemberName = "ExtraDigit")] Digit digit,
	[DataMember] scoped in CellMap cells
) : BivalueUniversalGraveStep(conclusions, views)
{
	/// <inheritdoc/>
	public override Technique Code => Technique.BivalueUniversalGraveType2;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => [new(ExtraDifficultyCaseNames.ExtraDigit, Sequences.A002024(Cells.Count) * .1M)];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [ExtraDigitStr, CellsStr]), new(ChineseLanguage, [CellsStr, ExtraDigitStr])];

	private string ExtraDigitStr => DigitNotation.ToString(ExtraDigit);

	private string CellsStr => Cells.ToString();
}
