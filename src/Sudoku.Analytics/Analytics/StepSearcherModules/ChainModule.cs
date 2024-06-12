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
		var isSukaku = grid.PuzzleType == SudokuType.Sukaku;
		var supportedRules =
			from type in linkTypes.GetAllFlags()
			where !isSukaku || type is not (LinkType.AlmostUniqueRectangle or LinkType.AlmostAvoidableRectangle)
			select ChainingRulePool.TryCreate(type)!;
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
	/// The collect method called by chain step searchers.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="accumulator">The instance that temporarily records for chain steps.</param>
	/// <param name="linkTypes">The link types supported in searching.</param>
	/// <param name="stepCreator">The creator method that creates a <see cref="Step"/> instance.</param>
	/// <param name="chainPatternChecker">The checker method that filters the <see cref="ChainPattern"/>.</param>
	/// <returns>The first found step.</returns>
	public static unsafe T? CollectCore<T>(
		ref AnalysisContext context,
		List<T> accumulator,
		LinkType linkTypes,
		delegate*<ChainPattern, Conclusion[], View[], StepSearcherOptions, T> stepCreator,
		delegate*<ChainPattern, bool> chainPatternChecker
	) where T : ChainStep
	{
		ref readonly var grid = ref context.Grid;
		var isSukaku = grid.PuzzleType == SudokuType.Sukaku;
		var supportedRules =
			from type in linkTypes.GetAllFlags()
			where !isSukaku || type is not (LinkType.AlmostUniqueRectangle or LinkType.AlmostAvoidableRectangle)
			select ChainingRulePool.TryCreate(type)!;
		foreach (var foundChain in ChainingDriver.CollectChainPatterns(in context.Grid, supportedRules))
		{
			if (!chainPatternChecker(foundChain))
			{
				continue;
			}

			var step = stepCreator(
				foundChain,
				CollectConclusions(foundChain, in grid, supportedRules),
				CollectViews(in grid, foundChain, supportedRules),
				context.Options
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
	private static ReadOnlySpan<CandidateViewNode> GetNormalCandidateViewNodes(ChainPattern foundChain)
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

	/// <summary>
	/// The backing method to collect views.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="foundChain">The found chain.</param>
	/// <param name="supportedRules">The supported rules.</param>
	/// <returns>The views.</returns>
	private static View[] CollectViews(ref readonly Grid grid, ChainPattern foundChain, ChainingRule[] supportedRules)
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

	/// <summary>
	/// Collect conclusions (especially called by continuous nice loops).
	/// </summary>
	/// <param name="foundChain">The found chain.</param>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="supportedRules">The supported rules.</param>
	/// <returns>The conclusions found.</returns>
	private static Conclusion[] CollectConclusions(ChainPattern foundChain, ref readonly Grid grid, ChainingRule[] supportedRules)
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
}
