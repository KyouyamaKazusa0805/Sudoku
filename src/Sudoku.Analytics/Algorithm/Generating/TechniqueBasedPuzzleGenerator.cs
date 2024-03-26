namespace Sudoku.Algorithm.Generating;

/// <summary>
/// Represents a generator type that generates puzzles, relating to a kind of technique.
/// </summary>
public abstract class TechniqueBasedPuzzleGenerator
{
	/// <summary>
	/// Indicates the supported technique.
	/// </summary>
	public abstract Technique SupportedTechnique { get; }


	/// <summary>
	/// Indicates the random number generator.
	/// </summary>
	private protected static Random Rng => Random.Shared;


	/// <summary>
	/// Generates a puzzle that has multiple solutions, with only one cell has only one possibility to be filled
	/// that can be solved in logic.
	/// </summary>
	/// <inheritdoc cref="TryGenerateUnique(out Grid, IProgress{int}, CancellationToken)"/>
	public abstract bool TryGenerateOnlyOneCell(out Grid result, IProgress<int>? progress = null, CancellationToken cancellationToken = default);

	/// <summary>
	/// Generates a puzzle that has a unique solution, with a must that contains the specified technique appeared in the puzzle.
	/// </summary>
	/// <param name="result">
	/// The puzzle returned. The argument becomes valid if and only if the return value is <see langword="true"/>.
	/// </param>
	/// <param name="progress">The progress object that can report the current state.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the operation.</param>
	/// <returns>
	/// <para>
	/// A <see cref="bool"/> result indicating whether the module supports for generating on this case, and returns a valid result.
	/// Always return <see langword="false"/> if not supported.
	/// </para>
	/// <para>
	/// <b>Never</b> throw an exception if you don't want to support for this case.
	/// Use "<see langword="return false"/>;" and assign argument <paramref name="result"/> with <see cref="Grid.Undefined"/> instead.
	/// </para>
	/// </returns>
	/// <seealso cref="Grid.Undefined"/>
	public abstract bool TryGenerateUnique(out Grid result, IProgress<int>? progress = null, CancellationToken cancellationToken = default);

	/// <summary>
	/// A default method that always return <see langword="false"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private protected bool ReturnDefault(out Grid result)
	{
		result = Grid.Undefined;
		return false;
	}
}
