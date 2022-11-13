namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Defines an algorithm that is used for solving a sudoku puzzle.
/// </summary>
public enum SolveAlgorithm
{
	/// <summary>
	/// Indicates the solver is manually solver.
	/// </summary>
	[SupportedArguments("logical", "l")]
	[RouteToType(typeof(LogicalSolver))]
	[Name(nameof(Logical))]
	Logical,

	/// <summary>
	/// Indicates the solver is bitwise solver.
	/// </summary>
	[SupportedArguments("bitwise", "b")]
	[RouteToType(typeof(BitwiseSolver))]
	[Name(nameof(Bitwise))]
	Bitwise,

	/// <summary>
	/// Indicates the solver is backtracking solver.
	/// </summary>
	[SupportedArguments("backtracking", "t")]
	[RouteToType(typeof(BacktrackingSolver))]
	[Name(nameof(Backtracking))]
	Backtracking,

	/// <summary>
	/// Indicates the solver is dancing links solver.
	/// </summary>
	[SupportedArguments("dancing-links", "dlx", "d")]
	[RouteToType(typeof(DancingLinksSolver))]
	[Name("Dancing Links")]
	DancingLinks,

	/// <summary>
	/// Indicates the solver is the LINQ solver based on query.
	/// </summary>
	[SupportedArguments("linq-query", "linq", "l")]
	[RouteToType(typeof(LinqSolver))]
	[Name("LINQ (Based on Query)")]
	LinqBasedOnQuery,

	/// <summary>
	/// Indicates the solver is the LINQ solver based on type <see cref="Dictionary{TKey, TValue}"/>.
	/// </summary>
	[SupportedArguments("linq-dictionary", "linq-dic")]
	[RouteToType(typeof(LinqSolver2))]
	[Name("LINQ (Based on Dictionary<string, string>)")]
	LinqBasedOnDictionary
}
