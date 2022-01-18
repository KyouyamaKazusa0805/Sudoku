namespace Sudoku.Solving.BruteForces;

/// <summary>
/// Defines a fast solver.
/// </summary>
public sealed unsafe class FastSolver : IPuzzleSolver
{
	/// <summary>
	/// The inner solver.
	/// </summary>
	private static readonly BitwiseSolver BitwiseSolver = new();


	/// <inheritdoc/>
	public ISolverResult Solve(in Grid puzzle, CancellationToken cancellationToken = default)
	{
		var stopwatch = new Stopwatch();
		char* solutionStr = stackalloc char[BitwiseSolver.BufferLength];

		stopwatch.Start();
		fixed (char* pPuzzleStr = puzzle.ToString("0"))
		{
			BitwiseSolver.Solve(pPuzzleStr, solutionStr, 2);
		}
		stopwatch.Stop();

		var solverResult = new BruteForceSolverResult(puzzle, ElapsedTime: stopwatch.Elapsed);
		return BitwiseSolver.LimitSolutions switch
		{
			0 => solverResult with { IsSolved = false, FailedReason = FailedReason.PuzzleHasNoSolution },
			1 => solverResult with { Solution = Grid.Parse(new ReadOnlySpan<char>(solutionStr, BitwiseSolver.BufferLength)) },
			_ => solverResult with { IsSolved = false, FailedReason = FailedReason.PuzzleHasMultipleSolutions }
		};
	}

	/// <inheritdoc/>
	public ValueTask<ISolverResult> SolveAsync(in Grid puzzle, CancellationToken cancellationToken = default) =>
		new(Solve(puzzle, cancellationToken));

	/// <inheritdoc cref="BitwiseSolver.Solve(char*, char*, int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal long Solve(char* puzzle, char* solution, int limit) =>
		BitwiseSolver.Solve(puzzle, solution, limit);

	/// <inheritdoc cref="BitwiseSolver.CheckValidity(char*)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool CheckValidity(char* grid) => BitwiseSolver.CheckValidity(grid);

	/// <inheritdoc cref="BitwiseSolver.CheckValidity(string)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool CheckValidity(string grid) => BitwiseSolver.CheckValidity(grid);
}
