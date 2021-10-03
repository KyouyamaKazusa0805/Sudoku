namespace Sudoku.Solving.Manual.Searchers.Chains;

/// <summary>
/// Defines a step searcher that searches for chain steps.
/// </summary>
public unsafe interface IChainStepSearcher : IStepSearcher
{
	/// <summary>
	/// Get highlight candidate offsets through the specified target node.
	/// </summary>
	/// <param name="target">The target node.</param>
	/// <param name="isSimpleAic">
	/// Indicates whether the current caller is in <see cref="IAlternatingInferenceChainStepSearcher"/>.
	/// The default value is <see langword="false"/>.
	/// </param>
	/// <returns>The candidate offsets.</returns>
	/// <seealso cref="IAlternatingInferenceChainStepSearcher"/>
	protected static IList<(int, ColorIdentifier)> GetCandidateOffsets(ChainNode target, bool isSimpleAic = false)
	{
		var result = new List<(int, ColorIdentifier)>();
		var chain = target.WholeChain;
		var (pCandidate, _) = chain[0];
		if (!isSimpleAic)
		{
			result.Add((pCandidate, (ColorIdentifier)2));
		}

		for (int i = 0, count = chain.Count; i < count; i++)
		{
			if (chain[i].Parents is { } parents)
			{
				foreach (var pr in parents)
				{
					var (prCandidate, prIsOn) = pr;
					if (isSimpleAic && i != count - 2 || !isSimpleAic)
					{
						result.AddIfDoesNotContain((prCandidate, (ColorIdentifier)(prIsOn ? 1 : 0)));
					}
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Get the links through the specified target node.
	/// </summary>
	/// <param name="target">The target node.</param>
	/// <param name="showAllLinks">
	/// Indicates whether the current chain will display all chains (even contains the weak links
	/// from the elimination node). The default value is <see langword="false"/>. If you want to
	/// draw the AIC, the elimination weak links don't need drawing.
	/// </param>
	/// <returns>The link.</returns>
	protected static IList<(ChainLink, ColorIdentifier)> GetLinks(ChainNode target, bool showAllLinks = false)
	{
		var result = new List<(ChainLink, ColorIdentifier)>();
		var chain = target.WholeChain;
		for (int i = showAllLinks ? 0 : 1, count = chain.Count - (showAllLinks ? 0 : 2); i < count; i++)
		{
			var p = chain[i];
			var (pCandidate, pIsOn) = p;
			var parents = p.Parents;
			for (int j = 0, parentsCount = parents?.Length ?? 0; j < parentsCount; j++)
			{
				var (prCandidate, prIsOn) = parents![j];
				result.Add(
					(
						new(pCandidate, prCandidate, (A: prIsOn, B: pIsOn) switch
						{
							(A: false, B: true) => ChainLinkTypes.Strong,
							(A: true, B: false) => ChainLinkTypes.Weak,
							_ => ChainLinkTypes.Default
						}),
						(ColorIdentifier)0
					)
				);
			}
		}

		return result;
	}

	/// <summary>
	/// Determine whether the specified list contains the specified node value.
	/// </summary>
	/// <param name="list">The list of nodes.</param>
	/// <param name="node">The node to check.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	protected static bool ListContainsNode(ICollection<ChainNode> list, ChainNode node)
	{
		foreach (var pNode in list)
		{
			if (pNode == node)
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Get all available weak links.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="p">The current node.</param>
	/// <param name="yEnabled">Indicates whether the Y-Chains are enabled.</param>
	/// <returns>All possible weak links.</returns>
	protected static ISet<ChainNode> GetOnToOff(in Grid grid, ChainNode p, bool yEnabled)
	{
		var result = new Set<ChainNode>();

		if (yEnabled)
		{
			// First rule: Other candidates for this cell get off.
			for (byte digit = 0; digit < 9; digit++)
			{
				if (digit != p.Digit && grid.Exists(p.Cell, digit) is true)
				{
					result.Add(new(p.Cell, digit, false, p));
				}
			}
		}

		// Second rule: Other positions for this digit get off.
		for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
		{
			int region = RegionCalculator.ToRegion(p.Cell, label);
			for (int pos = 0; pos < 9; pos++)
			{
				byte cell = (byte)RegionCells[region][pos];
				if (cell != p.Cell && grid.Exists(cell, p.Digit) is true)
				{
					result.Add(new(cell, p.Digit, false, p));
				}
			}
		}

		return result;
	}

#if DEBUG
	/// <summary>
	/// Get all available strong links.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="p">The current node.</param>
	/// <param name="xEnabled">Indicates whether the X-Chains are enabled.</param>
	/// <param name="yEnabled">Indicates whether the Y-Chains are enabled.</param>
	/// <param name="enableFastProperties">
	/// Indicates whether the caller has enabled <see cref="FastProperties"/>.
	/// </param>
	/// <param name="source">The source grid.</param>
	/// <param name="offNodes">All off nodes.</param>
	/// <param name="isDynamic">
	/// Indicates whether the current searcher is searching for dynamic chains. If so,
	/// we can't use those static properties to optimize the performance.
	/// </param>
	/// <returns>All possible strong links.</returns>
#else
	/// <summary>
	/// Get all available strong links.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="p">The current node.</param>
	/// <param name="xEnabled">Indicates whether the X-Chains are enabled.</param>
	/// <param name="yEnabled">Indicates whether the Y-Chains are enabled.</param>
	/// <param name="enableFastProperties">
	/// Indicates whether the caller has enabled <see cref="FastProperties"/>.
	/// </param>
	/// <param name="source">The source grid.</param>
	/// <param name="isDynamic">
	/// Indicates whether the current searcher is searching for dynamic chains. If so,
	/// we can't use those static properties to optimize the performance.
	/// </param>
	/// <returns>All possible strong links.</returns>
#endif
	protected static ISet<ChainNode> GetOffToOn(
		in Grid grid, ChainNode p, bool xEnabled, bool yEnabled,
		bool enableFastProperties, in Grid source = default,
#if DEBUG
		ISet<ChainNode>? offNodes = null,
#endif
		bool isDynamic = false
	)
	{
		var result = new Set<ChainNode>();
		if (yEnabled)
		{
			// First rule: If there's only two candidates in this cell, the other one gets on.
			short mask = (short)(grid.GetCandidates(p.Cell) & ~(1 << p.Digit));
			if (g(grid, p.Cell, isDynamic, enableFastProperties) && mask != 0 && (mask & mask - 1) == 0)
			{
				var pOn = new ChainNode(p.Cell, (byte)TrailingZeroCount(mask), true, p);

				if (
#if DEBUG
					!source.IsUndefined && offNodes is not null
#else
					!source.IsUndefined
#endif
				)
				{
#if DEBUG
					AddHiddenParentsOfCell(ref pOn, grid, source, offNodes);
#else
					AddHiddenParentsOfCell(ref pOn, grid, source);
#endif
				}

				result.Add(pOn);
			}
		}

		if (xEnabled)
		{
			// Second rule: If there's only two positions for this candidate, the other ont gets on.
			for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
			{
				var (pc, pd, _) = p;
				int region = RegionCalculator.ToRegion(pc, label);
				var cells = (h(grid, pd, region, isDynamic, enableFastProperties) & RegionMaps[region]) - pc;
				if (cells.Count == 1)
				{
					var pOn = new ChainNode((byte)cells[0], pd, true, p);

					if (
#if DEBUG
						!source.IsUndefined && offNodes is not null
#else
						!source.IsUndefined
#endif
					)
					{
#if DEBUG
						AddHiddenParentsOfRegion(ref pOn, grid, source, label, offNodes);
#else
						AddHiddenParentsOfRegion(ref pOn, grid, source, label);
#endif
					}

					result.Add(pOn);
				}
			}
		}

		return result;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool g(in Grid grid, int cell, bool isDynamic, bool enableFastProperties) => isDynamic
			? PopCount((uint)grid.GetCandidates(cell)) == 2
			: enableFastProperties ? BivalueMap.Contains(cell) : grid.BivalueCells.Contains(cell);

		static Cells h(in Grid grid, int digit, int region, bool isDynamic, bool enableFastProperties)
		{
			if (!isDynamic)
			{
				// If not dynamic chains, we can use this property to optimize performance.
				return enableFastProperties ? CandMaps[digit] : grid.CandidateMap[digit];
			}

			var result = Cells.Empty;
			for (int i = 0; i < 9; i++)
			{
				int cell = RegionCells[region][i];
				if (grid.Exists(cell, digit) is true)
				{
					result.AddAnyway(cell);
				}
			}

			return result;
		}
	}

#if DEBUG
	/// <summary>
	/// Add hidden parents of a cell.
	/// </summary>
	/// <param name="p">The node.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="source">The source grid.</param>
	/// <param name="offNodes">All off nodes.</param>
	/// <exception cref="Exception">Throws when the parent node of the specified node cannot be found.</exception>
#else
	/// <summary>
	/// Add hidden parents of a cell.
	/// </summary>
	/// <param name="p">The node.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="source">The source grid.</param>
#endif
	private static void AddHiddenParentsOfCell(
#if DEBUG
		ref ChainNode p, in Grid grid, in Grid source, ISet<ChainNode> offNodes
#else
		ref ChainNode p, in Grid grid, in Grid source
#endif
	)
	{
		foreach (byte digit in (short)(source.GetCandidates(p.Cell) & ~grid.GetCandidates(p.Cell)))
		{
			// Add a hidden parent.
			var parent = new ChainNode(p.Cell, digit, false);

#if DEBUG
			bool alreadyContains = false;
			foreach (var pNode in offNodes)
			{
				if (pNode == parent)
				{
					alreadyContains = true;
					break;
				}
			}

			if (!alreadyContains)
			{
				throw new("Parent node can't be found.");
			}
#endif

			p.AddParent(parent);
		}
	}

#if DEBUG
	/// <summary>
	/// Add hidden parents of a region.
	/// </summary>
	/// <param name="p">The node.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="source">The source grid.</param>
	/// <param name="currRegion">The current region label.</param>
	/// <param name="offNodes">All off nodes.</param>
	/// <exception cref="Exception">Throws when the parent node of the specified node cannot be found.</exception>
#else
	/// <summary>
	/// Add hidden parents of a region.
	/// </summary>
	/// <param name="p">The node.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="source">The source grid.</param>
	/// <param name="currRegion">The current region label.</param>
#endif
	private static void AddHiddenParentsOfRegion(
#if DEBUG
		ref ChainNode p, in Grid grid, in Grid source, RegionLabel currRegion, ISet<ChainNode> offNodes
#else
		ref ChainNode p, in Grid grid, in Grid source, RegionLabel currRegion
#endif
	)
	{
		int region = RegionCalculator.ToRegion(p.Cell, currRegion);
		foreach (int pos in (short)(m(source, p.Digit, region) & ~m(grid, p.Digit, region)))
		{
			// Add a hidden parent.
			var parent = new ChainNode((byte)RegionCells[region][pos], p.Digit, false);

#if DEBUG
			bool alreadyContains = false;
			foreach (var pNode in offNodes)
			{
				if (pNode == parent)
				{
					alreadyContains = true;
					break;
				}
			}
			if (!alreadyContains)
			{
				throw new("Parent node can't be found.");
			}
#endif

			p.AddParent(parent);
		}


		static short m(in Grid grid, int digit, int region)
		{
			short result = 0;
			for (int i = 0; i < 9; i++)
			{
				if (grid.Exists(RegionCells[region][i], digit) is true)
				{
					result |= (short)(1 << i);
				}
			}

			return result;
		}
	}
}
