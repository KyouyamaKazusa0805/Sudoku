using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;

namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Square Type 2</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="ExtraDigit">Indicates the extra digit used.</param>
public sealed record UniqueSquareType2Step(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	in Cells Cells,
	short DigitsMask,
	int ExtraDigit
) : UniqueSquareStep(Conclusions, Views, Cells, DigitsMask)
{
	/// <inheritdoc/>
	public override decimal Difficulty => base.Difficulty + .1M;

	/// <inheritdoc/>
	public override int Type => 2;

	[FormatItem]
	internal string ExtraDigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (ExtraDigit + 1).ToString();
	}
}
