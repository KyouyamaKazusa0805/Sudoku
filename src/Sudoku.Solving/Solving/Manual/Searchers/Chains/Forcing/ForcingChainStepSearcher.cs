namespace Sudoku.Solving.Manual.Searchers.Chains.Forcing;

/// <summary>
/// Provides with a <b>Forcing Chains</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Multiple Forcing Chains:
/// <list type="bullet">
/// <item>Cell Forcing Chains</item>
/// <item>Region Forcing Chains</item>
/// </list>
/// </item>
/// <item>
/// Dynamic Multiple Forcing Chains:
/// <list type="bullet">
/// <item>Dynamic Cell Forcing Chains</item>
/// <item>Dynamic Region Forcing Chains</item>
/// </list>
/// </item>
/// <item>
/// Dynamic Verifying Forcing Chains:
/// <list type="bullet">
/// <item>Dynamic Contradiction Forcing Chains</item>
/// <item>Dynamic Double Forcing Chains</item>
/// </list>
/// </item>
/// </list>
/// </summary>
[StepSearcher]
public unsafe class ForcingChainStepSearcher : IForcingChainStepSearcher, IDynamicForcingChainStepSearcher
{
	/// <summary>
	/// Indicates the grid that is used in processing.
	/// </summary>
	private Grid _temp;


	/// <inheritdoc/>
	public bool IsNishio { get; set; }

	/// <inheritdoc/>
	public bool IsMultiple { get; set; }

	/// <inheritdoc/>
	public bool IsDynamic { get; set; }

	/// <inheritdoc/>
	public byte Level { get; set; }

	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(30, DisplayingLevel.C);


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		var tempGrid = grid;
		var tempAccumulator = new List<ChainStep>();
		GetAll(tempAccumulator, ref tempGrid);

