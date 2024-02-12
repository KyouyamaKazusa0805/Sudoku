namespace SudokuStudio.Views.Pages.ContentDialogs;

/// <summary>
/// Represents a library modifiy properties page.
/// </summary>
[DependencyProperty<string>("LibraryName?", Accessibility = Accessibility.Internal)]
[DependencyProperty<string>("LibraryAuthor?", Accessibility = Accessibility.Internal)]
[DependencyProperty<string>("LibraryDescription?", Accessibility = Accessibility.Internal)]
[DependencyProperty<ObservableCollection<string>>("LibraryTags", Accessibility = Accessibility.Internal)]
public sealed partial class LibraryModifyPropertiesDialogContent : Page
{
	[Default]
	private static readonly ObservableCollection<string> LibraryTagsDefaultValue = [];


	/// <summary>
	/// Initializes a <see cref="LibraryModifyPropertiesDialogContent"/> instance.
	/// </summary>
	public LibraryModifyPropertiesDialogContent() => InitializeComponent();
}
