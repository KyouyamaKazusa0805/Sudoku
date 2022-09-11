namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Defines an algorithm that is used for solving a sudoku puzzle.
/// </summary>
public enum SolveAlgorithm
{
	/// <summary>
	/// Indicates the solver is manually solver.
	/// </summary>
	[SupportedArguments(new[] { "logical", "l" })]
	[RouteToType(typeof(LogicalSolver))]
	[Name("Logical")]
	Logical,

	/// <summary>
	/// Indicates the solver is bitwise solver.
	/// </summary>
	[SupportedArguments(new[] { "bitwise", "b" })]
	[RouteToType(typeof(BitwiseSolver))]
	[Name("Bitwise")]
	Bitwise,

	/// <summary>
	/// Indicates the solver is backtracking solver.
	/// </summary>
	[SupportedArguments(new[] { "backtracking", "t" })]
	[RouteToType(typeof(BacktrackingSolver))]
	[Name("Backtracking")]
	Backtracking,

	/// <summary>
	/// Indicates the solver is dancing links solver.
	/// </summary>
	[SupportedArguments(new[] { "dancing-links", "dlx", "d" })]
	[RouteToType(typeof(DancingLinksSolver))]
	[Name("Dancing Links")]
	DancingLinks,

	/// <summary>
	/// Indicates the solver is the LINQ solver based on query.
	/// </summary>
	[SupportedArguments(new[] { "linq-query", "linq", "l" })]
	[RouteToType(typeof(LinqSolver))]
	[Name("LINQ (Based on Query)")]
	LinqBasedOnQuery,

	/// <summary>
	/// Indicates the solver is the LINQ solver based on type <see cref="Dictionary{TKey, TValue}"/>.
	/// </summary>
	[SupportedArguments(new[] { "linq-dictionary", "linq-dic" })]
	[RouteToType(typeof(LinqSolver2))]
	[Name("LINQ (Based on Dictionary<string, string>)")]
	LinqBasedOnDictionary
}
