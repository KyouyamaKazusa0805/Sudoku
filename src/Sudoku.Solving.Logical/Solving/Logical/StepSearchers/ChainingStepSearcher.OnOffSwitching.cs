namespace Sudoku.Solving.Logical.StepSearchers;

partial class ChainingStepSearcher
{
	/// <summary>
	/// Get the set of all <see cref="Potential"/>s that cannot be valid (are "off") if the given potential is "on"
	/// (i.e. if its value is the correct one for the cell).
	/// </summary>
	/// <param name="grid"><inheritdoc cref="GetChainsOrCycles(in Grid, bool, bool)" path="/param[@name='grid']"/></param>
	/// <param name="p">The potential that is assumed to be "on"</param>
	/// <param name="isY"><inheritdoc cref="GetChainsOrCycles(in Grid, bool, bool)" path="/param[@name='isY']"/></param>
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
	/// <param name="grid"><inheritdoc cref="GetChainsOrCycles(in Grid, bool, bool)" path="/param[@name='grid']"/></param>
	/// <param name="p">The potential that is assumed to be "off"</param>
	/// <param name="source">
	/// <inheritdoc cref="DoCycles(in Grid, PotentialSet, PotentialSet, bool, bool, PotentialList, Potential)" path="/param[@name='source']"/>
	/// </param>
	/// <param name="offPotentials">Indicates the <see cref="Potential"/> instances that are already set "off".</param>
	/// <param name="isX"><inheritdoc cref="GetChainsOrCycles(in Grid, bool, bool)" path="/param[@name='isX']"/></param>
	/// <param name="isY"><inheritdoc cref="GetChainsOrCycles(in Grid, bool, bool)" path="/param[@name='isY']"/></param>
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
				var otherDigit = (byte)TrailingZeroCount(grid.GetCandidates(cell) & ~(1 << digit));
				var pOn = new Potential(cell, otherDigit, true) { SingletonParent = p };

				addHiddenParentsOfCell(ref pOn, grid, source, offPotentials);
				result.Add(pOn);
			}
		}

		if (isX)
		{
			// Second rule: if there is only two positions for this potential, the other one gets on.
			foreach (var houseType in HouseTypes)
			{
				var houseIndex = cell.ToHouseIndex(houseType);
				if ((HousesMap[houseIndex] & CandidatesMap[digit]) - cell is [var otherCell])
				{
					var pOn = new Potential((byte)otherCell, digit, true) { SingletonParent = p };

					addHiddenParentsOfHouse(ref pOn, grid, source, houseType, offPotentials);
					result.Add(pOn);
				}
			}
		}

		return result;


		static void addHiddenParentsOfCell(scoped ref Potential p, scoped in Grid grid, scoped in Grid source, PotentialSet offPotentials)
		{
			if (source.IsUndefined)
			{
				return;
			}

			var cell = p.Cell;
			for (byte digit = 0; digit < 9; digit++)
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

		static void addHiddenParentsOfHouse(
			scoped ref Potential p,
			scoped in Grid grid,
			scoped in Grid source,
			HouseType currentHouseType,
			PotentialSet offPotentials
		)
		{
			if (source.IsUndefined)
			{
				return;
			}

			var (cell, digit, _) = p;

			var houseIndex = cell.ToHouseIndex(currentHouseType);
			var sourceDigitDistribution = (short)0;
			var currentDigitDistribution = (short)0;
			for (var i = 0; i < 9; i++)
			{
				var houseCell = HouseCells[houseIndex][i];
				if (source.GetStatus(houseCell) == CellStatus.Empty && (source.GetCandidates(houseCell) >> digit & 1) != 0)
				{
					sourceDigitDistribution |= (short)(1 << i);
				}

				if ((grid.GetCandidates(houseCell) >> digit & 1) != 0)
				{
					currentDigitDistribution |= (short)(1 << i);
				}
			}

			// Get positions of the potential value that have been removed.
			foreach (var i in (short)(sourceDigitDistribution & ~currentDigitDistribution))
			{
				// Add a hidden parent.
				if (offPotentials.GetNullable(new((byte)HouseCells[houseIndex][i], digit, false)) is not { } parent)
				{
					throw new InvalidOperationException("Parent not found.");
				}

				p.Parents.Add(parent);
			}
		}
	}
}
