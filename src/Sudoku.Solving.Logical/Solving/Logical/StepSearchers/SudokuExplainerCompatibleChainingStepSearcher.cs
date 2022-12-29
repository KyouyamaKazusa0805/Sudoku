#undef ALLOW_ADVANCED_CHAINING

namespace Sudoku.Solving.Logical.StepSearchers;

using PotentialList = List<Potential>;
using PotentialSet = HashSet<Potential>;

[StepSearcher]
[SeparatedStepSearcher(0, nameof(AllowMultiple), false, nameof(AllowDynamic), false, nameof(AllowNishio), false, nameof(DynamicNestingLevel), 0)]
[SeparatedStepSearcher(1, nameof(AllowMultiple), true, nameof(AllowDynamic), false, nameof(AllowNishio), false, nameof(DynamicNestingLevel), 0)]
[SeparatedStepSearcher(2, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(AllowNishio), false, nameof(DynamicNestingLevel), 0)]
[SeparatedStepSearcher(2, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(AllowNishio), false, nameof(DynamicNestingLevel), 1)]
[SeparatedStepSearcher(2, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(AllowNishio), false, nameof(DynamicNestingLevel), 2)]
[SeparatedStepSearcher(2, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(AllowNishio), false, nameof(DynamicNestingLevel), 3)]
[SeparatedStepSearcher(2, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(AllowNishio), false, nameof(DynamicNestingLevel), 4)]
[SeparatedStepSearcher(2, nameof(AllowMultiple), true, nameof(AllowDynamic), true, nameof(AllowNishio), false, nameof(DynamicNestingLevel), 5)]
internal sealed partial class SudokuExplainerCompatibleChainingStepSearcher : ISudokuExplainerCompatibleChainingStepSearcher
{
	/// <summary>
	/// Indicates the temporary saved grid.
	/// </summary>
	private Grid _savedGrid;


	/// <summary>
	/// Indicates whether the step searcher allows nishio forcing chains.
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
	/// <item>Cell forcing chains</item>
	/// <item>Region (House) forcing chains</item>
	/// <item>Contradiction forcing chains</item>
	/// <item>Double forcing chains</item>
	/// </list>
	/// </item>
	/// </list>
	/// </summary>
	public bool AllowMultiple { get; init; }

	/// <summary>
	/// Indicates whether the step searcher allows dynamic forcing chains.
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
	/// <description>Dynamic forcing chains (+ Structural techniques)</description>
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
	public int DynamicNestingLevel { get; init; }


	/// <inheritdoc/>
	public IStep? GetAll(scoped in LogicalAnalysisContext context)
	{
		// TODO: Implement an implications cache.

		scoped ref readonly var grid = ref context.Grid;
		List<SudokuExplainerCompatibleChainStep> result;
		if (AllowMultiple || AllowDynamic)
		{
			result = new();//GetMultipleChainsHintList(grid);
		}
		else
		{
			var xLoops = GetLoopHintList(grid, true, false); // Cycles with X-Links (Coloring / Fishy).
			var yLoops = GetLoopHintList(grid, false, true); // Cycles with Y-Links.
			var xyLoops = GetLoopHintList(grid, true, true); // Cycles with both.

			result = xLoops;
			result.AddRange(yLoops);
			result.AddRange(xyLoops);
		}

		result.Sort(SudokuExplainerCompatibleChainStep.Compare);

		if (context.OnlyFindOne)
		{
			return result switch { [var firstFoundStep, ..] => firstFoundStep, _ => null };
		}

		context.Accumulator.AddRange(result);
		return null;
	}

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
	/// Gets the base difficulty rating via the settings of the current step searcher.
	/// </summary>
	/// <returns>The base difficulty rating.</returns>
	/// <exception cref="InvalidOperationException">Throws when the current state of the step searcher is invalid.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal decimal GetBaseDifficulty()
		=> this switch
		{
			{ DynamicNestingLevel: var l and >= 2 } => 9.5M + .5M * (l - 2),
			{ DynamicNestingLevel: var l and > 0 } => 8.5M + .5M * l,
			{ AllowNishio: true } => 7.5M,
			{ AllowDynamic: true } => 8.5M,
			{ AllowMultiple: true } => 8.0M,
			_ => throw new InvalidOperationException("The current state of the step searcher is invalid.")
		};

	/// <summary>
	/// Try to search for all AICs and continuous nice loops.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="isX">Indicates whether the chain allows X element (strong links in a house for a single digit).</param>
	/// <param name="isY">Indicates whether the chain allows Y element (strong links in a cell).</param>
	/// <returns>All possible found <see cref="SudokuExplainerCompatibleChainStep"/>s.</returns>
	private List<SudokuExplainerCompatibleChainStep> GetLoopHintList(scoped in Grid grid, bool isX, bool isY)
	{
		var result = new List<SudokuExplainerCompatibleChainStep>();

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
	/// Look for, and add single forcing chains, and bidirectional cycles.
	/// </summary>
	/// <param name="grid"><inheritdoc cref="GetLoopHintList(in Grid, bool, bool)" path="/param[@name='grid']"/></param>
	/// <param name="pOn">The start potential.</param>
	/// <param name="result">The result steps found.</param>
	/// <param name="isX"><inheritdoc cref="GetLoopHintList(in Grid, bool, bool)" path="/param[@name='isX']"/></param>
	/// <param name="isY"><inheritdoc cref="GetLoopHintList(in Grid, bool, bool)" path="/param[@name='isY']"/></param>
	private void DoUnaryChaining(scoped in Grid grid, Potential pOn, List<SudokuExplainerCompatibleChainStep> result, bool isX, bool isY)
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

#if ALLOW_ADVANCED_CHAINING
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
	/// the method will return a <see cref="SudokuExplainerCompatibleCycleStep"/> instance with a non-<see langword="null"/> value.
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
	/// A valid <see cref="SudokuExplainerCompatibleCycleStep"/> instance, or <see langword="null"/> if no available eliminations found.
	/// </returns>
	private SudokuExplainerCompatibleCycleStep? CreateCycleHint(scoped in Grid grid, Potential dstOn, bool isX, bool isY)
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
	/// Try to create a chain hint.
	/// </summary>
	/// <param name="target">Indicates the target node.</param>
	/// <param name="isX">
	/// <inheritdoc cref="DoCycles(in Grid, PotentialSet, PotentialSet, bool, bool, PotentialList, Potential)" path="/param[@name='isX']"/>
	/// </param>
	/// <param name="isY">
	/// <inheritdoc cref="DoCycles(in Grid, PotentialSet, PotentialSet, bool, bool, PotentialList, Potential)" path="/param[@name='isY']"/>
	/// </param>
	/// <returns>
	/// A valid <see cref="SudokuExplainerCompatibleAlternatingInferenceChainStep"/> instance.
	/// </returns>
	private SudokuExplainerCompatibleAlternatingInferenceChainStep CreateForcingChainHint(Potential target, bool isX, bool isY)
		=> new(
			ImmutableArray.Create(new Conclusion(target.IsOn ? Assignment : Elimination, target.Candidate)),
			target,
			isX,
			isY
		);
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
