using System.SourceGeneration;
using Sudoku.DataModel;
using Sudoku.Rendering;
using Sudoku.Text;
using Sudoku.Text.Notation;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Type 1</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="pattern"><inheritdoc/></param>
/// <param name="candidate">Indicates the target candidate.</param>
public sealed partial class QiuDeadlyPatternType1Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped in QiuDeadlyPattern pattern,
	[DataMember] Candidate candidate
) : QiuDeadlyPatternStep(conclusions, views, pattern)
{
	/// <inheritdoc/>
	public override int Type => 1;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [PatternStr, CandidateStr]), new(ChineseLanguage, [CandidateStr, PatternStr])];

	private string CandidateStr => CandidateNotation.ToString(Candidate);
}
