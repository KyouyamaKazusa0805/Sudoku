namespace Sudoku.Analytics.Construction.Searching;

/// <summary>
/// Represents a pattern searcher type.
/// </summary>
/// <typeparam name="T">The type of the found data.</typeparam>
public interface IPatternSearcher<T> where T : notnull
{
	/// <summary>
	/// Try to search patterns, and return the found data.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <returns>The found data.</returns>
	public static abstract ReadOnlySpan<T> Search(ref readonly Grid grid);
}
