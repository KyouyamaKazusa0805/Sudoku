namespace SudokuStudio.Views.Pages.ContentDialogs;

/// <summary>
/// Represents an "add library" page.
/// </summary>
[DependencyProperty<string>("FileId", Accessibility = Accessibility.Internal)]
[DependencyProperty<string>("FilePath", Accessibility = Accessibility.Internal)]
[DependencyProperty<string>("LibraryName?", Accessibility = Accessibility.Internal)]
[DependencyProperty<string>("LibraryAuthor?", Accessibility = Accessibility.Internal)]
[DependencyProperty<string>("LibraryDescription?", Accessibility = Accessibility.Internal)]
[DependencyProperty<ObservableCollection<string>>("LibraryTags", Accessibility = Accessibility.Internal)]
public sealed partial class AddLibraryDialogContent : Page
{
	[Default]
	private static readonly ObservableCollection<string> LibraryTagsDefaultValue = [];


	/// <summary>
	/// Initializes an <see cref="AddLibraryDialogContent"/> instance.
	/// </summary>
	public AddLibraryDialogContent() => InitializeComponent();


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

		throw new Exception("No available name can be used.");
	}


	[Callback]
	private static void LibraryNamePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is (AddLibraryDialogContent instance, { NewValue: string fileId }))
		{
			instance.FileId = GetAvailbleFileId(fileId);
			instance.FilePath = $@"{CommonPaths.Library}\{instance.FileId}{FileExtensions.PuzzleLibrary}";
		}
	}
}
