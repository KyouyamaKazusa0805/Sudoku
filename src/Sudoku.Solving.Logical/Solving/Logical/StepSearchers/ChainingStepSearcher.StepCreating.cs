namespace Sudoku.Solving.Logical.StepSearchers;

partial class ChainingStepSearcher
{
	/// <summary>
	/// Try to create a cycle hint. If any conclusion (elimination, assignment) found and is available,
	/// the method will return a <see cref="BidirectionalCycleStep"/> instance with a non-<see langword="null"/> value.
	/// </summary>
	/// <param name="grid">
	/// <inheritdoc cref="GetNonMultipleChains(in Grid, bool, bool)" path="/param[@name='grid']"/>
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
	private BidirectionalCycleStep? CreateCycleStep(scoped in Grid grid, Potential dstOn, bool isX, bool isY)
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
					foreach (var cell in (HousesMap[house] & CandidatesMap[d1]) - c1 - c2)
					{
						conclusions.Add(new(Elimination, cell, d1));
					}
				}
			}
		}

		if (conclusions.Count == 0)
		{
			return null;
		}

		var result = new BidirectionalCycleStep(ImmutableArray.CreateRange(conclusions), dstOn, isX, isY);
		return result with { Views = result.CreateViews(grid) };
	}

	/// <summary>
	/// Try to create an AIC hint.
	/// </summary>
	private ForcingChainStep CreateAicStep(scoped in Grid grid, Potential target, bool isX, bool isY)
	{
		var (candidate, isOn) = target;
		var conclusion = ImmutableArray.Create(new Conclusion(isOn ? Assignment : Elimination, candidate));
		var result = new ForcingChainStep(conclusion, target, isX, isY);
		return result with { Views = result.CreateViews(grid) };
	}

	/// <summary>
	/// Try to create a binary forcing chain hint on "on" state.
	/// </summary>
	private BinaryForcingChainsStep CreateChainingOnStep(scoped in Grid grid, Potential dstOn, Potential dstOff, Potential src, Potential target, bool isAbsurd)
	{
		var conclusion = ImmutableArray.Create(new Conclusion(Assignment, target.Candidate));
		var result = new BinaryForcingChainsStep(conclusion, src, dstOn, dstOff, isAbsurd, AllowNishio, DynamicNestingLevel);
		return result with { Views = result.CreateViews(grid) };
	}

	/// <summary>
	/// Try to create a binary forcing chain hint on "off" state.
	/// </summary>
	private BinaryForcingChainsStep CreateChainingOffStep(scoped in Grid grid, Potential dstOn, Potential dstOff, Potential src, Potential target, bool isAbsurd)
	{
		var conclusion = ImmutableArray.Create(new Conclusion(Elimination, target.Candidate));
		var result = new BinaryForcingChainsStep(conclusion, src, dstOn, dstOff, isAbsurd, AllowNishio, DynamicNestingLevel);
		return result with { Views = result.CreateViews(grid) };
	}

	/// <summary>
	/// Try to create a cell forcing chain hint.
	/// </summary>
	private CellForcingChainsStep CreateCellForcingStep(scoped in Grid grid, byte srcCell, Potential target, ChainBranch outcomes)
	{
		var (targetCell, targetDigit, targetIsOn) = target;
		var conclusion = ImmutableArray.Create(new Conclusion(targetIsOn ? Assignment : Elimination, targetCell, targetDigit));

		// Build chains.
		var chains = new MultipleForcingChains();
		for (byte tempDigit = 0; tempDigit < 9; tempDigit++)
		{
			if (CandidatesMap[tempDigit].Contains(srcCell))
			{
				// Get corresponding value with the matching parents.
				chains.Add(tempDigit, outcomes[tempDigit].GetNullable(target) ?? default);
			}
		}

		var result = new CellForcingChainsStep(conclusion, srcCell, chains, AllowDynamic, DynamicNestingLevel);
		return result with { Views = result.CreateViews(grid) };
	}

	/// <summary>
	/// Try to create a region (house) forcing chain hint.
	/// </summary>
	private RegionForcingChainsStep CreateHouseForcingStep(scoped in Grid grid, int houseIndex, byte digit, Potential target, ChainBranch outcomes)
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

		var result = new RegionForcingChainsStep(conclusions, houseIndex, digit, chains, AllowDynamic, DynamicNestingLevel);
		return result with { Views = result.CreateViews(grid) };
	}
}
