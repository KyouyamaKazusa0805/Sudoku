namespace Sudoku.Runtime.GeneratingServices;

/// <summary>
/// Represents a generator type that can generate puzzles with the specified technique used.
/// </summary>
public abstract class TechniqueGenerator
{
	/// <summary>
	/// Indicates the random number generator.
	/// </summary>
	protected static readonly Random Rng = Random.Shared;


	/// <summary>
	/// Generates a puzzle and return a <see cref="Grid"/> instance;
	/// using <paramref name="cancellationToken"/> to cancel the operation.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token instance that can cancel the current operation.</param>
	/// <returns>The result <see cref="Grid"/> instance generated.</returns>
	public abstract Grid GenerateUnique(CancellationToken cancellationToken = default);
}
