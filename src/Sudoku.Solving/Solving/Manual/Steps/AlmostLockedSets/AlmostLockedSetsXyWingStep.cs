using Sudoku.Collections;
using Sudoku.Data;
using Sudoku.Presentation;
using Sudoku.Solving.Collections;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Steps.AlmostLockedSets;

/// <summary>
/// Provides with a step that is an <b>Almost Locked Sets XY-Wing</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Als1">Indicates the first ALS used in this pattern.</param>
/// <param name="Als2">Indicates the second ALS used in this pattern.</param>
/// <param name="Bridge">Indicates the ALS that is as a bridge.</param>
/// <param name="XDigitsMask">Indicates the mask that holds the digits for the X value.</param>
/// <param name="YDigitsMask">Indicates the mask that holds the digits for the Y value.</param>
/// <param name="ZDigitsMask">Indicates the mask that holds the digits for the Z value.</param>
public sealed record AlmostLockedSetsXyWingStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	in AlmostLockedSet Als1,
	in AlmostLockedSet Als2,
	in AlmostLockedSet Bridge,
	short XDigitsMask,
	short YDigitsMask,
	short ZDigitsMask
) : AlmostLockedSetsStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 6.0M;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.ShortChaining;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.AlmostLockedSetsChainingLike;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.AlmostLockedSetsXyWing;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;

	[FormatItem]
	private string Als1Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Als1.ToString();
	}

	[FormatItem]
	private string BridgeStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Bridge.ToString();
	}

	[FormatItem]
	private string Als2Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Als2.ToString();
	}

	[FormatItem]
	private string XStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(XDigitsMask).ToString();
	}

	[FormatItem]
	private string YStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(YDigitsMask).ToString();
	}

	[FormatItem]
	private string ZStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(ZDigitsMask).ToString();
	}
}
