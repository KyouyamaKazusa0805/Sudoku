namespace Sudoku.UI.Models;

/// <summary>
/// Defines a set of properties to describe an image.
/// </summary>
public sealed class ImageInfo
{
	/// <summary>
	/// Indicates the name of the image.
	/// </summary>
	public string? Name { get; set; }

	/// <summary>
	/// Indicates the website which name is corresponding to.
	/// </summary>
	public string? Site { get; set; }

	/// <summary>
	/// Indicates the path of the image to show. The field can be <see langword="null"/>.
	/// </summary>
	public string? Path { get; set; }
}
