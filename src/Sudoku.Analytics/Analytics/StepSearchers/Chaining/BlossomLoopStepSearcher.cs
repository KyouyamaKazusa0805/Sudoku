namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Blossom Loop</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Blossom Loop</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed partial class BlossomLoopStepSearcher : ChainingStepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		var result = Collect(grid);
		if (result.Count == 0)
		{
			return null;
		}

		result.Order();

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
	private List<BlossomLoopStep> Collect(scoped in Grid grid)
	{
		var result = new List<BlossomLoopStep>();

		// Iterate on all empty cells.
		foreach (byte cell in EmptyCells)
		{
			var mask = grid.GetCandidates(cell);
			if (PopCount((uint)mask) > 2)
			{
				// Iterate on all potential values that are not alone.
				foreach (byte digit in mask)
				{
					var onToOn = new NodeSet { new(cell, digit, true) };
					DoChaining(grid, onToOn, new NodeSet(), false, false);

					// Do house chaining.
					DoHouseChaining(grid, result, cell, digit, onToOn);
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Search for region (house) forcing chains.
	/// </summary>
	/// <param name="grid"><inheritdoc cref="NonMultipleChainingStepSearcher.Collect(in Grid, bool, bool)" path="/param[@name='grid']"/></param>
	/// <param name="result">
	/// <inheritdoc
	///     cref="NonMultipleChainingStepSearcher.DoUnaryChaining(in Grid, ChainNode, List{ChainingStep}, bool, bool)"
	///     path="/param[@name='result']"/>
	/// </param>
	/// <param name="baseCell">Indicates the starting cell.</param>
	/// <param name="baseDigit">Indicates the digit that begins the chaining from starting house where <paramref name="baseCell"/> lies.</param>
	/// <param name="onToOn">An empty set, filled with potentials that get on if the given potential is on.</param>
	private void DoHouseChaining(scoped in Grid grid, List<BlossomLoopStep> result, byte baseCell, byte baseDigit, NodeSet onToOn)
	{
		foreach (var houseType in HouseTypes)
		{
			var houseIndex = baseCell.ToHouseIndex(houseType);
			var potentialPositions = HousesMap[houseIndex] & CandidatesMap[baseDigit];
			if (potentialPositions.Count >= 2 && potentialPositions[0] == baseCell)
			{
				// We meet region for the first time.

				// Different with normal forcing chains, here we may not use 'posToOff' to record relations
				// because this type is not defined by this technique.
				var posToOn = new ChainBranch();
				//var posToOff = new ChainBranch();

				// Iterate on potential positions within the house.
				foreach (byte otherCell in potentialPositions)
				{
					if (otherCell == baseCell)
					{
						posToOn.Add(otherCell, onToOn);
					}
					else
					{
						var otherToOn = new NodeSet { new(otherCell, baseDigit, true) };

						DoChaining(grid, otherToOn, new NodeSet(), false, false);

						posToOn.Add(otherCell, otherToOn);
					}
				}

				// Check for target types.
				CheckForCellTargetType(posToOn, potentialPositions, baseDigit, grid, houseIndex, result);
				CheckForHouseTargetType(posToOn, potentialPositions, baseDigit, grid, houseIndex, result);
			}
		}
	}

	/// <summary>
	/// Check for cell-target type.
	/// </summary>
	private void CheckForCellTargetType(
		ChainBranch posToOn,
		scoped in CellMap potentialPositions,
		byte baseDigit,
		scoped in Grid grid,
		House houseIndex,
		List<BlossomLoopStep> result
	)
	{
		// Gets the cells that all branches of 'posToOn' contain. This is used for cell type (into a cell).
		var cellsAllBranchesContain = ~CellMap.Empty;
		foreach (var (_, nodes) in posToOn)
		{
			var tempCells = CellMap.Empty;
			foreach (var node in nodes)
			{
				tempCells.Add(node.Cell);
			}

			cellsAllBranchesContain &= tempCells;
		}

		// Itertes on all possible target cells.
		foreach (byte targetCell in cellsAllBranchesContain)
		{
			// Records chain nodes from each branch, ending with target cell.
			var selectedPotentials = new List<ChainNode>(posToOn.Count);
			var selectedPotentialDigits = (Mask)0;
			foreach (var (_, nodes) in posToOn)
			{
				foreach (var node in nodes)
				{
					if (node.Cell == targetCell)
					{
						selectedPotentials.Add(node);
						selectedPotentialDigits |= (Mask)(1 << node.Digit);

						break;
					}
				}
			}

			// Determine whether the number of ending candidates is equal to the number of branches.
			// This is a very important rule.
			if (selectedPotentials.Count != potentialPositions.Count)
			{
				continue;
			}

			// Due to the design of the chaining rule, we cannot determine the connection between each branch
			// and its corresponding cell from starting house.
			// We should manually check for this, and determine whether the correspoding relations are "1 to 1".
			if (!IsOneToOneRelationBetweenStartAndEndNodes(selectedPotentials, potentialPositions, baseDigit, out var projectedStartNodes))
			{
				continue;
			}

			var step = CreateStepCellType(
				grid,
				houseIndex,
				baseDigit,
				projectedStartNodes,
				targetCell,
				(Mask)(grid.GetCandidates(targetCell) & ~selectedPotentialDigits)
			);
			if (step is not null)
			{
				result.Add(step);
			}
		}
	}

	/// <summary>
	/// Check for house-target type.
	/// </summary>
	private void CheckForHouseTargetType(
		ChainBranch posToOn,
		scoped in CellMap potentialPositions,
		byte baseDigit,
		scoped in Grid grid,
		House houseIndex,
		List<BlossomLoopStep> result
	)
	{
		var housesAllBranchesContain = AllHousesMask;
		var digitsAllBranchesContain = Grid.MaxCandidatesMask;
		foreach (var (_, nodes) in posToOn)
		{
			var tempHouseMask = 0;
			var tempDigitsMask = (Mask)0;
			foreach (var (c, d, _) in nodes)
			{
				tempHouseMask |= 1 << c.ToHouseIndex(HouseType.Block);
				tempHouseMask |= 1 << c.ToHouseIndex(HouseType.Row);
				tempHouseMask |= 1 << c.ToHouseIndex(HouseType.Column);

				tempDigitsMask |= (Mask)(1 << d);
			}

			housesAllBranchesContain &= tempHouseMask;
			digitsAllBranchesContain &= tempDigitsMask;
		}

		foreach (byte digit in digitsAllBranchesContain)
		{
			foreach (var house in housesAllBranchesContain)
			{
				var selectedPotentials = new List<ChainNode>(posToOn.Count);
				var selectedPotentialCells = CellMap.Empty;
				foreach (var (_, nodes) in posToOn)
				{
					foreach (var node in nodes)
					{
						var (tempCell, tempDigit, _) = node;
						if (HousesMap[house].Contains(tempCell) && tempDigit == digit)
						{
							selectedPotentials.Add(node);
							selectedPotentialCells.Add(tempCell);

							break;
						}
					}
				}

				if (selectedPotentials.Count != potentialPositions.Count)
				{
					continue;
				}

				if (!IsOneToOneRelationBetweenStartAndEndNodes(selectedPotentials, potentialPositions, baseDigit, out var projectedStartNodes))
				{
					continue;
				}

				var step = CreateStepHouseType(
					grid,
					houseIndex,
					baseDigit,
					projectedStartNodes,
					(HousesMap[house] & CandidatesMap[digit]) - selectedPotentialCells,
					digit
				);
				if (step is not null)
				{
					result.Add(step);
				}
			}
		}
	}

	/// <summary>
	/// Determine whether one target node corresponds to one base node begins the branch, and vice versa.
	/// </summary>
	/// <param name="selectedPotentials">The target nodes.</param>
	/// <param name="potentialPositions">The potential cells.</param>
	/// <param name="baseDigit">The digit.</param>
	/// <param name="projectedStartNodes">The projected starting nodes, which can be used if return value is <see langword="true"/>.</param>
	/// <returns>A <see cref="bool"/> indicating that.</returns>
	private bool IsOneToOneRelationBetweenStartAndEndNodes(
		List<ChainNode> selectedPotentials,
		scoped in CellMap potentialPositions,
		byte baseDigit,
		[NotNullWhen(true)] out Dictionary<ChainNode, Candidate>? projectedStartNodes
	)
	{
		projectedStartNodes = new(selectedPotentials.Count);

		foreach (var potential in selectedPotentials)
		{
			if (potential.ChainPotentials is not [.., var (branchStartCell, branchStartDigit, _)] branch)
			{
				projectedStartNodes = null;
				return false;
			}

			var selectedPositions = CandidateMap.Empty;
			foreach (var position in potentialPositions)
			{
				if (branchStartDigit != baseDigit && position == branchStartCell)
				{
					selectedPositions.Add(position * 9 + branchStartDigit);
				}
				else if (branchStartDigit == baseDigit && PeersMap[position].Contains(branchStartCell))
				{
					selectedPositions.Add(position * 9 + baseDigit);
				}
			}

			if (selectedPositions is not [var selectedPosition])
			{
				projectedStartNodes = null;
				return false;
			}

			if (!projectedStartNodes.TryAdd(potential, selectedPosition))
			{
				projectedStartNodes = null;
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Try to collect all possible eliminations for each branch.
	/// </summary>
	/// <param name="outcomes">Each branch.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="baseDigit">The base digit.</param>
	/// <returns>All possible eliminations. If none found, an empty list.</returns>
	private List<Conclusion> CollectLoopEliminations(Dictionary<ChainNode, Candidate> outcomes, scoped in Grid grid, byte baseDigit)
	{
		var conclusions = new List<Conclusion>();
		foreach (var (branch, headCandidate) in outcomes)
		{
			var nodes = branch.ChainPotentials;
			Array.Resize(ref nodes, nodes.Length + 1);
			nodes[^1] = new(headCandidate, true);

			for (var i = 1; i < nodes.Length; i += 2)
			{
				var (c1, d1, _) = nodes[i];
				var (c2, d2, _) = nodes[i + 1];
				if (c1 == c2)
				{
					foreach (var d in (Mask)(grid.GetCandidates(c1) & ~(1 << d1 | 1 << d2 | 1 << baseDigit)))
					{
						conclusions.Add(new(Elimination, c1, d));
					}
				}
				else if (d1 == d2)
				{
					foreach (var house in (CellsMap[c1] + c2).CoveredHouses)
					{
						foreach (var cell in (HousesMap[house] & CandidatesMap[d1]) - c1 - c2)
						{
							conclusions.Add(new(Elimination, cell, d1));
						}
					}
				}
			}
		}

		return conclusions;
	}

	/// <summary>
	/// Try to create a hint, for a cell type.
	/// </summary>
	/// <param name="grid">The grid that is used for checking existence of candidates in order to find eliminations.</param>
	/// <param name="houseIndex">Indicates the house where the base digits lies.</param>
	/// <param name="digit">Indicates the digit of the base house used.</param>
	/// <param name="outcomes">All branches.</param>
	/// <param name="targetCell">Indicates the target cell which makes all branches end with.</param>
	/// <param name="elimDigitsMask">Indicates mask of digits can be eliminated in target cell.</param>
	private BlossomLoopStep? CreateStepCellType(
		scoped in Grid grid,
		House houseIndex,
		byte digit,
		Dictionary<ChainNode, Candidate> outcomes,
		byte targetCell,
		Mask elimDigitsMask
	)
	{
		// Eliminates with all possible weak links in the whole loop.
		var conclusions = CollectLoopEliminations(outcomes, grid, digit);

		// Eliminates with digits from the target cell.
		foreach (var elimDigit in elimDigitsMask)
		{
			conclusions.Add(new(Elimination, targetCell, elimDigit));
		}

		if (conclusions.Count == 0)
		{
			return null;
		}

		// Build chains.
		var chains = new MultipleForcingChains();
		foreach (var (branch, headCandidate) in outcomes)
		{
			// Get corresponding value with the matching parents.
			chains.Add((byte)(headCandidate / 9), branch);
		}

		var result = new BlossomLoopStep(conclusions.ToArray(), houseIndex, digit, chains);
		return new(result, result.CreateViews());
	}

	/// <summary>
	/// Try to create a hint, for a house type.
	/// </summary>
	/// <param name="grid">The grid that is used for checking existence of candidates in order to find eliminations.</param>
	/// <param name="houseIndex">Indicates the house where the base digits lies.</param>
	/// <param name="digit">Indicates the digit of the base house used.</param>
	/// <param name="outcomes">All branches.</param>
	/// <param name="elimCells">Indicates cells can be eliminated in the target house.</param>
	/// <param name="targetDigit">Indicates the target digit.</param>
	private BlossomLoopStep? CreateStepHouseType(
		scoped in Grid grid,
		House houseIndex,
		byte digit,
		Dictionary<ChainNode, Candidate> outcomes,
		scoped in CellMap elimCells,
		byte targetDigit
	)
	{
		var conclusions = CollectLoopEliminations(outcomes, grid, digit);

		foreach (var cell in elimCells)
		{
			conclusions.Add(new(Elimination, cell, targetDigit));
		}

		if (conclusions.Count == 0)
		{
			return null;
		}

		var chains = new MultipleForcingChains();
		foreach (var (branch, headCandidate) in outcomes)
		{
			chains.Add((byte)(headCandidate / 9), branch);
		}

		var result = new BlossomLoopStep(conclusions.ToArray(), houseIndex, digit, chains);
		return new(result, result.CreateViews());
	}
}
