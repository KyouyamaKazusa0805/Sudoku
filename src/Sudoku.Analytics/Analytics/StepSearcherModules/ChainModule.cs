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
	/// <returns>The first found step.</returns>
	public static Step? CollectCore(ref AnalysisContext context, List<NormalChainStep> accumulator, LinkType linkTypes)
	{
		ref readonly var grid = ref context.Grid;
		var supportedRules = FilterSupportedChainingRules(linkTypes, grid.PuzzleType == SudokuType.Sukaku);
		foreach (var foundChain in ChainingDriver.CollectChainPatterns(in context.Grid, supportedRules))
		{
			var step = new NormalChainStep(
				CollectConclusions(foundChain, in grid, supportedRules),
				CollectViews(in grid, foundChain, supportedRules),
				context.Options,
				foundChain
			);
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
	}

	/// <summary>
	/// The collect method called by multiple forcing chains step searcher.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="accumulator">The instance that temporarily records for chain steps.</param>
	/// <param name="linkTypes">The link types supported in searching.</param>
	/// <returns>The first found step.</returns>
	public static Step? CollectMultipleCore(ref AnalysisContext context, List<MultipleForcingChainsStep> accumulator, LinkType linkTypes)
	{
		ref readonly var grid = ref context.Grid;
		var supportedRules = FilterSupportedChainingRules(linkTypes, grid.PuzzleType == SudokuType.Sukaku);
		foreach (var foundChain in ChainingDriver.CollectMultipleChainPatterns(in context.Grid, supportedRules))
		{
			var step = new MultipleForcingChainsStep(
				[foundChain.Conclusion],
				CollectViews(in grid, foundChain.Conclusion, foundChain, supportedRules),
				context.Options,
				foundChain
			);
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
	}

	/// <summary>
	/// Collect <see cref="CandidateViewNode"/> instances for the found chain.
	/// </summary>
	/// <param name="foundChain">The found chain.</param>
	/// <returns>The found nodes.</returns>
	/// <seealso cref="CandidateViewNode"/>
	private static ReadOnlySpan<CandidateViewNode> GetNormalCandidateViewNodes(ChainOrLoop foundChain)
	{
		var result = new List<CandidateViewNode>();
		for (var i = 0; i < foundChain.Length; i++)
		{
			var id = (i & 1) == 0 ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal;
			foreach (var candidate in foundChain[i].Map)
			{
				result.Add(new(id, candidate));
			}
		}
		return result.AsReadOnlySpan();
	}

	/// <summary>
	/// Collect <see cref="ViewNode"/> instances for the found chain.
	/// </summary>
	/// <param name="foundChain">The found chain.</param>
	/// <returns>The found nodes.</returns>
	private static ReadOnlySpan<ViewNode[]> GetNormalCandidateViewNodes(MultipleForcingChains foundChain)
	{
		var result = new ViewNode[foundChain.Count + 1][];
		ViewNode houseOrCellNode = foundChain.IsCellMultiple
			? new CellViewNode(ColorIdentifier.Normal, foundChain.First().Key / 9)
			: new HouseViewNode(ColorIdentifier.Normal, TrailingZeroCount(foundChain.CandidatesUsed.Cells.SharedHouses));

		var i = 0;
		var globalView = new List<ViewNode>();
		foreach (var key in foundChain.Keys)
		{
			var chain = foundChain[key];
			var subview = new List<ViewNode>();
			var j = 0;
			foreach (var node in chain)
			{
				var id = (++j & 1) == 0 ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal;
				foreach (var candidate in node.Map)
				{
					var currentViewNode = new CandidateViewNode(id, candidate);
					globalView.Add(currentViewNode);
					subview.Add(currentViewNode);
				}
			}

			j = 0;
			foreach (var link in chain.Links)
			{
				var id = (++j & 1) == 0 ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal;
				var currentViewNode = new ChainLinkViewNode(id, link.FirstNode.Map, link.SecondNode.Map, link.IsStrong);
				globalView.Add(currentViewNode);
				subview.Add(currentViewNode);
			}
			result[++i] = [houseOrCellNode, .. subview];
		}
		result[0] = [houseOrCellNode, .. globalView];
		return result.AsReadOnlySpan();
	}

	/// <summary>
	/// The backing method to collect views.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="foundChain">The found chain.</param>
	/// <param name="supportedRules">The supported rules.</param>
	/// <returns>The views.</returns>
	private static View[] CollectViews(ref readonly Grid grid, ChainOrLoop foundChain, ChainingRule[] supportedRules)
	{
		var views = (View[])[
			[
				.. GetNormalCandidateViewNodes(foundChain),
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
	}

	/// <inheritdoc cref="CollectViews(ref readonly Grid, ChainOrLoop, ChainingRule[])"/>
	private static View[] CollectViews(ref readonly Grid grid, Conclusion conclusion, MultipleForcingChains foundChain, ChainingRule[] supportedRules)
	{
		var nodesList = GetNormalCandidateViewNodes(foundChain);
		var result = new View[nodesList.Length];
		for (var i = 0; i < nodesList.Length; i++)
		{
			result[i] = [
				..
				from node in nodesList[i]
				where node is not CandidateViewNode { Candidate: var c } || c != conclusion.Candidate
				select node
			];
		}
		return result;
	}

	/// <summary>
	/// Collect conclusions (especially called by continuous nice loops).
	/// </summary>
	/// <param name="foundChain">The found chain.</param>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="supportedRules">The supported rules.</param>
	/// <returns>The conclusions found.</returns>
	private static Conclusion[] CollectConclusions(ChainOrLoop foundChain, ref readonly Grid grid, ChainingRule[] supportedRules)
	{
		var conclusions = foundChain.GetConclusions(in grid);
		if (foundChain is Loop loop)
		{
			foreach (var r in supportedRules)
			{
				conclusions |= r.CollectLoopConclusions(loop, in grid);
			}
		}
		return [.. conclusions];
	}

	/// <summary>
	/// Returns a new collection of <see cref="ChainingRule"/> instances that can be used in searching strong and weak links.
	/// </summary>
	/// <param name="linkTypes">The desired link types.</param>
	/// <param name="isSukaku">Indicates whether the puzzle is a Sukaku.</param>
	/// <returns>A list of <see cref="ChainingRule"/> instances.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ChainingRule[] FilterSupportedChainingRules(LinkType linkTypes, bool isSukaku)
		=>
		from type in linkTypes.GetAllFlags()
		where !isSukaku || type is not (LinkType.AlmostUniqueRectangle or LinkType.AlmostAvoidableRectangle)
		select ChainingRulePool.TryCreate(type)!;
}
