using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Solving.Collections;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Type 1</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Pattern"><inheritdoc/></param>
/// <param name="Candidate">Indicates the extra candidate used.</param>
public sealed record QiuDeadlyPatternType1Step(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	in QiuDeadlyPattern Pattern,
	int Candidate
) : QiuDeadlyPatternStep(Conclusions, Views, Pattern)
{
	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.QiuDeadlyPatternType1;

	[FormatItem]
	private string CandidateStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Candidates { Candidate }.ToString();
	}
}
