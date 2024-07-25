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
}
