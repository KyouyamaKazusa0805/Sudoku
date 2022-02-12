using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;

namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Square Type 4</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="Digit1">Indicates the digit 1 used.</param>
/// <param name="Digit2">Indicates the digit 2 used.</param>
/// <param name="ConjugateRegion">Indicates the cells used as the conjugation region.</param>
public sealed record UniqueSquareType4Step(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	in Cells Cells,
	short DigitsMask,
	int Digit1,
	int Digit2,
	in Cells ConjugateRegion
) : UniqueSquareStep(Conclusions, Views, Cells, DigitsMask)
{
	/// <inheritdoc/>
	public override int Type => 4;

	[FormatItem]
	internal string ConjStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ConjugateRegion.ToString();
	}

	[FormatItem]
	internal string Digit1Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Digit1 + 1).ToString();
	}

	[FormatItem]
	internal string Digit2Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Digit2 + 1).ToString();
	}
}
