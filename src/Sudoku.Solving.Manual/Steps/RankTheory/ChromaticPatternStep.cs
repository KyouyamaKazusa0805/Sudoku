namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Chromatic Pattern</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Blocks">The blocks used.</param>
/// <param name="Pattern">The cells used.</param>
/// <param name="DigitsMask">The digits mask.</param>
internal abstract record ChromaticPatternStep(
	ConclusionList Conclusions,
	ViewList Views,
	int[] Blocks,
	scoped in Cells Pattern,
	short DigitsMask
) : RankTheoryStep(Conclusions, Views), ILoopLikeStep, IStepWithRank
{
	/// <inheritdoc/>
	public bool? IsNice => null;

	/// <inheritdoc/>
	public override decimal Difficulty => 6.5M;

	/// <inheritdoc/>
	public int Rank => -1;

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.TrivalueOddagon;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public sealed override Rarity Rarity => Rarity.OnlyForSpecialPuzzles;


	[FormatItem]
	internal string BlocksStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => string.Join(", ", from block in Blocks select $"{block + 1}");
	}

	[FormatItem]
	internal string CellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Pattern.ToString();
	}

	[FormatItem]
	internal string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);
	}
}
