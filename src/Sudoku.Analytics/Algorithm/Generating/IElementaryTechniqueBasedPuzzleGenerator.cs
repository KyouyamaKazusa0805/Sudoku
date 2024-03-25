namespace Sudoku.Algorithm.Generating;

/// <summary>
/// Represents a generator type that binds with a technique usage.
/// </summary>
/// <typeparam name="TSelf">
/// The type of the generator itself; it must be derived from <see cref="TechniqueBasedPuzzleGenerator"/>.
/// </typeparam>
public interface IElementaryTechniqueBasedPuzzleGenerator<TSelf> where TSelf : TechniqueBasedPuzzleGenerator, IElementaryTechniqueBasedPuzzleGenerator<TSelf>
{
	/// <summary>
	/// Indicates whether the generator will generate a puzzle that only allows a user solves the puzzle,
	/// with only the current technique used.
	/// </summary>
	public abstract bool CanOnlyUseThisTechnique { get; set; }
}
