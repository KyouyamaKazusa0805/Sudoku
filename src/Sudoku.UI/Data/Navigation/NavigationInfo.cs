namespace Sudoku.UI.Data.Navigation;

/// <summary>
/// Defines a navigation information instance.
/// </summary>
/// <param name="ViewItemTag">The tag of the view item.</param>
/// <param name="PageType">The page type.</param>
internal readonly record struct NavigationInfo(string ViewItemTag, Type PageType)
{
	/// <summary>
	/// Get navigation information instances of pages.
	/// </summary>
	/// <returns>An array of pages' navigation information.</returns>
	public static NavigationInfo[] GetPages()
		=> (
			from type in typeof(MainWindow).Assembly.GetDerivedTypes<Page>()
			where type.GetCustomAttribute<PageAttribute>() is not null
			select new NavigationInfo(type.Name, type)
		).ToArray();
}
