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
				chain.GetViews(in grid, supportedRules),
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


		static Conclusion[] c(ChainOrLoop pattern, ref readonly Grid grid, ReadOnlySpan<ChainingRule> rules)
		{
			var conclusions = pattern.GetConclusions(in grid);
			if (pattern is Loop { Links: var links })
			{
				var context = new ChainingRuleLoopConclusionCollectingContext(in grid, links);
				foreach (var r in rules)
				{
					conclusions |= r.CollectLoopConclusions(in context);
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
		foreach (var chain in ChainingDriver.CollectMultipleChains(in context.Grid, context.OnlyFindOne))
		{
			if (chain.TryCastToFinnedChain(out var finnedChain, out var f) && onlyFindFinnedChain)
			{
				ref readonly var fins = ref Nullable.GetValueRefOrDefaultRef(in f);
				var views = (View[])[
					[
						.. from candidate in fins select new CandidateViewNode(ColorIdentifier.Auxiliary1, candidate),
						.. finnedChain.GetViews(in grid, supportedRules)[0]
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
					chain.GetViews(in grid, chain.Conclusion, supportedRules),
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
	/// The collect method called by blossom loop step searchers.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="accumulator">The instance that temporarily records for chain steps.</param>
	/// <param name="supportedRules">Indicates the supported chaining rules.</param>
	/// <returns>The first found step.</returns>
	public static Step? CollectBlossomLoopCore(ref AnalysisContext context, List<BlossomLoopStep> accumulator, ReadOnlySpan<ChainingRule> supportedRules)
	{
		ref readonly var grid = ref context.Grid;
		foreach (var blossomLoop in ChainingDriver.CollectBlossomLoops(in context.Grid, context.OnlyFindOne, supportedRules))
		{
			var step = new BlossomLoopStep(
				blossomLoop.Conclusions,
				getViews(blossomLoop, in grid, supportedRules),
				context.Options,
				blossomLoop
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


		static View[] getViews(BlossomLoop blossomLoop, ref readonly Grid grid, ReadOnlySpan<ChainingRule> supportedRules)
		{
			var globalView = new View();
			var loopView = blossomLoop.BurredLoop.GetViews(in grid, supportedRules)[0];
			globalView.AddRange(loopView);

			var otherViews = new View[blossomLoop.Count];
			var i = 0;
			foreach (var branch in blossomLoop.Values)
			{
				ref var view = ref otherViews[i++];
				view = [];

				foreach (var rule in supportedRules)
				{
					var context = new ChainingRuleViewNodesMappingContext(in grid, branch, view);
					rule.MapViewNodes(ref context);
					globalView.AddRange(context.ProducedViewNodes);
				}
			}

			// Append kraken house or cell (target cell or house that expands branches).
			var result = (View[])[globalView, loopView, .. otherViews];
#pragma warning disable IDE0004
			var node = blossomLoop.IsCellType
				? new CellViewNode(ColorIdentifier.Normal, (Cell)blossomLoop.KrakenCellOrHouse)
				: (ViewNode)new HouseViewNode(ColorIdentifier.Normal, (House)blossomLoop.KrakenCellOrHouse);
#pragma warning restore IDE0004
			foreach (var view in result)
			{
				view.Add(node);
			}

			// Adjust the burred branch start nodes to the color as same as fins.
			var krakenNodes = CandidateMap.Empty;
			foreach (var startNode in blossomLoop.Keys)
			{
				krakenNodes.Add(startNode.Map[0]);
			}
			foreach (var view in result)
			{
				var foundNode = view.FirstOrDefault(n => n is CandidateViewNode { Candidate: var c } && krakenNodes.Contains(c));
				if (foundNode is not null)
				{
					view.Remove(foundNode);
					view.Add(new CandidateViewNode(ColorIdentifier.Auxiliary2, ((CandidateViewNode)foundNode).Candidate));
				}
			}
			return result;
		}
	}
}
