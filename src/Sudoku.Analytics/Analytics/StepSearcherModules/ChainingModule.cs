namespace Sudoku.Analytics.StepSearcherModules;

/// <summary>
/// Represents a chaining module.
/// </summary>
/// <remarks>
/// The type is special: it uses source code from another project called Sudoku Explainer.
/// However unfortunately, I cannot find any sites available of the project.
/// One of the original website is <see href="https://diuf.unifr.ch/pai/people/juillera/Sudoku/Sudoku.html">this link</see> (A broken link).
/// </remarks>
internal class ChainingModule
{
	/// <summary>
	/// Get the set of all <see cref="ChainNode"/>s that cannot be valid (are "off") if the given potential is "on"
	/// (i.e. if its value is the correct one for the cell).
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="p">The potential that is assumed to be "on"</param>
	/// <param name="isY">Indicates whether the same-cell strong links are enabled.</param>
	/// <returns>The set of potentials that must be "off".</returns>
	public static NodeSet GetOnToOff(scoped ref readonly Grid grid, ChainNode p, bool isY)
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
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="p">The potential that is assumed to be "off"</param>
	/// <param name="source">The source grid to be used as a cache.</param>
	/// <param name="offPotentials">Indicates the <see cref="ChainNode"/> instances that are already set "off".</param>
	/// <param name="isX">Indicates whether the same-digit strong links are enabled.</param>
	/// <param name="isY">Indicates whether the same-cell strong links are enabled.</param>
	/// <param name="allowDynamic">Indicates whether the dynamic chaining rules are enabled.</param>
	/// <returns>The set of potentials that must be "off".</returns>
	public static NodeSet GetOffToOn(
		scoped ref readonly Grid grid,
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
			var mask = (Mask)(grid.GetCandidates(cell) & ~(1 << digit));
			if (allowDynamic ? IsPow2(mask) : BivalueCells.Contains(cell))
			{
				var otherDigit = (byte)TrailingZeroCount(mask);
				var pOn = new ChainNode(cell, otherDigit, true) { SingletonParent = p };
				if (source is { } original)
				{
					addHiddenParentsOfCell(ref pOn, in grid, in original, offPotentials);
				}

				result.Add(pOn);
			}
		}

		if (isX)
		{
			// Second rule: if there is only two positions for this potential, the other one gets on.
			scoped ref readonly var candMaps = ref allowDynamic ? ref grid.CandidatesMap[digit] : ref CandidatesMap[digit];
			foreach (var houseType in HouseTypes)
			{
				var houseIndex = cell.ToHouseIndex(houseType);
				if ((HousesMap[houseIndex] & candMaps) - cell is [var otherCell])
				{
					var pOn = new ChainNode((byte)otherCell, digit, true) { SingletonParent = p };
					if (source is { } original)
					{
						addHiddenParentsOfHouse(ref pOn, in grid, in original, houseType, offPotentials);
					}

					result.Add(pOn);
				}
			}
		}

		return result;


		static void addHiddenParentsOfCell(scoped ref ChainNode p, scoped ref readonly Grid current, scoped ref readonly Grid original, NodeSet offPotentials)
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
			scoped ref readonly Grid current,
			scoped ref readonly Grid original,
			HouseType currentHouseType,
			NodeSet offPotentials
		)
		{
			var (cell, digit, _) = p;
			var houseIndex = cell.ToHouseIndex(currentHouseType);

			// Get positions of the potential value that have been removed.
			foreach (var pos in (Mask)(g(in original, houseIndex, digit) & (Mask)~g(in current, houseIndex, digit)))
			{
				// Add a hidden parent.
				if (offPotentials.GetNullable(new((byte)HouseCells[houseIndex][pos], digit, false)) is not { } parent)
				{
					throw new InvalidOperationException("Parent not found.");
				}

				p.Parents.Add(parent);
			}


			static Mask g(scoped ref readonly Grid grid, House houseIndex, Digit digit)
			{
				var result = (Mask)0;
				for (var i = 0; i < 9; i++)
				{
					if (grid.Exists(HouseCells[houseIndex][i], digit) is true)
					{
						result |= (Mask)(1 << i);
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
	/// <param name="stepSearcher">The step searcher.</param>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="toOn">The potentials that are assumed to be "on".</param>
	/// <param name="toOff">The potentials that are assumed to be "off".</param>
	/// <param name="allowNishio">Indicates whether the Nishio chaining rules are enabled.</param>
	/// <param name="allowDynamic">Indicates whether the dynamic chaining rules are enabled.</param>
	/// <returns>If success, <see langword="null"/>.</returns>
	public static (ChainNode On, ChainNode Off)? DoChaining<T>(T stepSearcher, Grid grid, NodeSet toOn, NodeSet toOff, bool allowNishio, bool allowDynamic)
		where T : StepSearcher
	{
		scoped ref readonly var originalGrid = ref grid;
		var (pendingOn, pendingOff) = (new NodeList(toOn), new NodeList(toOff));
		while (pendingOn.Count > 0 || pendingOff.Count > 0)
		{
			if (pendingOn.Count > 0)
			{
				var p = pendingOn.RemoveFirst();
				foreach (var pOff in GetOnToOff(in grid, p, !allowNishio))
				{
					var pOn = new ChainNode(pOff, true); // Conjugate.
					if (toOn.GetNullable(pOn) is { } pOnInSet)
					{
						// Contradiction found.
						return (pOnInSet, pOff); // Cannot be both on and off at the same time.
					}

					// Not processed yet.
					if (toOff.Add(pOff))
					{
						pendingOff.AddLast(pOff);
					}
				}
			}
			else
			{
				var p = pendingOff.RemoveFirst();
				var makeOn = GetOffToOn(in grid, p, originalGrid, toOff, true, !allowNishio, allowDynamic);

				if (allowDynamic)
				{
					// Memorize the shut down potentials.
					grid.SetExistence(p.Cell, p.Digit, false);
				}

				foreach (var pOn in makeOn)
				{
					var pOff = new ChainNode(pOn, false); // Conjugate.
					if (toOff.GetNullable(pOff) is { } pOffInSet)
					{
						// Contradiction found.
						return (pOn, pOffInSet); // Cannot be both on and off at the same time.
					}

					if (toOn.Add(pOn))
					{
						// Not processed yet.
						pendingOn.AddLast(pOn);
					}
				}
			}

			OnAdvanced(stepSearcher, pendingOn, pendingOff, toOff, in grid, in originalGrid);
		}

		return null;
	}

	/// <summary>
	/// Handles on advanced chaining cases.
	/// </summary>
	/// <param name="stepSearcher">The step searcher.</param>
	/// <param name="pendingOn">The pending potentials that are assumed to be "on".</param>
	/// <param name="pendingOff">The pending potentials that are assumed to be "off".</param>
	/// <param name="toOff">The original potentials that are assumed to be "off".</param>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="original">Indicates the original grid.</param>
	public static void OnAdvanced<T>(
		T stepSearcher,
		NodeList pendingOn,
		NodeList pendingOff,
		NodeSet toOff,
		scoped ref readonly Grid grid,
		scoped ref readonly Grid original
	) where T : StepSearcher
	{
		return;
	}
}
