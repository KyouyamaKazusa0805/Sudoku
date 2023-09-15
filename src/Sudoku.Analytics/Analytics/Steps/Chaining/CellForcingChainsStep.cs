using System.Runtime.CompilerServices;
using System.SourceGeneration;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Rendering.Nodes;
using Sudoku.Text;
using Sudoku.Text.Notation;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Cell Forcing Chains</b> technique that is compatible with program <b>Sudoku Explainer</b>.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="sourceCell">Indicates the source cell that all branches start.</param>
/// <param name="chains">Indicates all possible branches in this technique.</param>
/// <param name="isDynamic"><inheritdoc/></param>
/// <param name="dynamicNestingLevel"><inheritdoc/></param>
public sealed partial class CellForcingChainsStep(
	Conclusion[] conclusions,
	View[]? views,
	[DataMember] byte sourceCell,
	[DataMember] MultipleForcingChains chains,
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


	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [SourceCellStr]), new(ChineseLanguage, [SourceCellStr])];

	private string SourceCellStr => CellNotation.ToString(SourceCell);


	/// <inheritdoc/>
	protected internal override View[] CreateViews(scoped ref readonly Grid grid)
	{
		var result = base.CreateViews(in grid);
		return [[.. result[0], new CellViewNode(WellKnownColorIdentifier.Normal, SourceCell)], .. result[1..]];
	}

	/// <inheritdoc/>
	protected override CandidateMap GetOnPotentials(int viewIndex)
		=> viewIndex >= FlatViewsCount ? GetNestedOnPotentials(viewIndex) : GetColorCandidates(GetPotentialAt(viewIndex), true, true);

	/// <inheritdoc/>
	protected override CandidateMap GetOffPotentials(int viewIndex)
		=> viewIndex >= FlatViewsCount ? GetNestedOffPotentials(viewIndex) : GetColorCandidates(GetPotentialAt(viewIndex), false, false);

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
