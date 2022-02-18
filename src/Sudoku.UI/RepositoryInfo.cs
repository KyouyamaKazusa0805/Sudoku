namespace Sudoku.UI;

/// <summary>
/// Defines a set of properties to describe an image.
/// </summary>
public sealed class RepositoryInfo
{
	/// <summary>
	/// Indicates whether the repository code is for reference.
	/// </summary>
	public bool IsForReference { get; set; }

	/// <summary>
	/// Indicates the open-source license being used for this repository.
	/// </summary>
	public string? OpenSourceLicense { get; set; }

	/// <summary>
	/// Indicates the initials displaying on the <see cref="PersonPicture"/> control.
	/// </summary>
	public string? Initials { get; set; }

	/// <summary>
	/// Indicates the name of the image.
	/// </summary>
	public string? Name { get; set; }

	/// <summary>
	/// Indicates the path of the image to show. The field can be <see langword="null"/>.
	/// </summary>
	public string? IconPath { get; set; }

	/// <summary>
	/// Indicates the website which name is corresponding to.
	/// </summary>
	public Uri? Site { get; set; }
}
