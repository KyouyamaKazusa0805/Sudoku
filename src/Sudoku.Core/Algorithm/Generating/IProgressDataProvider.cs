namespace Sudoku.Algorithm.Generating;

/// <summary>
/// Indicates the data provider type,
/// </summary>
/// <typeparam name="T">The type of the implementation data provider.</typeparam>
public interface IProgressDataProvider<T> where T : struct, IEquatable<T>, IProgressDataProvider<T>
{
	/// <summary>
	/// Indicates the number of puzzles having been generated.
	/// </summary>
	public abstract int Count { get; init; }


	/// <summary>
	/// Try to fetch display string for the current instance.
	/// </summary>
	/// <returns>The display string.</returns>
	public abstract string ToDisplayString();


	/// <summary>
	/// Try to create a <typeparamref name="T"/> instance.
	/// </summary>
	/// <param name="count">The number of puzzles generated.</param>
	/// <param name="succeeded">The number of puzzles has passed the checking.</param>
	/// <returns>A <typeparamref name="T"/> instance.</returns>
	public static abstract T Create(int count, int succeeded);
}
