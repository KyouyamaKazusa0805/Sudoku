using System.Text.Json;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Navigation;
using SudokuStudio.BindableSource;
using SudokuStudio.ComponentModel;
using SudokuStudio.Interaction.Conversions;
using SudokuStudio.Views.Controls;

namespace SudokuStudio.Views.Pages;

/// <summary>
/// Represents with a library puzzle details page.
/// </summary>
[DependencyProperty<Offset>("CurrentPuzzleIndex", DefaultValue = -1, DocSummary = "Indicates the current page to be shown.")]
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
		if ((d, e) is (
			LibraryPuzzleDetailsPage { Source.Puzzles.Length: var count } currentPage, { OldValue: Offset oldValue, NewValue: Offset value }
		) && (value < 0 || value >= count))
		{
			currentPage.CurrentPuzzleIndex = oldValue;
		}
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
			App.GetMainWindow(this).NavigateToPage<AnalyzePage, Grid>(z);
		}
	}

	private void GoToPreviousPuzzleButton_Click(object sender, RoutedEventArgs e) => CurrentPuzzleIndex--;

	private void GotoNextPuzzleButton_Click(object sender, RoutedEventArgs e) => CurrentPuzzleIndex++;

	private async void PuzzleRemoveButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		if (sender is not Button { Parent: StackPanel { Parent: FlyoutPresenter { Parent: Popup f } } })
		{
			return;
		}

		f.IsOpen = false;

		if (Source is null)
		{
			return;
		}

		var newInstance = new PuzzleLibraryBindableSource(
			Source,
			[.. Source.Puzzles[..CurrentPuzzleIndex], .. Source.Puzzles[(CurrentPuzzleIndex + 1)..]]
		);

		var json = JsonSerializer.Serialize(newInstance, LibraryPage.SerializerOptions);
		await File.WriteAllTextAsync(newInstance.FilePath, json);

		Source = newInstance;
		CurrentPuzzleIndexBox.Value = CurrentPuzzleIndex = Math.Min(CurrentPuzzleIndex, newInstance.PuzzlesCount - 1);
	}
}
