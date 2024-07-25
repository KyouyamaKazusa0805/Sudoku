namespace Sudoku.Runtime.GeneratingServices;

/// <summary>
/// Represents a generator that generates a puzzle.
/// </summary>
public interface ITechniqueBasedGenerator
{
	/// <summary>
	/// Represents the solver.
	/// </summary>
	protected static readonly BitwiseSolver Solver = new();


	/// <summary>
	/// Indicates the supported techniques that the current generator is supported.
	/// </summary>
	public abstract TechniqueSet SupportedTechniques { get; }


	/// <summary>
	/// Generates a puzzle and return a <see cref="Grid"/> instance;
	/// using <paramref name="cancellationToken"/> to cancel the operation.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token instance that can cancel the current operation.</param>
	/// <returns>The result <see cref="Grid"/> instance generated.</returns>
	public abstract Grid GenerateUnique(CancellationToken cancellationToken = default);
}
