namespace Sudoku.UI.Models;

/// <summary>
/// Defines a pair of information on a documentation file.
/// </summary>
public sealed class DocumentationFileInfo
{
	/// <summary>
	/// Indicates the display name.
	/// </summary>
	public string DisplayName { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the file path.
	/// </summary>
	public string FilePath { get; set; } = null!;
}
