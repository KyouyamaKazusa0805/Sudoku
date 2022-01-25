namespace Sudoku.Solving.Manual.Steps.Chains.Forcing;

/// <summary>
/// Provides with a step that is a <b>Cell Forcing Chains</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="SourceCell">Indicates the source cell used.</param>
/// <param name="Chains">Indicates the sub-chains that is grouped by each candidate in that cell.</param>
/// <param name="IsDynamic"><inheritdoc/></param>
/// <param name="Level"><inheritdoc/></param>
public sealed record CellChainingStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	int SourceCell,
	IReadOnlyDictionary<int, Node> Chains,
	bool IsDynamic,
	byte Level
) : ChainStep(
	Conclusions,
	Views,
	XEnabled: true,
	YEnabled: true,
	IsNishio: false,
	IsMultiple: true,
	IsDynamic,
	Level
)
{
	/// <inheritdoc/>
	public override int FlatComplexity
	{
		get
		{
			int result = 0;
			foreach (var node in Chains.Values)
			{
				result += node.AncestorsCount;
			}

			return result;
		}
	}

	/// <inheritdoc/>
	public override decimal Difficulty => BaseDifficulty + LengthDifficulty;

	/// <inheritdoc/>
	public override ChainTypeCode SortKey => IsDynamic ? ChainTypeCode.DynamicCellFc : ChainTypeCode.CellFc;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => TechniqueTags.LongChaining | TechniqueTags.ForcingChains;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.ForcingChains;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;

	[FormatItem]
	private string SourceCellStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Coordinate((byte)SourceCell).ToString();
	}
}
