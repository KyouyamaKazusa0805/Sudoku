namespace Sudoku.IO;

/// <summary>
/// Indicates the option that will be used for ignoring puzzles in the target file.
/// </summary>
public enum PuzzleLibraryIgnoringOption
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
