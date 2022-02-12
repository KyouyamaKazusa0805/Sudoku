using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;

namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Loop Type 3</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="Loop"><inheritdoc/></param>
/// <param name="SubsetDigitsMask">Indicates the mask that contains the subset digits used in this instance.</param>
/// <param name="SubsetCells">Indicates the subset cells.</param>
public sealed record UniqueLoopType3Step(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	int Digit1,
	int Digit2,
	in Cells Loop,
	short SubsetDigitsMask,
	in Cells SubsetCells
) : UniqueLoopStep(Conclusions, Views, Digit1, Digit2, Loop)
{
	/// <inheritdoc/>
	public override decimal Difficulty => base.Difficulty + SubsetCells.Count * .1M;

	/// <inheritdoc/>
	public override int Type => 3;

	[FormatItem]
	internal string SubsetCellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => SubsetCells.ToString();
	}

	[FormatItem]
	internal string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(SubsetDigitsMask).ToString();
	}

	[FormatItem]
	internal string SubsetName
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => R[$"SubsetNamesSize{SubsetCells.Count + 1}"]!;
	}

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;
}
