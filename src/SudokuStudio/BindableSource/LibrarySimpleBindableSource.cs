namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents the bindable source for libraries.
/// </summary>
/// <param name="lib">Indicates the library.</param>
[method: SetsRequiredMembers]
public sealed partial class LibrarySimpleBindableSource(
	[PrimaryConstructorParameter(Accessibility = "public required", SetterExpression = "set", GeneratedMemberName = "Library")]
	Library lib
)
{
	/// <summary>
	/// Indicates the display name.
	/// </summary>
	public string DisplayName => LibraryConversion.GetDisplayName(Library.Name, Library.FileId);


	/// <summary>
	/// Try to fetch all <see cref="Sudoku.Runtime.LibraryServices.Library"/> instances stored in the local path.
	/// </summary>
	/// <returns>All possible <see cref="Sudoku.Runtime.LibraryServices.Library"/> instances.</returns>
	internal static Library[] GetLibraries()
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
			let library = new Library(CommonPaths.Library, fileId)
			where library.IsInitialized
			let firstContentLine = File.ReadLines(library.ConfigFilePath).FirstOrDefault()
			where firstContentLine == Library.ConfigFileHeader
			select library
		];
	}
}
