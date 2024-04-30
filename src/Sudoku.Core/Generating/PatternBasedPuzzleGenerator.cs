namespace Sudoku.Generating;

/// <summary>
/// Represents a generator that is based on pattern.
/// </summary>
/// <param name="missingDigit">Indicates the missing digit that can be used.</param>
/// <param name="seedPattern">Indicates the predefind pattern used.</param>
[StructLayout(LayoutKind.Auto)]
public readonly ref partial struct PatternBasedPuzzleGenerator(
	[PrimaryConstructorParameter(MemberKinds.Field)] ref readonly CellMap seedPattern,
	Digit missingDigit = -1
)
{
	/// <inheritdoc cref="IGenerator{TResult}.Generate(IProgress{GeneratorProgress}, CancellationToken)"/>
	public Grid Generate(IProgress<GeneratorProgress>? progress = null, CancellationToken cancellationToken = default)
	{
		if (_seedPattern.Count < 17)
		{
			return Grid.Undefined;
		}

		var resultGrid = Grid.Undefined;
		try
		{
			var (playground, count, cellsOrdered) = (Grid.Empty, 0, OrderCellsViaConnectionComplexity());
			GenerateCore(cellsOrdered, ref playground, ref resultGrid, 0, ref count, progress, cancellationToken);
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
	/// The back method to generate a valid sudoku grid puzzle.
	/// </summary>
	/// <param name="patternCellsSorted">The cells ordered by the number of related cells.</param>
	/// <param name="playground">The playground to be operated with.</param>
	/// <param name="resultGrid">The result grid to be returned.</param>
	/// <param name="i">The index that the current searching is on.</param>
	/// <param name="count">The number of puzzles generated.</param>
	/// <param name="progress">The progress instance.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the operation.</param>
	private void GenerateCore(
		Cell[] patternCellsSorted,
		ref Grid playground,
		ref Grid resultGrid,
		int i,
		ref int count,
		IProgress<GeneratorProgress>? progress,
		CancellationToken cancellationToken
	)
	{
		if (i == patternCellsSorted.Length)
		{
			if (playground.FixedGrid is { Uniqueness: Uniqueness.Unique } result)
			{
				resultGrid = result;
				throw new OperationCanceledException("Finished.");
			}

			progress?.Report(new(count++));
			return;
		}

		var cell = patternCellsSorted[i];
		var digitsMask = playground.GetCandidates(cell);
		var digits = ((Mask)(missingDigit != -1 ? digitsMask & (Mask)~(1 << missingDigit) : digitsMask)).GetAllSets();
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

			GenerateCore(patternCellsSorted, ref playground, ref resultGrid, i + 1, ref count, progress, cancellationToken);
		}

		playground.SetDigit(cell, -1);
	}

	/// <summary>
	/// Order the pattern cells via connection complexity.
	/// </summary>
	/// <returns>The cells ordered.</returns>
	private Cell[] OrderCellsViaConnectionComplexity()
	{
		var (isOrdered, result) = (CellMap.Empty, new Cell[_seedPattern.Count]);
		for (var index = 0; index < _seedPattern.Count; index++)
		{
			var (maxRating, best) = (0, -1);
			for (var i = 0; i < 81; i++)
			{
				if (!_seedPattern.Contains(i) || isOrdered.Contains(i))
				{
					continue;
				}

				var rating = 0;
				for (var j = 0; j < 81; j++)
				{
					if (!_seedPattern.Contains(j) || i == j)
					{
						continue;
					}

					if (i.ToHouseIndex(HouseType.Block) == j.ToHouseIndex(HouseType.Block)
						|| i.ToHouseIndex(HouseType.Row) == j.ToHouseIndex(HouseType.Row)
						|| i.ToHouseIndex(HouseType.Column) == j.ToHouseIndex(HouseType.Column))
					{
						rating += isOrdered.Contains(j) ? 10000 : 100;
					}

					if (!isOrdered.Contains(j)
						&& (i.ToBandIndex() == j.ToBandIndex() || i.ToTowerIndex() == j.ToTowerIndex())
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
}
