namespace SudokuStudio.Views.Pages.ContentDialogs;

/// <summary>
/// Represents "Properties" dialog content for libraries.
/// </summary>
[DependencyProperty<bool>("IsLoadingPuzzlesCount", Accessibility = Accessibility.Internal)]
[DependencyProperty<string>("LibraryName", Accessibility = Accessibility.Internal)]
[DependencyProperty<string>("LibraryAuthor", Accessibility = Accessibility.Internal)]
[DependencyProperty<string>("LibraryDescription", Accessibility = Accessibility.Internal)]
[DependencyProperty<DateTime>("LibraryLastModifiedTime", Accessibility = Accessibility.Internal)]
[DependencyProperty<Library>("LibraryInfo", Accessibility = Accessibility.Internal)]
public sealed partial class LibraryPropertiesDialogContent : Page
{
	/// <summary>
	/// Initializes a <see cref="LibraryPropertiesDialogContent"/> instance.
	/// </summary>
	public LibraryPropertiesDialogContent()
	{
		InitializeComponent();

		IsLoadingPuzzlesCount = true;
	}


	private void Page_Loaded(object sender, RoutedEventArgs e)
		=> DispatcherQueue.TryEnqueue(
			async () =>
			{
				LibraryPuzzlesCountDisplayer.Text = (await LibraryInfo.GetCountAsync()).ToString();
				IsLoadingPuzzlesCount = false;
			}
		);

	private async void NavigateToLibraryFileButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		var folder = io::Path.GetDirectoryName(LibraryInfo.LibraryFilePath);
		await Launcher.LaunchFolderPathAsync(folder);
	}

	private async void NavigateToLibraryFileButton2_ClickAsync(object sender, RoutedEventArgs e)
	{
		var folder = io::Path.GetDirectoryName(LibraryInfo.ConfigFilePath);
		await Launcher.LaunchFolderPathAsync(folder);
	}
}
