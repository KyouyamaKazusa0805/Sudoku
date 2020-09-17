namespace Sudoku.Windows
{
	/// <summary>
	/// Indicates the difficulty information used for gather the technique information of a puzzle.
	/// </summary>
	/// <param name="Technique">
	/// The technique name. If the value is <see langword="null"/>, the information will be a summary.
	/// </param>
	/// <param name="Count">The number of the technique used.</param>
	/// <param name="Total">The total difficulty.</param>
	/// <param name="Max">The maximum difficulty of all this technique instances.</param>
	public sealed record DifficultyInfo(string? Technique, int Count, decimal Total, decimal Max);
}
