namespace Sudoku.Solving;

/// <summary>
/// Provides extra data for event <see cref="ISolutionEnumerableSolver{TSelf}.SolutionFound"/>.
/// </summary>
/// <param name="solution">Indicates the target solution.</param>
/// <seealso cref="ISolutionEnumerableSolver{TSelf}.SolutionFound"/>
public sealed partial class SolverSolutionFoundEventArgs([Property] Grid solution) : EventArgs;
