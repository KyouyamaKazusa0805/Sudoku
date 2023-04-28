namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Blossom Loops</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Blossom Loops</item>
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

		result.Sort(BlossomLoopStep.Compare);

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
	/// <param name="cell"></param>
	/// <param name="digit"></param>
	/// <param name="onToOn">An empty set, filled with potentials that get on if the given potential is on.</param>
	private void DoHouseChaining(scoped in Grid grid, List<BlossomLoopStep> result, byte cell, byte digit, NodeSet onToOn)
	{
		foreach (var houseType in HouseTypes)
		{
			var houseIndex = cell.ToHouseIndex(houseType);
			var potentialPositions = HousesMap[houseIndex] & CandidatesMap[digit];
			if (potentialPositions.Count >= 2 && potentialPositions[0] == cell)
			{
				// We meet region for the first time.

				// Different with normal forcing chains, here we may not use 'posToOff' to record relations
				// because this type is not defined by this technique.
				var posToOn = new ChainBranch();
				//var posToOff = new ChainBranch();

				// Iterate on potential positions within the house.
				foreach (byte otherCell in potentialPositions)
				{
					if (otherCell == cell)
					{
						posToOn.Add(otherCell, onToOn);
					}
					else
					{
						var otherToOn = new NodeSet { new(otherCell, digit, true) };

						DoChaining(grid, otherToOn, new NodeSet(), false, false);

						posToOn.Add(otherCell, otherToOn);
					}
				}

				// Gather results.
				{
					// For cell type.

					// Gets the cells that all branches of 'posToOn' contain. This is used for cell type (into a cell).
					var cellsAllBranchesContain = ~CellMap.Empty;
					foreach (var (_, nodes) in posToOn)
					{
						var map = CellMap.Empty;
						foreach (var node in nodes)
						{
							map.Add(node.Cell);
						}

						cellsAllBranchesContain &= map;
					}

					// Itertes on all possible target cells.
					foreach (byte targetCell in cellsAllBranchesContain)
					{
						// Records chain nodes from each branch, ending with target cell.
						var selectedPotentials = new List<ChainNode>(posToOn.Count);
						foreach (var (_, nodes) in posToOn)
						{
							foreach (var node in nodes)
							{
								if (node.Cell == targetCell)
								{
									selectedPotentials.Add(node);

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

						var projectedStartNodes = new Dictionary<ChainNode, Candidate>(selectedPotentials.Count);

						// Due to the design of the chaining rule, we cannot determine the connection between each branch
						// and its corresponding cell from starting house.
						// We should manually check for this, and determine whether the correspoding relations are "1 to 1".
						var finalFlag = true;
						foreach (var potential in selectedPotentials)
						{
							if (potential.ChainPotentials is not [.., var (branchStartCell, startDigit, _)] branch)
							{
								finalFlag = false;
								break;
							}

							var selectedPositions = CandidateMap.Empty;
							foreach (var position in potentialPositions)
							{
								if (startDigit != digit && position == branchStartCell)
								{
									selectedPositions.Add(position * 9 + startDigit);
								}
								else if (startDigit == digit && PeersMap[position].Contains(branchStartCell))
								{
									selectedPositions.Add(position * 9 + digit);
								}
							}

							if (selectedPositions is not [var selectedPosition])
							{
								finalFlag = false;
								break;
							}

							if (!projectedStartNodes.TryAdd(potential, selectedPosition))
							{
								finalFlag = false;
								break;
							}
						}
						if (!finalFlag)
						{
							continue;
						}

						// Collect digits that target node used. This variable can be used for finding eliminations in the target cell.
						var targetDigitsMask = (Mask)0;
						foreach (var selectedPotential in selectedPotentials)
						{
							targetDigitsMask |= (Mask)(1 << selectedPotential.Digit);
						}

						var step = CreateStepCellType(
							grid,
							houseIndex,
							digit,
							projectedStartNodes,
							targetCell,
							(Mask)(grid.GetCandidates(targetCell) & ~targetDigitsMask)
						);
						if (step is not null)
						{
							result.Add(step);
						}
					}
				}
				{
					// For house type.


				}
			}
		}
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
		var conclusions = new List<Conclusion>();

		// Eliminates with all possible weak links in the whole loop.
		foreach (var (branch, headCandidate) in outcomes)
		{
			var nodes = branch.ChainPotentials;
			nodes = nodes.Append(new(headCandidate, true)).ToArray();
			for (var i = 1; i < nodes.Length; i += 2)
			{
				var (c1, d1, _) = nodes[i];
				var (c2, d2, _) = nodes[i + 1];
				if (c1 == c2)
				{
					foreach (var d in (Mask)(grid.GetCandidates(c1) & ~(1 << d1 | 1 << d2 | 1 << digit)))
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

		// Eliminates with digits from the target cell.
		foreach (var elimDigit in elimDigitsMask)
		{
			conclusions.Add(new(Elimination, targetCell * 9 + elimDigit));
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
}
