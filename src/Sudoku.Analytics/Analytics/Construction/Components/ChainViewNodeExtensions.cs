namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Provides with extension methods on <see cref="Chain"/>.
/// </summary>
/// <seealso cref="Chain"/>
internal static class ChainViewNodeExtensions
{
	/// <summary>
	/// Collect views for the current chain, without nested paths checking.
	/// </summary>
	/// <param name="this">The chain to be checked.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="supportedRules">The supported rules.</param>
	/// <param name="alsIndex">Indicates the currently operated ALS index.</param>
	/// <returns>The views.</returns>
	public static View[] GetViews_Monoparental(this Chain @this, in Grid grid, ChainingRuleCollection supportedRules, ref int alsIndex)
	{
		var result = (View[])[
			[
				.. v(),
				..
				from link in @this.Links
				let node1 = link.FirstNode
				let node2 = link.SecondNode
				select new ChainLinkViewNode(ColorIdentifier.Normal, node1.Map, node2.Map, link.IsStrong)
			]
		];

		foreach (var supportedRule in supportedRules)
		{
			var context = new ChainingRuleViewNodeContext(in grid, @this, result[0]) { CurrentAlmostLockedSetIndex = alsIndex };
			supportedRule.GetViewNodes(ref context);
			alsIndex = context.CurrentAlmostLockedSetIndex;
		}
		return result;


		ReadOnlySpan<CandidateViewNode> v()
		{
			var result = new List<CandidateViewNode>();
			for (var i = 0; i < @this.Length; i++)
			{
				var id = (i & 1) == 0 ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal;
				foreach (var candidate in @this[i].Map)
				{
					result.Add(new(id, candidate));
				}
			}
			return result.AsSpan();
		}
	}
}
