using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;

namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is an <b>Extended Rectangle Type 4</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="ConjugatePair">Indicates the conjugate pair used.</param>
public sealed record ExtendedRectangleType4Step(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	in Cells Cells,
	short DigitsMask,
	in ConjugatePair ConjugatePair
) : ExtendedRectangleStep(Conclusions, Views, Cells, DigitsMask)
{
	/// <inheritdoc/>
	public override int Type => 4;

	/// <inheritdoc/>
	public override decimal Difficulty => base.Difficulty + .1M;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;

	[FormatItem]
	internal string ConjStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ConjugatePair.ToString();
	}
}
