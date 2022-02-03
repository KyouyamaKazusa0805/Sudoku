using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;
using static System.Algorithm.Sequences;

namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Multi-sector Locked Sets</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells">Indicates the cells used.</param>
public sealed record MultisectorLockedSetsStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	in Cells Cells
) : RankTheoryStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty =>
		9.4M // Base difficulty.
		+ A002024(Cells.Count) * .1M; // Size difficulty.

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.Msls;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.MultisectorLockedSets;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.MultisectorLockedSets;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.HardlyEver;

	[FormatItem]
	internal string CellsCountStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Cells.Count.ToString();
	}

	[FormatItem]
	internal string CellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Cells.ToString();
	}
}
