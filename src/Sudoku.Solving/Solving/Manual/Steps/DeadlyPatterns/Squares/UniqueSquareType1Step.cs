using Sudoku.Collections;
using Sudoku.Data;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Square Type 1</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="Candidate">Indicates the true candidate.</param>
public sealed record UniqueSquareType1Step(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	in Cells Cells,
	short DigitsMask,
	int Candidate
) : UniqueSquareStep(Conclusions, Views, Cells, DigitsMask)
{
	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.UniqueSquareType1;

	[FormatItem]
	private string CandidateStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Candidates { Candidate }.ToString();
	}
}
