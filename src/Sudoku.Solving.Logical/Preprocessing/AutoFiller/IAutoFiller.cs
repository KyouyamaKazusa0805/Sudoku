namespace Sudoku.Preprocessing.AutoFiller;

/// <summary>
/// Defines an auto filler instance that can automatically fill some empty cells via the specified rule.
/// </summary>
public interface IAutoFiller
{
	/// <summary>
	/// To fill the current grid automatically with some digits input.
	/// </summary>
	/// <param name="grid">The target grid.</param>
	/// <exception cref="InvalidOperationException">Throws when the argument <paramref name="grid"/> is not unique.</exception>
	void Fill(scoped ref Grid grid);
}
