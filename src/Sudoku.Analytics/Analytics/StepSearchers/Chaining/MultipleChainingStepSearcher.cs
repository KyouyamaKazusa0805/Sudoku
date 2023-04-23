namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Chain</b> step searcher using same algorithm with <b>Chaining</b> used by a program called Sudoku Explainer.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Multiple Forcing Chains:
/// <list type="bullet">
/// <item>Cell Forcing Chains</item>
/// <item>Region (House) Forcing Chains</item>
/// </list>
/// </item>
/// <item>
/// Dynamic Forcing Chains:
/// <list type="bullet">
/// <item>Dynamic Cell Forcing Chains</item>
/// <item>Dynamic Region (House) Forcing Chains</item>
/// <item>Dynamic Contradiction Forcing Chains</item>
/// <item>Dynamic Double Forcing Chains</item>
/// </list>
/// </item>
/// </list>
/// </summary>
/// <remarks>
/// The type is special: it uses source code from another project called Sudoku Explainer.
/// However unfortunately, I cannot find any sites available of the project.
/// One of the original website is <see href="https://diuf.unifr.ch/pai/people/juillera/Sudoku/Sudoku.html">this link</see> (A broken link).
/// </remarks>
[StepSearcher, Polymorphism]
[Separated(0, nameof(AllowNishio), true, nameof(AllowDynamic), true)]
[Separated(1, nameof(AllowMultiple), true)]
[Separated(2, nameof(AllowMultiple), true, nameof(AllowDynamic), true)]
public partial class MultipleChainingStepSearcher : ChainingStepSearcher
{
	/// <summary>
	/// Indicates whether the step searcher allows nishio forcing chains, which is equivalent to a dynamic forcing chains
	/// that only uses a single digit. It is a brute-force view of a fish.
	/// </summary>
	public bool AllowNishio { get; init; }

	/// <summary>
	/// Indicates whether the step searcher allows multiple forcing chains:
	/// <list type="bullet">
	/// <item>
	/// For non-dynamic forcing chains:
	/// <list type="bullet">
	/// <item>Cell forcing chains</item>
	/// <item>Region (House) forcing chains</item>
	/// </list>
	/// </item>
	/// <item>
	/// For dynamic forcing chains:
	/// <list type="bullet">
	/// <item>Dynamic cell forcing chains</item>
	/// <item>Dynamic region (house) forcing chains</item>
	/// </list>
	/// </item>
	/// </list>
	/// </summary>
	public bool AllowMultiple { get; init; }

	/// <summary>
	/// Indicates whether the step searcher allows dynamic forcing chains:
	/// <list type="bullet">
	/// <item>Dynamic contradiction forcing chains</item>
	/// <item>Dynamic double forcing chains</item>
	/// </list>
	/// </summary>
	/// <remarks>
	/// If step searcher enables for dynamic forcing chains, forcing chains will contain branches,
	/// or even branches over branches (recursively). It will be very useful on complex inferences.
	/// </remarks>
	public bool AllowDynamic { get; init; }


	/// <inheritdoc/>
	protected internal sealed override Step? Collect(scoped ref AnalysisContext context)
	{
		// TODO: Implement an implications cache.

		scoped ref readonly var grid = ref context.Grid;
		var result = GetAll(grid);
		if (result.Count == 0)
		{
			return null;
		}

		result.Sort(ChainingStep.Compare);

		if (context.OnlyFindOne)
		{
			return result[0];
		}

		context.Accumulator.AddRange(result);
		return null;
	}

