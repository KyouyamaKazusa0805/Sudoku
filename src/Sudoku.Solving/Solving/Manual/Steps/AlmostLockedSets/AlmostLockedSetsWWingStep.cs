using Sudoku.Collections;
using Sudoku.Data;
using Sudoku.Presentation;
using Sudoku.Solving.Collections;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is an <b>Almost Locked Sets W-Wing</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Als1">Indicates the first ALS used in this pattern.</param>
/// <param name="Als2">Indicates the second ALS used in this pattern.</param>
/// <param name="ConjugatePair">Indiactes the conjugate pair used.</param>
/// <param name="WDigitsMask">Indicates the mask that holds the W digit.</param>
/// <param name="X">Indicates the X digit.</param>
public sealed record AlmostLockedSetsWWingStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	in AlmostLockedSet Als1,
	in AlmostLockedSet Als2,
	in ConjugatePair ConjugatePair,
	short WDigitsMask,
	int X
) : AlmostLockedSetsStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 6.2M;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.ShortChaining;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.AlmostLockedSetsChainingLike;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.AlmostLockedSetsWWing;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	[FormatItem]
	private string Als1Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Als1.ToString();
	}

	[FormatItem]
	private string Als2Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Als2.ToString();
	}

	[FormatItem]
	private string ConjStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ConjugatePair.ToString();
	}

	[FormatItem]
	private string WStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(WDigitsMask).ToString();
	}

	[FormatItem]
	private string XStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (X + 1).ToString();
	}
}
