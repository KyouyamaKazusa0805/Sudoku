namespace Sudoku.Generating;

/// <summary>
/// Represents a generator that is based on pattern.
/// </summary>
/// <param name="seedPattern">Indicates the predefind pattern used.</param>
public ref struct PatternBasedPuzzleGenerator(params CellMap seedPattern)
{
	/// <summary>
	/// Indicates the test grid.
	/// </summary>
	private Grid _playground;

	/// <summary>
	/// Indicates the result grid.
	/// </summary>
	private Grid _resultGrid;


	/// <summary>
	/// Try to generate a puzzle using the specified pattern.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token that can cancel the operation.</param>
	/// <returns>A valid <see cref="Grid"/> pattern that has a specified pattern, with specified digits should be filled in.</returns>
	public Grid Generate(CancellationToken cancellationToken = default)
	{
		try
		{
			(_playground, _resultGrid, var patternCellsSorted) = (Grid.Empty, Grid.Undefined, OrderPatternCellsViaConnectionDegrees());
			getGrid(patternCellsSorted, ref _playground, ref _resultGrid, 0);
		}
		catch (OperationCanceledException)
		{
		}
		catch
		{
			throw;
		}

		return _resultGrid.FixedGrid;


		void getGrid(Cell[] patternCellsSorted, ref Grid playground, ref Grid resultGrid, int currentIndex)
		{
			if (currentIndex == patternCellsSorted.Length)
			{
				if (playground.FixedGrid is { Uniqueness: Uniqueness.Unique } result)
				{
					resultGrid = result;
					throw new OperationCanceledException("Finished.");
				}

				return;
			}

			if (playground.GetCandidates(patternCellsSorted[currentIndex]) == 0)
			{
				// Invalid state.
				return;
			}

			var cell = patternCellsSorted[currentIndex];
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

				getGrid(patternCellsSorted, ref playground, ref resultGrid, currentIndex + 1);
			}

			playground.SetDigit(cell, -1);
		}
	}

	/// <summary>
	/// Order the pattern cells via connection complexity.
	/// </summary>
	/// <returns>The cells ordered.</returns>
	private readonly Cell[] OrderPatternCellsViaConnectionDegrees()
	{
		var isOrdered = (CellMap)[];
		var result = new Cell[seedPattern.Count];
		for (var index = 0; index < seedPattern.Count; index++)
		{
			var maxRating = 0;
			var best = -1;
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
					maxRating = rating;
					best = i;
				}
			}

			isOrdered.Add(best);
			result[index] = best;
		}

		return result;
	}
}
