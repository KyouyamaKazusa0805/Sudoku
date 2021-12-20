namespace Sudoku.Solving.Manual.Searchers.Chains;

/// <summary>
/// Provides with an <b>Alternating Inference Chain</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Irregular Wings:
/// <list type="bullet">
/// <item>M-Wing</item>
/// <item>Local-Wing</item>
/// <item>Split-Wing</item>
/// <item>Hybrid-Wing</item>
/// <!--<item>Purple Cow</item>-->
/// </list>
/// </item>
/// <item>
/// Basic techniques:
/// <list type="bullet">
/// <item>
/// Alternating Infercence Chains:
/// <list type="bullet">
/// <item>X-Chain</item>
/// <item>XY-Chain</item>
/// <item>XY-X-Chain</item>
/// <item>Discontinuous Nice Loop</item>
/// <item>Normal Alternating Inference Chain</item>
/// </list>
/// </item>
/// <item>
/// Continuous Nice Loops:
/// <list type="bullet">
/// <item>Fishy Cycle (X-Cycle)</item>
/// <item>XY-Cycle</item>
/// <item>Normal Continuous Nice Loop</item>
/// </list>
/// </item>
/// </list>
/// </item>
/// </list>
/// </summary>
[StepSearcher]
public sealed unsafe class AlternatingInferenceChainStepSearcher : IAlternatingInferenceChainStepSearcher
{
	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(13, DisplayingLevel.B);


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		var list = new List<ChainStep>();
		if (GetAll(list, grid, xEnabled: true, yEnabled: false, onlyFindOne) is { } step1) return step1;
		if (GetAll(list, grid, xEnabled: false, yEnabled: true, onlyFindOne) is { } step2) return step2;
		if (GetAll(list, grid, xEnabled: true, yEnabled: true, onlyFindOne) is { } step3) return step3;

		if (list.Count == 0)
		{
			goto ReturnNull;
		}

		accumulator.AddRange(
			from step in IDistinctableStep<ChainStep>.Distinct(list)
			orderby step.Difficulty, step.FlatComplexity, (byte)step.SortKey
			select step
		);

