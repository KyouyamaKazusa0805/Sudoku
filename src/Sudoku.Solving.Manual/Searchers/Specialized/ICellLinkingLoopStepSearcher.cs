namespace Sudoku.Solving.Manual.Searchers.Specialized;

/// <summary>
/// Defines a steps searcher that searches for cell-linking loop steps.
/// </summary>
public unsafe interface ICellLinkingLoopStepSearcher : IStepSearcher
{
	/// <summary>
	/// Converts all cells to the links that is used in drawing ULs or Reverse BUGs.
	/// </summary>
	/// <param name="this">The list of cells.</param>
	/// <param name="offset">The offset. The default value is <c>4</c>.</param>
	/// <returns>All links.</returns>
	protected static sealed IEnumerable<LinkViewNode> GetLinks(IReadOnlyList<int> @this, int offset = 4)
	{
		var result = new List<LinkViewNode>();

		for (int i = 0, length = @this.Count - 1; i < length; i++)
		{
			result.Add(
				new(
					DisplayColorKind.Normal,
					new(offset, @this[i]),
					new(offset, @this[i + 1]),
					Inference.Default
				)
			);
		}

		result.Add(new(DisplayColorKind.Normal, new(offset, @this[^1]), new(offset, @this[0]), Inference.Default));

		return result;
	}

	/// <summary>
	/// Try to gather all possible loops which should satisfy the specified condition.
	/// </summary>
	/// <param name="digit">The digit used.</param>
	/// <param name="condition">
	/// The condition to verify the specified loop satisfies the current condition, as a function pointer.
	/// </param>
	/// <returns>
	/// Returns a list of array of candidates used in the loop, as the data of possible found loops.
	/// If none found, <see langword="null"/>.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="condition"/> is <see langword="null"/>.
	/// </exception>
	protected static sealed GuardianData[] GatherGuardianLoops(int digit, delegate*<in Cells, bool> condition)
	{
		ArgumentNullException.ThrowIfNull(condition);

		var result = new List<GuardianData>();
		foreach (int cell in CandidatesMap[digit])
		{
			dfs(cell, cell, 0, Cells.Empty + cell, Cells.Empty, digit, condition, result);
		}

		return result.Distinct().ToArray();


		static void dfs(
			int startCell, int lastCell, int lastHouse, scoped in Cells currentLoop,
			scoped in Cells currentGuardians, int digit, delegate*<in Cells, bool> condition,
			List<GuardianData> result)
		{
			foreach (var houseType in HouseTypes)
			{
				int house = lastCell.ToHouseIndex(houseType);
				if ((lastHouse >> house & 1) != 0)
				{
					continue;
				}

				var cellsToBeChecked = CandidatesMap[digit] & HouseMaps[house];
				if (cellsToBeChecked.Count < 2 || (currentLoop & HouseMaps[house]).Count > 2)
				{
					continue;
				}

				foreach (int tempCell in cellsToBeChecked)
				{
					if (tempCell == lastCell)
					{
						continue;
					}

					int housesUsed = 0;
					foreach (var tempHouseType in HouseTypes)
					{
						if (tempCell.ToHouseIndex(tempHouseType) == lastCell.ToHouseIndex(tempHouseType))
						{
							housesUsed |= 1 << lastCell.ToHouseIndex(tempHouseType);
						}
					}

					var tempGuardians = (CandidatesMap[digit] & HouseMaps[house]) - tempCell - lastCell;
					if (tempCell == startCell && condition(currentLoop)
						&& (!(currentGuardians | tempGuardians) & CandidatesMap[digit]) is not [])
					{
						result.Add(new(currentLoop, currentGuardians | tempGuardians, digit));

						// Exit the current of this recursion frame.
						return;
					}

					if ((currentLoop | currentGuardians).Contains(tempCell)
						|| (!(currentGuardians | tempGuardians) & CandidatesMap[digit]) is []
						|| (HouseMaps[house] & currentLoop).Count > 1)
					{
						continue;
					}

					dfs(
						startCell, tempCell, lastHouse | housesUsed, currentLoop + tempCell,
						currentGuardians | tempGuardians, digit, condition, result);
				}
			}
		}
	}

	/// <summary>
	/// Try to gather all possible loops being used in technique unique loops,
	/// which should satisfy the specified condition.
	/// </summary>
	/// <param name="digit">The digit used.</param>
	/// <param name="condition">
	/// The condition to verify the specified loop satisfies the current condition, as a function pointer.
	/// </param>
	/// <returns>
	/// Returns a list of array of candidates used in the loop, as the data of possible found loops.
	/// If none found, <see langword="null"/>.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="condition"/> is <see langword="null"/>.
	/// </exception>
	protected static sealed Cells[] GatherUniqueLoops(int digit, delegate*<in Cells, bool> condition)
	{
		ArgumentNullException.ThrowIfNull(condition);

		return Array.Empty<Cells>();
	}
}
