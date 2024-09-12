namespace Sudoku.Analytics.Primitives;

/// <summary>
/// Represents an instance that describes the result after executed the method
/// <see cref="IAnalyzer{TSolver, TContext, TSolverResult}.Analyze(ref readonly TContext)"/>.
/// </summary>
/// <typeparam name="TSelf">
/// <include file="../../global-doc-comments.xml" path="/g/self-type-constraint"/>
/// </typeparam>
/// <typeparam name="TAnalyzer">The solver's type.</typeparam>
/// <typeparam name="TContext">The type of the context.</typeparam>
/// <seealso cref="IAnalyzer{TSolver, TContext, TSolverResult}.Analyze(ref readonly TContext)"/>
public interface IAnalysisResult<out TSelf, in TAnalyzer, TContext>
	where TSelf : IAnalysisResult<TSelf, TAnalyzer, TContext>, allows ref struct
	where TAnalyzer : IAnalyzer<TAnalyzer, TContext, TSelf>, allows ref struct
	where TContext : allows ref struct
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
