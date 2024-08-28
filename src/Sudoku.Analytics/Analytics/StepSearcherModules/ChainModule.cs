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
	/// <param name="allowsAdvancedLinks">Indicates whether the method allows advanced links.</param>
	/// <returns>The first found step.</returns>
	public static Step? CollectCore(ref StepAnalysisContext context, SortedSet<NormalChainStep> accumulator, bool allowsAdvancedLinks)
	{
		ref readonly var grid = ref context.Grid;
		InitializeLinks(
			in grid,
			((LinkType[])[.. ChainingRule.ElementaryLinkTypes, .. allowsAdvancedLinks ? ChainingRule.AdvancedLinkTypes : []])
				.Aggregate(@delegate.EnumFlagMerger),
			context.Options,
			out var supportedRules
		);

		var cachedAlsIndex = 0;
		foreach (var chain in ChainingDriver.CollectChains(in context.Grid, context.OnlyFindOne))
		{
			var step = new NormalChainStep(
				c(chain, in grid, supportedRules),
				chain.GetViews(in grid, supportedRules, ref cachedAlsIndex),
				context.Options,
				chain
			);
			if (!step.IsAdvancedAllowed(allowsAdvancedLinks))
			{
				continue;
			}

			if (context.OnlyFindOne)
			{
				return step;
			}

			accumulator.Add(step);
		}
		return null;


		static Conclusion[] c(NamedChain pattern, ref readonly Grid grid, ChainingRules rules)
		{
			var conclusions = pattern.GetConclusions(in grid);
			if (pattern is Loop { Links: var links })
			{
				var context = new ChainingRuleLoopConclusionContext(in grid, links);
				foreach (var r in rules)
				{
					r.GetLoopConclusions(ref context);
					conclusions |= context.Conclusions;
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
	/// <param name="allowsAdvancedLinks">Indicates whether the method allows advanced links.</param>
	/// <param name="onlyFindFinnedChain">Indicates whether the method only finds for (grouped) finned chains.</param>
	/// <returns>The first found step.</returns>
	public static Step? CollectMultipleCore(
		ref StepAnalysisContext context,
		SortedSet<ChainStep> accumulator,
		bool allowsAdvancedLinks,
		bool onlyFindFinnedChain
	)
	{
		ref readonly var grid = ref context.Grid;
		InitializeLinks(
			in grid,
			((LinkType[])[.. ChainingRule.ElementaryLinkTypes, .. allowsAdvancedLinks ? ChainingRule.AdvancedLinkTypes : []])
				.Aggregate(@delegate.EnumFlagMerger),
			context.Options,
			out var supportedRules
		);

		var cachedAlsIndex = 0;
		foreach (var chain in ChainingDriver.CollectMultipleChains(in context.Grid, context.OnlyFindOne))
		{
			if (onlyFindFinnedChain && chain.TryCastToFinnedChain(out var finnedChain, out var f))
			{
				ref readonly var fins = ref Nullable.GetValueRefOrDefaultRef(in f);
				var views = (View[])[
					[
						.. from candidate in fins select new CandidateViewNode(ColorIdentifier.Auxiliary1, candidate),
						.. finnedChain.GetViews(in grid, supportedRules, ref cachedAlsIndex)[0]
					]
				];

				// Change nodes into fin-like view nodes.
				foreach (var node in (ViewNode[])[.. views[0]])
				{
					if (node is CandidateViewNode { Candidate: var candidate } && fins.Contains(candidate))
					{
						views[0].Remove(node);
						views[0].Add(new CandidateViewNode(ColorIdentifier.Auxiliary2, candidate));
					}
				}

				var finnedChainStep = new FinnedChainStep(chain.Conclusions, views, context.Options, finnedChain, in fins);
				if (!finnedChainStep.IsAdvancedAllowed(allowsAdvancedLinks))
				{
					continue;
				}

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
					chain.Conclusions,
					chain.GetViews(in grid, chain.Conclusions, supportedRules),
					context.Options,
					chain
				);
				if (!mfcStep.IsAdvancedAllowed(allowsAdvancedLinks))
				{
					continue;
				}

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
	/// <returns>The first found step.</returns>
	public static Step? CollectBlossomLoopCore(ref StepAnalysisContext context, SortedSet<BlossomLoopStep> accumulator)
	{
		const bool allowsAdvancedLinks = true;
		ref readonly var grid = ref context.Grid;
		InitializeLinks(
			in grid,
			((LinkType[])[.. ChainingRule.ElementaryLinkTypes, .. allowsAdvancedLinks ? ChainingRule.AdvancedLinkTypes : []])
				.Aggregate(@delegate.EnumFlagMerger),
			context.Options,
			out var supportedRules
		);

		foreach (var blossomLoop in ChainingDriver.CollectBlossomLoops(in context.Grid, context.OnlyFindOne, supportedRules))
		{
			var step = new BlossomLoopStep(
				[.. blossomLoop.Conclusions],
				getViews(blossomLoop, in grid, supportedRules),
				context.Options,
				blossomLoop
			);
			if (!step.IsAdvancedAllowed(allowsAdvancedLinks))
			{
				continue;
			}
			if (context.OnlyFindOne)
			{
				return step;
			}

			accumulator.Add(step);
		}
		return null;


		static View[] getViews(BlossomLoop blossomLoop, ref readonly Grid grid, ChainingRules supportedRules)
		{
			var globalView = View.Empty;
			var otherViews = new View[blossomLoop.Count];
			otherViews.InitializeArray(static ([NotNull] ref View? view) => view = View.Empty);

			var (i, cachedAlsIndex) = (0, 0);
			foreach (var (startCandidate, branch) in blossomLoop)
			{
				var viewNodes = branch.GetViews(in grid, supportedRules, ref cachedAlsIndex)[0];
				globalView |= viewNodes;
				otherViews[i] |= viewNodes;
				globalView.Add(new CandidateViewNode(ColorIdentifier.Normal, startCandidate));
				i++;
			}

			var entryHouseOrCellViewNode = (ViewNode)(
				blossomLoop.Entries is var entryCandidates && blossomLoop.EntryIsCellType
					? new CellViewNode(ColorIdentifier.Normal, entryCandidates[0] / 9)
					: new HouseViewNode(ColorIdentifier.Normal, HouseMask.TrailingZeroCount(entryCandidates.Cells.SharedHouses))
			);
			var exitHouseOrCellViewNode = (ViewNode)(
				blossomLoop.Exits is var exitCandidates && blossomLoop.ExitIsCellType
					? new CellViewNode(ColorIdentifier.Auxiliary1, exitCandidates[0] / 9)
					: new HouseViewNode(ColorIdentifier.Auxiliary1, HouseMask.TrailingZeroCount(exitCandidates.Cells.SharedHouses))
			);

			globalView.Add(entryHouseOrCellViewNode);
			globalView.Add(exitHouseOrCellViewNode);
			foreach (ref var otherView in otherViews.AsSpan())
			{
				otherView.Add(entryHouseOrCellViewNode);
				otherView.Add(exitHouseOrCellViewNode);
			}
			return [globalView, .. otherViews];
		}
	}
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// Determine whether a <see cref="ChainStep"/> instance is allowed to contain advanced links.
	/// </summary>
	/// <param name="this">The chain to be checked.</param>
	/// <param name="allowsAdvancedLinks">Indicates whether the method allows advanced links.</param>
	/// <returns>A <see cref="bool"/> result indicating whether the element has already been added.</returns>
	public static bool IsAdvancedAllowed(this ChainStep @this, bool allowsAdvancedLinks)
	{
		if (!allowsAdvancedLinks)
		{
			// Check whether the chain contains grouped links.
			switch (@this)
			{
				case NormalChainStep { Pattern.Links: var links }:
				{
					if (links.Any(static link => link.GroupedLinkPattern is not null))
					{
						return false;
					}
					break;
				}
				case MultipleForcingChainsStep { Pattern: var pattern }:
				{
					foreach (var (_, branch) in pattern)
					{
						if (branch.Links.Any(static link => link.GroupedLinkPattern is not null))
						{
							return false;
						}
					}
					break;
				}
				case BlossomLoopStep { Pattern: var pattern }:
				{
					foreach (var (_, branch) in pattern)
					{
						if (branch.Links.Any(static link => link.GroupedLinkPattern is not null))
						{
							return false;
						}
					}
					break;
				}
				default:
				{
					return false;
				}
			}
		}
		return true;
	}
}
