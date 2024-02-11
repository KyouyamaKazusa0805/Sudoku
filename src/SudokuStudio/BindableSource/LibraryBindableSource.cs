namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents a library bindable source. This type is different with <see cref="LibrarySimpleBindableSource"/>
/// - this type contains more properties that can be bound with UI.
/// </summary>
/// <seealso cref="LibrarySimpleBindableSource"/>
[DependencyProperty<string>("Name", DocSummary = "Indicates the name of the library.")]
[DependencyProperty<string>("FileId", DocSummary = "Indicates the unique file name of the library.")]
[DependencyProperty<string>("Author", DocSummary = "Indicates the author of the library.")]
[DependencyProperty<string>("Description", DocSummary = "Indicates the description to the library.")]
[DependencyProperty<string[]>("Tags", DocSummary = "Indicates the tags of the library.")]
public sealed partial class LibraryBindableSource : DependencyObject
{
	/// <summary>
	/// Creates a <see cref="Library"/> instance, and initialize the files from local.
	/// </summary>
	/// <returns>A <see cref="Library"/> instance initialized.</returns>
	public Library ToLibrary()
	{
		var result = new Library(CommonPaths.Library, FileId);
		result.Initialize();
		return result;
	}
}
