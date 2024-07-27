namespace Sudoku.Runtime.GeneratingServices;

/// <summary>
/// Represents a generator type that creates a puzzle that can only uses the current technique to solve.
/// </summary>
public interface IPrimaryGenerator
{
	/// <summary>
	/// Generates a puzzle and return a <see cref="Grid"/> instance that can be solved by only using the specified technique;
	/// using <paramref name="cancellationToken"/> to cancel the operation.
	/// </summary>
	/// <param name="result">The result <see cref="Grid"/> instance generated.</param>
	/// <param name="cancellationToken">The cancellation token instance that can cancel the current operation.</param>
	/// <returns>
	/// A <see cref="bool"/> value indicating whether the result has already been generated without any error.
	/// For example, a user has cancelled the task, the return value should be <see langword="false"/>.
	/// </returns>
	public abstract bool GeneratePrimary(out Grid result, CancellationToken cancellationToken = default);
}
