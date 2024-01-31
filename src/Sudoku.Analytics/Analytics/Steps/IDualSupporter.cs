namespace Sudoku.Analytics.Steps;

/// <summary>
/// Represents an interface type that makes a type support for Dual concept.
/// </summary>
/// <typeparam name="T">The type of the step.</typeparam>
public interface IDualSupporter<T> where T : Step
{
	/// <summary>
	/// Get all possible Dual patterns of type <typeparamref name="T"/> for the specified accumulator.
	/// </summary>
	/// <param name="accumulator">The accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <returns>All Dual steps of type <typeparamref name="T"/>.</returns>
	public static abstract ReadOnlySpan<T> GetDual(List<T> accumulator, scoped ref readonly Grid grid);
}
