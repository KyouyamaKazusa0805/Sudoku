namespace Sudoku.Concepts;

/// <summary>
/// Provides with extension methods on <see cref="Grid"/>.
/// </summary>
/// <seealso cref="Grid"/>
public static class GridExtensions
{
	/// <summary>
	/// Applies for all conclusions into the current <see cref="Grid"/> instance.
	/// </summary>
	/// <param name="this">A <see cref="Grid"/> instance that receives the conclusions to be applied.</param>
	/// <param name="step">A conclusion-provider <see cref="Step"/> instance.</param>
	public static void Apply(this scoped ref Grid @this, Step step)
	{
		foreach (var conclusion in step.Conclusions)
		{
			@this.Apply(conclusion);
		}
	}
}
