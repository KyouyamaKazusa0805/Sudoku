namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents a library bindable source. This type is different with <see cref="LibrarySimpleBindableSource"/>
/// - this type contains more properties that can be bound with UI.
/// </summary>
/// <seealso cref="LibrarySimpleBindableSource"/>
public sealed partial class LibraryBindableSource : DependencyObject
{
	[Default]
	internal static readonly string NameDefaultValue = SR.Get("NoName", App.CurrentCulture);

	[Default]
	internal static readonly string AuthorDefaultValue = SR.Get("Anonymous", App.CurrentCulture);

	[Default]
	internal static readonly string DescriptionDefaultValue = SR.Get("NoDescription", App.CurrentCulture);


	/// <summary>
	/// Indicates whether the current library is loading, updating, etc..
	/// </summary>
	[DependencyProperty]
	public partial bool IsActive { get; set; }

	/// <summary>
	/// Indicates the name of the library.
	/// </summary>
	[DependencyProperty]
	public partial string Name { get; set; }

	/// <summary>
	/// Indicates the unique file name of the library.
	/// </summary>
	[DependencyProperty]
	public partial string FileId { get; set; }

	/// <summary>
	/// Indicates the author of the library.
	/// </summary>
	[DependencyProperty]
	public partial string Author { get; set; }

	/// <summary>
	/// Indicates the description to the library.
	/// </summary>
	[DependencyProperty]
	public partial string Description { get; set; }

	/// <summary>
	/// Indicates the tags of the library.
	/// </summary>
	[DependencyProperty]
	public partial string[] Tags { get; set; }


	/// <summary>
	/// Indicates the corresponding <see cref="Library"/> instance.
	/// </summary>
	public LibraryInfo Library => new(CommonPaths.Library, FileId);


	/// <summary>
	/// Try to create <see cref="LibraryBindableSource"/> instances from local path.
	/// </summary>
	/// <returns>A list of <see cref="LibraryBindableSource"/> instances.</returns>
	internal static ObservableCollection<LibraryBindableSource> GetLibrariesFromLocal()
	{
		if (!Directory.Exists(CommonPaths.Library))
		{
			// Implicitly create directory if the library directory does not exist.
			Directory.CreateDirectory(CommonPaths.Library);
			return [];
		}

		return [
			..
			from file in Directory.EnumerateFiles(CommonPaths.Library)
			let extension = io::Path.GetExtension(file)
			where extension == FileExtensions.PuzzleLibrary
			let fileId = io::Path.GetFileNameWithoutExtension(file)
			let library = new LibraryInfo(CommonPaths.Library, fileId)
			where library.IsInitialized
			let firstContentLine = File.ReadLines(library.ConfigFilePath).FirstOrDefault()
			where firstContentLine == LibraryInfo.ConfigFileHeader
			select new LibraryBindableSource
			{
				Name = library.Name ?? NameDefaultValue,
				Author = library.Author ?? AuthorDefaultValue,
				Description = library.Description ?? DescriptionDefaultValue,
				Tags = library.Tags ?? [],
				FileId = fileId,
				IsActive = false
			}
		];
	}
}
