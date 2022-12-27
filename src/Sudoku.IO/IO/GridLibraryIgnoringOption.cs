namespace Sudoku.IO;

/// <summary>
/// Indicates the option that will be used for ignoring <see cref="Grid"/> puzzles in the target file.
/// </summary>
/// <seealso cref="Grid"/>
public enum GridLibraryIgnoringOption
{
	/// <summary>
	/// Indicates the puzzle will be never ignored.
	/// </summary>
	Never,
	
	/// <summary>
	/// Indicates the puzzle will be ignored when it is not unique.
	/// </summary>
	NotUnique
}
