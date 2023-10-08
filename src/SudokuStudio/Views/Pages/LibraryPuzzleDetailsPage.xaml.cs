using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SudokuStudio.BindableSource;
using SudokuStudio.ComponentModel;
using SudokuStudio.Interaction.Conversions;
using SudokuStudio.Views.Controls;
using SudokuStudio.Views.Windows;

namespace SudokuStudio.Views.Pages;

/// <summary>
/// Represents with a library puzzle details page.
/// </summary>
[DependencyProperty<int>("CurrentPuzzleIndex", DefaultValue = -1, DocSummary = "Indicates the current page to be shown.")]
[DependencyProperty<PuzzleLibraryBindableSource>("Source", IsNullable = true, DocSummary = "Indicates the source of the file.")]
public sealed partial class LibraryPuzzleDetailsPage : Page
{
	/// <summary>
	/// Initializes a <see cref="LibraryPuzzleDetailsPage"/> instance.
	/// </summary>
	public LibraryPuzzleDetailsPage() => InitializeComponent();


	/// <inheritdoc/>
	protected override void OnNavigatedTo(NavigationEventArgs e)
	{
		if (e.Parameter is PuzzleLibraryBindableSource source)
		{
			Source = source;
		}
	}

	/// <summary>
	/// Gets the main window.
	/// </summary>
	/// <returns>The main window.</returns>
	/// <exception cref="InvalidOperationException">Throws when the base window cannot be found.</exception>
	private MainWindow GetMainWindow()
		=> ((App)Application.Current).WindowManager.GetWindowForElement(this) switch
		{
			MainWindow mainWindow => mainWindow,
			_ => throw new InvalidOperationException("Main window cannot be found.")
		};


	[Callback]
	private static void SourcePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is LibraryPuzzleDetailsPage currentPage)
		{
			currentPage.CurrentPuzzleIndex = 0;
		}
	}

	[Callback]
	private static void CurrentPuzzleIndexPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is not (LibraryPuzzleDetailsPage { Source.Puzzles: var allPuzzles } currentPage, { OldValue: int originalValue, NewValue: int value }))
		{
			return;
		}

		if (value < 0 || value >= allPuzzles.Length)
		{
			currentPage.CurrentPuzzleIndex = originalValue;
			return;
		}

		currentPage.CurrentPuzzleIndex = value;
	}


	private void ApplyButton_Click(object sender, RoutedEventArgs e)
	{
		if (Source is null)
		{
			goto RevertValue;
		}

		var puzzleIndex = CurrentPuzzleIndexBox.Value - 1;
		if (puzzleIndex == CurrentPuzzleIndex)
		{
			return;
		}

		if (puzzleIndex < 0 || puzzleIndex >= PuzzleLibraryConversion.GetTotalPagesCount(Source))
		{
			goto RevertValue;
		}

		CurrentPuzzleIndex = puzzleIndex;
		return;

	RevertValue:
		CurrentPuzzleIndexBox.Value = CurrentPuzzleIndex + 1;
	}

	private void RedirectButton_Click(object sender, RoutedEventArgs e)
	{
		if (sender is Button { Content: SudokuPane { Puzzle: var z } })
		{
			GetMainWindow().NavigateToPage<AnalyzePage, Grid>(z);
		}
	}
}
