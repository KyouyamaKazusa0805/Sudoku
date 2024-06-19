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
	/// <param name="supportedRules">Indicates the supported chaining rules.</param>
	/// <returns>The first found step.</returns>
	public static Step? CollectCore(ref AnalysisContext context, List<NormalChainStep> accumulator, ReadOnlySpan<ChainingRule> supportedRules)
	{
		ref readonly var grid = ref context.Grid;
		foreach (var chain in ChainingDriver.CollectChains(in context.Grid, context.OnlyFindOne))
		{
			var step = new NormalChainStep(
				c(chain, in grid, supportedRules),
				CollectViews(in grid, chain, supportedRules),
				context.Options,
				chain
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


		static Conclusion[] c(ChainOrLoop foundChain, ref readonly Grid grid, ReadOnlySpan<ChainingRule> rules)
		{
			var conclusions = foundChain.GetConclusions(in grid);
			if (foundChain is Loop loop)
			{
				foreach (var r in rules)
				{
					conclusions |= r.CollectLoopConclusions(loop, in grid);
				}
			}
			return [.. conclusions];
		}
	}

	/// <summary>
	/// The collect method called by multiple forcing chains step searcher.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="accumulator">The instance that temporarily records for chain steps.</param>
	/// <param name="supportedRules">Indicates the supported rules.</param>
	/// <param name="onlyFindFinnedChain">Indicates whether the method only finds for (grouped) finned chains.</param>
	/// <returns>The first found step.</returns>
	public static Step? CollectMultipleCore(
		ref AnalysisContext context,
		List<ChainStep> accumulator,
		ReadOnlySpan<ChainingRule> supportedRules,
		bool onlyFindFinnedChain
	)
	{
		ref readonly var grid = ref context.Grid;
		foreach (var chain in ChainingDriver.CollectMultipleChains(in context.Grid, supportedRules, context.OnlyFindOne))
		{
			if (chain.TryCastToFinnedChain(out var finnedChain, out var f) && onlyFindFinnedChain)
			{
				ref readonly var fins = ref Nullable.GetValueRefOrDefaultRef(in f);
				var views = (View[])[
					[
						.. from candidate in fins select new CandidateViewNode(ColorIdentifier.Auxiliary1, candidate),
						.. CollectViews(in grid, finnedChain, supportedRules)[0]
					]
				];

				// Change nodes into fin-like view nodes.
				foreach (var node in views[0].ToArray())
				{
					if (node is CandidateViewNode { Candidate: var candidate } && fins.Contains(candidate))
					{
						views[0].Remove(node);
						views[0].Add(new CandidateViewNode(ColorIdentifier.Auxiliary2, candidate));
					}
				}

				var finnedChainStep = new FinnedChainStep(
					[chain.Conclusion],
					views,
					context.Options,
					finnedChain,
					in fins
				);
				if (context.OnlyFindOne)
				{
					return finnedChainStep;
				}
				accumulator.Add(finnedChainStep);
				continue;
			}

			if (!onlyFindFinnedChain)
			{
				var mfcStep = new MultipleForcingChainsStep(
					[chain.Conclusion],
					CollectViews(in grid, chain.Conclusion, chain, supportedRules),
					context.Options,
					chain
				);
				if (context.OnlyFindOne)
				{
					return mfcStep;
				}
				accumulator.Add(mfcStep);
			}
		}
		return null;
	}

	/// <summary>
	/// The backing method to collect views.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="foundChain">The found chain.</param>
	/// <param name="supportedRules">The supported rules.</param>
	/// <returns>The views.</returns>
	private static View[] CollectViews(ref readonly Grid grid, ChainOrLoop foundChain, ReadOnlySpan<ChainingRule> supportedRules)
	{
		var result = (View[])[
			[
				.. v(foundChain),
				..
				from link in foundChain.Links
				let node1 = link.FirstNode
				let node2 = link.SecondNode
				select new ChainLinkViewNode(ColorIdentifier.Normal, node1.Map, node2.Map, link.IsStrong)
			]
		];
		foreach (var supportedRule in supportedRules)
		{
			supportedRule.CollectExtraViewNodes(in grid, foundChain, result[0], out _);
		}
		return result;


		static ReadOnlySpan<CandidateViewNode> v(ChainOrLoop foundChain)
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
	}

	/// <inheritdoc cref="CollectViews(ref readonly Grid, ChainOrLoop, ReadOnlySpan{ChainingRule})"/>
	private static View[] CollectViews(
		ref readonly Grid grid,
		Conclusion conclusion,
		MultipleForcingChains foundChain,
		ReadOnlySpan<ChainingRule> supportedRules
	)
	{
		var viewNodes = v(in grid, foundChain, supportedRules);
		var result = new View[viewNodes.Length];
		for (var i = 0; i < viewNodes.Length; i++)
		{
			result[i] = [
				..
				from node in viewNodes[i]
				where node is not CandidateViewNode { Candidate: var c } || c != conclusion.Candidate
				select node
			];
		}
		foreach (var supportedRule in supportedRules)
		{
			supportedRule.CollectExtraViewNodes(in grid, foundChain, result);
		}
		return result;


		static ReadOnlySpan<ViewNode[]> v(ref readonly Grid grid, MultipleForcingChains foundChain, ReadOnlySpan<ChainingRule> rules)
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
				var subview = new View();
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
			return result;
		}
	}
}
