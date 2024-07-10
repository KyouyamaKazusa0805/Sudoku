namespace SudokuStudio.Views.Pages.ContentDialogs;

/// <summary>
/// Represents "Properties" dialog content for libraries.
/// </summary>
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


	/// <summary>
	/// Indicates whether the page is loading.
	/// </summary>
	[AutoDependencyProperty]
	internal partial bool IsLoadingPuzzlesCount { get; set; }

	/// <summary>
	/// Indicates the library name.
	/// </summary>
	[AutoDependencyProperty]
	internal partial string LibraryName { get; set; }

	/// <summary>
	/// Indicates the author of the library.
	/// </summary>
	[AutoDependencyProperty]
	internal partial string LibraryAuthor { get; set; }

	/// <summary>
	/// Indicates the library description.
	/// </summary>
	[AutoDependencyProperty]
	internal partial string LibraryDescription { get; set; }

	/// <summary>
	/// Indicates the last modified time of library.
	/// </summary>
	[AutoDependencyProperty]
	internal partial DateTime LibraryLastModifiedTime { get; set; }

	/// <summary>
	/// Indicates the library information.
	/// </summary>
	[AutoDependencyProperty]
	internal partial LibraryInfo LibraryInfo { get; set; }


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
