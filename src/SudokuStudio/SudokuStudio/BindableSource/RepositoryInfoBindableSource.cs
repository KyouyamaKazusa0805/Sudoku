namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents a type that can be used for binding as source, for repository information
/// used for displaying the source code referencing.
/// </summary>
public sealed class RepositoryInfoBindableSource
{
	/// <summary>
	/// Indicates whether the repository code is for reference.
	/// </summary>
	public bool IsForReference { get; set; }

	/// <summary>
	/// Indicates the open-source license being used for this repository.
	/// </summary>
	public string OpenSourceLicense { get; set; } = string.Empty;

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
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the path of the image to show.
	/// This property can be <see langword="null"/> if you have set the property <see cref="Initials"/>.
	/// </summary>
	/// <seealso cref="Initials"/>
	public ImageSource? IconPath { get; set; }

	/// <summary>
	/// Indicates the website which name is corresponding to.
	/// </summary>
	public Uri? Site { get; set; }
}
