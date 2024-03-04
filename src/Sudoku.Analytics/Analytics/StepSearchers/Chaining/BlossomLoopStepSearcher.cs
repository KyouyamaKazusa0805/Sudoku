namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Blossom Loop</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Blossom Loop</item>
/// </list>
/// </summary>
[StepSearcher(Technique.BlossomLoop)]
[StepSearcherRuntimeName("StepSearcherName_BlossomLoopStepSearcher")]
public sealed partial class BlossomLoopStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		var result = Collect(in grid, ref context);
		if (result.Count == 0)
		{
			return null;
		}

		Step.SortItems(result);

		if (context.OnlyFindOne)
		{
			return result[0];
		}

		context.Accumulator.AddRange(result);
		return null;
	}

	/// <summary>
	/// <inheritdoc cref="Collect(ref AnalysisContext)" path="/summary"/>
	/// </summary>
	/// <param name="grid">The grid on which to search for hints.</param>
	/// <param name="context">The context.</param>
	/// <returns>The hints found.</returns>
	private List<BlossomLoopStep> Collect(scoped ref readonly Grid grid, scoped ref AnalysisContext context)
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
					var onToOn = (NodeSet)([new(cell, digit, true)]);
					ChainingModule.DoChaining(this, grid, onToOn, [], false, false);

					// Do house chaining.
					DoHouseChaining(in grid, ref context, result, cell, digit, onToOn);
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Search for region (house) forcing chains.
	/// </summary>
	/// <param name="grid">
	/// <inheritdoc cref="NonMultipleChainingStepSearcher.Collect(ref readonly Grid, ref AnalysisContext, bool, bool)" path="/param[@name='grid']"/>
	/// </param>
	/// <param name="context">The context.</param>
	/// <param name="result">
	/// <inheritdoc
	///     cref="NonMultipleChainingStepSearcher.DoUnaryChaining(ref readonly Grid, ref AnalysisContext, ChainNode, List{ChainingStep}, bool, bool)"
	///     path="/param[@name='result']"/>
	/// </param>
	/// <param name="baseCell">Indicates the starting cell.</param>
	/// <param name="baseDigit">Indicates the digit that begins the chaining from starting house where <paramref name="baseCell"/> lies.</param>
	/// <param name="onToOn">An empty set, filled with potentials that get on if the given potential is on.</param>
	/// <remarks>
	/// This method is nearly same with
	/// <see cref="MultipleChainingStepSearcher.DoHouseChaining(ref readonly Grid, ref AnalysisContext, List{ChainingStep}, byte, byte, NodeSet, NodeSet)"/>,
	/// with variables <c>houseToOn</c> and <c>houseToOff</c> removed.
	/// </remarks>
	/// <seealso cref="MultipleChainingStepSearcher.DoHouseChaining(ref readonly Grid, ref AnalysisContext, List{ChainingStep}, byte, byte, NodeSet, NodeSet)"/>
	private void DoHouseChaining(
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		List<BlossomLoopStep> result,
		byte baseCell,
		byte baseDigit,
		NodeSet onToOn
	)
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
				var posToOn = new ChainBranchCollection();
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
						var otherToOn = (NodeSet)([new(otherCell, baseDigit, true)]);
						ChainingModule.DoChaining(this, grid, otherToOn, [], false, false);

						posToOn.Add(otherCell, otherToOn);
					}
				}

				// Check for target types.
				CheckForCellTargetType(posToOn, in potentialPositions, baseDigit, in grid, ref context, houseIndex, result);
				CheckForHouseTargetType(posToOn, in potentialPositions, baseDigit, in grid, ref context, houseIndex, result);
			}
		}
	}

	/// <summary>
	/// Check for cell-target type.
	/// </summary>
	private void CheckForCellTargetType(
		ChainBranchCollection posToOn,
		scoped ref readonly CellMap potentialPositions,
		byte baseDigit,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
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

		// Iterates on all possible target cells.
		foreach (byte targetCell in cellsAllBranchesContain)
		{
			// Records chain nodes from each branch, ending with target cell.
			var selectedPotentials = new NodeList();
			var selectedPotentialDigits = (Mask)0;
			foreach (var (_, nodes) in posToOn)
			{
				foreach (var node in nodes)
				{
					if (node.Cell == targetCell)
					{
						selectedPotentials.AddLast(node);
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
			// We should manually check for this, and determine whether the corresponding relations are "1 to 1".
			if (!IsOneToOneRelationBetweenStartAndEndNodes(selectedPotentials, in potentialPositions, baseDigit, out var projectedStartNodes))
			{
				continue;
			}

			var step = CreateStepCellType(
				in grid,
				ref context,
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
		ChainBranchCollection posToOn,
		scoped ref readonly CellMap potentialPositions,
		byte baseDigit,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		House houseIndex,
		List<BlossomLoopStep> result
	)
	{
		var (housesAllBranchesContain, digitsAllBranchesContain) = (HouseMaskOperations.AllHousesMask, Grid.MaxCandidatesMask);
		foreach (var (_, nodes) in posToOn)
		{
			var (tempHouseMask, tempDigitsMask) = (0, (Mask)0);
			foreach (var (c, d, _) in nodes)
			{
				tempHouseMask |= c.ToHouseIndices();
				tempDigitsMask |= (Mask)(1 << d);
			}

			housesAllBranchesContain &= tempHouseMask;
			digitsAllBranchesContain &= tempDigitsMask;
		}

		foreach (byte digit in digitsAllBranchesContain)
		{
			foreach (var house in housesAllBranchesContain)
			{
				var selectedPotentials = new NodeList();
				var selectedPotentialCells = CellMap.Empty;
				foreach (var (_, nodes) in posToOn)
				{
					foreach (var node in nodes)
					{
						var (tempCell, tempDigit, _) = node;
						if (HousesMap[house].Contains(tempCell) && tempDigit == digit)
						{
							selectedPotentials.AddLast(node);
							selectedPotentialCells.Add(tempCell);

							break;
						}
					}
				}

				if (selectedPotentials.Count != potentialPositions.Count)
				{
					continue;
				}

				if (!IsOneToOneRelationBetweenStartAndEndNodes(selectedPotentials, in potentialPositions, baseDigit, out var projectedStartNodes))
				{
					continue;
				}

				var step = CreateStepHouseType(
					in grid,
					ref context,
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
		NodeList selectedPotentials,
		scoped ref readonly CellMap potentialPositions,
		byte baseDigit,
		[NotNullWhen(true)] out ChainNodeListWithHeadCandidate? projectedStartNodes
	)
	{
		projectedStartNodes = new(selectedPotentials.Count);

		foreach (var potential in selectedPotentials)
		{
			if (potential.ChainPotentials is not [.., var (branchStartCell, branchStartDigit, _)])
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
	private List<Conclusion> CollectLoopEliminations(ChainNodeListWithHeadCandidate outcomes, scoped ref readonly Grid grid, byte baseDigit)
	{
		var conclusions = new List<Conclusion>();
		foreach (var (branch, headCandidate) in outcomes)
		{
			var nodes = branch.ChainPotentials;
			Array.Resize(ref nodes, nodes.Length + 1);
			nodes[^1] = new(headCandidate, true);

			for (var i = 1; i < nodes.Length; i += 2)
			{
				var ((c1, d1, _), (c2, d2, _)) = (nodes[i], nodes[i + 1]);
				if (c1 == c2)
				{
					foreach (var d in (Mask)(grid.GetCandidates(c1) & (Mask)~(1 << d1 | 1 << d2 | 1 << baseDigit)))
					{
						conclusions.Add(new(Elimination, c1, d));
					}
				}
				else if (d1 == d2)
				{
					foreach (var house in ((CellMap)c1 + c2).SharedHouses)
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
	/// <param name="context">The context.</param>
	/// <param name="houseIndex">Indicates the house where the base digits lies.</param>
	/// <param name="digit">Indicates the digit of the base house used.</param>
	/// <param name="outcomes">All branches.</param>
	/// <param name="targetCell">Indicates the target cell which makes all branches end with.</param>
	/// <param name="elimDigitsMask">Indicates mask of digits can be eliminated in target cell.</param>
	private BlossomLoopStep? CreateStepCellType(
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		House houseIndex,
		byte digit,
		ChainNodeListWithHeadCandidate outcomes,
		byte targetCell,
		Mask elimDigitsMask
	)
	{
		// Eliminates with all possible weak links in the whole loop.
		var conclusions = CollectLoopEliminations(outcomes, in grid, digit);

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

		var result = new BlossomLoopStep([.. conclusions], context.PredefinedOptions, houseIndex, digit, chains);
		return new(result, result.CreateViews());
	}

	/// <summary>
	/// Try to create a hint, for a house type.
	/// </summary>
	/// <param name="grid">The grid that is used for checking existence of candidates in order to find eliminations.</param>
	/// <param name="context">The context.</param>
	/// <param name="houseIndex">Indicates the house where the base digits lies.</param>
	/// <param name="digit">Indicates the digit of the base house used.</param>
	/// <param name="outcomes">All branches.</param>
	/// <param name="elimCells">Indicates cells can be eliminated in the target house.</param>
	/// <param name="targetDigit">Indicates the target digit.</param>
	private BlossomLoopStep? CreateStepHouseType(
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		House houseIndex,
		byte digit,
		ChainNodeListWithHeadCandidate outcomes,
		scoped ref readonly CellMap elimCells,
		byte targetDigit
	)
	{
		var conclusions = CollectLoopEliminations(outcomes, in grid, digit);
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

		var result = new BlossomLoopStep([.. conclusions], context.PredefinedOptions, houseIndex, digit, chains);
		return new(result, result.CreateViews());
	}
}
