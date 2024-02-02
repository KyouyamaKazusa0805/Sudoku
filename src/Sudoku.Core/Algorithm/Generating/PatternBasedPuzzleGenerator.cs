namespace Sudoku.Algorithm.Generating;

/// <summary>
/// Represents a generator that is based on pattern.
/// </summary>
/// <param name="seedPattern"><inheritdoc cref="Pattern" path="/summary"/></param>
[StructLayout(LayoutKind.Auto)]
[LargeStructure]
[Equals]
[GetHashCode]
[ToString]
[method: DebuggerStepThrough]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public ref partial struct PatternBasedPuzzleGenerator([PrimaryCosntructorParameter(MemberKinds.Field, RefKind = "ref readonly")] ref readonly CellMap seedPattern)
{
	/// <summary>
	/// The internal solver.
	/// </summary>
	private readonly BitwiseSolver _solver = new();

	/// <summary>
	/// Indicates the test grid.
	/// </summary>
	private Grid _playground;

	/// <summary>
	/// Indicates the result grid.
	/// </summary>
	private Grid _resultGrid;


	/// <summary>
	/// Indicates the predefind pattern used.
	/// </summary>
	public readonly ref readonly CellMap Pattern => ref _seedPattern;


	/// <summary>
	/// Try to generate a puzzle using the specified pattern.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token that can cancel the operation.</param>
	/// <returns>A valid <see cref="Grid"/> pattern that has a specified pattern, with specified digits should be filled in.</returns>
	[UnscopedRef]
	public ref readonly Grid Generate(CancellationToken cancellationToken = default)
	{
		try
		{
			var patternCellsSorted = OrderPatternCellsViaConnectionDegrees();
			_playground = Grid.Empty;
			getGrid(_solver, patternCellsSorted, ref _playground, ref _resultGrid, 0);
			return ref _resultGrid;
		}
		catch (OperationCanceledException)
		{
			return ref Grid.Undefined;
		}
		catch
		{
			throw;
		}


		void getGrid(
			BitwiseSolver solver,
			Cell[] patternCellsSorted,
			scoped ref Grid playground,
			scoped ref Grid resultGrid,
			int currentIndex
		)
		{
			if (currentIndex == patternCellsSorted.Length)
			{
				if (solver.CheckValidity(playground.ToString("!0")))
				{
					resultGrid = playground;
					throw new("Finished.");
				}

				return;
			}

			if (playground.GetCandidates(patternCellsSorted[currentIndex]) == 0)
			{
				// Invalid state.
				return;
			}

			var cell = patternCellsSorted[currentIndex];
			scoped var digits = playground.GetCandidates(cell).GetAllSets();
			var indexArray = Digits[..digits.Length];
			Random.Shared.Shuffle(indexArray);
			foreach (var randomizedIndex in indexArray)
			{
				playground.SetDigit(cell, -1);
				playground.SetDigit(cell, digits[randomizedIndex]);

				cancellationToken.ThrowIfCancellationRequested();

				getGrid(solver, patternCellsSorted, ref playground, ref resultGrid, currentIndex + 1);
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
		var isOrdered = CellMap.Empty;
		var result = new Cell[_seedPattern.Count];
		for (var index = 0; index < _seedPattern.Count; index++)
		{
			var maxRating = 0;
			var best = -1;
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
