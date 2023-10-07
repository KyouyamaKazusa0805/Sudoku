using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
using SudokuStudio.BindableSource;

namespace SudokuStudio.Views.Pages;

/// <summary>
/// Represents for a library page.
/// </summary>
public sealed partial class LibraryPage : Page
{
	/// <summary>
	/// Indicates the puzzle library.
	/// </summary>
	private readonly ObservableCollection<PuzzleLibraryBindableSource> _puzzleLibraries = PuzzleLibraryBindableSource.LocalPuzzleLibraries;


	/// <summary>
	/// Initializes a <see cref="LibraryPage"/> instance.
	/// </summary>
	public LibraryPage() => InitializeComponent();


	private void PuzzleLibraiesDisplayer_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (e.ClickedItem is not PuzzleLibraryBindableSource { IsAddingOperationPlaceholder: var isPlaceholder })
		{
			return;
		}

		if (isPlaceholder)
		{
			// Create a dialog to display adding operation.

		}
		else
		{
			// TODO: Add a new detail page to display a library of puzzles.
		}
	}
}
