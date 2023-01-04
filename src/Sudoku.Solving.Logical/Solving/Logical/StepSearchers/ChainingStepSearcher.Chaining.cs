#undef ALLOW_ADVANCED_CHAINING

namespace Sudoku.Solving.Logical.StepSearchers;

partial class ChainingStepSearcher
{
	/// <summary>
	/// Look for, and add single forcing chains, and bidirectional cycles.
	/// </summary>
	/// <param name="grid"><inheritdoc cref="GetNonMultipleChains(in Grid, bool, bool)" path="/param[@name='grid']"/></param>
	/// <param name="pOn">The start potential.</param>
	/// <param name="result">The result steps found.</param>
	/// <param name="isX"><inheritdoc cref="GetNonMultipleChains(in Grid, bool, bool)" path="/param[@name='isX']"/></param>
	/// <param name="isY"><inheritdoc cref="GetNonMultipleChains(in Grid, bool, bool)" path="/param[@name='isY']"/></param>
	private void DoUnaryChaining(scoped in Grid grid, Potential pOn, List<ChainingStep> result, bool isX, bool isY)
	{
		if (BivalueCells.Contains(pOn.Cell) && !isX)
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
			if (CreateCycleStep(grid, dstOn, isX, isY) is { } step)
			{
				result.Add(step);
			}
		}

