namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides base methods for type <see cref="NonMultipleChainingStepSearcher"/>.
/// </summary>
/// <typeparam name="TSelf">The type of the step searcher.</typeparam>
/// <seealso cref="NonMultipleChainingStepSearcher"/>
public interface IChainingStepSearcher<TSelf> where TSelf : StepSearcher, IChainingStepSearcher<TSelf>
{
	/// <summary>
	/// Get the set of all <see cref="ChainNode"/>s that cannot be valid (are "off") if the given potential is "on"
	/// (i.e. if its value is the correct one for the cell).
	/// </summary>
	/// <param name="grid"><inheritdoc cref="NonMultipleChainingStepSearcher.GetAll(in Grid, bool, bool)" path="/param[@name='grid']"/></param>
	/// <param name="p">The potential that is assumed to be "on"</param>
	/// <param name="isY"><inheritdoc cref="NonMultipleChainingStepSearcher.GetAll(in Grid, bool, bool)" path="/param[@name='isY']"/></param>
	/// <returns>The set of potentials that must be "off".</returns>
	protected internal static NodeSet GetOnToOff(scoped in Grid grid, ChainNode p, bool isY)
	{
		var result = new NodeSet();
		var cell = p.Cell;
		var digit = p.Digit;

		if (isY)
		{
			// This rule is not used with X-Chains.
			// First rule: other potential values for this cell get off.
			var candidateMask = grid.GetCandidates(cell);
			for (byte tempDigit = 0; tempDigit < 9; tempDigit++)
			{
				if (tempDigit != digit && (candidateMask >> tempDigit & 1) != 0)
				{
					result.Add(new(cell, tempDigit, false) { SingletonParent = p });
				}
			}
		}

		// Second rule: other potential position for this value get off.
		foreach (var houseType in HouseTypes)
		{
			var houseIndex = cell.ToHouseIndex(houseType);
			foreach (byte tempCell in HouseCells[houseIndex])
			{
				if (tempCell != cell && (grid.GetCandidates(tempCell) >> digit & 1) != 0)
				{
					result.Add(new(tempCell, digit, false) { SingletonParent = p });
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Get the set of all <see cref="ChainNode"/>s that cannot be valid (are "off") if the given potential is "on"
	/// (i.e. if its value is the correct one for the cell).
	/// </summary>
	/// <param name="grid"><inheritdoc cref="NonMultipleChainingStepSearcher.GetAll(in Grid, bool, bool)" path="/param[@name='grid']"/></param>
	/// <param name="p">The potential that is assumed to be "off"</param>
	/// <param name="source">
	/// <inheritdoc
	///     cref="NonMultipleChainingStepSearcher.DoCycles(in Grid, NodeSet, NodeSet, bool, bool, NodeList, ChainNode)"
	///     path="/param[@name='source']"/>
	/// </param>
	/// <param name="offPotentials">Indicates the <see cref="ChainNode"/> instances that are already set "off".</param>
	/// <param name="isX"><inheritdoc cref="NonMultipleChainingStepSearcher.GetAll(in Grid, bool, bool)" path="/param[@name='isX']"/></param>
	/// <param name="isY"><inheritdoc cref="NonMultipleChainingStepSearcher.GetAll(in Grid, bool, bool)" path="/param[@name='isY']"/></param>
	/// <param name="allowDynamic"><inheritdoc cref="MultipleChainingStepSearcher.AllowDynamic" path="/summary"/></param>
	/// <returns>The set of potentials that must be "off".</returns>
	protected internal static NodeSet GetOffToOn(
		scoped in Grid grid,
		ChainNode p,
		scoped in Grid? source,
		NodeSet offPotentials,
		bool isX,
		bool isY,
		bool allowDynamic
	)
	{
		var (cell, digit, _) = p;
		var result = new NodeSet();

		if (isY)
		{
			// First rule: if there is only two potentials in this cell, the other one gets on.
			var mask = (short)(grid.GetCandidates(cell) & ~(1 << digit));
			if (allowDynamic ? IsPow2(mask) : BivalueCells.Contains(cell))
			{
				var otherDigit = (byte)TrailingZeroCount(mask);
				var pOn = new ChainNode(cell, otherDigit, true) { SingletonParent = p };
				if (source is { } original)
				{
					addHiddenParentsOfCell(ref pOn, grid, original, offPotentials);
				}

				result.Add(pOn);
			}
		}

		if (isX)
		{
			// Second rule: if there is only two positions for this potential, the other one gets on.
			var candMaps = allowDynamic ? grid.CandidatesMap[digit] : CandidatesMap[digit];
			foreach (var houseType in HouseTypes)
			{
				var houseIndex = cell.ToHouseIndex(houseType);
				if ((HousesMap[houseIndex] & candMaps) - cell is [var otherCell])
				{
					var pOn = new ChainNode((byte)otherCell, digit, true) { SingletonParent = p };
					if (source is { } original)
					{
						addHiddenParentsOfHouse(ref pOn, grid, original, houseType, offPotentials);
					}

					result.Add(pOn);
				}
			}
		}

		return result;


		static void addHiddenParentsOfCell(scoped ref ChainNode p, scoped in Grid current, scoped in Grid original, NodeSet offPotentials)
		{
			var cell = p.Cell;
			for (byte digit = 0; digit < 9; digit++)
			{
				if ((original.Exists(cell, digit), current.Exists(cell, digit)) is (true, false))
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

		static void addHiddenParentsOfHouse(
			scoped ref ChainNode p,
			scoped in Grid current,
			scoped in Grid original,
			HouseType currentHouseType,
			NodeSet offPotentials
		)
		{
			var (cell, digit, _) = p;
			var houseIndex = cell.ToHouseIndex(currentHouseType);

			// Get positions of the potential value that have been removed.
			foreach (var pos in (short)(g(original, houseIndex, digit) & ~g(current, houseIndex, digit)))
			{
				// Add a hidden parent.
				if (offPotentials.GetNullable(new((byte)HouseCells[houseIndex][pos], digit, false)) is not { } parent)
				{
					throw new InvalidOperationException("Parent not found.");
				}

				p.Parents.Add(parent);
			}


			static short g(scoped in Grid grid, int houseIndex, int digit)
			{
				var result = (short)0;

				for (var i = 0; i < 9; i++)
				{
					if (grid.Exists(HouseCells[houseIndex][i], digit) is true)
					{
						result |= (short)(1 << i);
					}
				}

				return result;
			}
		}
	}

	/// <summary>
	/// Given the initial sets of potentials that are assumed to be "on" and "off",
	/// complete the sets with all other potentials that must be "on" or "off" as a result of the assumption.
	/// </summary>
	/// <param name="grid"><inheritdoc cref="NonMultipleChainingStepSearcher.GetAll(in Grid, bool, bool)" path="/param[@name='grid']"/></param>
	/// <param name="toOn">The potentials that are assumed to be "on".</param>
	/// <param name="toOff">The potentials that are assumed to be "off".</param>
	/// <param name="allowNishio"><inheritdoc cref="MultipleChainingStepSearcher.AllowNishio" path="/summary"/></param>
	/// <param name="allowDynamic"><inheritdoc cref="MultipleChainingStepSearcher.AllowDynamic" path="/summary"/></param>
	/// <returns>If success, <see langword="null"/>.</returns>
	protected static (ChainNode On, ChainNode Off)? DoChaining(Grid grid, NodeSet toOn, NodeSet toOff, bool allowNishio, bool allowDynamic)
	{
		var originalGrid = grid;
		var pendingOn = new NodeList(toOn);
		var pendingOff = new NodeList(toOff);
		while (pendingOn.Count > 0 || pendingOff.Count > 0)
		{
			if (pendingOn.Count > 0)
			{
				var p = pendingOn.RemoveFirst();
				foreach (var pOff in GetOnToOff(grid, p, !allowNishio))
				{
					var pOn = new ChainNode(pOff, true); // Conjugate.
					if (toOn.GetNullable(pOn) is { } pOnInSet)
					{
						// Contradiction found.
						return (pOnInSet, pOff); // Cannot be both on and off at the same time.
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
				var makeOn = GetOffToOn(grid, p, originalGrid, toOff, true, !allowNishio, allowDynamic);

				if (allowDynamic)
				{
					// Memorize the shut down potentials.
					grid[p.Cell, p.Digit] = false;
				}

				foreach (var pOn in makeOn)
				{
					var pOff = new ChainNode(pOn, false); // Conjugate.
					if (toOff.GetNullable(pOff) is { } pOffInSet)
					{
						// Contradiction found.
						return (pOn, pOffInSet); // Cannot be both on and off at the same time.
					}
					else if (!toOn.Contains(pOn))
					{
						// Not processed yet.
						toOn.Add(pOn);
						pendingOn.AddLast(pOn);
					}
				}
			}

			TSelf.OnAdvanced(pendingOn, pendingOff, toOff, grid, originalGrid);
		}

		return null;
	}

	/// <summary>
	/// Handles on advanced chaining cases.
	/// </summary>
	/// <param name="pendingOn">The pending potentials that are assumed to be "on".</param>
	/// <param name="pendingOff">The pending potentials that are assumed to be "off".</param>
	/// <param name="toOff">The original potentials that are assumed to be "off".</param>
	/// <param name="grid"><inheritdoc cref="NonMultipleChainingStepSearcher.GetAll(in Grid, bool, bool)" path="/param[@name='grid']"/></param>
	/// <param name="original">Indicates the original grid.</param>
	protected static virtual void OnAdvanced(NodeList pendingOn, NodeList pendingOff, NodeSet toOff, scoped in Grid grid, scoped in Grid original)
	{
		return;
	}
}
