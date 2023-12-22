using System.SourceGeneration;
using Sudoku.Analytics.Configuration;
using Sudoku.Linq;
using Sudoku.Rendering;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Type 1</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="is2LinesWith2Cells"><inheritdoc/></param>
/// <param name="houses"><inheritdoc/></param>
/// <param name="corner1"><inheritdoc/></param>
/// <param name="corner2"><inheritdoc/></param>
/// <param name="targetCell">Indicates the target cell.</param>
/// <param name="targetDigits">Indicates the target digits.</param>
public sealed partial class QiuDeadlyPatternType1Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	bool is2LinesWith2Cells,
	HouseMask houses,
	Cell? corner1,
	Cell? corner2,
	[Data] Cell targetCell,
	[Data] Mask targetDigits
) : QiuDeadlyPatternStep(conclusions, views, options, is2LinesWith2Cells, houses, corner1, corner2)
{
	/// <inheritdoc/>
	public override int Type => 1;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [PatternStr, TargetDigitsStr, CandidatesStr]), new(ChineseLanguage, [PatternStr, CandidatesStr, TargetDigitsStr])];

	private string CandidatesStr
		=> Options.Converter.CandidateConverter([.. from digit in TargetDigits select TargetCell * 9 + digit]);

	private string TargetDigitsStr => Options.Converter.DigitConverter(TargetDigits);
}
