using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Region Foring Chains</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Region">Indicates the region that the forcing chains occurs.</param>
/// <param name="Digit">Indicates the digit used in that region.</param>
/// <param name="Chains">IIndicates the regions that binds with the relative position in this region.</param>
/// <param name="IsDynamic"><inheritdoc/></param>
/// <param name="Level"><inheritdoc/></param>
public sealed record RegionChainingStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	int Region,
	int Digit,
	IReadOnlyDictionary<int, Node> Chains,
	bool IsDynamic,
	byte Level
) : ChainStep(Conclusions, Views, true, true, false, true, IsDynamic, Level)
{
	/// <inheritdoc/>
	public override decimal Difficulty => BaseDifficulty + LengthDifficulty;

	/// <inheritdoc/>
	public override int FlatComplexity => Chains.Values.Sum(static node => node.AncestorsCount);

	/// <inheritdoc/>
	public override ChainTypeCode SortKey => IsDynamic ? ChainTypeCode.DynamicRegionFc : ChainTypeCode.RegionFc;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => TechniqueTags.LongChaining | TechniqueTags.ForcingChains;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.ForcingChains;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

	[FormatItem]
	internal string DigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Digit + 1).ToString();
	}

	[FormatItem]
	internal string RegionStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new RegionCollection(Region).ToString();
	}
}
