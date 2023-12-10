using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using SudokuStudio.BindableSource;
using SudokuStudio.ComponentModel;

namespace SudokuStudio.Views.Pages.ContentDialogs;

/// <summary>
/// Represents with a dialog content "add a puzzle to the library" from analyze page.
/// </summary>
[DependencyProperty<string>("LibraryName?", DocSummary = "Indicates the library name configured.")]
[DependencyProperty<string>("FileId?", DocSummary = "Indicates the file ID configured.")]
[DependencyProperty<string>("LibraryAuthor?", DocSummary = "Indicates the library author configured.")]
[DependencyProperty<string>("LibraryDescription?", DocSummary = "Indicates the library description configured.")]
[DependencyProperty<PuzzleLibraryBindableSource>("SelectedLibrary?", DocSummary = "Indicates the currently-selected library.")]
[DependencyProperty<ObservableCollection<string>>("LibraryTags", DocSummary = "Indicates the library description configured.")]
public sealed partial class AddToLibraryContent : Page
{
	[Default]
	private static readonly ObservableCollection<string> LibraryTagsDefaultValue = [];


	/// <summary>
	/// Initializes an <see cref="AddToLibraryContent"/> instance.
	/// </summary>
	public AddToLibraryContent() => InitializeComponent();


	/// <summary>
	/// The property that can assign the internal field with the initialized value.
	/// </summary>
	[DisallowNull]
	internal ObservableCollection<PuzzleLibraryBindableSource>? PuzzleLibraries { get; set; } = PuzzleLibraryBindableSource.LocalPuzzleLibraries(false);


	private async void ApplyButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		if (sender is not Button
			{
				Parent: StackPanel
				{
					Parent: Border
					{
						Parent: GridLayout
						{
							Parent: GridLayout
							{
								Parent: GridLayout
								{
									Parent: Popup f
								}
							}
						}
					}
				}
			})
		{
			return;
		}

		f.IsOpen = false;

		if (FileId is null or [])
		{
			ErrorDialog_FileIdCannotBeEmpty.IsOpen = true;
			return;
		}

		var instance = new PuzzleLibraryBindableSource
		{
			Name = LibraryName ?? "",
			Author = LibraryAuthor ?? "",
			Description = LibraryDescription ?? "",
			FileId = FileId,
			Tags = [.. LibraryTags],
			Puzzles = [],
			PuzzlesCount = 0
		};

		if (File.Exists(instance.FilePath))
		{
			ErrorDialog_FileAlreadyExists.IsOpen = true;
			return;
		}

		PuzzleLibraries!.Add(instance);

		var json = JsonSerializer.Serialize(instance, LibraryPage.SerializerOptions);
		await File.WriteAllTextAsync(instance.FilePath, json);
	}

	private void AddNewLibraryButton_Click(object sender, RoutedEventArgs e)
	{
		AddNewLibraryDialog.IsOpen = false; // A little bug fix.
		AddNewLibraryDialog.IsOpen = true;
	}

	private void AddNewLibraryDialog_CloseButtonClick(TeachingTip sender, object args)
	{
		AddNewLibraryDialog.IsOpen = true; // A little bug fix.
		AddNewLibraryDialog.IsOpen = false;
	}
}
