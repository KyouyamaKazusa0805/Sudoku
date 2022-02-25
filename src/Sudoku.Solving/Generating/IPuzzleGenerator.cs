using Sudoku.Collections;
using Sudoku.Solving.BruteForces;

namespace Sudoku.Generating;

/// <summary>
/// Defines a puzzle generator.
/// </summary>
public interface IPuzzleGenerator
{
	/// <summary>
	/// Provides a default fast solver to solve the puzzle.
	/// </summary>
	protected internal static readonly FastSolver Solver = new();


	/// <summary>
	/// Creates a sudoku grid puzzle.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
	/// <returns>
	/// The result puzzle generated. If cancelled, the return value will be <see cref="Grid.Undefined"/>.
	/// </returns>
	Grid Generate(CancellationToken cancellationToken = default);
}
