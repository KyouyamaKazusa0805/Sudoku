namespace Sudoku.Analytics;

/// <summary>
/// Represents an instance that describes the result after executed the method
/// <see cref="IAnalyzer{TSolver, TSolverResult}.Analyze(ref readonly Grid, IProgress{AnalysisProgress}, CancellationToken)"/>.
/// </summary>
/// <typeparam name="TSolver">The solver's type.</typeparam>
/// <typeparam name="TSelf">The type of the target result itself.</typeparam>
/// <seealso cref="IAnalyzer{TSolver, TSolverResult}.Analyze(ref readonly Grid, IProgress{AnalysisProgress}, CancellationToken)"/>
public interface IAnalysisResult<in TSolver, out TSelf>
	where TSolver :
		IAnalyzer<TSolver, TSelf>
#if NET9_0_OR_GREATER
		,
		allows ref struct
#endif
	where TSelf :
		IAnalysisResult<TSolver, TSelf>
#if NET9_0_OR_GREATER
		,
		allows ref struct
#endif
{
	/// <summary>
	/// Indicates whether the solver has solved the puzzle.
	/// </summary>
	public abstract bool IsSolved { get; init; }

	/// <summary>
	/// Indicates the elapsed time used during solving the puzzle. The value may not be an useful value.
	/// Some case if the puzzle doesn't contain a valid unique solution, the value may be
	/// <see cref="TimeSpan.Zero"/>.
	/// </summary>
	/// <seealso cref="TimeSpan.Zero"/>
	public abstract TimeSpan ElapsedTime { get; init; }

	/// <summary>
	/// Indicates the original puzzle to be solved.
	/// </summary>
	public abstract Grid Puzzle { get; init; }

	/// <summary>
	/// Indicates the result sudoku grid solved. If the solver can't solve this puzzle, the value will be
	/// <see cref="Grid.Undefined"/>.
	/// </summary>
	/// <seealso cref="Grid.Undefined"/>
	public abstract Grid Solution { get; init; }

	/// <summary>
	/// Indicates the unhandled exception thrown.
	/// </summary>
	public abstract Exception? UnhandledException { get; init; }
}
