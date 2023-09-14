using SudokuStudio.Configuration;
using static SudokuStudio.Strings.StringsAccessor;

namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents a type that describes for a library of sudoku puzzles and its introduction.
/// </summary>
public sealed class GridLibraryBindableSource
{
	/// <summary>
	/// Indicates the name of the library.
	/// </summary>
	public required string LibraryName { get; set; }

	/// <summary>
	/// Indicates the author of the library. The default value is "<c><![CDATA[<Anonymous>]]></c>".
	/// </summary>
	public string Author { get; set; } = GetString("AnonymousAuthor");

	/// <summary>
	/// Indicates the puzzles. The default value is empty list. This property is never <see langword="null"/>.
	/// </summary>
	public List<GridInfo> Puzzles { get; set; } = [];
}