	ReturnNull:
		return null;
	}


	private Step? GetAll(
		ICollection<ChainStep> accumulator,
		in Grid grid,
		bool xEnabled,
		bool yEnabled,
		bool onlyFindOne
	)
	{
		foreach (byte cell in EmptyMap)
		{
			short mask = grid.GetCandidates(cell);
			if (PopCount((uint)mask) >= 2)
			{
				// Iterate on all candidates that aren't alone.
				foreach (byte digit in mask)
				{
					var pOn = new ChainNode(cell, digit, true);
					if (DoUnaryChaining(accumulator, grid, pOn, xEnabled, yEnabled, onlyFindOne) is { } step)
					{
						return step;
					}
				}
			}
		}

		return null;
	}

	private Step? DoUnaryChaining(
		ICollection<ChainStep> accumulator,
		in Grid grid,
		ChainNode pOn,
		bool xEnabled,
		bool yEnabled,
		bool onlyFindOne
	)
	{
		if (PopCount((uint)grid.GetCandidates(pOn.Cell)) > 2 && !xEnabled)
		{
			// Y-Chains can only start with the bi-value cell.
			goto ReturnNull;
		}

		List<ChainNode> loops = new(), chains = new();
		Set<ChainNode> onToOn = new() { pOn }, onToOff = new();
		DoLoops(grid, onToOn, onToOff, xEnabled, yEnabled, loops, pOn);

		if (xEnabled)
		{
			// AICs with off implication.
			onToOn = new() { pOn };
			onToOff = new();
			DoAic(grid, onToOn, onToOff, yEnabled, chains, pOn);

			// AICs with on implication.
			var pOff = new ChainNode(pOn.Cell, pOn.Digit, false);
			onToOn = new();
			onToOff = new() { pOff };
			DoAic(grid, onToOn, onToOff, yEnabled, chains, pOff);
		}

		foreach (var pDestOn in loops)
		{
			if ((pDestOn.WholeChain.Count & 1) == 0
				&& CreateLoopHint(grid, pDestOn, xEnabled, yEnabled) is { } result)
			{
				if (onlyFindOne)
				{
					return result;
				}

				accumulator.Add(result);
			}
		}
		foreach (var pTarget in chains)
		{
			if (CreateAicHint(grid, pTarget, xEnabled, yEnabled) is { } result)
			{
				if (onlyFindOne)
				{
					return result;
				}

				accumulator.Add(result);
			}
		}

	ReturnNull:
		return null;
	}

	private void DoAic(
		in Grid grid,
		ISet<ChainNode> onToOn,
		ISet<ChainNode> onToOff,
		bool yEnabled,
		IList<ChainNode> chains,
		ChainNode source
	)
	{
		List<ChainNode> pendingOn = new(onToOn), pendingOff = new(onToOff);

		while (pendingOn.Count != 0 || pendingOff.Count != 0)
		{
			while (pendingOn.Count != 0)
			{
				var p = pendingOn[^1];
				pendingOn.RemoveLastElement();

				var makeOff = IChainStepSearcher.GetOnToOff(grid, p, yEnabled);
				foreach (var pOff in makeOff)
				{
					var (sourceCell, sourceDigit, sourceIsOn) = source;
					var (pOffCell, pOffDigit, _) = pOff;

					if (sourceCell == pOffCell && sourceDigit == pOffDigit && sourceIsOn)
					{
						// Loopy contradiction (AIC) found.
						chains.AddIfDoesNotContain(pOff);
					}

					if (!onToOff.Contains(pOff))
					{
						// Not processed yet.
						pendingOff.Add(pOff);
						onToOff.Add(pOff);
					}
				}
			}

			while (pendingOff.Count != 0)
			{
				var p = pendingOff[^1];
				pendingOff.RemoveLastElement();

				var makeOn = IChainStepSearcher.GetOffToOn(grid, p, true, yEnabled, true);
				foreach (var pOn in makeOn)
				{
					var pOff = new ChainNode(pOn.Cell, pOn.Digit, false);
					if (source == pOff)
					{
						// Loopy contradiction (AIC) found.
						chains.AddIfDoesNotContain(pOn);
					}

					if (!pOff.IsParentOf(p) && !IChainStepSearcher.ListContainsNode(onToOn, pOn))
					{
						// Not processed yet.
						pendingOn.Add(pOn);
						onToOn.Add(pOn);
					}
				}
			}
		}
	}

	private void DoLoops(
		in Grid grid,
		ISet<ChainNode> onToOn,
		ISet<ChainNode> onToOff,
		bool xEnabled,
		bool yEnabled,
		IList<ChainNode> loops,
		ChainNode source
	)
	{
		List<ChainNode> pendingOn = new(onToOn), pendingOff = new(onToOff);

		int length = 0;
		while (pendingOn.Count != 0 || pendingOff.Count != 0)
		{
			length++;
			while (pendingOn.Count != 0)
			{
				var p = pendingOn[^1];
				pendingOn.RemoveLastElement();

				var makeOff = IChainStepSearcher.GetOnToOff(grid, p, yEnabled);
				foreach (var pOff in makeOff)
				{
					// Not processed yet.
					pendingOff.AddIfDoesNotContain(pOff);
					onToOff.Add(pOff);
				}
			}

			length++;
			while (pendingOff.Count != 0)
			{
				var p = pendingOff[^1];
				pendingOff.RemoveLastElement();

				var makeOn = IChainStepSearcher.GetOffToOn(grid, p, xEnabled, yEnabled, true);
				foreach (var pOn in makeOn)
				{
					if (length >= 4 && pOn == source)
					{
						// Loop found.
						loops.Add(pOn);
					}

					if (!IChainStepSearcher.ListContainsNode(onToOn, pOn))
					{
						// Not processed yet.
						pendingOn.AddIfDoesNotContain(pOn);
						onToOn.Add(pOn);
					}
				}
			}
		}
	}

	private ContinuousNiceLoopStep? CreateLoopHint(in Grid grid, ChainNode destOn, bool xEnabled, bool yEnabled)
	{
		var conclusions = new List<Conclusion>();
		var links = IChainStepSearcher.GetLinks(destOn, true); //! Maybe wrong when adding grouped nodes.
		foreach (var ((start, end, type), _) in links)
		{
			if (type == ChainLinkTypes.Weak
				&& new Candidates { start, end }.PeerIntersection is { IsEmpty: false } elimMap)
			{
				foreach (int candidate in elimMap)
				{
					if (grid.Exists(candidate) is true)
					{
						conclusions.Add(new(ConclusionType.Elimination, candidate));
					}
				}
			}
		}

		if (conclusions.Count == 0)
		{
			return null;
		}

		var candidateOffsets = IChainStepSearcher.GetCandidateOffsets(destOn, true);

		var (destCandidate, _) = destOn.WholeChain[^1];
		candidateOffsets.Add((destCandidate, (ColorIdentifier)0));

		return new(
			ImmutableArray.CreateRange(conclusions),
			ImmutableArray.Create(new PresentationData { Candidates = candidateOffsets, Links = links }),
			xEnabled,
			yEnabled,
			destOn
		);
	}

	private AlternatingInferenceChainStep? CreateAicHint(in Grid grid, ChainNode target, bool xEnabled, bool yEnabled)
	{
		var conclusions = new List<Conclusion>();
		if (!target.IsOn)
		{
			// Get eliminations as an AIC.
			var (startCandidate, _) = target.WholeChain[1];
			var (endCandidate, _) = target.WholeChain[^2];
			var elimMap = new Candidates { startCandidate, endCandidate }.PeerIntersection;
			if (elimMap.IsEmpty)
			{
				return null;
			}

			foreach (int candidate in elimMap)
			{
				if (grid.Exists(candidate) is true)
				{
					conclusions.Add(new(ConclusionType.Elimination, candidate));
				}
			}
		}
		//else
		//{
		//	conclusions.Add(new(Assignment, target.Cell, target.Digit));
		//}

		if (conclusions.Count == 0)
		{
			return null;
		}

		return new(
			ImmutableArray.CreateRange(conclusions),
			ImmutableArray.Create(new PresentationData
			{
				Candidates = IChainStepSearcher.GetCandidateOffsets(target, true),
				Links = IChainStepSearcher.GetLinks(target)
			}),
			xEnabled,
			yEnabled,
			target
		);
	}
}
