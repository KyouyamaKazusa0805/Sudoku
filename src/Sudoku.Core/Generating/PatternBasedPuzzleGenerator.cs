namespace Sudoku.Generating;

/// <summary>
/// Represents a generator that is based on pattern.
/// </summary>
/// <param name="seedPattern">Indicates the predefind pattern used.</param>
public readonly ref struct PatternBasedPuzzleGenerator(params CellMap seedPattern)
{
	/// <summary>
	/// Try to generate a puzzle using the specified pattern.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token that can cancel the operation.</param>
	/// <returns>A valid <see cref="Grid"/> pattern that has a specified pattern, with specified digits should be filled in.</returns>
	public Grid Generate(CancellationToken cancellationToken = default)
	{
		var resultGrid = Grid.Undefined;
		try
		{
			var playground = Grid.Empty;
			GenerateCore(OrderPatternCellsViaConnectionDegrees(), ref playground, ref resultGrid, 0, cancellationToken);
		}
		catch (OperationCanceledException)
		{
		}
		catch
		{
			throw;
		}

		return resultGrid;
	}

	/// <summary>
	/// Order the pattern cells via connection complexity.
	/// </summary>
	/// <returns>The cells ordered.</returns>
	private Cell[] OrderPatternCellsViaConnectionDegrees()
	{
		var (isOrdered, result) = ((CellMap)[], new Cell[seedPattern.Count]);
		for (var index = 0; index < seedPattern.Count; index++)
		{
			var (maxRating, best) = (0, -1);
			for (var i = 0; i < 81; i++)
			{
				if (!seedPattern.Contains(i) || isOrdered.Contains(i))
				{
					continue;
				}

				var rating = 0;
				for (var j = 0; j < 81; j++)
				{
					if (!seedPattern.Contains(j) || i == j)
					{
						continue;
					}

					if (i.ToHouseIndex(HouseType.Block) == j.ToHouseIndex(HouseType.Block)
						|| i.ToHouseIndex(HouseType.Row) == j.ToHouseIndex(HouseType.Row)
						|| i.ToHouseIndex(HouseType.Column) == j.ToHouseIndex(HouseType.Column))
					{
						rating += isOrdered.Contains(j) ? 10000 : 100;
					}

					if (!isOrdered.Contains(j) && (i.ToBandIndex() == j.ToBandIndex() || i.ToTowerIndex() == j.ToTowerIndex())
						&& i.ToHouseIndex(HouseType.Block) == j.ToHouseIndex(HouseType.Block))
					{
						rating++;
					}
				}

				if (maxRating < rating)
				{
					(maxRating, best) = (rating, i);
				}
			}

			(_, result[index]) = (isOrdered.Add(best), best);
		}

		return result;
	}


	/// <summary>
	/// The back method to generate a valid sudoku grid puzzle.
	/// </summary>
	/// <param name="patternCellsSorted">The cells ordered by the number of related cells.</param>
	/// <param name="playground">The playground to be operated with.</param>
	/// <param name="resultGrid">The result grid to be returned.</param>
	/// <param name="i">The index that the current searching is on.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the operation.</param>
	private static void GenerateCore(Cell[] patternCellsSorted, ref Grid playground, ref Grid resultGrid, int i, CancellationToken cancellationToken)
	{
		if (i == patternCellsSorted.Length)
		{
			if (playground.FixedGrid is { Uniqueness: Uniqueness.Unique } result)
			{
				resultGrid = result;
				throw new OperationCanceledException("Finished.");
			}
			return;
		}

		var cell = patternCellsSorted[i];
		var digits = playground.GetCandidates(cell).GetAllSets();
		var indexArray = Digits[..digits.Length];
		Random.Shared.Shuffle(indexArray);
		foreach (var randomizedIndex in indexArray)
		{
			playground.ReplaceDigit(cell, digits[randomizedIndex]);

			if (playground.FixedGrid.Uniqueness == Uniqueness.Bad)
			{
				playground.SetDigit(cell, -1);
				continue;
			}

			cancellationToken.ThrowIfCancellationRequested();

			GenerateCore(patternCellsSorted, ref playground, ref resultGrid, i + 1, cancellationToken);
		}

		playground.SetDigit(cell, -1);
	}
}
