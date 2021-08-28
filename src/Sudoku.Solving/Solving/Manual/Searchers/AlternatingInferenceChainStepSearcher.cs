using Sudoku.Solving.Manual.Steps.Chains;

namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Alternating Inference Chain</b> step searcher.
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
internal sealed unsafe class AlternatingInferenceChainStepSearcher : IAlternatingInferenceChainStepSearcher
{
	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(13, DisplayingLevel.B);


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		var list = new List<ChainStep>();
		if (GetAll(list, grid, true, false, onlyFindOne) is { } step1) return step1;
		if (GetAll(list, grid, false, true, onlyFindOne) is { } step2) return step2;
		if (GetAll(list, grid, true, true, onlyFindOne) is { } step3) return step3;

		if (list.Count == 0)
		{
			goto ReturnNull;
		}

		accumulator.AddRange(
			from step in StepAccumulating.Distinct(list)
			orderby step.Difficulty, step.FlatComplexity, (byte)step.SortKey
			select step
		);

	ReturnNull:
		return null;
	}


	private Step? GetAll(
		ICollection<ChainStep> accumulator, in Grid grid, bool xEnabled, bool yEnabled, bool onlyFindOne)
	{
		var listOfUnmanagedAllocations = new List<IntPtr>();
		try
		{
			foreach (int cell in EmptyMap)
			{
				short mask = grid.GetCandidates(cell);
				if (PopCount((uint)mask) >= 2)
				{
					// Iterate on all candidates that aren't alone.
					foreach (int digit in mask)
					{
						var pOn = (ChainNode*)NativeMemory.Alloc((nuint)sizeof(ChainNode));
						*pOn = new(cell, digit, true);

						listOfUnmanagedAllocations.Add((IntPtr)pOn);

						if (DoUnaryChaining(accumulator, grid, pOn, xEnabled, yEnabled, onlyFindOne) is { } step)
						{
							return step;
						}
					}
				}
			}

			return null;
		}
		finally
		{
			// TODO: Release the unmanaged memory if no longer being used.
			//foreach (ChainNode* p in listOfUnmanagedAllocations)
			//{
			//	p->Dispose();
			//}
		}
	}

	private Step? DoUnaryChaining(
		ICollection<ChainStep> accumulator, in Grid grid, [NotNull, DisallowNull] ChainNode* pOn,
		bool xEnabled, bool yEnabled, bool onlyFindOne)
	{
		if (PopCount((uint)grid.GetCandidates(pOn->Cell)) > 2 && !xEnabled)
		{
			// Y-Chains can only start with the bi-value cell.
			goto ReturnNull;
		}

		List<IntPtr> loops = new(), chains = new();
		Set<IntPtr> onToOn = new() { (IntPtr)pOn }, onToOff = new();
		DoLoops(grid, onToOn, onToOff, xEnabled, yEnabled, loops, pOn);

		if (xEnabled)
		{
			// AICs with off implication.
			onToOn = new() { (IntPtr)pOn };
			onToOff = new();
			DoAic(grid, onToOn, onToOff, yEnabled, chains, pOn);

			// AICs with on implication.
			var pOff = new ChainNode(pOn->Cell, pOn->Digit, false);
			onToOn = new();
			onToOff = new() { (IntPtr)(&pOff) };
			DoAic(grid, onToOn, onToOff, yEnabled, chains, &pOff);
		}

		foreach (ChainNode* pDestOn in loops)
		{
			if ((pDestOn->Chain.Count & 1) == 0 && CreateLoopHint(grid, pDestOn, xEnabled, yEnabled) is { } result)
			{
				if (onlyFindOne)
				{
					return result;
				}

				accumulator.Add(result);
			}
		}
		foreach (ChainNode* pTarget in chains)
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
		in Grid grid, ISet<IntPtr> onToOn, ISet<IntPtr> onToOff, bool yEnabled, IList<IntPtr> chains,
		[NotNull, DisallowNull] ChainNode* source)
	{
		List<IntPtr> pendingOn = new(onToOn), pendingOff = new(onToOff);

		while (pendingOn.Count != 0 || pendingOff.Count != 0)
		{
			while (pendingOn.Count != 0)
			{
				var p = (ChainNode*)pendingOn[^1];
				pendingOn.RemoveLastElement();

				var makeOff = IChainStepSearcher.GetOnToOff(grid, p, yEnabled);
				foreach (ChainNode* pOff in makeOff)
				{
					if (source->Cell == pOff->Cell && source->Digit == pOff->Digit && source->IsOn)
					{
						// Loopy contradiction (AIC) found.
						chains.AddIfDoesNotContain((IntPtr)pOff);
					}

					if (!onToOff.Contains((IntPtr)pOff))
					{
						// Not processed yet.
						pendingOff.Add((IntPtr)pOff);
						onToOff.Add((IntPtr)pOff);
					}
				}
			}

			while (pendingOff.Count != 0)
			{
				var p = (ChainNode*)pendingOff[^1];
				pendingOff.RemoveLastElement();

				var makeOn = IChainStepSearcher.GetOffToOn(grid, p, true, yEnabled, true);
				foreach (ChainNode* pOn in makeOn)
				{
					var pOff = new ChainNode(pOn->Cell, pOn->Digit, false);
					if (*source == pOff)
					{
						// Loopy contradiction (AIC) found.
						chains.AddIfDoesNotContain((IntPtr)pOn);
					}

					if (!pOff.IsParentOf(p) && !IChainStepSearcher.ListContainsNode(onToOn, pOn))
					{
						// Not processed yet.
						pendingOn.Add((IntPtr)pOn);
						onToOn.Add((IntPtr)pOn);
					}
				}
			}
		}
	}

	private void DoLoops(
		in Grid grid, ISet<IntPtr> onToOn, ISet<IntPtr> onToOff,
		bool xEnabled, bool yEnabled, IList<IntPtr> loops, [NotNull, DisallowNull] ChainNode* source)
	{
		var pendingOn = new List<IntPtr>(onToOn);
		var pendingOff = new List<IntPtr>(onToOff);

		int length = 0;
		while (pendingOn.Count != 0 || pendingOff.Count != 0)
		{
			length++;
			while (pendingOn.Count != 0)
			{
				var p = (ChainNode*)pendingOn[^1];
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
				var p = (ChainNode*)pendingOff[^1];
				pendingOff.RemoveLastElement();

				var makeOn = IChainStepSearcher.GetOffToOn(grid, p, xEnabled, yEnabled, true);
				foreach (ChainNode* pOn in makeOn)
				{
					if (length >= 4 && *pOn == *source)
					{
						// Loop found.
						loops.Add((IntPtr)pOn);
					}

					if (!IChainStepSearcher.ListContainsNode(onToOn, pOn))
					{
						// Not processed yet.
						pendingOn.AddIfDoesNotContain((IntPtr)pOn);
						onToOn.Add((IntPtr)pOn);
					}
				}
			}
		}
	}

	private ContinuousNiceLoopStep? CreateLoopHint(
		in Grid grid, [NotNull, DisallowNull] ChainNode* destOn, bool xEnabled, bool yEnabled)
	{
		var conclusions = new List<Conclusion>();
		var links = IChainStepSearcher.GetLinks(destOn, true); //! Maybe wrong when adding grouped nodes.
		foreach (var ((start, end, type), _) in links)
		{
			if (type == LinkType.Weak
				&& new Candidates { start, end }.PeerIntersection is { IsEmpty: false } elimMap)
			{
				foreach (int candidate in elimMap)
				{
					if (grid.Exists(candidate / 9, candidate % 9) is true)
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

		var candidateOffsets = IChainStepSearcher.GetCandidateOffsets(destOn, simpleChain: true);

		var (destCandidate, _) = *(ChainNode*)destOn->Chain[^1];
		candidateOffsets.Add((destCandidate, (ColorIdentifier)0));

		return new(
			ImmutableArray.CreateRange(conclusions),
			ImmutableArray.Create(new PresentationData
			{
				Candidates = candidateOffsets,
				Links = links
			}),
			xEnabled,
			yEnabled,
			*destOn
		);
	}

	private AlternatingInferenceChainStep? CreateAicHint(
		in Grid grid, [NotNull, DisallowNull] ChainNode* target, bool xEnabled, bool yEnabled)
	{
		var conclusions = new List<Conclusion>();
		if (!target->IsOn)
		{
			// Get eliminations as an AIC.
			var (startCandidate, _) = *(ChainNode*)target->Chain[1];
			var (endCandidate, _) = *(ChainNode*)target->Chain[^2];
			var elimMap = new Candidates { startCandidate, endCandidate }.PeerIntersection;
			if (elimMap.IsEmpty)
			{
				return null;
			}

			foreach (int candidate in elimMap)
			{
				if (grid.Exists(candidate / 9, candidate % 9) is true)
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
				Candidates = IChainStepSearcher.GetCandidateOffsets(target, simpleChain: true),
				Links = IChainStepSearcher.GetLinks(target)
			}),
			xEnabled,
			yEnabled,
			*target
		);
	}
}
