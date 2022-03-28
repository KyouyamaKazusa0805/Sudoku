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
	ImmutableArray<View> Views,
	in Cells Cells,
	short DigitsMask,
	int Candidate
) : UniqueSquareStep(Conclusions, Views, Cells, DigitsMask)
{
	/// <inheritdoc/>
	public override int Type => 1;

	[FormatItem]
	internal string CandidateStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Candidates.Empty + Candidate).ToString();
	}
}
