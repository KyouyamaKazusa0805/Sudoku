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
			var globalView = View.Empty;
			var otherViews = new View[blossomLoop.Count];
			otherViews.InitializeArray(static ([NotNull] ref View? view) => view = View.Empty);

			var i = 0;
			foreach (var (startCandidate, branch) in blossomLoop)
			{
				var viewNodes = branch.GetViews(in grid, supportedRules)[0];
				globalView |= viewNodes;
				otherViews[i] |= viewNodes;

				var context = new ChainingRuleViewNodesMappingContext(in grid, branch, otherViews[i]);
				foreach (var rule in supportedRules)
				{
					rule.MapViewNodes(ref context);
					globalView.AddRange(context.ProducedViewNodes);
				}
				globalView.Add(new CandidateViewNode(ColorIdentifier.Normal, startCandidate));

				i++;
			}

			globalView.Add(
				blossomLoop.Entries is var entryCandidates && blossomLoop.EntryIsCellType
					? new CellViewNode(ColorIdentifier.Normal, entryCandidates[0] / 9)
					: new HouseViewNode(ColorIdentifier.Normal, TrailingZeroCount(entryCandidates.Cells.SharedHouses))
			);
			globalView.Add(
				blossomLoop.Exits is var exitCandidates && blossomLoop.ExitIsCellType
					? new CellViewNode(ColorIdentifier.Auxiliary1, exitCandidates[0] / 9)
					: new HouseViewNode(ColorIdentifier.Auxiliary1, TrailingZeroCount(exitCandidates.Cells.SharedHouses))
			);
			return [globalView, .. otherViews];
		}
	}
}
