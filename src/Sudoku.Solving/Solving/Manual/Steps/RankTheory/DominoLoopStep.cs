namespace Sudoku.Solving.Manual.Steps.RankTheory;

/// <summary>
/// Provides with a step that is a <b>Domino Loop</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells">Indicates the cells used.</param>
public sealed record DominoLoopStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	Cells Cells
) : RankTheoryStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 9.6M;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags =>
		base.TechniqueTags | TechniqueTags.LongChaining | TechniqueTags.RankTheory;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.SkLoop;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.SkLoop;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.OnlyForSpecialPuzzles;

	[FormatItem]
	private string CellsCountStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Cells.Count.ToString();
	}

	[FormatItem]
	private string CellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Cells.ToString();
	}
}