		foreach (var target in chains)
		{
			result.Add(CreateAicStep(grid, target, isX, isY));
		}
	}

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
	/// <param name="grid"><inheritdoc cref="GetNonMultipleChains(in Grid, bool, bool)" path="/param[@name='grid']"/></param>
	/// <param name="pOn"></param>
	/// <param name="pOff"></param>
	/// <param name="result">
	/// <inheritdoc cref="DoUnaryChaining(in Grid, Potential, List{ChainingStep}, bool, bool)" path="/param[@name='result']"/>
	/// </param>
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
		if (doContradiction && DoChaining(grid, onToOn, onToOff) is var (absurdOn1, absurdOff1))
		{
			// p cannot hold its value, because else it would lead to a contradiction.
			result.Add(CreateChainingOffStep(grid, absurdOn1, absurdOff1, pOn, pOn, true));
		}

		// Test p = "off"
		offToOff.Add(pOff);
		if (doContradiction && DoChaining(grid, offToOn, offToOff) is var (absurdOn2, absurdOff2))
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
	/// <param name="grid"><inheritdoc cref="GetNonMultipleChains(in Grid, bool, bool)" path="/param[@name='grid']"/></param>
	/// <param name="result">
	/// <inheritdoc cref="DoUnaryChaining(in Grid, Potential, List{ChainingStep}, bool, bool)" path="/param[@name='result']"/>
	/// </param>
	/// <param name="cell"></param>
	/// <param name="digit"></param>
	/// <param name="onToOn">
	/// <inheritdoc
	///     cref="DoBinaryChaining(ref Grid, Potential, Potential, List{ChainingStep}, PotentialSet, PotentialSet, bool, bool)"
	///     path="/param[@name='onToOn']"/>
	/// </param>
	/// <param name="onToOff">
	/// <inheritdoc
	///     cref="DoBinaryChaining(ref Grid, Potential, Potential, List{ChainingStep}, PotentialSet, PotentialSet, bool, bool)"
	///     path="/param[@name='onToOff']"/>
	/// </param>
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
			var potentialPositions = HousesMap[houseIndex] & CandidatesMap[digit];
			if (potentialPositions.Count == 2 || AllowMultiple && potentialPositions.Count > 2)
			{
				// Do we meet region for the first time?
				if (potentialPositions[0] == cell)
				{
					var posToOn = new ChainBranch();
					var posToOff = new ChainBranch();
					var houseToOn = new PotentialSet();
					var houseToOff = new PotentialSet();

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
							var other = new Potential(otherCell, digit, true);
							var otherToOn = new PotentialSet { other };
							var otherToOff = new PotentialSet();

							DoChaining(grid, otherToOn, otherToOff);

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
	/// Construct cycles and return them, by recording them into argument <paramref name="cycles"/>.
	/// </summary>
	/// <param name="grid"><inheritdoc cref="GetNonMultipleChains(in Grid, bool, bool)" path="/param[@name='grid']"/></param>
	/// <param name="toOn">The potentials that are assumed to be "on".</param>
	/// <param name="toOff">The potentials that are assumed to be "off".</param>
	/// <param name="isX"><inheritdoc cref="GetNonMultipleChains(in Grid, bool, bool)" path="/param[@name='isX']"/></param>
	/// <param name="isY"><inheritdoc cref="GetNonMultipleChains(in Grid, bool, bool)" path="/param[@name='isY']"/></param>
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
				var p = pendingOn.RemoveFirst();
				var makeOff = GetOnToOff(grid, p, isY);
				foreach (var pOff in makeOff)
				{
					if (!IsParent(p, pOff))
					{
						// Not processed yet.
						pendingOff.AddLast(pOff);

						toOff.Add(pOff);
					}
				}
			}

			length++;
			while (pendingOff.Count > 0)
			{
				var p = pendingOff.RemoveFirst();
				var makeOn = GetOffToOn(grid, p, _savedGrid, toOff, isX, isY);
				foreach (var pOn in makeOn)
				{
					if (length >= 4 && pOn == source)
					{
						// Cycle found.
						cycles.AddLast(pOn);
					}

					if (!toOn.Contains(pOn))
					{
						// Not processed yet.
						pendingOn.AddLast(pOn);

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
	/// <param name="grid"><inheritdoc cref="GetNonMultipleChains(in Grid, bool, bool)" path="/param[@name='grid']"/></param>
	/// <param name="toOn">
	/// <inheritdoc cref="DoCycles(in Grid, PotentialSet, PotentialSet, bool, bool, PotentialList, Potential)" path="/param[@name='toOn']"/>
	/// </param>
	/// <param name="toOff">
	/// <inheritdoc cref="DoCycles(in Grid, PotentialSet, PotentialSet, bool, bool, PotentialList, Potential)" path="/param[@name='toOff']"/>
	/// </param>
	/// <param name="isY"><inheritdoc cref="GetNonMultipleChains(in Grid, bool, bool)" path="/param[@name='isY']"/></param>
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
				var p = pendingOn.RemoveFirst();
				var makeOff = GetOnToOff(grid, p, isY);
				foreach (var pOff in makeOff)
				{
					var pOn = new Potential(pOff, true); // Conjugate.
					if (source == pOn)
					{
						// Cyclic contradiction (forcing chain) found.
						if (!chains.Contains(pOff))
						{
							chains.AddLast(pOff);
						}
					}

					if (!IsParent(p, pOff))
					{
						// Why this filter? (seems useless).
						if (!toOff.Contains(pOff))
						{
							// Not processed yet.
							pendingOff.AddLast(pOff);
							toOff.Add(pOff);
						}
					}
				}
			}

			while (pendingOff.Count > 0)
			{
				var p = pendingOff.RemoveFirst();
				var makeOn = GetOffToOn(grid, p, _savedGrid, toOff, true, isY);
				foreach (var pOn in makeOn)
				{
					var pOff = new Potential(pOn, false); // Conjugate.
					if (source == pOff)
					{
						// Cyclic contradiction (forcing chain) found.
						if (!chains.Contains(pOn))
						{
							chains.AddLast(pOn);
						}
					}

					if (!toOn.Contains(pOn))
					{
						// Not processed yet.
						pendingOn.AddLast(pOn);
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
	/// <param name="grid"><inheritdoc cref="GetNonMultipleChains(in Grid, bool, bool)" path="/param[@name='grid']"/></param>
	/// <param name="toOn">The potentials that are assumed to be "on".</param>
	/// <param name="toOff">The potentials that are assumed to be "off".</param>
	/// <returns>If success, <see langword="null"/>.</returns>
	private (Potential On, Potential Off)? DoChaining(Grid grid, PotentialSet toOn, PotentialSet toOff)
	{
		_savedGrid = grid;

		var pendingOn = new PotentialList(toOn);
		var pendingOff = new PotentialList(toOff);
		while (pendingOn.Count > 0 || pendingOff.Count > 0)
		{
			if (pendingOn.Count > 0)
			{
				var p = pendingOn.RemoveFirst();
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
						pendingOff.AddLast(pOff);
					}
				}
			}
			else
			{
				var p = pendingOff.RemoveFirst();
				var makeOn = GetOffToOn(grid, p, _savedGrid, toOff, true, !AllowNishio);
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
						return (pOn, toOff.GetNullable(pOff) ?? default); // Cannot be both on and off at the same time.
					}
					else if (!toOn.Contains(pOn))
					{
						// Not processed yet.
						toOn.Add(pOn);
						pendingOn.AddLast(pOn);
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
}