		switch (tempAccumulator)
		{
			case []:
			{
				return null;
			}
			case [var firstStep] when onlyFindOne:
			{
				return firstStep;
			}
			default:
			{
				accumulator.AddRange(
					from info in IDistinctableStep<ChainStep>.Distinct(tempAccumulator)
					orderby info.Difficulty, info.Complexity, info.SortKey
					select info
				);

				return null;
			}
		}
	}

	/// <summary>
	/// Search for chains of each type.
	/// </summary>
	/// <param name="accumulator">The accumulator.</param>
	/// <param name="grid">The grid.</param>
	private void GetAll(ICollection<ChainStep> accumulator, ref Grid grid)
	{
		// Iterate on empty cells.
		foreach (byte cell in EmptyMap)
		{
			short mask = grid.GetCandidates(cell);
			int count = PopCount((uint)mask);
			switch (count)
			{
				case > 2:
				case > 1 when IsDynamic:
				{
					// Prepare storage and accumulator for cell eliminations.
					Dictionary<int, NodeSet> valueToOn = new(), valueToOff = new();
					NodeSet cellToOn = NodeSet.Uninitialized, cellToOff = NodeSet.Uninitialized;

					// Iterate on all candidates that aren't alone.
					foreach (byte digit in mask)
					{
						Node pOn = new(cell, digit, true), pOff = new(cell, digit, false);
						NodeSet onToOn = new(), onToOff = new();

						bool doDouble = count >= 3 && !IsNishio && IsDynamic;
						bool doContradiction = IsDynamic || IsNishio;

						DoBinaryChaining(
							accumulator, ref grid, pOn, pOff, ref onToOn, ref onToOff,
							doDouble, doContradiction
						);

						if (!IsNishio)
						{
							DoRegionChaining(accumulator, ref grid, cell, digit, ref onToOn, ref onToOff);
						}

						// Collect results for cell chaining.
						valueToOn.Add(digit, onToOn);
						valueToOff.Add(digit, onToOff);
						if (cellToOn.IsUninitialized || cellToOff.IsUninitialized)
						{
							cellToOn = new(onToOn);
							cellToOff = new(onToOff);
						}
						else
						{
							cellToOn &= onToOn;
							cellToOff &= onToOff;
						}
					}

					// Do cell eliminations.
					if (!IsNishio && (count == 2 || IsMultiple))
					{
						if (!cellToOn.IsUninitialized)
						{
							foreach (var p in cellToOn)
							{
								var hint = CreateCellFcHint(grid, cell, p, valueToOn);
								accumulator.Add(hint);
							}
						}
						if (!cellToOff.IsUninitialized)
						{
							foreach (var p in cellToOff)
							{
								var hint = CreateCellFcHint(grid, cell, p, valueToOff);
								accumulator.Add(hint);
							}
						}
					}
					break;
				}
			}
		}
	}

	/// <summary>
	/// Do binary chaining.
	/// </summary>
	/// <param name="accumulator">The current accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="pOn">The on node.</param>
	/// <param name="pOff">The off node.</param>
	/// <param name="onToOn">The list for <c>on</c> nodes to <c>on</c> nodes.</param>
	/// <param name="onToOff">The list for <c>on</c> nodes to <c>off</c> nodes.</param>
	/// <param name="doReduction">Indicates whether the method executes double chaining.</param>
	/// <param name="doContradiction">Indicates whether the method executes contradiction chaining.</param>
	private void DoBinaryChaining(
		ICollection<ChainStep> accumulator,
		ref Grid grid,
		in Node pOn,
		in Node pOff,
		ref NodeSet onToOn,
		ref NodeSet onToOff,
		bool doReduction,
		bool doContradiction
	)
	{
		NodeSet offToOn = new(), offToOff = new();

		// Circular Forcing Chains (hypothesis implying its negation)
		// are already covered by Cell Forcing Chains, and are therefore
		// not checked for.

		// Test 'p' is on.
		onToOn.Add(pOn);
		var absurdNodes = DoChaining(ref grid, ref onToOn, ref onToOff);
		if (doContradiction && absurdNodes is var (on1, off1))
		{
			// 'p' can't hold its value, otherwise it'd lead to a contradiction.
			var hint = CreateChainingOffHint(on1, off1, pOn, pOn, true);
			accumulator.Add(hint);
		}

		// Test 'p' is off.
		offToOff.Add(pOff);
		absurdNodes = DoChaining(ref grid, ref offToOn, ref offToOff);
		if (doContradiction && absurdNodes is var (on2, off2))
		{
			// 'p' must hold its value, otherwise it'd lead to a contradiction.
			var hint = CreateChainingOnHint(on2, off2, pOff, pOff, true);
			accumulator.Add(hint);
		}

		if (doReduction)
		{
			// Check candidates that must be on in both cases.
			foreach (var pFromOn in onToOn)
			{
				ref var pFromOff = ref offToOn[pFromOn];
				if (!Unsafe.IsNullRef(ref pFromOff))
				{
					var hint = CreateChainingOnHint(pFromOn, pFromOff, pOn, pFromOn, false);
					accumulator.Add(hint);
				}
			}

			// Check candidates that must be off in both cases.
			foreach (var pFromOn in onToOff)
			{
				ref var pFromOff = ref offToOff[pFromOn];
				if (!Unsafe.IsNullRef(ref pFromOff))
				{
					var hint = CreateChainingOffHint(pFromOn, pFromOff, pOff, pFromOff, false);
					accumulator.Add(hint);
				}
			}
		}
	}

	/// <summary>
	/// Do region chaining.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid"></param>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	/// <param name="onToOn">The list for <c>on</c> nodes to <c>on</c> nodes.</param>
	/// <param name="onToOff">The list for <c>on</c> nodes to <c>off</c> nodes.</param>
	private void DoRegionChaining(
		ICollection<ChainStep> accumulator,
		ref Grid grid,
		byte cell,
		byte digit,
		ref NodeSet onToOn,
		ref NodeSet onToOff
	)
	{
		for (var label = RegionLabels.Block; label <= RegionLabels.Column; label++)
		{
			int region = RegionLabel.ToRegion(cell, label);
			var worthMap = CandMaps[digit] & RegionMaps[region];
			switch (worthMap)
			{
				case [_, _]:
				case [_, _, ..] when IsMultiple:
				{
					if (worthMap[0] == cell)
					{
						// Determine whether we meet this region for the first time.
						Dictionary<int, NodeSet> posToOn = new(), posToOff = new();
						NodeSet regionToOn = new(), regionToOff = new();

						// Iterate on node positions within the region.
						foreach (byte otherCell in worthMap)
						{
							if (otherCell == cell)
							{
								posToOn.Add(otherCell, onToOn);
								posToOff.Add(otherCell, onToOff);
								regionToOn |= onToOn;
								regionToOff |= onToOff;
							}
							else
							{
								var other = new Node(otherCell, digit, true);
								NodeSet otherToOn = new() { other }, otherToOff = new();

								DoChaining(ref grid, ref otherToOn, ref otherToOff);

								posToOn.Add(otherCell, otherToOn);
								posToOff.Add(otherCell, otherToOff);
								regionToOn &= otherToOn;
								regionToOff &= otherToOff;
							}
						}

						// Gather results.
						foreach (var p in regionToOn)
						{
							var hint = CreateRegionFcHint(region, digit, p, posToOn);
							accumulator.Add(hint);
						}
						foreach (var p in regionToOff)
						{
							var hint = CreateRegionFcHint(region, digit, p, posToOff);
							accumulator.Add(hint);
						}
					}

					break;
				}
			}
		}
	}

	/// <summary>
	/// Do chaining (i.e. multiple chaining).
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="toOn">The list to <c>on</c> nodes.</param>
	/// <param name="toOff">The list to <c>off</c> nodes.</param>
	/// <returns>The result.</returns>
	private (Node On, Node Off)? DoChaining(
		ref Grid grid,
		ref NodeSet toOn,
		ref NodeSet toOff
	)
	{
		_temp = grid;

		try
		{
			NodeSet pendingOn = new(toOn), pendingOff = new(toOff);
			while (pendingOn.Count != 0 || pendingOff.Count != 0)
			{
				if (pendingOn.Count != 0)
				{
					var p = pendingOn.Remove();
					var makeOff = IChainStepSearcher.GetOnToOff(grid, p, !IsNishio);

					foreach (var pOff in makeOff)
					{
						var pOn = new Node(pOff.Cell, pOff.Digit, true); // Conjugate
						ref var pOnResult = ref toOn[pOn];
						if (!Unsafe.IsNullRef(ref pOnResult))
						{
							// Contradiction found.
							return (pOnResult, pOff); // Cannot be both on and off at the same time.
						}
						else if (!toOff.Contains(pOff))
						{
							// Not processed yet.
							toOff.Add(pOff);
							pendingOff.Add(pOff);
						}
					}
				}
				else
				{
					var p = pendingOff.Remove();
					var makeOn =
#if DEBUG
						IChainStepSearcher.GetOffToOn(grid, p, true, !IsNishio, true, _temp, toOff, IsDynamic);
#else
						IChainStepSearcher.GetOffToOn(grid, p, true, !IsNishio, true, _temp, IsDynamic);
#endif

					if (IsDynamic)
					{
						// Remove that digit.
						grid[p.Cell, p.Digit] = false;
					}

					foreach (var pOn in makeOn)
					{
						var pOff = new Node(pOn.Cell, pOn.Digit, false); // Conjugate.
						ref var pOffResult = ref toOff[pOff];
						if (!Unsafe.IsNullRef(ref pOffResult))
						{
							// Contradiction found.
							return (pOn, pOffResult); // Cannot be both on and off at the same time.
						}
						else if (!toOn.Contains(pOn))
						{
							// Not processed yet.
							toOn.Add(pOn);
							pendingOn.Add(pOn);
						}
					}
				}
			}

			return null;
		}
		finally
		{
			grid = _temp;
		}
	}

	/// <summary>
	/// Create a chaining hint whose conclusion is an on result.
	/// </summary>
	/// <param name="destOn">The destination on node.</param>
	/// <param name="destOff">The destination off node.</param>
	/// <param name="source">The source node.</param>
	/// <param name="target">The target node.</param>
	/// <param name="isAbsurd">Indicates whether the chain is absurd.</param>
	/// <returns>The hint.</returns>
	private BinaryChainingStep CreateChainingOnHint(
		in Node destOn,
		in Node destOff,
		in Node source,
		in Node target,
		bool isAbsurd
	)
	{
		// Get views.
		var appendedInfo = (target.Cell * 9 + target.Digit, (ColorIdentifier)0);
		var cellOffset = new[] { ((int)destOn.Cell, (ColorIdentifier)0) };
		var views = new List<PresentationData>();
		var globalCandidates = new List<(int, ColorIdentifier)> { appendedInfo };
		var globalLinks = new List<(Link, ColorIdentifier)>();

		var candidateOffsets = new List<(int, ColorIdentifier)>(IChainStepSearcher.GetCandidateOffsets(destOn))
		{
			appendedInfo
		};
		var links = IChainStepSearcher.GetLinks(destOn, true);
		foreach (var candidateOffset in candidateOffsets)
		{
			if (!globalCandidates.Contains(candidateOffset))
			{
				globalCandidates.Add(candidateOffset);
			}
		}
		globalLinks.AddRange(links);
		views.Add(new()
		{
			Cells = cellOffset,
			Candidates = candidateOffsets,
			Links = links
		});

		candidateOffsets = new List<(int, ColorIdentifier)>(IChainStepSearcher.GetCandidateOffsets(destOff))
		{
			appendedInfo
		};
		links = IChainStepSearcher.GetLinks(destOff, true);
		foreach (var candidateOffset in candidateOffsets)
		{
			if (!globalCandidates.Contains(candidateOffset))
			{
				globalCandidates.Add(candidateOffset);
			}
		}
		globalLinks.AddRange(links);
		views.Add(new()
		{
			Cells = cellOffset,
			Candidates = candidateOffsets,
			Links = links
		});

		// Insert the global view at head.
		views.Insert(0, new()
		{
			Cells = cellOffset,
			Candidates = globalCandidates,
			Links = globalLinks
		});

		return new(
			ImmutableArray.Create(new Conclusion(ConclusionType.Assignment, target.Cell, target.Digit)),
			ImmutableArray.CreateRange(views),
			source,
			destOn,
			destOff,
			isAbsurd,
			IsMultiple,
			IsNishio,
			Level
		);
	}

	/// <summary>
	/// Create a chaining hint whose conclusion is an off result.
	/// </summary>
	/// <param name="destOn">The destination on node.</param>
	/// <param name="destOff">The destination off node.</param>
	/// <param name="source">The source node.</param>
	/// <param name="target">The target node.</param>
	/// <param name="isAbsurd">Indicates whether the chain is absurd.</param>
	/// <returns>The hint.</returns>
	private BinaryChainingStep CreateChainingOffHint(
		in Node destOn,
		in Node destOff,
		in Node source,
		in Node target,
		bool isAbsurd
	)
	{
		// Get views.
		var cellOffset = new[] { ((int)destOn.Cell, (ColorIdentifier)0) };
		var views = new List<PresentationData>();
		var globalCandidates = new List<(int, ColorIdentifier)>();
		var globalLinks = new List<(Link, ColorIdentifier)>();

		var candidateOffsets = IChainStepSearcher.GetCandidateOffsets(destOn);
		var links = IChainStepSearcher.GetLinks(destOn, true);
		foreach (var candidateOffset in candidateOffsets)
		{
			if (!globalCandidates.Contains(candidateOffset))
			{
				globalCandidates.Add(candidateOffset);
			}
		}
		globalLinks.AddRange(links);
		views.Add(new()
		{
			Cells = cellOffset,
			Candidates = candidateOffsets,
			Links = links
		});

		candidateOffsets = IChainStepSearcher.GetCandidateOffsets(destOff);
		links = IChainStepSearcher.GetLinks(destOff, true);
		foreach (var candidateOffset in candidateOffsets)
		{
			if (!globalCandidates.Contains(candidateOffset))
			{
				globalCandidates.Add(candidateOffset);
			}
		}
		globalLinks.AddRange(links);
		views.Add(new() { Cells = cellOffset, Candidates = candidateOffsets, Links = links });

		// Insert the global view at head.
		views.Insert(0, new() { Cells = cellOffset, Candidates = globalCandidates, Links = globalLinks });

		return new(
			ImmutableArray.Create(new Conclusion(ConclusionType.Elimination, target.Cell, target.Digit)),
			ImmutableArray.CreateRange(views),
			source,
			destOn,
			destOff,
			isAbsurd,
			IsMultiple,
			IsNishio,
			Level
		);
	}

	/// <summary>
	/// Create the hint for cell forcing chains.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="sourceCell">The source cell.</param>
	/// <param name="target">The target elimination node.</param>
	/// <param name="outcomes">All outcomes (conclusions).</param>
	/// <returns>The information instance.</returns>
	private CellChainingStep CreateCellFcHint(
		in Grid grid,
		int sourceCell,
		in Node target,
		Dictionary<int, NodeSet> outcomes
	)
	{
		var (targetCandidate, targetIsOn) = target;

		// Build chains.
		var chains = new Dictionary<int, Node>();
		foreach (int digit in grid.GetCandidates(sourceCell))
		{
			// Get the node that contains the same cell, digit and isOn property.
			var valueTarget = outcomes[digit][target];
			chains.Add(digit, valueTarget);
		}

		// Get views.
		var views = new List<PresentationData>();
		var globalCandidates = new List<(int, ColorIdentifier)> { (targetCandidate, (ColorIdentifier)0) };
		var globalLinks = new List<(Link, ColorIdentifier)>();
		foreach (var (digit, node) in chains)
		{
			var candidateOffsets = new List<(int, ColorIdentifier)>(IChainStepSearcher.GetCandidateOffsets(node))
			{
				(sourceCell * 9 + digit, (ColorIdentifier)2),
				(targetCandidate, (ColorIdentifier)0)
			};

			var links = IChainStepSearcher.GetLinks(node, true);
			views.Add(new()
			{
				Cells = new[] { (sourceCell, (ColorIdentifier)0) },
				Candidates = candidateOffsets,
				Links = links
			});
			candidateOffsets.RemoveAt(candidateOffsets.Count - 1);
			foreach (var candidateOffset in candidateOffsets)
			{
				if (!globalCandidates.Contains(candidateOffset))
				{
					globalCandidates.Add(candidateOffset);
				}
			}
			globalLinks.AddRange(links);
		}

		// Insert the global view at head.
		views.Insert(0, new()
		{
			Cells = new[] { (sourceCell, (ColorIdentifier)0) },
			Candidates = globalCandidates,
			Links = globalLinks
		});

		return new(
			ImmutableArray.Create(
				new Conclusion(
					targetIsOn ? ConclusionType.Assignment : ConclusionType.Elimination,
					target.Cell,
					target.Digit
				)
			),
			ImmutableArray.CreateRange(views),
			sourceCell,
			chains,
			IsDynamic,
			Level
		);
	}

	/// <summary>
	/// Create a hint of region forcing chains.
	/// </summary>
	/// <param name="region">The region.</param>
	/// <param name="digit">The digit.</param>
	/// <param name="target">The target node.</param>
	/// <param name="outcomes">All outcomes (conclusions).</param>
	/// <returns>The technique information instance.</returns>
	private RegionChainingStep CreateRegionFcHint(
		int region,
		int digit,
		in Node target,
		Dictionary<int, NodeSet> outcomes
	)
	{
		var (targetCandidate, targetIsOn) = target;

		// Build chains.
		var chains = new Dictionary<int, Node>();
		var map = RegionMaps[region] & CandMaps[digit];
		foreach (int cell in map)
		{
			// Get the node that contains the same cell, digit and isOn property.
			var posTarget = outcomes[cell][target];
			chains.Add(cell, posTarget);
		}

		// Get views.
		var views = new List<PresentationData>();
		var globalCandidates = new List<(int, ColorIdentifier)> { (targetCandidate, (ColorIdentifier)0) };
		var globalLinks = new List<(Link, ColorIdentifier)>();
		foreach (var (cell, node) in chains)
		{
			var candidateOffsets = new List<(int, ColorIdentifier)>(IChainStepSearcher.GetCandidateOffsets(node))
			{
				(cell * 9 + digit, (ColorIdentifier)2),
				(targetCandidate, (ColorIdentifier)0)
			};

			var links = IChainStepSearcher.GetLinks(node, true);
			views.Add(new()
			{
				Candidates = candidateOffsets,
				Regions = new[] { (region, (ColorIdentifier)0) },
				Links = links
			});
			candidateOffsets.RemoveAt(candidateOffsets.Count - 1);
			foreach (var candidateOffset in candidateOffsets)
			{
				if (!globalCandidates.Contains(candidateOffset))
				{
					globalCandidates.Add(candidateOffset);
				}
			}
			globalLinks.AddRange(links);
		}

		views.Insert(0, new()
		{
			Candidates = globalCandidates,
			Regions = new[] { (region, (ColorIdentifier)0) },
			Links = globalLinks
		});

		return new(
			ImmutableArray.Create(
				new Conclusion(
					targetIsOn ? ConclusionType.Assignment : ConclusionType.Elimination,
					target.Cell,
					target.Digit
				)
			),
			ImmutableArray.CreateRange(views),
			region,
			digit,
			chains,
			IsDynamic,
			Level
		);
	}
}
