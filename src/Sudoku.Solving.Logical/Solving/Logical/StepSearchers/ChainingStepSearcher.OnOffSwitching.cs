namespace Sudoku.Solving.Logical.StepSearchers;

partial class ChainingStepSearcher
{
	/// <summary>
	/// Get the set of all <see cref="Potential"/>s that cannot be valid (are "off") if the given potential is "on"
	/// (i.e. if its value is the correct one for the cell).
	/// </summary>
	/// <param name="grid"><inheritdoc cref="GetNonMultipleChains(in Grid, bool, bool)" path="/param[@name='grid']"/></param>
	/// <param name="p">The potential that is assumed to be "on"</param>
	/// <param name="isY"><inheritdoc cref="GetNonMultipleChains(in Grid, bool, bool)" path="/param[@name='isY']"/></param>
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
	/// Get the set of all <see cref="Potential"/>s that cannot be valid (are "off") if the given potential is "on"
	/// (i.e. if its value is the correct one for the cell).
	/// </summary>
	/// <param name="grid"><inheritdoc cref="GetNonMultipleChains(in Grid, bool, bool)" path="/param[@name='grid']"/></param>
	/// <param name="p">The potential that is assumed to be "off"</param>
	/// <param name="source">
	/// <inheritdoc cref="DoCycles(in Grid, PotentialSet, PotentialSet, bool, bool, PotentialList, Potential)" path="/param[@name='source']"/>
	/// </param>
	/// <param name="offPotentials">Indicates the <see cref="Potential"/> instances that are already set "off".</param>
	/// <param name="isX"><inheritdoc cref="GetNonMultipleChains(in Grid, bool, bool)" path="/param[@name='isX']"/></param>
	/// <param name="isY"><inheritdoc cref="GetNonMultipleChains(in Grid, bool, bool)" path="/param[@name='isY']"/></param>
	/// <returns>The set of potentials that must be "off".</returns>
	private PotentialSet GetOffToOn(scoped in Grid grid, Potential p, scoped in Grid? source, PotentialSet offPotentials, bool isX, bool isY)
	{
		var (cell, digit, _) = p;
		var result = new PotentialSet();

		if (isY)
		{
			// First rule: if there is only two potentials in this cell, the other one gets on.
			var mask = (short)(grid.GetCandidates(cell) & ~(1 << digit));
			if (AllowDynamic ? IsPow2(mask) : BivalueCells.Contains(cell))
			{
				var otherDigit = (byte)TrailingZeroCount(mask);
				var pOn = new Potential(cell, otherDigit, true) { SingletonParent = p };
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
			var candMaps = AllowDynamic ? grid.CandidatesMap[digit] : CandidatesMap[digit];
			foreach (var houseType in HouseTypes)
			{
				var houseIndex = cell.ToHouseIndex(houseType);
				if ((HousesMap[houseIndex] & candMaps) - cell is [var otherCell])
				{
					var pOn = new Potential((byte)otherCell, digit, true) { SingletonParent = p };
					if (source is { } original)
					{
						addHiddenParentsOfHouse(ref pOn, grid, original, houseType, offPotentials);
					}

					result.Add(pOn);
				}
			}
		}

		return result;


		static void addHiddenParentsOfCell(scoped ref Potential p, scoped in Grid current, scoped in Grid original, PotentialSet offPotentials)
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
			scoped ref Potential p,
			scoped in Grid current,
			scoped in Grid original,
			HouseType currentHouseType,
			PotentialSet offPotentials
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
}
