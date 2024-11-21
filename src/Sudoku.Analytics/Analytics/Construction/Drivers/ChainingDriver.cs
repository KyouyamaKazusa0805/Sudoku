namespace Sudoku.Analytics.Construction.Drivers;

/// <summary>
/// Provides a driver that can generate normal chains and forcing chains.
/// </summary>
internal static partial class ChainingDriver
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
		LinkType[] linkTypes = [.. ChainingRule.ElementaryLinkTypes, .. allowsAdvancedLinks ? ChainingRule.AdvancedLinkTypes : []];
		ref readonly var grid = ref context.Grid;
		InitializeLinks(in grid, linkTypes.Aggregate(@delegate.EnumFlagMerger), context.Options, out var supportedRules);

		var cachedAlsIndex = 0;
		foreach (var chain in CollectChains(in context.Grid, allowsAdvancedLinks, context.OnlyFindOne))
		{
			var step = new NormalChainStep(
				CollectChainConclusions(chain, in grid, supportedRules),
				chain.GetViews_Monoparental(in grid, supportedRules, ref cachedAlsIndex),
				context.Options,
				chain
			);
			if (chain.IsStrictlyGrouped ^ allowsAdvancedLinks)
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
	}

	/// <summary>
	/// The collect method called by multiple forcing chains step searcher.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="accumulator">The instance that temporarily records for chain steps.</param>
	/// <param name="allowsAdvancedLinks">Indicates whether the method allows advanced links.</param>
	/// <param name="onlyFindFinnedChain">Indicates whether the method only finds for (grouped) finned chains.</param>
	/// <returns>The first found step.</returns>
	public static unsafe Step? CollectMultipleCore(
		ref StepAnalysisContext context,
		SortedSet<ChainStep> accumulator,
		bool allowsAdvancedLinks,
		bool onlyFindFinnedChain
	)
	{
		return CollectGeneralizedMultipleCore(
			ref context,
			accumulator,
			allowsAdvancedLinks,
			onlyFindFinnedChain,
			&component,
			&CollectMultipleForcingChains,
			&stepCreator
		);


		static MultipleChainBasedComponent component(MultipleForcingChains mfc)
			=> mfc.IsCellMultiple ? MultipleChainBasedComponent.Cell : MultipleChainBasedComponent.House;

		static MultipleForcingChainsStep stepCreator(
			MultipleForcingChains chain,
			ref readonly Grid grid,
			ref readonly StepAnalysisContext context,
			ChainingRuleCollection supportedRules
		) => new(chain.Conclusions, chain.Cast().GetViews(in grid, chain.Conclusions, supportedRules), context.Options, chain);
	}

	/// <summary>
	/// The collect method called by rectangle forcing chains step searcher.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="accumulator">The instance that temporarily records for chain steps.</param>
	/// <param name="allowsAdvancedLinks">Indicates whether the method allows advanced links.</param>
	/// <param name="onlyFindFinnedChain">Indicates whether the method only finds for (grouped) finned chains.</param>
	/// <returns>The first found step.</returns>
	public static unsafe Step? CollectRectangleMultipleCore(
		ref StepAnalysisContext context,
		SortedSet<ChainStep> accumulator,
		bool allowsAdvancedLinks,
		bool onlyFindFinnedChain
	)
	{
		return CollectGeneralizedMultipleCore(
			ref context,
			accumulator,
			allowsAdvancedLinks,
			onlyFindFinnedChain,
			&component,
			&CollectRectangleMultipleForcingChains,
			&stepCreator
		);


		static MultipleChainBasedComponent component(MultipleForcingChains mfc) => MultipleChainBasedComponent.Rectangle;

		static RectangleForcingChainsStep stepCreator(
			RectangleForcingChains chain,
			ref readonly Grid grid,
			ref readonly StepAnalysisContext context,
			ChainingRuleCollection supportedRules
		) => new(chain.Conclusions, chain.Cast().GetViews(in grid, chain.Conclusions, supportedRules), context.Options, chain);
	}

	/// <summary>
	/// The collect method called by rectangle forcing chains step searcher.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="accumulator">The instance that temporarily records for chain steps.</param>
	/// <param name="allowsAdvancedLinks">Indicates whether the method allows advanced links.</param>
	/// <param name="onlyFindFinnedChain">Indicates whether the method only finds for (grouped) finned chains.</param>
	/// <returns>The first found step.</returns>
	public static unsafe Step? CollectBivalueUniversalGraveMultipleCore(
		ref StepAnalysisContext context,
		SortedSet<ChainStep> accumulator,
		bool allowsAdvancedLinks,
		bool onlyFindFinnedChain
	)
	{
		return CollectGeneralizedMultipleCore(
			ref context,
			accumulator,
			allowsAdvancedLinks,
			onlyFindFinnedChain,
			&component,
			&CollectBivalueUniversalGraveMultipleForcingChains,
			&stepCreator
		);


		static MultipleChainBasedComponent component(MultipleForcingChains mfc) => MultipleChainBasedComponent.BivalueUniversalGrave;

		static BivalueUniversalGraveForcingChainsStep stepCreator(
			BivalueUniversalGraveForcingChains chain,
			ref readonly Grid grid,
			ref readonly StepAnalysisContext context,
			ChainingRuleCollection supportedRules
		) => new(chain.Conclusions, chain.Cast().GetViews(in grid, chain.Conclusions, supportedRules), context.Options, chain);
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
		LinkType[] linkTypes = [.. ChainingRule.ElementaryLinkTypes, .. allowsAdvancedLinks ? ChainingRule.AdvancedLinkTypes : []];
		ref readonly var grid = ref context.Grid;
		InitializeLinks(in grid, linkTypes.Aggregate(@delegate.EnumFlagMerger), context.Options, out var supportedRules);

		foreach (var blossomLoop in CollectBlossomLoops(in context.Grid, context.OnlyFindOne, supportedRules))
		{
			var step = new BlossomLoopStep(
				blossomLoop.Conclusions.ToArray(),
				getViews(blossomLoop, in grid, supportedRules),
				context.Options,
				blossomLoop
			);
			if (context.OnlyFindOne)
			{
				return step;
			}

			accumulator.Add(step);
		}
		return null;


		static View[] getViews(BlossomLoop blossomLoop, ref readonly Grid grid, ChainingRuleCollection supportedRules)
		{
			var globalView = View.Empty;
			var otherViews = new View[blossomLoop.Count];
			otherViews.InitializeArray(static ([NotNull] ref View? view) => view = View.Empty);

			var (i, cachedAlsIndex) = (0, 0);
			foreach (var (startCandidate, branch) in blossomLoop)
			{
				var viewNodes = branch.GetViews_Monoparental(in grid, supportedRules, ref cachedAlsIndex)[0];
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

	/// <summary>
	/// The collect method called by dynamic forcing chains step searchers.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="accumulator">The instance that temporarily records for chain steps.</param>
	/// <returns>The first found step.</returns>
	public static Step? CollectDynamicForcingChainsCore(ref StepAnalysisContext context, List<PatternBasedChainStep> accumulator)
	{
		var linkTypes = ChainingRule.ElementaryLinkTypes.Aggregate(@delegate.EnumFlagMerger);
		ref readonly var grid = ref context.Grid;
		InitializeLinks(in grid, linkTypes, context.Options, out var supportedRules);

		foreach (var forcingChains in CollectDynamicForcingChains(in grid, in context, supportedRules))
		{
			PatternBasedChainStep step = forcingChains switch
			{
				BinaryForcingChains b => new BinaryForcingChainsStep(
					new SingletonArray<Conclusion>(b.Conclusion),
					b.Cast().GetViews(in grid, [b.Conclusion], supportedRules),
					context.Options,
					b
				),
				MultipleForcingChains m => new MultipleForcingChainsStep(
					m.Conclusions,
					m.Cast().GetViews(in grid, m.Conclusions, supportedRules),
					context.Options,
					m
				)
			};
			if (context.OnlyFindOne)
			{
				return step;
			}

			accumulator.Add(step);
		}
		return null;
	}

	/// <summary>
	/// The internal method that can collect for general-typed multiple forcing chains.
	/// </summary>
	/// <typeparam name="TMultipleForcingChains">The type of multiple forcing chains.</typeparam>
	/// <typeparam name="TMultipleForcingChainsStep">The type of target step can be created.</typeparam>
	/// <param name="context">The context.</param>
	/// <param name="accumulator">The instance that temporarily records for chain steps.</param>
	/// <param name="allowsAdvancedLinks">Indicates whether the method allows advanced links.</param>
	/// <param name="onlyFindFinnedChain">Indicates whether the method only finds for (grouped) finned chains.</param>
	/// <param name="componentCreator">Indicates the component that the current forcing chains pattern is.</param>
	/// <param name="chainsCollector">
	/// The collector method that can find a list of <typeparamref name="TMultipleForcingChains"/> instances in the grid.
	/// </param>
	/// <param name="stepCreator">The creator method that can create a chain step.</param>
	/// <returns>The first found step.</returns>
	private static unsafe Step? CollectGeneralizedMultipleCore<TMultipleForcingChains, TMultipleForcingChainsStep>(
		ref StepAnalysisContext context,
		SortedSet<ChainStep> accumulator,
		bool allowsAdvancedLinks,
		bool onlyFindFinnedChain,
		delegate*<TMultipleForcingChains, MultipleChainBasedComponent> componentCreator,
		delegate*<ref readonly Grid, bool, ReadOnlySpan<TMultipleForcingChains>> chainsCollector,
		delegate*<TMultipleForcingChains, ref readonly Grid, ref readonly StepAnalysisContext, ChainingRuleCollection, TMultipleForcingChainsStep> stepCreator
	)
		where TMultipleForcingChains : MultipleForcingChains
		where TMultipleForcingChainsStep : PatternBasedChainStep
	{
		LinkType[] l = [.. ChainingRule.ElementaryLinkTypes, .. allowsAdvancedLinks ? ChainingRule.AdvancedLinkTypes : []];
		var linkTypes = l.Aggregate(@delegate.EnumFlagMerger);
		ref readonly var grid = ref context.Grid;
		InitializeLinks(in grid, linkTypes, context.Options, out var supportedRules);

		foreach (var chain in chainsCollector(in context.Grid, context.OnlyFindOne))
		{
			var cachedAlsIndex = 0;
			if (onlyFindFinnedChain && chain.TryCastToFinnedChain(out var finnedChain, out var f))
			{
				ref readonly var fins = ref Nullable.GetValueRefOrDefaultRef(in f);
				chain.PrepareFinnedChainViewNodes(finnedChain, ref cachedAlsIndex, supportedRules, in grid, in fins, out var views);

				var finnedChainStep = new FinnedChainStep(
					chain.Conclusions,
					views,
					context.Options,
					finnedChain,
					in fins,
					componentCreator(chain)
				);
				if (finnedChain.IsStrictlyGrouped ^ allowsAdvancedLinks)
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
				var rfcStep = stepCreator(chain, in grid, in context, supportedRules);
				if (context.OnlyFindOne)
				{
					return rfcStep;
				}

				accumulator.Add(rfcStep);
			}
		}
		return null;
	}

	/// <summary>
	/// The backing method to collect chain conclusions.
	/// </summary>
	/// <param name="pattern">The pattern.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="supportedRules">All supported rules used.</param>
	/// <returns>Found conclusions.</returns>
	[InterceptorMethodCaller]
	[InterceptorInstanceTypes(
		typeof(AlmostLockedSetsChainingRule), typeof(KrakenNormalFishChainingRule), typeof(LockedCandidatesChainingRule),
		DefaultBehavior = InterceptorInstanceRoutingDefaultBehavior.DoNothingOrReturnDefault)]
	private static Conclusion[] CollectChainConclusions(NamedChain pattern, ref readonly Grid grid, ChainingRuleCollection supportedRules)
	{
		var conclusions = pattern.GetConclusions(in grid);
		if (pattern is ContinuousNiceLoop { Links: var links })
		{
			var context = new ChainingRuleLoopConclusionContext(in grid, links);
			foreach (var rule in supportedRules)
			{
				rule.GetLoopConclusions(ref context);
				conclusions |= context.Conclusions;
			}
		}
		return [.. conclusions];
	}
}

/// <summary>
/// Provides a file-local type that can simplify casting expression of <see cref="IForcingChains"/>.
/// </summary>
/// <seealso cref="IForcingChains"/>
file static class ForcingChainsCastingExtensions
{
	/// <summary>
	/// Cast the object into <see cref="IForcingChains"/> regardless of its base type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of <paramref name="this"/>.</typeparam>
	/// <param name="this">The current instance.</param>
	/// <returns>Casted <see cref="IForcingChains"/> instance.</returns>
	public static IForcingChains Cast<T>(this T @this) where T : IForcingChains => @this;
}
