#undef ALLOW_ADVANCED_CHAINING

namespace Sudoku.Solving.Logical.StepSearchers;

using PotentialList = List<Potential>;
using PotentialSet = HashSet<Potential>;

[StepSearcher]
[SeparatedStepSearcher(0, nameof(AllowMultiple), false, nameof(AllowDynamic), false, nameof(AllowNishio), false, nameof(DynamicNestingLevel), 0)]
[SeparatedStepSearcher(1, nameof(AllowMultiple), true, nameof(AllowDynamic), false, nameof(AllowNishio), false, nameof(DynamicNestingLevel), 0)]
[SeparatedStepSearcher(2, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(AllowNishio), false, nameof(DynamicNestingLevel), 0)]
[SeparatedStepSearcher(3, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(AllowNishio), false, nameof(DynamicNestingLevel), 1)]
[SeparatedStepSearcher(4, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(AllowNishio), false, nameof(DynamicNestingLevel), 2)]
[SeparatedStepSearcher(5, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(AllowNishio), false, nameof(DynamicNestingLevel), 3)]
[SeparatedStepSearcher(6, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(AllowNishio), false, nameof(DynamicNestingLevel), 4)]
[SeparatedStepSearcher(7, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(AllowNishio), false, nameof(DynamicNestingLevel), 5)]
internal sealed partial class ChainingStepSearcher : IChainingStepSearcher
{
	/// <summary>
	/// <para>Indicates the temporary saved grid.</para>
	/// <para><i>
	/// This field will be used when the step searcher uses advanced logic (dynamic forcing chains or dynamic forcing chains (+))
	/// to search for chains.
	/// </i></para>
	/// </summary>
	private Grid _savedGrid;


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
	/// <item>Dynamic Cell forcing chains</item>
	/// <item>Dynamic Region (House) forcing chains</item>
	/// </list>
	/// </item>
	/// </list>
	/// </summary>
	public bool AllowMultiple { get; init; }

	/// <summary>
	/// Indicates whether the step searcher allows dynamic forcing chains:
	/// <list type="bullet">
	/// <item>Dynamic Contradiction forcing chains</item>
	/// <item>Dynamic Double forcing chains</item>
	/// </list>
	/// </summary>
	/// <remarks>
	/// If step searcher enables for dynamic forcing chains, forcing chains will contain branches,
	/// or even branches over branches (recursively). It will be very useful on complex inferences.
	/// </remarks>
	public bool AllowDynamic { get; init; }

	/// <summary>
	/// Indicates the level of dynamic recursion. The value can be 0, 1, 2, 3, 4 and 5.
	/// </summary>
	/// <remarks>
	/// All possible values corresponds to their own cases respectively:
	/// <list type="table">
	/// <listheader>
	/// <term>Value</term>
	/// <description>Supported nesting rule</description>
	/// </listheader>
	/// <item>
	/// <term>0</term>
	/// <description>Non-dynamic forcing chains</description>
	/// </item>
	/// <item>
	/// <term>1</term>
	/// <description>Dynamic forcing chains (+ Structural techniques, e.g. <see cref="ILockedCandidatesStepSearcher"/>)</description>
	/// </item>
	/// <item>
	/// <term>2</term>
	/// <description>Dynamic forcing chains (+ AIC)</description>
	/// </item>
	/// <item>
	/// <term>3</term>
	/// <description>Dynamic forcing chains (+ Multiple forcing chains)</description>
	/// </item>
	/// <item>
	/// <term>4</term>
	/// <description>Dynamic forcing chains (+ Dynamic forcing chains)</description>
	/// </item>
	/// <item>
	/// <term>5</term>
	/// <description>Dynamic forcing chains (+ Dynamic forcing chains (+))</description>
	/// </item>
	/// </list>
	/// </remarks>
	/// <seealso cref="ILockedCandidatesStepSearcher"/>
	public int DynamicNestingLevel { get; init; }


	/// <inheritdoc/>
	public IStep? GetAll(scoped in LogicalAnalysisContext context)
	{
		// TODO: Implement an implications cache.

		scoped ref readonly var grid = ref context.Grid;
		List<ChainingStep> result;
		if (AllowMultiple || AllowDynamic)
		{
			var tempGrid = grid;
			result = GetMultipleChainsHintList(ref tempGrid);
		}
		else
		{
			var xCycles = GetLoopHintList(grid, true, false);
			var yCycles = GetLoopHintList(grid, false, true);
			var xyCycles = GetLoopHintList(grid, true, true);

			result = xCycles;
			result.AddRange(yCycles);
			result.AddRange(xyCycles);
		}

		result.Sort(ChainingStep.Compare);

		if (context.OnlyFindOne)
		{
			return result switch { [var firstFoundStep, ..] => firstFoundStep, _ => null };
		}

		context.Accumulator.AddRange(result);
		return null;
	}

