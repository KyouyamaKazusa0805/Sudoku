using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Rendering;
using Sudoku.Text;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave False Candidate Type</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="falseCandidate">Indicates the false candidate that will cause a BUG deadly pattern if it is true.</param>
public sealed partial class BivalueUniversalGraveFalseCandidateTypeStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[DataMember] Candidate falseCandidate
) : BivalueUniversalGraveStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .1M;

	/// <inheritdoc/>
	public override Technique Code => Technique.BivalueUniversalGraveFalseCandidateType;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [FalseCandidateStr]), new(ChineseLanguage, [FalseCandidateStr])];

	private string FalseCandidateStr => Options.CoordinateConverter.CandidateConverter([FalseCandidate]);
}
