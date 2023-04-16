namespace Sudoku.Algorithm.Generating;

/// <summary>
/// Represents an instance that has ability to create a new sudoku puzzle game, allow cancelling.
/// </summary>
public interface IPuzzleGenerator
{
	/// <summary>
	/// Creates a sudoku grid puzzle.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
	/// <returns>
	/// The result puzzle generated. If canceled, the return value will be <see cref="Grid.Undefined"/>.
	/// </returns>
	Grid Generate(CancellationToken cancellationToken = default);
}
