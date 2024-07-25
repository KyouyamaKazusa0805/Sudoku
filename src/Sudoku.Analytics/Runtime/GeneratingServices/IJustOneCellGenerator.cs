namespace Sudoku.Runtime.GeneratingServices;

/// <summary>
/// Represents a type that can generate puzzles that can only be solved for one cell.
/// </summary>
public interface IJustOneCellGenerator
{
	/// <summary>
	/// Generates a puzzle and return a <see cref="Grid"/> instance that satisfies rules of Just-One-Cell puzzles;
	/// using <paramref name="cancellationToken"/> to cancel the operation.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token instance that can cancel the current operation.</param>
	/// <returns>The result <see cref="Grid"/> instance generated.</returns>
	public abstract Grid GenerateJustOneCell(CancellationToken cancellationToken = default);
}
