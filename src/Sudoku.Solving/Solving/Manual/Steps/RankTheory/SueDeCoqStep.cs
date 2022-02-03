using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Sue de Coq</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Block">The block.</param>
/// <param name="Line">The line.</param>
/// <param name="BlockMask">The block mask.</param>
/// <param name="LineMask">The line mask.</param>
/// <param name="IntersectionMask">The intersection mask.</param>
/// <param name="IsCannibalistic">Indicates whether the SdC is cannibalistic.</param>
/// <param name="IsolatedDigitsMask">The isolated digits mask.</param>
/// <param name="BlockCells">The map of block cells.</param>
/// <param name="LineCells">The map of line cells.</param>
/// <param name="IntersectionCells">The map of intersection cells.</param>
public sealed record SueDeCoqStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	int Block,
	int Line,
	short BlockMask,
	short LineMask,
	short IntersectionMask,
	bool IsCannibalistic,
	short IsolatedDigitsMask,
	in Cells BlockCells,
	in Cells LineCells,
	in Cells IntersectionCells
) : RankTheoryStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty =>
		5.0M
		+ (IsolatedDigitsMask != 0 ? .1M : 0) // The extra difficulty for isolated digit existence.
		+ (IsCannibalistic ? .2M : 0); // The extra difficulty for cannibalism.

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => TechniqueTags.RankTheory | TechniqueTags.Als;

	/// <inheritdoc/>
	public override Technique TechniqueCode =>
		IsCannibalistic ? Technique.SueDeCoqCannibalism : Technique.SueDeCoq;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.SueDeCoq;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;

	[FormatItem]
	internal string IntersectionCellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => IntersectionCells.ToString();
	}

	[FormatItem]
	internal string IntersectionDigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(IntersectionMask).ToString(null);
	}

	[FormatItem]
	internal string BlockCellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => BlockCells.ToString();
	}

	[FormatItem]
	internal string BlockDigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(BlockMask).ToString(null);
	}

	[FormatItem]
	internal string LineCellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => LineCells.ToString();
	}

	[FormatItem]
	internal string LineDigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(LineMask).ToString(null);
	}
}
