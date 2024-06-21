namespace SudokuStudio.Views.Pages.ContentDialogs;

/// <summary>
/// Represents "save to library" page.
/// </summary>
[DependencyProperty<object>("SelectedLibrary", Accessibility = Accessibility.Internal)]
[DependencyProperty<bool>("IsNameValidAsFileId", Accessibility = Accessibility.Internal)]
[DependencyProperty<int>("SelectedMode", Accessibility = Accessibility.Internal)]
[DependencyProperty<string>("FileId", Accessibility = Accessibility.Internal)]
[DependencyProperty<string>("FilePath", Accessibility = Accessibility.Internal)]
[DependencyProperty<string>("LibraryName?", Accessibility = Accessibility.Internal)]
[DependencyProperty<string>("LibraryAuthor?", Accessibility = Accessibility.Internal)]
[DependencyProperty<string>("LibraryDescription?", Accessibility = Accessibility.Internal)]
[DependencyProperty<ObservableCollection<string>>("LibraryTags", Accessibility = Accessibility.Internal)]
[DependencyProperty<ObservableCollection<LibraryBindableSource>>("AvailableLibraries", Accessibility = Accessibility.Internal)]
public sealed partial class SaveToLibraryDialogContent : Page
{
	[Default]
	private static readonly ObservableCollection<string> LibraryTagsDefaultValue = [];

	[Default]
	private static readonly ObservableCollection<LibraryBindableSource> AvailableLibrariesDefaultValue = [];


	/// <summary>
	/// Initializes a <see cref="SaveToLibraryDialogContent"/> instance.
	/// </summary>
	public SaveToLibraryDialogContent() => InitializeComponent();


	/// <summary>
	/// <para>Determines whether the specified file ID is available and stored in local, as the real file name.</para>
	/// <para>
	/// If the local path doesn't contain the same-name file, the file ID will be returned; otherwise,
	/// add a suffix to prevent same name.
	/// </para>
	/// </summary>
	/// <param name="fileId">The file ID.</param>
	/// <returns>An expected available file ID.</returns>
	private static string GetAvailbleFileId(string fileId)
	{
		var directory = CommonPaths.Library;
#if false
		// Create the current directory if not exist.
		if (!Directory.Exists(directory))
		{
			Directory.CreateDirectory(directory);
		}
#endif

		var filePath = $@"{directory}\{fileId}{FileExtensions.PuzzleLibrary}";
		if (!File.Exists(filePath))
		{
			return fileId;
		}

		// Try for 1024 times.
		for (var i = 1U; i <= 1U << 10; i++)
		{
			filePath = $@"{directory}\{fileId}_{i}{FileExtensions.PuzzleLibrary}";
			if (!File.Exists(filePath))
			{
				return $"{fileId}_{i}";
			}
		}

		throw new(SR.ExceptionMessage("NoAvailableNameCanBeUsed"));
	}


	[Callback]
	private static void IsNameValidAsFileIdPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is (SaveToLibraryDialogContent instance, { NewValue: bool value }))
		{
			if (value)
			{
				instance.ErrorInfoDisplayer.Visibility = Visibility.Collapsed;
				instance.PathDisplayer.Visibility = Visibility.Visible;
			}
			else
			{
				instance.ErrorInfoDisplayer.Visibility = Visibility.Visible;
				instance.PathDisplayer.Visibility = Visibility.Collapsed;
			}
		}
	}

	[Callback]
	private static void LibraryNamePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is (SaveToLibraryDialogContent instance, { NewValue: string fileId }))
		{
			if (!File2.IsValidFileName(fileId))
			{
				instance.IsNameValidAsFileId = false;
			}
			else
			{
				instance.FileId = GetAvailbleFileId(fileId);
				instance.FilePath = $@"{CommonPaths.Library}\{instance.FileId}{FileExtensions.PuzzleLibrary}";
				instance.IsNameValidAsFileId = true;
			}
		}
	}


	private void ModeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (sender is Segmented { SelectedItem: SegmentedItem { Tag: int value } })
		{
			SelectedMode = value;
		}
	}
}
