namespace SudokuStudio.Views.Pages.ContentDialogs;

/// <summary>
/// Represents a library modifiy properties page.
/// </summary>
public sealed partial class LibraryModifyPropertiesDialogContent : Page
{
	[Default]
	private static readonly ObservableCollection<string> LibraryTagsDefaultValue = [];


	[AutoDependencyProperty]
	internal partial string? LibraryName { get; set; }

	[AutoDependencyProperty]
	internal partial string? LibraryAuthor { get; set; }

	[AutoDependencyProperty]
	internal partial string? LibraryDescription { get; set; }

	[AutoDependencyProperty]
	internal partial ObservableCollection<string> LibraryTags { get; set; }


	/// <summary>
	/// Initializes a <see cref="LibraryModifyPropertiesDialogContent"/> instance.
	/// </summary>
	public LibraryModifyPropertiesDialogContent() => InitializeComponent();
}
