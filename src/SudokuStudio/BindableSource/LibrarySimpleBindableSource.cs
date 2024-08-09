namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents the bindable source for libraries.
/// </summary>
public sealed partial class LibrarySimpleBindableSource
{
	/// <summary>
	/// Indicates the display name.
	/// </summary>
	public string DisplayName => LibraryConversion.GetDisplayName(Library.Name, Library.FileId);

	/// <summary>
	/// Indicates the library.
	/// </summary>
	public required LibraryInfo Library { get; set; }


	/// <summary>
	/// Try to fetch all <see cref="LibraryInfo"/> instances stored in the local path.
	/// </summary>
	/// <returns>All possible <see cref="LibraryInfo"/> instances.</returns>
	internal static LibraryInfo[] GetLibraries()
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
			select library
		];
	}
}
