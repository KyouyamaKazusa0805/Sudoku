namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Cell Forcing Chains</b> technique that is compatible with program <b>Sudoku Explainer</b>.
/// </summary>
public sealed class CellForcingChainsStep(
	Conclusion[] conclusions,
	View[]? views,
	byte sourceCell,
	MultipleForcingChains chains,
	bool isDynamic,
	int dynamicNestingLevel = 0
) : ChainingStep(conclusions, views, isMultiple: true, isDynamic: isDynamic, dynamicNestingLevel: dynamicNestingLevel)
{
	internal CellForcingChainsStep(
		Conclusion[] conclusions,
		byte sourceCell,
		MultipleForcingChains chains,
		bool isDynamic,
		int dynamicNestingLevel = 0
	) : this(conclusions, null!, sourceCell, chains, isDynamic, dynamicNestingLevel)
	{
	}

	internal CellForcingChainsStep(CellForcingChainsStep @base, View[]? views) :
		this(@base.Conclusions, views, @base.SourceCell, @base.Chains, @base.IsDynamic, @base.DynamicNestingLevel)
	{
	}


	/// <summary>
	/// Indicates the source cell that all branches start.
	/// </summary>
	public byte SourceCell { get; } = sourceCell;

	/// <summary>
	/// Indicates all possible branches in this technique.
	/// </summary>
	public MultipleForcingChains Chains { get; } = chains;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { SourceCellStr } }, { "zh", new[] { SourceCellStr } } };

	private string SourceCellStr => RxCyNotation.ToCellString(SourceCell);


	/// <inheritdoc/>
	protected override CandidateMap GetGreenPotentials(int viewIndex)
		=> viewIndex >= FlatViewsCount ? GetNestedGreenPotentials(viewIndex) : GetColorCandidates(GetPotentialAt(viewIndex), true, true);

	/// <inheritdoc/>
	protected override CandidateMap GetRedPotentials(int viewIndex)
		=> viewIndex >= FlatViewsCount ? GetNestedRedPotentials(viewIndex) : GetColorCandidates(GetPotentialAt(viewIndex), false, false);

	/// <inheritdoc/>
	protected override List<LinkViewNode> GetLinks(int viewIndex)
		=> viewIndex >= FlatViewsCount ? GetNestedLinks(viewIndex) : GetLinks(GetPotentialAt(viewIndex));

	/// <summary>
	/// Gets the potential at the specified index.
	/// </summary>
	/// <param name="viewIndex">The view index.</param>
	/// <returns>The view index.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private ChainNode GetPotentialAt(int viewIndex) => Chains[viewIndex].Potential;
}
