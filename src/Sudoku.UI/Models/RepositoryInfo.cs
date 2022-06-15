namespace Sudoku.UI.Models;

/// <summary>
/// Defines a set of properties to describe an image.
/// </summary>
public sealed class RepositoryInfo
{
	/// <summary>
	/// Initializes a <see cref="RepositoryInfo"/> instance.
	/// </summary>
	[SetsRequiredMembers]
	public RepositoryInfo() => (OpenSourceLicense, Initials, Name, IconPath, Site) = (null!, null!, null!, null!, null!);


	/// <summary>
	/// Indicates whether the repository code is for reference.
	/// </summary>
	public bool IsForReference { get; set; }

	/// <summary>
	/// Indicates the open-source license being used for this repository.
	/// </summary>
	public required string OpenSourceLicense { get; set; }

	/// <summary>
	/// Indicates the initials displaying on the <see cref="PersonPicture"/> control.
	/// </summary>
	/// <seealso cref="PersonPicture"/>
	public required string Initials { get; set; }

	/// <summary>
	/// Indicates the name of the image.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// Indicates the path of the image to show. The field can be <see langword="null"/>.
	/// </summary>
	public required string IconPath { get; set; }

	/// <summary>
	/// Indicates the website which name is corresponding to.
	/// </summary>
	public required Uri Site { get; set; }
}
