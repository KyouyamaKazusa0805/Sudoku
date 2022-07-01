namespace Sudoku.UI.Navigation;

/// <summary>
/// Defines a navigation information instance.
/// </summary>
/// <param name="ViewItemTag">The tag of the view item.</param>
/// <param name="PageType">The page type.</param>
/// <param name="DisplayTitle">The displaying title.</param>
internal readonly record struct NavigationInfo(string ViewItemTag, Type PageType, bool DisplayTitle)
{
	/// <summary>
	/// Get navigation information instances of pages.
	/// </summary>
	/// <returns>An array of pages' navigation information.</returns>
	public static NavigationInfo[] GetPages()
		=> (
			from type in typeof(MainWindow).Assembly.GetDerivedTypes<Page>()
			let attribute = type.GetCustomAttribute<PageAttribute>()
			where attribute is not null
			select new NavigationInfo(type.Name, type, attribute.DisplayTitle)
		).ToArray();
}
