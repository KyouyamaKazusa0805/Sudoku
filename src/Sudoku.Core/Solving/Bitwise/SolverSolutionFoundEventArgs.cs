namespace Sudoku.Solving.Bitwise;

/// <summary>
/// Provides extra data for event <see cref="ISolutionEnumerableSolver{TSelf, TInput}.SolutionFound"/>.
/// </summary>
/// <param name="solution">Indicates the target solution.</param>
/// <seealso cref="ISolutionEnumerableSolver{TSelf, TInput}.SolutionFound"/>
public sealed partial class SolverSolutionFoundEventArgs<TInput>([Property] TInput solution) : EventArgs;
