namespace SudokuStudio.Views.Pages.ContentDialogs;

/// <summary>
/// Represents "Properties" dialog content for libraries.
/// </summary>
[DependencyProperty<string>("LibraryName", Accessibility = Accessibility.Internal)]
[DependencyProperty<string>("LibraryAuthor", Accessibility = Accessibility.Internal)]
[DependencyProperty<string>("LibraryPath", Accessibility = Accessibility.Internal)]
[DependencyProperty<string>("LibraryConfigPath", Accessibility = Accessibility.Internal)]
[DependencyProperty<string>("LibraryDescription", Accessibility = Accessibility.Internal)]
[DependencyProperty<string[]>("LibraryTags?", Accessibility = Accessibility.Internal)]
[DependencyProperty<DateTime>("LibraryLastModifiedTime", Accessibility = Accessibility.Internal)]
public sealed partial class LibraryPropertiesDialogContent : Page
{
	/// <summary>
	/// Initializes a <see cref="LibraryPropertiesDialogContent"/> instance.
	/// </summary>
	public LibraryPropertiesDialogContent() => InitializeComponent();


	private async void NavigateToLibraryFileButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		var folder = io::Path.GetDirectoryName(LibraryPath);
		await Launcher.LaunchFolderPathAsync(folder);
	}

	private async void NavigateToLibraryFileButton2_ClickAsync(object sender, RoutedEventArgs e)
	{
		var folder = io::Path.GetDirectoryName(LibraryConfigPath);
		await Launcher.LaunchFolderPathAsync(folder);
	}
}
