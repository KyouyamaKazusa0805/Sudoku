namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents the bindable source for libraries.
/// </summary>
/// <param name="lib">Indicates the library.</param>
[method: SetsRequiredMembers]
public sealed partial class LibrarySimpleBindableSource(
	[PrimaryConstructorParameter(Accessibility = "public required", SetterExpression = "set", GeneratedMemberName = "Library")]
	Library lib
)
{
	/// <summary>
	/// Indicates the display name.
	/// </summary>
	public string DisplayName => LibraryConversion.GetDisplayName(Library.Name, Library.FileId);
}
