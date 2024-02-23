namespace Sudoku.Analytics.Traits;

/// <summary>
/// Represents an interface type that makes a type support for Dual concept.
/// </summary>
/// <typeparam name="TSelf">The type of the step.</typeparam>
public interface IDual<TSelf> where TSelf : Step, IDual<TSelf>
{
	/// <summary>
	/// Get all possible Dual patterns of type <typeparamref name="TSelf"/> for the specified accumulator.
	/// </summary>
	/// <param name="accumulator">The accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <returns>All Dual steps of type <typeparamref name="TSelf"/>.</returns>
	public static abstract ReadOnlySpan<TSelf> GetDual(List<TSelf> accumulator, scoped ref readonly Grid grid);
}
