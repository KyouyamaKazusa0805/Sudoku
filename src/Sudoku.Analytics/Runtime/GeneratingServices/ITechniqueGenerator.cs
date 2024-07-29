namespace Sudoku.Runtime.GeneratingServices;

/// <summary>
/// Represents a generator that can generate a puzzle with a unique solution, and using the specified technique.
/// </summary>
public interface ITechniqueGenerator
{
	/// <summary>
	/// Indicates the techniques supported in the current generator.
	/// </summary>
	public abstract TechniqueSet SupportedTechniques { get; }

	/// <summary>
	/// Indicates the internal random number generator to be used.
	/// </summary>
	protected static abstract ref readonly Random RandomNumberGenerator { get; }


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
