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
	public RepositoryInfo() => (OpenSourceLicense, Name, Site) = (null!, null!, null!);


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
	/// This property can be <see langword="null"/> if you have set the property <see cref="IconPath"/>.
	/// </summary>
	/// <seealso cref="PersonPicture"/>
	/// <seealso cref="IconPath"/>
	public string? Initials { get; set; }

	/// <summary>
	/// Indicates the name of the image.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// Indicates the path of the image to show.
	/// This property can be <see langword="null"/> if you have set the property <see cref="Initials"/>.
	/// </summary>
	/// <seealso cref="Initials"/>
	public ImageSource? IconPath { get; set; }

	/// <summary>
	/// Indicates the website which name is corresponding to.
	/// </summary>
	public required Uri Site { get; set; }
}
