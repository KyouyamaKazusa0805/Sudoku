namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Provides a solver that use logical methods to solve a specified sudoku puzzle.
	/// </summary>
	public sealed partial class ManualSolver : ISolver
	{
		/// <inheritdoc/>
		[JsonIgnore]
		public string SolverName => TextResources.Current.Manual;


		/// <inheritdoc/>
		public AnalysisResult Solve(in SudokuGrid grid) => Solve(grid, null);

		/// <summary>
		/// To solve the specified puzzle in asynchronous way.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="progress">The progress.</param>
		/// <param name="countryCode">The country code.</param>
		/// <param name="cancellationToken">
		/// The cancellation token that is used to cancel the operation. If the value is <see langword="null"/>,
		/// the method won't support cancellation.
		/// </param>
		/// <returns>The task of the execution.</returns>
		/// <remarks>
		/// <para>
		/// If the task is cancelled, the inner result value (i.e. the value of the property
		/// <see cref="Task{TResult}.Result"/>) should be <see langword="null"/>. In this way, we can raise
		/// an exception called <see cref="OperationCanceledException"/>.
		/// </para>
		/// <para>
		/// However, please <b>don't</b> raise this exception here, because here is in the asynchronous
		/// environment and we can't process and handle any exception here.
		/// </para>
		/// </remarks>
		public async Task<AnalysisResult?> SolveAsync(
			SudokuGrid grid, IProgress<IProgressResult>? progress, CountryCode countryCode = CountryCode.EnUs,
			CancellationToken? cancellationToken = null)
		{
			return await (cancellationToken is { } ct ? Task.Run(innerAnalysis, ct) : Task.Run(innerAnalysis));

			AnalysisResult? innerAnalysis()
			{
				try
				{
					return Solve(grid, progress, countryCode, cancellationToken);
				}
				catch (OperationCanceledException)
				{
					return null;
				}
			}
		}

		/// <summary>
		/// To solve the puzzle.
		/// </summary>
		/// <param name="grid">The puzzle.</param>
		/// <param name="progress">The progress instance to report the state.</param>
		/// <param name="countryCode">The country code.</param>
		/// <param name="cancellationToken">The cancellation token that is used to cancel the operation.</param>
		/// <returns>The analysis result.</returns>
		public unsafe AnalysisResult Solve(
			in SudokuGrid grid, IProgress<IProgressResult>? progress, CountryCode countryCode = CountryCode.EnUs,
			CancellationToken? cancellationToken = null)
		{
			if (grid.IsValid(out var solution, out bool? sukaku))
			{
				// Solve the puzzle.
				int emptyCellsCount = grid.EmptiesCount, candsCount = grid.CandidatesCount;
				try
				{
					GridProgressResult
						defaultValue = new(),
						defaultPr = new(candsCount, emptyCellsCount, candsCount, countryCode);
					ref var pr = ref progress is null ? ref defaultValue : ref defaultPr;
					progress?.Report(defaultPr);

					var tempList = new List<StepInfo>();
					var copied = grid;
					delegate*<
						ManualSolver, in SudokuGrid, ref SudokuGrid, IList<StepInfo>,
						in SudokuGrid, bool, ref GridProgressResult, IProgress<IProgressResult>?,
						CancellationToken?, AnalysisResult
					> f = AnalyzeDifficultyStrictly ? &SolveSeMode : &SolveNaively;

					return f(
						this, grid, ref copied, tempList, solution, sukaku.Value, ref pr,
						progress, cancellationToken
					);
				}
				catch (WrongStepException ex)
				{
					return new(SolverName, grid, false, TimeSpan.Zero) { Additional = ex };
				}
			}
			else
			{
				return new(SolverName, grid, false, TimeSpan.Zero)
				{
					Additional = "The puzzle doesn't have a unique solution (multiple solutions or no solution)."
				};
			}
		}
	}
}