	/// <summary>
	/// Search for hints on the given grid.
	/// </summary>
	/// <param name="grid">The grid on which to search for hints.</param>
	/// <returns>The hints found.</returns>
	private List<ChainingStep> GetAll(scoped in Grid grid)
	{
		var result = new List<ChainingStep>();

		// Iterate on all empty cells.
		foreach (byte cell in EmptyCells)
		{
			// The cell is empty.
			var mask = grid.GetCandidates(cell);
			var count = PopCount((uint)mask);
			if (count > 2 || count > 1 && AllowDynamic)
			{
				// Prepare storage and accumulator for "Cell Reduction".
				var digitToOn = new ChainBranch();
				var digitToOff = new ChainBranch();
				var cellToOn = default(NodeSet?);
				var cellToOff = default(NodeSet?);

				// Iterate on all potential values that are not alone.
				foreach (byte digit in mask)
				{
					// Do Binary chaining (same potential either on or off).
					var pOn = new ChainNode(cell, digit, true);
					var pOff = new ChainNode(cell, digit, false);
					var onToOn = new NodeSet();
					var onToOff = new NodeSet();
					var doDouble = count >= 3 && !AllowNishio && AllowDynamic;
					var doContradiction = AllowDynamic || AllowNishio;
					DoBinaryChaining(grid, pOn, pOff, result, onToOn, onToOff, doDouble, doContradiction);

					if (!AllowNishio)
					{
						// Do house chaining.
						DoHouseChaining(grid, result, cell, digit, onToOn, onToOff);
					}

					// Collect results for cell chaining.
					digitToOn.Add(digit, onToOn);
					digitToOff.Add(digit, onToOff);
					if (cellToOn is null || cellToOff is null)
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

				// Do cell reduction.
				if (!AllowNishio && (count == 2 || AllowMultiple))
				{
					if (cellToOn is not null)
					{
						foreach (var p in cellToOn)
						{
							result.Add(CreateCellForcingStep(grid, cell, p, digitToOn));
						}
					}
					if (cellToOff is not null)
					{
						foreach (var p in cellToOff)
						{
							result.Add(CreateCellForcingStep(grid, cell, p, digitToOff));
						}
					}
				}
			}
		}

		return result;
	}

#if ALLOW_ADVANCED_CHAINING
	/// <summary>
	/// Get all non-trivial implications (involving fished, naked/hidden sets, etc).
	/// </summary>
	/// <returns>All found potentials off.</returns>
	private List<ChainNode> GetAdvancedPotentials(
		scoped in Grid grid,
		scoped in Grid source,
		PotentialSet offPotentials
	)
	{
		throw new NotImplementedException();
	}
#endif

	/// <summary>
	/// <para>From the potential <c>p</c>, compute the consequences from both states.</para>
	/// <para>
	/// More precisely, <c>p</c> is first assumed to be correct ("on"), and then to be incorrect ("off");
	/// and the following sets are created:
	/// <list type="bullet">
	/// <item><c><paramref name="onToOn"/></c> the set of potentials that must be "on" when <c>p</c> is "on".</item>
	/// <item><c><paramref name="onToOff"/></c> the set of potentials that must be "off" when <c>p</c> is "on".</item>
	/// <item><c>offToOn</c> the set of potentials that must be "on" when <c>p</c> is "off".</item>
	/// <item><c>offToOff</c> the set of potentials that must be "off" when <c>p</c> is "off".</item>
	/// </list>
	/// Then the following rules are applied:
	/// <list type="bullet">
	/// <item>
	/// If a potential belongs to both <c><paramref name="onToOn"/></c> and <c><paramref name="onToOff"/></c>,
	/// the potential <c>p</c> cannot be "on" because it would imply a potential
	/// to be both "on" and "off", which is an absurd.
	/// </item>
	/// <item>
	/// If a potential belongs to both <c>offToOn</c> and <c>offToOff</c>,
	/// the potential <c>p</c> cannot be "off" because it would imply a potential
	/// to be both "on" and "off", which is an absurd.
	/// </item>
	/// <item>
	/// If a potential belongs to both <c><paramref name="onToOn"/></c> and <c>offToOn</c>,
	/// this potential must be "on", because it is implied to be "on" by the two possible
	/// states of <c>p</c>.
	/// </item>
	/// <item>
	/// If a potential belongs to both <c><paramref name="onToOff"/></c> and <c>offToOff</c>,
	/// this potential must be "off", because it is implied to be "off" by the two possible
	/// states of <c>p</c>.
	/// </item>
	/// </list>
	/// Note that if a potential belongs to all the four sets, the sudoku has no solution. This is not checked.
	/// </para>
	/// </summary>
	/// <param name="grid"><inheritdoc cref="NonMultipleChainingStepSearcher.GetAll(in Grid, bool, bool)" path="/param[@name='grid']"/></param>
	/// <param name="pOn"></param>
	/// <param name="pOff"></param>
	/// <param name="result">
	/// <inheritdoc
	///     cref="NonMultipleChainingStepSearcher.DoUnaryChaining(in Grid, ChainNode, List{ChainingStep}, bool, bool)"
	///     path="/param[@name='result']"/>
	/// </param>
	/// <param name="onToOn">An empty set, filled with potentials that get on if the given potential is on.</param>
	/// <param name="onToOff">An empty set, filled with potentials that get off if the given potential is on.</param>
	/// <param name="doReduction"></param>
	/// <param name="doContradiction"></param>
	private void DoBinaryChaining(
		scoped in Grid grid,
		ChainNode pOn,
		ChainNode pOff,
		List<ChainingStep> result,
		NodeSet onToOn,
		NodeSet onToOff,
		bool doReduction,
		bool doContradiction
	)
	{
		var offToOn = new NodeSet();
		var offToOff = new NodeSet();

		// Circular Forcing Chains (hypothesis implying its negation) are already covered by Cell Forcing Chains,
		// and are therefore not checked for.

		// Test p = "on".
		onToOn.Add(pOn);
		var pair = DoChaining(grid, onToOn, onToOff, AllowNishio, AllowDynamic);
		if (doContradiction && pair is var (absurdOn1, absurdOff1))
		{
			// p cannot hold its value, because else it would lead to a contradiction.
			result.Add(CreateChainingOffStep(grid, absurdOn1, absurdOff1, pOn, pOn, true));
		}

		// Test p = "off".
		offToOff.Add(pOff);
		pair = DoChaining(grid, offToOn, offToOff, AllowNishio, AllowDynamic);
		if (doContradiction && pair is var (absurdOn2, absurdOff2))
		{
			// p must hold its value, because else it would lead to a contradiction.
			result.Add(CreateChainingOnStep(grid, absurdOn2, absurdOff2, pOff, pOff, true));
		}

		if (doReduction)
		{
			// Check potentials that must be on in both case.
			foreach (var pFromOn in onToOn)
			{
				if (offToOn.GetNullable(pFromOn) is { } pFromOff)
				{
					result.Add(CreateChainingOnStep(grid, pFromOn, pFromOff, pOn, pFromOn, false));
				}
			}

			// Check potentials that must be off in both case.
			foreach (var pFromOn in onToOff)
			{
				if (offToOff.GetNullable(pFromOn) is { } pFromOff)
				{
					result.Add(CreateChainingOffStep(grid, pFromOn, pFromOff, pOff, pFromOff, false));
				}
			}
		}
	}

	/// <summary>
	/// Search for region (house) forcing chains.
	/// </summary>
	/// <param name="grid"><inheritdoc cref="NonMultipleChainingStepSearcher.GetAll(in Grid, bool, bool)" path="/param[@name='grid']"/></param>
	/// <param name="result">
	/// <inheritdoc
	///     cref="NonMultipleChainingStepSearcher.DoUnaryChaining(in Grid, ChainNode, List{ChainingStep}, bool, bool)"
	///     path="/param[@name='result']"/>
	/// </param>
	/// <param name="cell"></param>
	/// <param name="digit"></param>
	/// <param name="onToOn">
	/// <inheritdoc
	///     cref="DoBinaryChaining(in Grid, ChainNode, ChainNode, List{ChainingStep}, NodeSet, NodeSet, bool, bool)"
	///     path="/param[@name='onToOn']"/>
	/// </param>
	/// <param name="onToOff">
	/// <inheritdoc
	///     cref="DoBinaryChaining(in Grid, ChainNode, ChainNode, List{ChainingStep}, NodeSet, NodeSet, bool, bool)"
	///     path="/param[@name='onToOff']"/>
	/// </param>
	private void DoHouseChaining(
		scoped in Grid grid,
		List<ChainingStep> result,
		byte cell,
		byte digit,
		NodeSet onToOn,
		NodeSet onToOff
	)
	{
		foreach (var houseType in HouseTypes)
		{
			var houseIndex = cell.ToHouseIndex(houseType);
			var potentialPositions = HousesMap[houseIndex] & CandidatesMap[digit];
			if (potentialPositions.Count == 2 || AllowMultiple && potentialPositions.Count > 2)
			{
				// Do we meet region for the first time?
				if (potentialPositions[0] == cell)
				{
					var posToOn = new ChainBranch();
					var posToOff = new ChainBranch();
					var houseToOn = new NodeSet();
					var houseToOff = new NodeSet();

					// Iterate on potential positions within the region.
					foreach (byte otherCell in potentialPositions)
					{
						if (otherCell == cell)
						{
							posToOn.Add(otherCell, onToOn);
							posToOff.Add(otherCell, onToOff);
							houseToOn |= onToOn;
							houseToOff |= onToOff;
						}
						else
						{
							var other = new ChainNode(otherCell, digit, true);
							var otherToOn = new NodeSet { other };
							var otherToOff = new NodeSet();

							DoChaining(grid, otherToOn, otherToOff, AllowNishio, AllowDynamic);

							posToOn.Add(otherCell, otherToOn);
							posToOff.Add(otherCell, otherToOff);
							houseToOn &= otherToOn;
							houseToOff &= otherToOff;
						}
					}

					// Gather results.
					foreach (var p in houseToOn)
					{
						result.Add(CreateHouseForcingStep(grid, houseIndex, digit, p, posToOn));
					}
					foreach (var p in houseToOff)
					{
						result.Add(CreateHouseForcingStep(grid, houseIndex, digit, p, posToOff));
					}
				}
			}
		}
	}

	/// <summary>
	/// Try to create a binary forcing chain hint on "on" state.
	/// </summary>
	private BinaryForcingChainsStep CreateChainingOnStep(
		scoped in Grid grid,
		ChainNode dstOn,
		ChainNode dstOff,
		ChainNode src,
		ChainNode target,
		bool isAbsurd
	)
	{
		var conclusion = new[] { new Conclusion(Assignment, target.Candidate) };
		var result = new BinaryForcingChainsStep(conclusion, src, dstOn, dstOff, isAbsurd, AllowNishio);
		return new(result, result.CreateViews(grid));
	}

	/// <summary>
	/// Try to create a binary forcing chain hint on "off" state.
	/// </summary>
	private BinaryForcingChainsStep CreateChainingOffStep(
		scoped in Grid grid,
		ChainNode dstOn,
		ChainNode dstOff,
		ChainNode src,
		ChainNode target,
		bool isAbsurd
	)
	{
		var conclusion = new[] { new Conclusion(Elimination, target.Candidate) };
		var result = new BinaryForcingChainsStep(conclusion, src, dstOn, dstOff, isAbsurd, AllowNishio);
		return new(result, result.CreateViews(grid));
	}

	/// <summary>
	/// Try to create a cell forcing chain hint.
	/// </summary>
	private CellForcingChainsStep CreateCellForcingStep(scoped in Grid grid, byte srcCell, ChainNode target, ChainBranch outcomes)
	{
		var (targetCell, targetDigit, targetIsOn) = target;
		var conclusion = new[] { new Conclusion(targetIsOn ? Assignment : Elimination, targetCell, targetDigit) };

		// Build chains.
		var chains = new MultipleForcingChains();
		for (byte tempDigit = 0; tempDigit < 9; tempDigit++)
		{
			if (CandidatesMap[tempDigit].Contains(srcCell))
			{
				// Get corresponding value with the matching parents.
				chains.Add(tempDigit, outcomes[tempDigit].GetNullable(target) ?? default);
			}
		}

		var result = new CellForcingChainsStep(conclusion, srcCell, chains, AllowDynamic);
		return new(result, result.CreateViews(grid));
	}

	/// <summary>
	/// Try to create a region (house) forcing chain hint.
	/// </summary>
	private RegionForcingChainsStep CreateHouseForcingStep(
		scoped in Grid grid,
		int houseIndex,
		byte digit,
		ChainNode target,
		ChainBranch outcomes
	)
	{
		var (targetCell, targetDigit, targetIsOn) = target;
		var conclusions = new[] { new Conclusion(targetIsOn ? Assignment : Elimination, targetCell, targetDigit) };

		// Build chains.
		var chains = new MultipleForcingChains();
		foreach (byte tempCell in CandidatesMap[digit] & HousesMap[houseIndex])
		{
			// Get corresponding value with the matching parents.
			chains.Add(tempCell, outcomes[tempCell].GetNullable(target) ?? default);
		}

		var result = new RegionForcingChainsStep(conclusions, houseIndex, digit, chains, AllowDynamic);
		return new(result, result.CreateViews(grid));
	}
}
