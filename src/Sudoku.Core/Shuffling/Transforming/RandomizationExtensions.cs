namespace Sudoku.Shuffling.Transforming;

/// <summary>
/// Provides with extension methods for <see cref="Random"/>.
/// </summary>
/// <seealso cref="Random"/>
public static class RandomizationExtensions
{
	/// <summary>
	/// Returns a random integer that is within valid digit range (0..9).
	/// </summary>
	/// <param name="random">The random instance.</param>
	/// <returns>
	/// An integer that represents a valid <see cref="Digit"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Digit NextDigit(this Random random) => random.Next(0, 9);

	/// <summary>
	/// Returns a random integer that is within valid cell range (0..81).
	/// </summary>
	/// <param name="random">The random instance.</param>
	/// <returns>
	/// An integer that represents a valid <see cref="Cell"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Cell NextCell(this Random random) => random.Next(0, 81);

	/// <summary>
	/// Returns a random integer that is within valid house range (0..27).
	/// </summary>
	/// <param name="random">The random instance.</param>
	/// <returns>
	/// An integer that represents a valid <see cref="House"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static House NextHouse(this Random random) => random.Next(0, 27);

	/// <summary>
	/// Randomly select the specified number of elements from the current collection.
	/// </summary>
	/// <param name="random">The random instance.</param>
	/// <param name="cells">The cells to be chosen.</param>
	/// <param name="count">The desired number of elements.</param>
	/// <returns>The specified number of elements returned, represented as a <see cref="CellMap"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap RandomlySelect(this Random random, in CellMap cells, int count)
	{
		var result = cells.Offsets[..];
		random.Shuffle(result);
		return [.. result[..count]];
	}

	/// <summary>
	/// Randomly select the specified number of elements from the current collection.
	/// </summary>
	/// <param name="random">The random instance.</param>
	/// <param name="cells">The cells to be chosen.</param>
	/// <param name="count">The desired number of elements.</param>
	/// <returns>The specified number of elements returned, represented as a <see cref="CandidateMap"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap RandomlySelect(this Random random, in CandidateMap cells, int count)
	{
		var result = cells.Offsets[..];
		random.Shuffle(result);
		return [.. result[..count]];
	}

	/// <summary>
	/// Creates a <see cref="CellMap"/> instance, with the specified number of <see cref="Cell"/>s stored in the collection.
	/// </summary>
	/// <param name="random">The random instance.</param>
	/// <param name="count">The desired number of elements.</param>
	/// <returns>A <see cref="CellMap"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap CreateCellMap(this Random random, int count) => random.RandomlySelect(CellMap.Full, count);

	/// <summary>
	/// Creates a <see cref="CandidateMap"/> instance, with the specified number of <see cref="Candidate"/>s stored in the collection.
	/// </summary>
	/// <param name="random">The random instance.</param>
	/// <param name="count">The desired number of elements.</param>
	/// <returns>A <see cref="CandidateMap"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap CreateCandidateMap(this Random random, int count) => random.RandomlySelect(CandidateMap.Full, count);
}
