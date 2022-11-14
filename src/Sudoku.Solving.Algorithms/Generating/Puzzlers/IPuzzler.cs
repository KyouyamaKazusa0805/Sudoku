namespace Sudoku.Generating.Puzzlers;

/// <summary>
/// Defines a type that can create a new sudoku puzzle game.
/// </summary>
public interface IPuzzler
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
