namespace Sudoku.Concepts;

/// <summary>
/// Provides with extension methods on <see cref="Grid"/>.
/// </summary>
/// <seealso cref="Grid"/>
public static class GridExtensions
{
	/// <inheritdoc cref="Grid.Apply(Conclusion[])"/>
	/// <param name="grid"><inheritdoc/></param>
	/// <param name="renderable">The renderable instance providing with conclusions to be applied.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Apply(this scoped ref Grid grid, IRenderable renderable) => grid.Apply(renderable.Conclusions);
}
