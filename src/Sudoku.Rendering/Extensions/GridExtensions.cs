namespace Sudoku.Concepts;

/// <summary>
/// Provides with extension methods on <see cref="Grid"/>.
/// </summary>
/// <seealso cref="Grid"/>
public static class GridExtensions
{
	/// <summary>
	/// <inheritdoc cref="Grid.Apply(Conclusion[])" path="/summary"/>
	/// </summary>
	/// <param name="this">The puzzle to be applied.</param>
	/// <param name="renderable">The renderable instance providing with conclusions to be applied.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Apply(this scoped ref Grid @this, IRenderable renderable) => @this.Apply(renderable.Conclusions);
}
