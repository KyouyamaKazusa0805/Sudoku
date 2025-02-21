namespace Sudoku.Solving;

/// <summary>
/// Provides extra data for event <see cref="ISolutionEnumerableSolver.SolutionFound"/>.
/// </summary>
/// <param name="solution">Indicates the target solution.</param>
/// <seealso cref="ISolutionEnumerableSolver.SolutionFound"/>
public sealed partial class SolverSolutionFoundEventArgs([Property] Grid solution) : EventArgs;