	/// <summary>
	/// Checks whether hte specified <paramref name="child"/> is the real child node of <paramref name="parent"/>.
	/// </summary>
	/// <param name="child">The child node to be checked.</param>
	/// <param name="parent">The parent node to be checked.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	internal bool IsParent(Potential child, Potential parent)
	{
		var pTest = child;
		while (pTest.Parents.Count > 0)
		{
			pTest = pTest.Parents[0];

			if (pTest == parent)
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Try to search for all AICs and continuous nice loops.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="isX">Indicates whether the chain allows X element (strong links in a house for a single digit).</param>
	/// <param name="isY">Indicates whether the chain allows Y element (strong links in a cell).</param>
	/// <returns>All possible found <see cref="ChainingStep"/>s.</returns>
	private List<ChainingStep> GetLoopHintList(scoped in Grid grid, bool isX, bool isY)
	{
		var result = new List<ChainingStep>();

		foreach (byte cell in EmptyCells)
		{
			if (grid.GetStatus(cell) != CellStatus.Empty)
			{
				continue;
			}

			for (byte digit = 1; digit <= 9; digit++)
			{
				if (CandidatesMap[digit].Contains(cell))
				{
					var pOn = new Potential(cell, digit, true);
					DoUnaryChaining(grid, pOn, result, isX, isY);
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Search for hints on the given grid.
	/// </summary>
	/// <param name="grid">The grid on which to search for hints.</param>
	/// <returns>The hints found.</returns>
	private List<ChainingStep> GetMultipleChainsHintList(scoped ref Grid grid)
	{
		var result = new List<ChainingStep>();

		// Iterate on all empty cells.
		foreach (byte cell in EmptyCells)
		{
			// The cell is empty.
			var cardinality = PopCount((uint)grid.GetCandidates(cell));
			if (cardinality > 2 || cardinality > 1 && AllowDynamic)
			{
				// Prepare storage and accumulator for "Cell Reduction".
				var digitToOn = new Dictionary<byte, PotentialSet>();
				var digitToOff = new Dictionary<byte, PotentialSet>();
				var cellToOn = default(PotentialSet?);
				var cellToOff = default(PotentialSet?);

				// Iterate on all potential values that are not alone.
				for (byte digit = 1; digit <= 9; digit++)
				{
					if (CandidatesMap[digit].Contains(cell))
					{
						// Do Binary chaining (same potential either on or off).
						var pOn = new Potential(cell, digit, true);
						var pOff = new Potential(cell, digit, false);
						var onToOn = new PotentialSet();
						var onToOff = new PotentialSet();
						var doDouble = cardinality >= 3 && !AllowNishio && AllowDynamic;
						var doContradiction = AllowDynamic || AllowNishio;
						DoBinaryChaining(ref grid, pOn, pOff, result, onToOn, onToOff, doDouble, doContradiction);

						if (!AllowNishio)
						{
							// Do house chaining.
							DoHouseChaining(ref grid, result, cell, digit, onToOn, onToOff);
						}

						// Collect results for cell chaining.
						digitToOn.Add(digit, onToOn);
						digitToOff.Add(digit, onToOff);
						if (cellToOn is null)
						{
							cellToOn = new();
							cellToOff = new();
							cellToOn.AddRange(onToOn);
							cellToOff.AddRange(onToOff);
						}
						else
						{
							cellToOn.IntersectWith(onToOn);
							cellToOff!.IntersectWith(onToOff);
						}
					}
				}

				if (!AllowNishio)
				{
					// Do Cell reduction
					if (cardinality == 2 || AllowMultiple)
					{
						if (cellToOn is not null)
						{
							foreach (var p in cellToOn)
							{
								result.Add(CreateCellReductionHint(cell, p, digitToOn));
							}
						}
						if (cellToOff is not null)
						{
							foreach (var p in cellToOff)
							{
								result.Add(CreateCellReductionHint(cell, p, digitToOff));
							}
						}
					}
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Look for, and add single forcing chains, and bidirectional cycles.
	/// </summary>
	/// <param name="grid"><inheritdoc cref="GetLoopHintList(in Grid, bool, bool)" path="/param[@name='grid']"/></param>
	/// <param name="pOn">The start potential.</param>
	/// <param name="result">The result steps found.</param>
	/// <param name="isX"><inheritdoc cref="GetLoopHintList(in Grid, bool, bool)" path="/param[@name='isX']"/></param>
	/// <param name="isY"><inheritdoc cref="GetLoopHintList(in Grid, bool, bool)" path="/param[@name='isY']"/></param>
	private void DoUnaryChaining(scoped in Grid grid, Potential pOn, List<ChainingStep> result, bool isX, bool isY)
	{
		if (!BivalueCells.Contains(pOn.Cell) && !isX)
		{
			// Y-Cycles can only start if cell has 2 potential values.
			return;
		}

		var cycles = new PotentialList();
		var chains = new PotentialList();
		var onToOn = new PotentialSet { pOn };
		var onToOff = new PotentialSet();

		DoCycles(grid, onToOn, onToOff, isX, isY, cycles, pOn);

		if (isX)
		{
			// Forcing Y-Chains do not exist (length must be both odd and even).

			// Forcing chain with "off" implication.
			onToOn = new() { pOn };
			onToOff = new();
			DoForcingChains(grid, onToOn, onToOff, isY, chains, pOn);

			// Forcing chain with "on" implication.
			var pOff = new Potential(pOn, false);
			onToOn = new();
			onToOff = new() { pOff };
			DoForcingChains(grid, onToOn, onToOff, isY, chains, pOff);
		}

		foreach (var dstOn in cycles)
		{
			// Cycle found.
			if (CreateCycleHint(grid, dstOn, isX, isY) is { } step)
			{
				result.Add(step);
			}
		}

		foreach (var target in chains)
		{
			result.Add(CreateForcingChainHint(target, isX, isY));
		}
	}

	/// <summary>
	/// <para>From the potential <c>p</c>, compute the consequences from both states.</para>
	/// <para>
	/// More precisely, <c>p</c> is first assumed to be correct ("on"), and then to be incorrect ("off");
	/// and the following sets are created:
	/// <list type="bullet">
	/// <item><c><paramref name="onToOn"/></c> the set of potentials that must be "on" when <c>p</c> is "on"</item>
	/// <item><c><paramref name="onToOff"/></c> the set of potentials that must be "off" when <c>p</c> is "on"</item>
	/// <item><c>offToOn</c> the set of potentials that must be "on" when <c>p</c> is "off"</item>
	/// <item><c>offToOff</c> the set of potentials that must be "off" when <c>p</c> is "off"</item>
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
	/// <param name="grid"><inheritdoc cref="GetLoopHintList(in Grid, bool, bool)" path="/param[@name='grid']"/></param>
	/// <param name="pOn"></param>
	/// <param name="pOff"></param>
	/// <param name="result"></param>
	/// <param name="onToOn">An empty set, filled with potentials that get on if the given potential is on.</param>
	/// <param name="onToOff">An empty set, filled with potentials that get off if the given potential is on.</param>
	/// <param name="doReduction"></param>
	/// <param name="doContradiction"></param>
	private void DoBinaryChaining(
		scoped ref Grid grid,
		Potential pOn,
		Potential pOff,
		List<ChainingStep> result,
		PotentialSet onToOn,
		PotentialSet onToOff,
		bool doReduction,
		bool doContradiction
	)
	{
		var offToOn = new PotentialSet();
		var offToOff = new PotentialSet();

		// Circular Forcing Chains (hypothesis implying its negation) are already covered by Cell Forcing Chains,
		// and are therefore not checked for.

		// Test p = "on"
		onToOn.Add(pOn);
		var absurdPotential = DoChaining(ref grid, onToOn, onToOff);
		if (doContradiction && absurdPotential is var (absurdOn1, absurdOff1))
		{
			// p cannot hold its value, because else it would lead to a contradiction.
			result.Add(CreateChainingOffHint(absurdOn1, absurdOff1, pOn, pOn, true));
		}

		// Test p = "off"
		offToOff.Add(pOff);
		absurdPotential = DoChaining(ref grid, offToOn, offToOff);
		if (doContradiction && absurdPotential is var (absurdOn2, absurdOff2))
		{
			// p must hold its value, because else it would lead to a contradiction.
			result.Add(CreateChainingOnHint(absurdOn2, absurdOff2, pOff, pOff, true));
		}

		if (doReduction)
		{
			// Check potentials that must be on in both case.
			foreach (var pFromOn in onToOn)
			{
				if (offToOn.GetNullable(pFromOn) is { } pFromOff)
				{
					result.Add(CreateChainingOnHint(pFromOn, pFromOff, pOn, pFromOn, false));
				}
			}

			// Check potentials that must be off in both case.
			foreach (var pFromOn in onToOff)
			{
				if (offToOff.GetNullable(pFromOn) is { } pFromOff)
				{
					result.Add(CreateChainingOffHint(pFromOn, pFromOff, pOff, pFromOff, false));
				}
			}
		}
	}

	private void DoHouseChaining(
		scoped ref Grid grid,
		List<ChainingStep> result,
		byte cell,
		byte digit,
		PotentialSet onToOn,
		PotentialSet onToOff
	)
	{
		foreach (var houseType in HouseTypes)
		{
			var houseIndex = cell.ToHouseIndex(houseType);
			var potentialPositions = EmptyCells & HousesMap[houseIndex] & CandidatesMap[digit];
			if (potentialPositions.Count == 2 || AllowMultiple && potentialPositions.Count > 2)
			{
				var firstCell = potentialPositions[0];

				// Do we meet region for the first time?
				if (firstCell == cell)
				{
					var posToOn = new Dictionary<byte, PotentialSet>();
					var posToOff = new Dictionary<byte, PotentialSet>();
					var regionToOn = new PotentialSet();
					var regionToOff = new PotentialSet();

					// Iterate on potential positions within the region.
					foreach (byte otherCell in potentialPositions)
					{
						if (otherCell == cell)
						{
							posToOn.Add(otherCell, onToOn);
							posToOff.Add(otherCell, onToOff);
							regionToOn.AddRange(onToOn);
							regionToOff.AddRange(onToOff);
						}
						else
						{
							var other = new Potential(otherCell, digit, true);
							var otherToOn = new PotentialSet { other };
							var otherToOff = new PotentialSet();

							DoChaining(ref grid, otherToOn, otherToOff);

							posToOn.Add(otherCell, otherToOn);
							posToOff.Add(otherCell, otherToOff);

							regionToOn.IntersectWith(otherToOn);
							regionToOff.IntersectWith(otherToOff);
						}
					}

					// Gather results.
					foreach (var p in regionToOn)
					{
						result.Add(CreateHouseReductionHint(houseIndex, digit, p, posToOn));
					}
					foreach (var p in regionToOff)
					{
						result.Add(CreateHouseReductionHint(houseIndex, digit, p, posToOff));
					}
				}
			}
		}
	}

	/// <summary>
	/// Construct cycles and return them, by recording them into argument <paramref name="cycles"/>.
	/// </summary>
	/// <param name="grid"><inheritdoc cref="GetLoopHintList(in Grid, bool, bool)" path="/param[@name='grid']"/></param>
	/// <param name="toOn">The potentials that are assumed to be "on".</param>
	/// <param name="toOff">The potentials that are assumed to be "off".</param>
	/// <param name="isX"><inheritdoc cref="GetLoopHintList(in Grid, bool, bool)" path="/param[@name='isX']"/></param>
	/// <param name="isY"><inheritdoc cref="GetLoopHintList(in Grid, bool, bool)" path="/param[@name='isY']"/></param>
	/// <param name="cycles">
	/// <para>All found cycles, represented as their final <see cref="Potential"/> node.</para>
	/// <para>By using <see cref="Potential.ChainPotentials"/>, we can get the whole chain.</para>
	/// </param>
	/// <param name="source">The source node.</param>
	private void DoCycles(scoped in Grid grid, PotentialSet toOn, PotentialSet toOff, bool isX, bool isY, PotentialList cycles, Potential source)
	{
		var pendingOn = new PotentialList(toOn);
		var pendingOff = new PotentialList(toOff);

		// Mind why this is a BFS and works. I learned that cycles are only found by DFS
		// Maybe we are missing loops.

		var length = 0; // Cycle length.
		while (pendingOn.Count > 0 || pendingOff.Count > 0)
		{
			length++;
			while (pendingOn.Count > 0)
			{
				var p = pendingOn.RemoveFirstElement();
				var makeOff = GetOnToOff(grid, p, isY);
				foreach (var pOff in makeOff)
				{
					if (!IsParent(p, pOff))
					{
						// Not processed yet
						pendingOff.Add(pOff);

						Debug.Assert(length >= 1);

						toOff.Add(pOff);
					}
				}
			}

			length++;
			while (pendingOff.Count > 0)
			{
				var p = pendingOff.RemoveFirstElement();
				var makeOn = GetOffToOn(grid, p, _savedGrid, toOff, isX, isY);
				foreach (var pOn in makeOn)
				{
					if (length >= 4 && pOn == source)
					{
						// Cycle found
						cycles.Add(pOn);
					}

					if (!toOn.Contains(pOn))
					{
						// Not processed yet
						pendingOn.Add(pOn);

						Debug.Assert(length >= 1);

						toOn.Add(pOn);
					}
				}
			}
		}
	}

	/// <summary>
	/// Construct forcing chains (in Sudoku Explainer, AICs will be treated as forcing chains).
	/// In other words, this method does find for AICs.
	/// </summary>
	/// <param name="grid"><inheritdoc cref="GetLoopHintList(in Grid, bool, bool)" path="/param[@name='grid']"/></param>
	/// <param name="toOn">
	/// <inheritdoc cref="DoCycles(in Grid, PotentialSet, PotentialSet, bool, bool, PotentialList, Potential)" path="/param[@name='toOn']"/>
	/// </param>
	/// <param name="toOff">
	/// <inheritdoc cref="DoCycles(in Grid, PotentialSet, PotentialSet, bool, bool, PotentialList, Potential)" path="/param[@name='toOff']"/>
	/// </param>
	/// <param name="isY"><inheritdoc cref="GetLoopHintList(in Grid, bool, bool)" path="/param[@name='isY']"/></param>
	/// <param name="chains">
	/// <para>All found chains, represented as their final <see cref="Potential"/> node.</para>
	/// <para>
	/// <inheritdoc
	///     cref="DoCycles(in Grid, PotentialSet, PotentialSet, bool, bool, PotentialList, Potential)"
	///     path="//param[@name='cycles']/para[2]"/>
	/// </para>
	/// </param>
	/// <param name="source">
	/// <inheritdoc cref="DoCycles(in Grid, PotentialSet, PotentialSet, bool, bool, PotentialList, Potential)" path="/param[@name='source']"/>
	/// </param>
	private void DoForcingChains(scoped in Grid grid, PotentialSet toOn, PotentialSet toOff, bool isY, PotentialList chains, Potential source)
	{
		var pendingOn = new PotentialList(toOn);
		var pendingOff = new PotentialList(toOff);
		while (pendingOn.Count > 0 || pendingOff.Count > 0)
		{
			while (pendingOn.Count > 0)
			{
				var p = pendingOn.RemoveFirstElement();
				var makeOff = GetOnToOff(grid, p, isY);
				foreach (var pOff in makeOff)
				{
					var pOn = new Potential(pOff, true); // Conjugate.
					if (source == pOn)
					{
						// Cyclic contradiction (forcing chain) found.
						if (!chains.Contains(pOff))
						{
							chains.Add(pOff);
						}
					}

					if (!IsParent(p, pOff))
					{
						// Why this filter? (seems useless).
						if (!toOff.Contains(pOff))
						{
							// Not processed yet.
							pendingOff.Add(pOff);
							toOff.Add(pOff);
						}
					}
				}
			}

			while (pendingOff.Count > 0)
			{
				var p = pendingOff.RemoveFirstElement();
				var makeOn = GetOffToOn(grid, p, _savedGrid, toOff, true, isY);
				foreach (var pOn in makeOn)
				{
					var pOff = new Potential(pOn, false); // Conjugate.
					if (source == pOff)
					{
						// Cyclic contradiction (forcing chain) found.
						if (!chains.Contains(pOn))
						{
							chains.Add(pOn);
						}
					}

					if (!toOn.Contains(pOn))
					{
						// Not processed yet.
						pendingOn.Add(pOn);
						toOn.Add(pOn);
					}
				}
			}
		}
	}

	/// <summary>
	/// Given the initial sets of potentials that are assumed to be "on" and "off",
	/// complete the sets with all other potentials that must be "on" or "off" as a result of the assumption.
	/// </summary>
	/// <param name="grid"><inheritdoc cref="GetLoopHintList(in Grid, bool, bool)" path="/param[@name='grid']"/></param>
	/// <param name="toOn">The potentials that are assumed to be "on".</param>
	/// <param name="toOff">The potentials that are assumed to be "off".</param>
	/// <returns>If success, <see langword="null"/>.</returns>
	private (Potential On, Potential Off)? DoChaining(scoped ref Grid grid, PotentialSet toOn, PotentialSet toOff)
	{
		_savedGrid = grid;

		try
		{
			var pendingOn = new PotentialList(toOn);
			var pendingOff = new PotentialList(toOff);
			while (pendingOn.Count > 0 || pendingOff.Count > 0)
			{
				if (pendingOn.Count > 0)
				{
					var p = pendingOn.RemoveFirstElement();
					var makeOff = GetOnToOff(grid, p, !AllowNishio);
					foreach (var pOff in makeOff)
					{
						var pOn = new Potential(pOff, true); // Conjugate.
						if (toOn.Contains(pOn))
						{
							// Contradiction found.
							return (toOn.GetNullable(pOn) ?? default, pOff); // Cannot be both on and off at the same time.
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
					var p = pendingOff.RemoveFirstElement();
					var makeOn = GetOffToOn(grid, p, _savedGrid, toOff, !AllowNishio, true);
					if (AllowDynamic)
					{
						// Memorize the shutted down potentials.
						p.MakeOffIn(ref grid);
					}
					foreach (var pOn in makeOn)
					{
						var pOff = new Potential(pOn, false); // Conjugate.
						if (toOff.Contains(pOff))
						{
							// Contradiction found.
							return (toOff.GetNullable(pOff) ?? default, pOff); // Cannot be both on and off at the same time.
						}
						else if (!toOn.Contains(pOn))
						{
							// Not processed yet
							toOn.Add(pOn);
							pendingOn.Add(pOn);
						}
					}
				}

#if ALLOW_ADVANCED_CHAINING
				if (pendingOn.Count == 0 && pendingOff.Count == 0 && DynamicNestingLevel > 0)
				{
					foreach (var pOff in GetAdvancedPotentials(grid, _savedGrid, toOff))
					{
						if (!toOff.Contains(pOff))
						{
							// Not processed yet.
							toOff.Add(pOff);
							pendingOff.Add(pOff);
						}
					}
				}
#endif
			}

			return null;
		}
		finally
		{
			grid = _savedGrid;
		}
	}

#if ALLOW_ADVANCED_CHAINING
	/// <summary>
	/// Get all non-trivial implications (involving fished, naked/hidden sets, etc).
	/// </summary>
	/// <returns>All found potentials off.</returns>
	private List<Potential> GetAdvancedPotentials(
		scoped in Grid grid,
		scoped in Grid source,
		HashSet<Potential> offPotentials
	)
	{
		throw new NotImplementedException();
	}
#endif

	/// <summary>
	/// Get the set of all <see cref="Potential"/>s that cannot be valid (are "off") if the given potential is "on"
	/// (i.e. if its value is the correct one for the cell).
	/// </summary>
	/// <param name="grid"><inheritdoc cref="GetLoopHintList(in Grid, bool, bool)" path="/param[@name='grid']"/></param>
	/// <param name="p">The potential that is assumed to be "on"</param>
	/// <param name="isY"><inheritdoc cref="GetLoopHintList(in Grid, bool, bool)" path="/param[@name='isY']"/></param>
	/// <returns>The set of potentials that must be "off".</returns>
	private PotentialSet GetOnToOff(scoped in Grid grid, Potential p, bool isY)
	{
		var result = new PotentialSet();

		var cell = p.Cell;
		var digit = p.Digit;

		if (isY)
		{
			// This rule is not used with X-Chains.
			// First rule: other potential values for this cell get off.
			var potentialValues = grid.GetCandidates(cell);
			for (byte tempDigit = 1; tempDigit <= 9; tempDigit++)
			{
				if (tempDigit != digit && (potentialValues >> tempDigit & 1) != 0)
				{
					result.Add(new(cell, tempDigit, false) { SingletonParent = p });
				}
			}
		}

		// Second rule: other potential position for this value get off.
		foreach (var houseType in HouseTypes)
		{
			var houseIndex = cell.ToHouseIndex(houseType);

			for (var pos = 0; pos < 9; pos++)
			{
				var tempCell = (byte)HouseCells[houseIndex][pos];
				if (tempCell != cell && (grid.GetCandidates(tempCell) >> digit & 1) != 0)
				{
					result.Add(new(tempCell, digit, false) { SingletonParent = p });
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Get the set of all <see cref="Potential"/>s that cannot be valid (are "off") if the given potential is "on"
	/// (i.e. if its value is the correct one for the cell).
	/// </summary>
	/// <param name="grid"><inheritdoc cref="GetLoopHintList(in Grid, bool, bool)" path="/param[@name='grid']"/></param>
	/// <param name="p">The potential that is assumed to be "off"</param>
	/// <param name="source">
	/// <inheritdoc cref="DoCycles(in Grid, PotentialSet, PotentialSet, bool, bool, PotentialList, Potential)" path="/param[@name='source']"/>
	/// </param>
	/// <param name="offPotentials">Indicates the <see cref="Potential"/> instances that are already set "off".</param>
	/// <param name="isX"><inheritdoc cref="GetLoopHintList(in Grid, bool, bool)" path="/param[@name='isX']"/></param>
	/// <param name="isY"><inheritdoc cref="GetLoopHintList(in Grid, bool, bool)" path="/param[@name='isY']"/></param>
	/// <returns>The set of potentials that must be "off".</returns>
	private PotentialSet GetOffToOn(scoped in Grid grid, Potential p, scoped in Grid source, PotentialSet offPotentials, bool isX, bool isY)
	{
		var (cell, digit, _) = p;
		var result = new PotentialSet();

		if (isY)
		{
			// First rule: if there is only two potentials in this cell, the other one gets on.
			if (BivalueCells.Contains(cell))
			{
				var mask = grid.GetCandidates(cell);
				var otherDigit = (byte)mask.GetNextSet(0);
				if (otherDigit == digit)
				{
					otherDigit = (byte)mask.GetNextSet(otherDigit + 1);
				}

				var pOn = new Potential(cell, otherDigit, true) { SingletonParent = p };

				addHiddenParentsOfCell(pOn, grid, source, offPotentials);
				result.Add(pOn);
			}
		}

		if (isX)
		{
			// Second rule: if there is only two positions for this potential, the other one gets on.
			foreach (var houseType in HouseTypes)
			{
				var houseIndex = cell.ToHouseIndex(houseType);

				var potentialPositions = HousesMap[houseIndex] & CandidatesMap[digit];
				if (potentialPositions.Count == 2)
				{
					var otherCell = (byte)potentialPositions[0];
					if (otherCell == cell)
					{
						otherCell = (byte)potentialPositions[1];
					}

					var pOn = new Potential(otherCell, digit, true) { SingletonParent = p };

					addHiddenParentsOfHouse(pOn, source, houseType, offPotentials);
					result.Add(pOn);
				}
			}
		}

		return result;


		static void addHiddenParentsOfCell(Potential p, scoped in Grid grid, scoped in Grid source, PotentialSet offPotentials)
		{
			var cell = p.Cell;
			for (byte digit = 1; digit <= 9; digit++)
			{
				if ((source.GetCandidates(cell) >> digit & 1) != 0 && (grid.GetCandidates(cell) >> digit & 1) == 0)
				{
					// Add a hidden parent.
					if (offPotentials.GetNullable(new(cell, digit, false)) is not { } parent)
					{
						throw new InvalidOperationException("Parent not found.");
					}

					p.Parents.Add(parent);
				}
			}
		}

		static void addHiddenParentsOfHouse(Potential p, scoped in Grid source, HouseType currentHouseType, PotentialSet offPotentials)
		{
			var digit = p.Digit;
			var houseIndex = p.Cell.ToHouseIndex(currentHouseType);
			var sourceDigitDistribution = (short)0;
			var currentDigitDistribution = (short)0;
			for (var i = 0; i < 9; i++)
			{
				var houseCell = HouseCells[houseIndex][i];
				if (source.GetStatus(houseCell) == CellStatus.Empty && (source.GetCandidates(houseCell) >> digit & 1) != 0)
				{
					sourceDigitDistribution |= (short)(1 << i);
				}

				if (CandidatesMap[digit].Contains(houseCell))
				{
					currentDigitDistribution |= (short)(1 << i);
				}
			}

			// Get positions of the potential value that have been removed.
			foreach (var i in (short)(sourceDigitDistribution & ~currentDigitDistribution))
			{
				// Add a hidden parent.
				if (offPotentials.GetNullable(new((byte)HouseCells[houseIndex][i], p.Digit, false)) is not { } parent)
				{
					throw new InvalidOperationException("Parent not found.");
				}

				p.Parents.Add(parent);
			}
		}
	}

	/// <summary>
	/// Try to create a cycle hint. If any conclusion (elimination, assignment) found and is available,
	/// the method will return a <see cref="BidirectionalCycleStep"/> instance with a non-<see langword="null"/> value.
	/// </summary>
	/// <param name="grid">
	/// <inheritdoc cref="GetLoopHintList(in Grid, bool, bool)" path="/param[@name='grid']"/>
	/// </param>
	/// <param name="dstOn">Indicates the destination node that is at the state "on".</param>
	/// <param name="isX">
	/// <inheritdoc cref="DoCycles(in Grid, PotentialSet, PotentialSet, bool, bool, PotentialList, Potential)" path="/param[@name='isX']"/>
	/// </param>
	/// <param name="isY">
	/// <inheritdoc cref="DoCycles(in Grid, PotentialSet, PotentialSet, bool, bool, PotentialList, Potential)" path="/param[@name='isY']"/>
	/// </param>
	/// <returns>
	/// A valid <see cref="BidirectionalCycleStep"/> instance, or <see langword="null"/> if no available eliminations found.
	/// </returns>
	private BidirectionalCycleStep? CreateCycleHint(scoped in Grid grid, Potential dstOn, bool isX, bool isY)
	{
		var nodes = dstOn.ChainPotentials;

		Debug.Assert((nodes.Length & 1) == 0);

		var conclusions = new List<Conclusion>();
		for (var i = 0; i < nodes.Length; i += 2)
		{
			var (c1, d1, _) = nodes[i];
			var (c2, d2, _) = nodes[i + 1];
			if (c1 == c2)
			{
				foreach (var digit in (short)(grid.GetCandidates(c1) & ~(1 << d1 | 1 << d2)))
				{
					conclusions.Add(new(Elimination, c1, digit));
				}
			}
			else if (d1 == d2)
			{
				foreach (var house in (CellsMap[c1] + c2).CoveredHouses)
				{
					foreach (var cell in (HousesMap[house] & DigitsMap[d1]) - c1 - c2)
					{
						conclusions.Add(new(Elimination, cell, d1));
					}
				}
			}
		}

		return conclusions.Count == 0 ? null : new(ImmutableArray.CreateRange(conclusions), dstOn, isX, isY);
	}

	/// <summary>
	/// Try to create an AIC hint.
	/// </summary>
	private ForcingChainStep CreateForcingChainHint(Potential target, bool isX, bool isY)
		=> new(ImmutableArray.Create(new Conclusion(target.IsOn ? Assignment : Elimination, target.Candidate)), target, isX, isY);

	/// <summary>
	/// Try to create a binary forcing chain hint on "on" state.
	/// </summary>
	private BinaryForcingChainsStep CreateChainingOnHint(
		Potential dstOn,
		Potential dstOff,
		Potential source,
		Potential target,
		bool isAbsurd
	) => new(
		ImmutableArray.Create(new Conclusion(Assignment, target.Cell, target.Digit)),
		source,
		dstOn,
		dstOff,
		isAbsurd,
		AllowNishio,
		DynamicNestingLevel
	);

	/// <summary>
	/// Try to create a binary forcing chain hint on "off" state.
	/// </summary>
	private BinaryForcingChainsStep CreateChainingOffHint(
		Potential dstOn,
		Potential dstOff,
		Potential source,
		Potential target,
		bool isAbsurd
	) => new(
		ImmutableArray.Create(new Conclusion(Elimination, target.Cell, target.Digit)),
		source,
		dstOn,
		dstOff,
		isAbsurd,
		AllowNishio,
		DynamicNestingLevel
	);

	/// <summary>
	/// Try to create a cell forcing chain hint.
	/// </summary>
	private CellForcingChainsStep CreateCellReductionHint(
		byte srcCell,
		Potential target,
		Dictionary<byte, PotentialSet> outcomes
	)
	{
		var (targetCell, targetDigit, targetIsOn) = target;
		var conclusions = ImmutableArray.Create(new Conclusion(targetIsOn ? Assignment : Elimination, targetCell, targetDigit));

		// Build chains.
		var chains = new MultipleForcingChains();
		for (byte tempDigit = 1; tempDigit <= 9; tempDigit++)
		{
			if (CandidatesMap[targetDigit].Contains(srcCell))
			{
				// Get corresponding value with the matching parents.
				chains.Add(tempDigit, outcomes[tempDigit].GetNullable(target) ?? default);
			}
		}

		return new(conclusions, srcCell, chains, AllowDynamic, DynamicNestingLevel);
	}

	/// <summary>
	/// Try to create a region (house) forcing chain hint.
	/// </summary>
	private RegionForcingChainsStep CreateHouseReductionHint(
		int houseIndex,
		byte digit,
		Potential target,
		Dictionary<byte, PotentialSet> outcomes
	)
	{
		var (targetCell, targetDigit, targetIsOn) = target;
		var conclusions = ImmutableArray.Create(new Conclusion(targetIsOn ? Assignment : Elimination, targetCell, targetDigit));

		// Build chains.
		var chains = new MultipleForcingChains();
		foreach (byte tempCell in CandidatesMap[digit] & HousesMap[houseIndex])
		{
			// Get corresponding value with the matching parents.
			chains.Add(tempCell, outcomes[tempCell].GetNullable(target) ?? default);
		}

		return new(conclusions, houseIndex, digit, chains, AllowDynamic, DynamicNestingLevel);
	}
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// <para>
	/// Try to get the target <see cref="Potential"/> instance whose internal value
	/// (i.e. properties <see cref="Potential.Cell"/>, <see cref="Potential.Digit"/> and <see cref="Potential.IsOn"/>) are same as
	/// the specified one.
	/// </para>
	/// <para>
	/// Please note that this method will return an instance inside the collection of <paramref name="this"/>
	/// whose value equals to the specified one; however, property <see cref="Potential.Parents"/> may not be equal.
	/// </para>
	/// </summary>
	/// <param name="this">The collection.</param>
	/// <param name="base">The value to be checked.</param>
	/// <returns>
	/// <para>
	/// The found value whose value is equal to <paramref name="base"/>; without checking for property <see cref="Potential.Parents"/>.
	/// </para>
	/// <para>If none found, <see langword="null"/> will be returned.</para>
	/// </returns>
	public static Potential? GetNullable(this PotentialSet @this, Potential @base)
	{
		foreach (var potential in @this)
		{
			if (potential == @base)
			{
				return potential;
			}
		}

		return null;
	}

	/// <summary>
	/// Removes the element at the first position of the collection, and returns it.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The colllection.</param>
	/// <returns>The removed element.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T RemoveFirstElement<T>(this List<T> @this) where T : struct
	{
		var first = @this[0];
		@this.RemoveAt(0);
		return first;
	}
}
