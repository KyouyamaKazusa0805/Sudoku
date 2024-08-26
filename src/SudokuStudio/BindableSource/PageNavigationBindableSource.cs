namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents a bindable source for page navigation.
/// </summary>
public sealed class PageNavigationBindableSource
{
	/// <summary>
	/// Indicates the page type.
	/// </summary>
	public Type? PageType { get; set; }

	/// <summary>
	/// Indicates the page title.
	/// </summary>
	public string? PageTitle { get; set; }
}
