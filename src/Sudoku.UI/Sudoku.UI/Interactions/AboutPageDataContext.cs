namespace Sudoku.UI.Interactions;

/// <summary>
/// Provides with the data context that binds with <see cref="AboutPage"/>.
/// </summary>
/// <seealso cref="AboutPage"/>
public sealed class AboutPageDataContext : IDataContext<AboutPageDataContext>
{
	/// <inheritdoc/>
	public static AboutPageDataContext CreateInstance() => new();
}