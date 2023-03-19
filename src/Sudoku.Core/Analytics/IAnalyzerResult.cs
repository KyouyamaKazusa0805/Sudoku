namespace Sudoku.Analytics;

/// <summary>
/// Represents an instance that describes the result after executed the method
/// <see cref="IAnalyzer{TSolver, TSolverResult}.Analyze(in Grid, IProgress{double}, CancellationToken)"/>.
/// </summary>
/// <typeparam name="TSolver">The solver's type.</typeparam>
/// <typeparam name="TSolverResult">The type of the target result.</typeparam>
/// <seealso cref="IAnalyzer{TSolver, TSolverResult}.Analyze(in Grid, IProgress{double}, CancellationToken)"/>
public interface IAnalyzerResult<in TSolver, out TSolverResult>
	where TSolver : IAnalyzer<TSolver, TSolverResult>
	where TSolverResult : IAnalyzerResult<TSolver, TSolverResult>
{
	/// <summary>
	/// Indicates whether the solver has solved the puzzle.
	/// </summary>
	bool IsSolved { get; init; }

	/// <summary>
	/// Indicates the elapsed time used during solving the puzzle. The value may not be an useful value.
	/// Some case if the puzzle doesn't contain a valid unique solution, the value may be
	/// <see cref="TimeSpan.Zero"/>.
	/// </summary>
	/// <seealso cref="TimeSpan.Zero"/>
	TimeSpan ElapsedTime { get; init; }

	/// <summary>
	/// Indicates the original puzzle to be solved.
	/// </summary>
	Grid Puzzle { get; init; }

	/// <summary>
	/// Indicates the result sudoku grid solved. If the solver can't solve this puzzle, the value will be
	/// <see cref="Grid.Undefined"/>.
	/// </summary>
	/// <seealso cref="Grid.Undefined"/>
	Grid Solution { get; init; }

	/// <summary>
	/// Indicates the unhandled exception thrown.
	/// </summary>
	Exception? UnhandledException { get; init; }
}
