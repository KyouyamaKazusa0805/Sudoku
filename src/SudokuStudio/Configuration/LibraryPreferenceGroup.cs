namespace SudokuStudio.Configuration;

/// <summary>
/// Represents with preference items that is used in library-related pages.
/// </summary>
[DependencyProperty<LibraryCandidatesVisibility>("LibraryCandidatesVisibility", DocSummary = "Indicates whether the candidates in a puzzle defined in puzzle libraries should be shown.")]
[DependencyProperty<TransformType>("LibraryPuzzleTransformations", DefaultValue = 0, DocSummary = "Indicates the transforming kinds for library puzzles.")]
public sealed partial class LibraryPreferenceGroup : PreferenceGroup
{
	[Default]
	private static readonly LibraryCandidatesVisibility LibraryCandidatesVisibilityDefaultValue = LibraryCandidatesVisibility.AlwaysShown;
}
