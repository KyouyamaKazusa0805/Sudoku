namespace SudokuStudio.Views.Pages.ContentDialogs;

/// <summary>
/// Represents a library modifiy properties page.
/// </summary>
public sealed partial class LibraryModifyPropertiesDialogContent : Page
{
	[Default]
	private static readonly ObservableCollection<string> LibraryTagsDefaultValue = [];


	[DependencyProperty]
	internal partial string? LibraryName { get; set; }

	[DependencyProperty]
	internal partial string? LibraryAuthor { get; set; }

	[DependencyProperty]
	internal partial string? LibraryDescription { get; set; }

	[DependencyProperty]
	internal partial ObservableCollection<string> LibraryTags { get; set; }


	/// <summary>
	/// Initializes a <see cref="LibraryModifyPropertiesDialogContent"/> instance.
	/// </summary>
	public LibraryModifyPropertiesDialogContent() => InitializeComponent();
}
