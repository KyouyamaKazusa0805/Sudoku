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
	/// Indicates the techniques supported in the current generator.
	/// </summary>
	public abstract TechniqueSet SupportedTechniques { get; }


	/// <summary>
	/// Generates a puzzle and return a <see cref="Grid"/> instance;
	/// using <paramref name="cancellationToken"/> to cancel the operation.
	/// </summary>
	/// <param name="result">The result <see cref="Grid"/> instance generated.</param>
	/// <param name="cancellationToken">The cancellation token instance that can cancel the current operation.</param>
	/// <returns>
	/// A <see cref="bool"/> value indicating whether the result has already been generated without any error.
	/// For example, a user has cancelled the task, the return value should be <see langword="false"/>.
	/// </returns>
	public abstract bool TryGenerateUnique(out Grid result, CancellationToken cancellationToken = default);
}
