namespace Sudoku.Generating.Puzzlers;

/// <summary>
/// Indicates a puzzle generator that uses the pattern-based sudoku generation algorithm.
/// </summary>
/// <remarks>
/// The main idea can be shortened into the several following steps:
/// <list type="number">
/// <item>Randomize to generate a pattern which contains about 24 cells.</item>
/// <item>
/// Randomize to get some numbers (Just about 3 digits is okay).
/// These digits should guarantee the validity of the pattern.
/// </item>
/// <item>Try to solve this as the multiple solution puzzle. Our aim in this step is to get a valid solution.</item>
/// <item>Remove digits not filled in the pattern.</item>
/// <item>
/// Check validity of the removed puzzle.
/// <br/>
/// If the puzzle is unique, generated successfully, and return this puzzle;
/// <br/>
/// otherwise, adjust pattern slightly and try again. If failed after having tried 10000 times, we will re-generate
/// a sudoku puzzle.
/// </item>
/// </list>
/// </remarks>
public sealed class PatternBasedPuzzleGenerator : IPuzzler
{
	/// <summary>
	/// Indicates the times that can retry a new pattern without updating sudoku solution template.
	/// </summary>
	private const int RetrialTimes = 1000;

	/// <summary>
	/// Indicates the number of pattern cells used that can be used in the generation for a pattern.
	/// </summary>
	private const int MinPatternCellsCount = 20, MaxPatternCellsCount = 28;


	/// <summary>
	/// Indicates the empty grid characters.
	/// </summary>
	private static readonly char[] EmptyGridCharArray = Grid.EmptyString.ToCharArray();

	/// <summary>
	/// Indicates the swapper house starts.
	/// </summary>
	private static readonly int[] SwapperHouseStarts = { 9, 12, 15, 18, 21, 24 };

	/// <summary>
	/// Indicates the swappable factor.
	/// </summary>
	private static readonly (int, int)[] SwappableFactor = { (0, 1), (0, 2), (1, 2) };

	/// <inheritdoc cref="HardLikePuzzleGenerator.Solver"/>
	private static readonly BitwiseSolver Solver = new();

	/// <summary>
	/// Indicates the solver with solution grid can be used.
	/// </summary>
	private static readonly BacktrackingSolver SolverWithSolution = new();


	/// <summary>
	/// Indicates the base candidates.
	/// </summary>
	private readonly int[]? _baseCandidates;

	/// <summary>
	/// Indicates the pattern.
	/// </summary>
	private readonly CellMap? _pattern;

	/// <inheritdoc cref="HardLikePuzzleGenerator._random"/>
	private readonly Random _random = new();


	/// <summary>
	/// Initializes a <see cref="PatternBasedPuzzleGenerator"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PatternBasedPuzzleGenerator()
	{
	}

	/// <summary>
	/// Initializes a <see cref="PatternBasedPuzzleGenerator"/> instance via the specified pattern.
	/// </summary>
	/// <param name="pattern">The pattern.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PatternBasedPuzzleGenerator(scoped in CellMap pattern) => _pattern = pattern;

	/// <summary>
	/// Initializes a <see cref="PatternBasedPuzzleGenerator"/> instance via the specified pattern
	/// and the initial candidates as the pattern.
	/// </summary>
	/// <param name="pattern">The pattern.</param>
	/// <param name="baseCandidates">The base candidates.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PatternBasedPuzzleGenerator(scoped in CellMap pattern, int[] baseCandidates)
		=> (_pattern, _baseCandidates) = (pattern, baseCandidates);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Grid Generate(CancellationToken cancellationToken = default) => Generate(int.MaxValue, cancellationToken);

