using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SudokuStudio.BindableSource;
using SudokuStudio.Storage;
using Windows.System;

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
		if ((sender, e.ClickedItem) is not (
			GridView { ItemsPanelRoot.Children: var items },
			PuzzleLibraryBindableSource { IsAddingOperationPlaceholder: var isPlaceholder } clickedSource
		))
		{
			return;
		}

		if (isPlaceholder)
		{
			var selectedControl = (GridViewItem)items.First(item => ReferenceEquals(((ContentControl)item).Content, clickedSource));
			((MenuFlyout)selectedControl.ContextFlyout!).ShowAt(selectedControl);
		}
		else
		{
			// TODO: Add a new detail page to display a library of puzzles.
		}
	}

	private async void OpenLibraryFolderButton_ClickAsync(object sender, RoutedEventArgs e)
		=> await Launcher.LaunchFolderPathAsync(CommonPaths.PuzzleLibrariesFolder);
}
