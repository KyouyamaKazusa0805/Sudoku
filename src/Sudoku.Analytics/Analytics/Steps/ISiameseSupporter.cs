namespace Sudoku.Analytics.Steps;

/// <summary>
/// Represents an interface type that makes a type support for Siamese concept.
/// </summary>
/// <typeparam name="TSelf">The type of the step.</typeparam>
public interface ISiameseSupporter<TSelf> where TSelf : Step, ISiameseSupporter<TSelf>
{
	/// <summary>
	/// Get all possible Siamese patterns of type <typeparamref name="TSelf"/> for the specified accumulator.
	/// </summary>
	/// <param name="accumulator">The accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <returns>All Siamese steps of type <typeparamref name="TSelf"/>.</returns>
	public static abstract ReadOnlySpan<TSelf> GetSiamese(List<TSelf> accumulator, scoped ref readonly Grid grid);
}
