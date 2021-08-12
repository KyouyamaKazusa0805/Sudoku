using static Sudoku.Generating.IPuzzleGenerator;
using Pointer = System.Pointer;

namespace Sudoku.Generating
{
	/// <summary>
	/// Provides an extended puzzle generator.
	/// </summary>
	public class HardPatternPuzzleGenerator : DiggingPuzzleGenerator
	{
		/// <summary>
		/// The block factor.
		/// </summary>
		private static readonly int[] BlockFactor = { 0, 6, 54, 60, 3, 27, 33, 57, 30 };

		/// <summary>
		/// Indicates the swapping factor.
		/// </summary>
		private static readonly int[,] SwappingFactor =
		{
			{ 0, 1, 2 }, { 0, 2, 1 }, { 1, 0, 2 },
			{ 1, 2, 0 }, { 2, 0, 1 }, { 2, 1, 0 }
		};

		/// <summary>
		/// The backdoor searcher.
		/// </summary>
		private static readonly BackdoorSearcher BackdoorSearcher = new();


		/// <inheritdoc/>
		public override SudokuGrid? Generate() => Generate(-1, null);

		/// <summary>
		/// To generate a sudoku grid with a backdoor filter depth.
		/// </summary>
		/// <param name="backdoorFilterDepth">
		/// The backdoor filter depth. When the value is -1, the generator won't check
		/// any backdoors.
		/// </param>
		/// <param name="progress">The progress.</param>
		/// <param name="difficultyLevel">The difficulty level.</param>
		/// <param name="countryCode">The country code.</param>
		/// <param name="cancellationToken">The cancellation token used for cancelling an operation.</param>
		/// <returns>The grid.</returns>
		/// <exception cref="OperationCanceledException">Throws when the operation is cancelled.</exception>
		public SudokuGrid? Generate(
			int backdoorFilterDepth, IProgress<IProgressResult>? progress,
			DifficultyLevel difficultyLevel = DifficultyLevel.Unknown,
			CountryCode countryCode = CountryCode.Default, CancellationToken? cancellationToken = null)
		{
			PuzzleGeneratingProgressResult
				defaultValue = default,
				pr = new(0, countryCode == CountryCode.Default ? CountryCode.EnUs : countryCode);

			ref var progressResult = ref progress is null ? ref defaultValue : ref pr;
			progress?.Report(defaultValue);

			StringBuilder
				puzzle = new() { Length = 81 },
				solution = new() { Length = 81 },
				emptyGridStr = new(SudokuGrid.EmptyString);

			var holeCells = (stackalloc int[81]);

			while (true)
			{
				emptyGridStr.CopyTo(puzzle);
				emptyGridStr.CopyTo(solution);
				GenerateAnswerGrid(puzzle, solution);

				holeCells.Fill(0);
				CreatePattern(ref holeCells);
				for (int trial = 0; trial < 1000; trial++)
				{
					if (cancellationToken is { IsCancellationRequested: true })
					{
						throw new OperationCanceledException();
					}

					for (int cell = 0; cell < 81; cell++)
					{
						int p = holeCells[cell];
						char temp = solution[p];
						solution[p] = '0';

						if (!FastSolver.CheckValidity(solution.ToString()))
						{
							// Reset the value.
							solution[p] = temp;
						}
					}

					if (progress is not null)
					{
						progressResult.GeneratingTrial++;
						progress.Report(progressResult);
					}

					if (
						FastSolver.CheckValidity(solution.ToString())
						&& SudokuGrid.Parse(solution.ToString()) is var grid
						&& (
							backdoorFilterDepth != -1
							&& !BackdoorSearcher.SearchForBackdoors(grid, backdoorFilterDepth).Any()
							|| backdoorFilterDepth == -1
						) && (
							difficultyLevel != DifficultyLevel.Unknown
							&& grid.GetDifficultyLevel() == difficultyLevel
							|| difficultyLevel == DifficultyLevel.Unknown
						)
					)
					{
						return grid;
					}

					RecreatePattern(ref holeCells);
				}
			}
		}

		/// <summary>
		/// To generate a sudoku grid with a backdoor filter depth asynchronously.
		/// </summary>
		/// <param name="backdoorFilterDepth">
		/// The backdoor filter depth. When the value is -1, the generator won't check
		/// any backdoors.
		/// </param>
		/// <param name="progress">The progress.</param>
		/// <param name="difficultyLevel">The difficulty level.</param>
		/// <param name="countryCode">The country code.</param>
		/// <param name="cancellationToken">The cancellation token used for cancelling an operation.</param>
		/// <returns>The task.</returns>
		public async Task<SudokuGrid?> GenerateAsync(
			int backdoorFilterDepth, IProgress<IProgressResult>? progress,
			DifficultyLevel difficultyLevel = DifficultyLevel.Unknown,
			CountryCode countryCode = CountryCode.Default, CancellationToken? cancellationToken = null)
		{
			return await (cancellationToken is { } t ? Task.Run(innerGenerate, t) : Task.Run(innerGenerate));

			SudokuGrid? innerGenerate()
			{
				try
				{
					return Generate(backdoorFilterDepth, progress, difficultyLevel, countryCode, cancellationToken);
				}
				catch (OperationCanceledException)
				{
					return null;
				}
			}
		}

		/// <inheritdoc/>
		protected sealed override unsafe void CreatePattern(ref Span<int> pattern)
		{
			int a = 54, b = 0;
			for (int i = 0; i < 9; i++)
			{
				int n = (int)(Rng.NextDouble() * 6);
				for (int j = 0; j < 3; j++)
				{
					for (int k = 0; k < 3; k++)
					{
						pattern[(*(k == SwappingFactor[n, j] ? &a : &b))++] = BlockFactor[i] + j * 9 + k;
					}
				}
			}

			fixed (int* pPattern = pattern)
			{
				for (int i = 23; i >= 0; i--)
				{
					Pointer.Swap(pPattern + i, pPattern + (int)((i + 1) * Rng.NextDouble()));
				}
				for (int i = 47; i >= 24; i--)
				{
					Pointer.Swap(pPattern + i, pPattern + 24 + (int)((i - 23) * Rng.NextDouble()));
				}
				for (int i = 53; i >= 48; i--)
				{
					Pointer.Swap(pPattern + i, pPattern + 48 + (int)((i - 47) * Rng.NextDouble()));
				}
				for (int i = 80; i >= 54; i--)
				{
					Pointer.Swap(pPattern + i, pPattern + 54 + (int)(27 * Rng.NextDouble()));
				}
			}
		}


		/// <summary>
		/// To re-create the pattern.
		/// </summary>
		/// <param name="pattern">The pattern array.</param>
		private static unsafe void RecreatePattern(ref Span<int> pattern)
		{
			fixed (int* pPattern = pattern)
			{
				for (int i = 23; i >= 0; i--)
				{
					Pointer.Swap(pPattern + i, pPattern + (int)((i + 1) * Rng.NextDouble()));
				}
				for (int i = 47; i >= 24; i--)
				{
					Pointer.Swap(pPattern + i, pPattern + 24 + (int)((i - 23) * Rng.NextDouble()));
				}
				for (int i = 53; i >= 48; i--)
				{
					Pointer.Swap(pPattern + i, pPattern + 48 + (int)((i - 47) * Rng.NextDouble()));
				}
				for (int i = 80; i >= 54; i--)
				{
					Pointer.Swap(pPattern + i, pPattern + 54 + (int)(27 * Rng.NextDouble()));
				}
			}
		}
	}
}
