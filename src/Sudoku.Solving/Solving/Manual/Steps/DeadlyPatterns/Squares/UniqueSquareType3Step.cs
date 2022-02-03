using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Resources;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;
using static System.Numerics.BitOperations;

namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Square Type 3</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="ExtraCells">Indicates the extra cells used.</param>
/// <param name="ExtraDigitsMask">
/// Indicates the extra digits that forms a subset with <paramref name="DigitsMask"/>.
/// </param>
public sealed record UniqueSquareType3Step(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	in Cells Cells,
	short DigitsMask,
	short ExtraDigitsMask,
	in Cells ExtraCells
) : UniqueSquareStep(Conclusions, Views, Cells, DigitsMask)
{
	/// <inheritdoc/>
	public override decimal Difficulty => base.Difficulty + PopCount((uint)ExtraDigitsMask) * .1M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.UniqueSquareType3;

	[FormatItem]
	private string ExtraCellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ExtraCells.ToString();
	}

	[FormatItem]
	private string ExtraDigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(ExtraDigitsMask).ToString();
	}

	[FormatItem]
	private string SubsetName
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ResourceManager.Shared[$"SubsetNames{ExtraCells.Count + 1}"];
	}
}
