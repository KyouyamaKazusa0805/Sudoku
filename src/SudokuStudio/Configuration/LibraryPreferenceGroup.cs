namespace SudokuStudio.Configuration;

/// <summary>
/// Represents with preference items that is used in library-related pages.
/// </summary>
public sealed partial class LibraryPreferenceGroup : PreferenceGroup
{
	[Default]
	private static readonly LibraryCandidatesVisibility LibraryCandidatesVisibilityDefaultValue = LibraryCandidatesVisibility.AlwaysShown;


	/// <summary>
	/// Indicates whether the candidates in a puzzle defined in puzzle libraries should be shown.
	/// </summary>
	[AutoDependencyProperty]
	public partial LibraryCandidatesVisibility LibraryCandidatesVisibility { get; set; }

	/// <summary>
	/// Indicates the transforming kinds for library puzzles.
	/// </summary>
	[AutoDependencyProperty(DefaultValue = 0)]
	public partial TransformType LibraryPuzzleTransformations { get; set; }
}