	/// <summary>
	/// Creates a sudoku puzzle via the specified trial times.
	/// </summary>
	/// <param name="times">The trial times.</param>
	/// <param name="cancellationToken">
	/// <inheritdoc cref="Generate(CancellationToken)" path="/param[@name='cancellationToken']"/>
	/// </param>
	/// <returns>
	/// If user has canceled the operation or the maximum trial times has been reached, <see cref="Grid.Undefined"/>;
	/// otherwise, the valid grid.
	/// </returns>
	/// <exception cref="InvalidOperationException">Throws when the field <c>_baseCandidates</c> is invalid.</exception>
	public unsafe Grid Generate(int times, CancellationToken cancellationToken = default)
	{
		for (var trialTimeIndex = 0; trialTimeIndex < times; trialTimeIndex++)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				// Generate a pattern.
				var pattern = _pattern is { } p ? p : GeneratePattern();

				var emptyChars = new char[BitwiseSolver.BufferLength];
				var solutionBuffer = new char[BitwiseSolver.BufferLength];
				emptyChars[81] = '\0';
				solutionBuffer[81] = '\0';

				// Randomize a multiple-solution grid.
				fixed (char* ptr = emptyChars, clonedPtr = solutionBuffer, fixedEmptyGridCharsPtr = EmptyGridCharArray)
				{
					CopyBlock(ptr, fixedEmptyGridCharsPtr, sizeof(char) * 81);

					if (_baseCandidates is null)
					{
						var (c1, c2, c3) = RandomizeThreeDigits();
						ptr[c1 / 9] = (char)(c1 % 9 + '1');
						ptr[c2 / 9] = (char)(c2 % 9 + '1');
						ptr[c3 / 9] = (char)(c3 % 9 + '1');
					}
					else
					{
						foreach (var baseCandidate in _baseCandidates)
						{
							ptr[baseCandidate / 9] = (char)(baseCandidate % 9 + '1');
						}
					}

					if (SolverWithSolution.Solve(Grid.Parse(new string(ptr)), out var solution) is null)
					{
						if (_baseCandidates is not null)
						{
							throw new InvalidOperationException($"The field {nameof(_baseCandidates)} is invalid to be set as the initial case.");
						}

						continue;
					}

					// Shuffle the grid.
					Shuffle(ref solution);

					// Try to apply the pattern and check the validity of the uniqueness of the target puzzle.
					for (var retrialTimeIndex = 0; retrialTimeIndex < RetrialTimes; retrialTimeIndex++)
					{
						fixed (char* solutionPtr = solution.ToString("!"))
						{
							CopyBlock(clonedPtr, solutionPtr, sizeof(char) * 81);
						}

						// Remove digits not being filled in the pattern.
						for (var index = 0; index < 81; index++)
						{
							if (!pattern.Contains(index))
							{
								clonedPtr[index] = '0';
							}
						}

						// Checks the validity.
						if (Solver.Solve(clonedPtr, null, 2) == 1)
						{
							// Unique puzzle. Return the value.
							return Grid.Parse(new string(clonedPtr));
						}

						// If the puzzle is invalid, we can adjust the pattern and try again.
						AdjustPattern(ref pattern);

						// Check whether user has canceled the operation.
						if (cancellationToken.IsCancellationRequested)
						{
							goto ReturnDefault;
						}
					}
				}
			}
		}

	ReturnDefault:
		return Grid.Undefined;
	}

	/// <summary>
	/// Creates a sudoku puzzle asynchoronously.
	/// </summary>
	/// <param name="cancellationToken">
	/// <inheritdoc cref="Generate(CancellationToken)" path="/param[@name='cancellationToken']"/>
	/// </param>
	/// <returns>A <see cref="Task{TResult}"/> instance that returns a <see cref="Grid"/> value.</returns>
	public async Task<Grid> GenerateAsync(CancellationToken cancellationToken = default)
		=> await Task.Run(() => Generate(cancellationToken), cancellationToken);

	/// <summary>
	/// To shuffle the grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	private void Shuffle(scoped ref Grid grid)
	{
		for (var times = 0; times < 6; times++)
		{
			var ((a, b), baseHouse) = (SwappableFactor[_random.Next(3)], SwapperHouseStarts[times]);
			grid.SwapTwoHouses(baseHouse + a, baseHouse + b);
		}
	}

	/// <summary>
	/// Adjust a pattern slightly.
	/// </summary>
	/// <param name="pattern">The pattern.</param>
	private void AdjustPattern(scoped ref CellMap pattern)
	{
		var index = _random.Next(pattern.Count);
		var cellToBeDeleted = pattern[index];
		int[] tempCells = GetCells(cellToBeDeleted / 9, cellToBeDeleted % 9);
		pattern -= (CellMap)tempCells;

		int newCell;
		while (true)
		{
			newCell = _random.Next(81);
			if (!pattern.Contains(newCell) && newCell != 40)
			{
				break;
			}
		}

		pattern |= GetCells(newCell / 9, newCell % 9);
	}

	/// <summary>
	/// Randomize to generate a pattern.
	/// </summary>
	/// <returns>A pattern result.</returns>
	private CellMap GeneratePattern()
	{
		var result = CellMap.Empty;
		var resultCellsCount = _random.Next(MinPatternCellsCount, MaxPatternCellsCount);
		if ((resultCellsCount & 1) != 0)
		{
			result.Add(40);
		}

		while (true)
		{
			var cell = _random.Next(81);
			result |= GetCells(cell / 9, cell % 9);

			if (result.Count - resultCellsCount is 1 or 0 or -1)
			{
				return result;
			}
		}
	}

	/// <summary>
	/// Randomize to generate three candidates at different places.
	/// </summary>
	/// <returns>A triplet of cells.</returns>
	private unsafe (int, int, int) RandomizeThreeDigits()
	{
		using scoped var list = new ValueList<int>(3);
		while (true)
		{
			var candidate = _random.Next(729);
			if (!list.Contains(candidate, &predicate))
			{
				list.Add(candidate);
				if (list is [var a, var b, var c])
				{
					return (a, b, c);
				}
			}
		}


		static bool predicate(int a, int b)
			=> a / 9 == b / 9 // Cannot in a same cell.
			|| PeersMap[a / 9] + b / 9 == PeersMap[a / 9] && a % 9 == b % 9; // Cannot be a same digit in a same house.
	}


	/// <summary>
	/// Get the cells that is used for swapping via <see cref="SymmetryType.Central"/>,
	/// and the specified row and column value.
	/// </summary>
	/// <param name="row">The row value.</param>
	/// <param name="column">The column value.</param>
	/// <returns>The cells.</returns>
	/// <seealso cref="SymmetryType.Central"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static CellMap GetCells(int row, int column)
		=> (CellMap)(
			row == 4 && column == 4
				? new[] { 40 }
				: new[] { row * 9 + column, (8 - row) * 9 + 8 - column }
		);
}
