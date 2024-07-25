namespace Sudoku.Runtime.GeneratingServices;

/// <summary>
/// Represents a generator type that creates a puzzle that can only uses the current technique to solve.
/// </summary>
public interface IPrimaryGenerator : ITechniqueBasedGenerator
{
	/// <summary>
	/// Represents a seed array for cells that can be used in core methods.
	/// </summary>
	protected static readonly Cell[] CellSeed = Enumerable.Range(0, 81).ToArray();

	/// <summary>
	/// Represents a seed array for houses that can be used in core methods.
	/// </summary>
	protected static readonly House[] HouseSeed = Enumerable.Range(0, 27).ToArray();

	/// <summary>
	/// Represents a seed array for digits that can be used in core methods.
	/// </summary>
	protected static readonly Digit[] DigitSeed = Enumerable.Range(0, 9).ToArray();

	/// <summary>
	/// Indicates the random number generator.
	/// </summary>
	protected static readonly Random Rng = Random.Shared;


	/// <summary>
	/// Try to shuffle the sequence for 3 times.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="values">The values to be shuffled.</param>
	protected static void ShuffleSequence<T>(T[] values)
	{
		Rng.Shuffle(values);
		Rng.Shuffle(values);
		Rng.Shuffle(values);
	}
}
