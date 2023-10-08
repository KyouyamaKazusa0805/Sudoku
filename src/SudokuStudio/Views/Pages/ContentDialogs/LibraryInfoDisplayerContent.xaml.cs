using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
using SudokuStudio.ComponentModel;
using SudokuStudio.Interaction;
using static SudokuStudio.Strings.StringsAccessor;

namespace SudokuStudio.Views.Pages.ContentDialogs;

/// <summary>
/// Defines a dialog content for adding or loading a puzzle library.
/// </summary>
[DependencyProperty<string>("FilePath", IsNullable = true, DocSummary = "Indicates the target file path.")]
[DependencyProperty<string>("FileId", IsNullable = true, DocSummary = "Indicates the file ID.")]
[DependencyProperty<string>("LibraryName", IsNullable = true, DocSummary = "Indicates the library name.")]
[DependencyProperty<string>("LibraryAuthor", IsNullable = true, DocSummary = "Indicates the author of the library.")]
[DependencyProperty<string>("LibraryDescription", IsNullable = true, DocSummary = "Indicates the description of the library.")]
[DependencyProperty<string>("AppendingPuzzleTextCode", IsNullable = true, DocSummary = "Indicates the text code of the new appending puzzle.")]
[DependencyProperty<LibraryDataUpdatingMode>("Mode", DocSummary = "Indicates whether the puzzle library is adding a new one, instead of loading.")]
[DependencyProperty<ObservableCollection<string>>("LibraryTags", DocSummary = "Indicates the tags to the library.")]
public sealed partial class LibraryInfoDisplayerContent : Page
{
	[Default]
	private static readonly string LibraryAuthorDefaultValue = GetString("AnonymousAuthor");

	[Default]
	private static readonly ObservableCollection<string> LibraryTagsDefaultValue = [];


	/// <summary>
	/// Initializes a <see cref="LibraryInfoDisplayerContent"/> instance.
	/// </summary>
	public LibraryInfoDisplayerContent() => InitializeComponent();
}
