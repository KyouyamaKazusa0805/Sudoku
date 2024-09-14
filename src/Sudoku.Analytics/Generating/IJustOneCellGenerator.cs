namespace Sudoku.Generating;

/// <summary>
/// Represents a type that can generate puzzles that can only be solved for one cell.
/// </summary>
public interface IJustOneCellGenerator
{
	/// <summary>
	/// Generates a puzzle and return a <see cref="Grid"/> instance that satisfies rules of Just-One-Cell puzzles;
	/// using <paramref name="cancellationToken"/> to cancel the operation.
	/// </summary>
	/// <param name="result">The result <see cref="Grid"/> instance generated.</param>
	/// <param name="cancellationToken">The cancellation token instance that can cancel the current operation.</param>
	/// <returns>
	/// A <see cref="bool"/> value indicating whether the result has already been generated without any error.
	/// For example, a user has cancelled the task, the return value should be <see langword="false"/>.
	/// </returns>
	public abstract bool TryGenerateJustOneCell(out Grid result, CancellationToken cancellationToken = default);

	/// <summary>
	/// Generates a puzzle and return a <see cref="Grid"/> instance that satisfies rules of Just-One-Cell puzzles;
	/// using <paramref name="cancellationToken"/> to cancel the operation.
	/// </summary>
	/// <param name="result">The result <see cref="Grid"/> instance generated.</param>
	/// <param name="step">Indicates the step that records the current technique usage.</param>
	/// <param name="cancellationToken">The cancellation token instance that can cancel the current operation.</param>
	/// <returns>
	/// A <see cref="bool"/> value indicating whether the result has already been generated without any error.
	/// For example, a user has cancelled the task, the return value should be <see langword="false"/>.
	/// </returns>
	public abstract bool TryGenerateJustOneCell(out Grid result, [NotNullWhen(true)] out Step? step, CancellationToken cancellationToken = default);

	/// <summary>
	/// Generates a puzzle and return a <see cref="Grid"/> instance that satisfies rules of Just-One-Cell puzzles,
	/// with <paramref name="phasedGrid"/> to record the base grid to be used;
	/// using <paramref name="cancellationToken"/> to cancel the operation.
	/// </summary>
	/// <param name="result">The result <see cref="Grid"/> instance generated.</param>
	/// <param name="phasedGrid">Indicates the base grid that produces the current result.</param>
	/// <param name="step">Indicates the step that records the current technique usage.</param>
	/// <param name="cancellationToken">The cancellation token instance that can cancel the current operation.</param>
	/// <returns>
	/// A <see cref="bool"/> value indicating whether the result has already been generated without any error.
	/// For example, a user has cancelled the task, the return value should be <see langword="false"/>.
	/// </returns>
	public abstract bool TryGenerateJustOneCell(out Grid result, out Grid phasedGrid, [NotNullWhen(true)] out Step? step, CancellationToken cancellationToken = default);
}
