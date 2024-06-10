namespace Sudoku.Analytics.StepSearcherModules;

/// <summary>
/// Represents a chain module.
/// </summary>
internal static class ChainModule
{
	/// <summary>
	/// The collect method called by chain step searchers.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="accumulator">The instance that temporarily records for chain steps.</param>
	/// <param name="linkTypes">The link types supported in searching.</param>
	/// <param name="ruleRouter">The rule router dictionary.</param>
	/// <returns>The first found step.</returns>
	public static Step? CollectCore(ref AnalysisContext context, List<NormalChainStep> accumulator, LinkType linkTypes, Dictionary<LinkType, ChainingRule> ruleRouter)
	{
		ref readonly var grid = ref context.Grid;
		var isSukaku = grid.PuzzleType == SudokuType.Sukaku;
		var supportedRules =
			from type in linkTypes.GetAllFlags()
			where !isSukaku || type is not (LinkType.AlmostUniqueRectangle or LinkType.AlmostAvoidableRectangle)
			select ruleRouter[type];
		foreach (var foundChain in ChainingDriver.CollectChainPatterns(in context.Grid, supportedRules))
		{
			var conclusions = collectConclusions(foundChain, in grid);
			var step = new NormalChainStep([.. conclusions], collectViews(in grid, foundChain), context.Options, foundChain);
			if (context.OnlyFindOne)
			{
				return step;
			}
			if (!accumulator.Contains(step))
			{
				accumulator.Add(step);
			}
		}
		return null;


		ConclusionSet collectConclusions(ChainPattern foundChain, ref readonly Grid grid)
		{
			var conclusions = foundChain.GetConclusions(in grid);
			if (foundChain is Loop loop)
			{
				foreach (var r in supportedRules)
				{
					conclusions |= r.CollectLoopConclusions(loop, in grid);
				}
			}
			return conclusions;
		}

		View[] collectViews(ref readonly Grid grid, ChainPattern foundChain)
		{
			var views = (View[])[
				[
					.. getCandidateNodes(foundChain),
					..
					from link in foundChain.Links
					let node1 = link.FirstNode
					let node2 = link.SecondNode
					select new ChainLinkViewNode(ColorIdentifier.Normal, node1.Map, node2.Map, link.IsStrong)
				]
			];
			foreach (var supportedRule in supportedRules)
			{
				supportedRule.CollectExtraViewNodes(in grid, foundChain, ref views);
			}
			return views;


			static ReadOnlySpan<CandidateViewNode> getCandidateNodes(ChainPattern foundChain)
			{
				var result = new List<CandidateViewNode>();
				for (var i = 0; i < foundChain.Length; i++)
				{
					ref readonly var map = ref foundChain[i].Map;
					var id = (i & 1) == 0 ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal;
					foreach (var candidate in map)
					{
						result.Add(new(id, candidate));
					}
				}
				return result.AsReadOnlySpan();
			}
		}
	}
}
