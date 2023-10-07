using System.Collections.ObjectModel;
using System.Text.Json;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using SudokuStudio.BindableSource;
using SudokuStudio.Storage;
using SudokuStudio.Views.Pages.ContentDialogs;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.ViewManagement;
using static SudokuStudio.Strings.StringsAccessor;

namespace SudokuStudio.Views.Pages;

/// <summary>
/// Represents for a library page.
/// </summary>
public sealed partial class LibraryPage : Page
{
	/// <summary>
	/// The internal serialization options.
	/// </summary>
	private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };



	/// <summary>
	/// Indicates the puzzle library.
	/// </summary>
	private readonly ObservableCollection<PuzzleLibraryBindableSource> _puzzleLibraries = PuzzleLibraryBindableSource.LocalPuzzleLibraries;


	/// <summary>
	/// Initializes a <see cref="LibraryPage"/> instance.
	/// </summary>
	public LibraryPage() => InitializeComponent();


	/// <summary>
	/// To determine whether the current application view is in an unsnapped state.
	/// </summary>
	/// <returns>The <see cref="bool"/> value indicating that.</returns>
	private bool EnsureUnsnapped()
	{
		// 'FileOpenPicker' APIs will not work if the application is in a snapped state.
		// If an app wants to show a 'FileOpenPicker' while snapped, it must attempt to unsnap first.
		var unsnapped = ApplicationView.Value != ApplicationViewState.Snapped || ApplicationView.TryUnsnap();
		if (!unsnapped)
		{
			throw new InvalidOperationException("Ensure the file should be unsnapped.");
		}

		return unsnapped;
	}

	/// <summary>
	/// Loads a file from local.
	/// </summary>
	/// <returns>A <see cref="bool"/> result indicating whether the operation is success.</returns>
	private async Task<bool> LoadFileAsync()
	{
		if (!EnsureUnsnapped())
		{
			return false;
		}

		var fop = new FileOpenPicker();
		fop.Initialize(this);
		fop.ViewMode = PickerViewMode.Thumbnail;
		fop.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
		fop.AddFileFormat(FileFormats.PlainText);
		fop.AddFileFormat(FileFormats.CommaSeparated);
		fop.AddFileFormat(FileFormats.PuzzleLibrary);

		if (await fop.PickSingleFileAsync() is not { Path: var filePath })
		{
			return false;
		}

		var result = await new ContentDialog
		{
			XamlRoot = XamlRoot,
			IsPrimaryButtonEnabled = true,
			Style = (Style)Application.Current.Resources["DefaultContentDialogStyle"]!,
			CloseButtonText = GetString("AnalyzePage_ErrorStepDialogCloseButtonText"),
			DefaultButton = ContentDialogButton.Close,
			Content = new LibraryLoadOrAddContent { IsAdding = false, FilePath = filePath }
		}.ShowAsync();
		if (result != ContentDialogResult.Primary)
		{
			return false;
		}

		// TODO: Fill logic here
		switch (Path.GetExtension(filePath))
		{
			case FileExtensions.PlainText:
			{
				break;
			}
			case FileExtensions.CommaSeparated:
			{
				break;
			}
			case FileExtensions.PuzzleLibrary:
			{
				break;
			}
		}

		return false;
	}


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
			((MenuFlyout)selectedControl.ContextFlyout!).ShowAt(selectedControl, new FlyoutShowOptions { Placement = FlyoutPlacementMode.Auto });
		}
		else
		{
			// TODO: Add a new detail page to display a library of puzzles.
		}
	}

	private async void OpenLibraryFolderButton_ClickAsync(object sender, RoutedEventArgs e)
		=> await Launcher.LaunchFolderPathAsync(CommonPaths.PuzzleLibrariesFolder);

	private async void AddLibraryMenuFlyoutItem_ClickAsync(object sender, RoutedEventArgs e)
	{
		var contentDialog = new ContentDialog
		{
			XamlRoot = XamlRoot,
			IsPrimaryButtonEnabled = true,
			Style = (Style)Application.Current.Resources["DefaultContentDialogStyle"]!,
			CloseButtonText = GetString("LibraryPage_Close"),
			DefaultButton = ContentDialogButton.Primary,
			Content = new LibraryLoadOrAddContent { IsAdding = true },
			PrimaryButtonText = GetString("LibraryPage_LoadOrAddingButtonText")
		};
		if (await contentDialog.ShowAsync() == ContentDialogResult.Primary)
		{
			var content = (LibraryLoadOrAddContent)contentDialog.Content;
			if (content.FileId is not { Length: not 0 } fileId)
			{
				ErrorDialog_FileIdCannotBeEmpty.IsOpen = true;
				return;
			}

			var filePath = $@"{CommonPaths.PuzzleLibrariesFolder}\{fileId}{FileExtensions.PuzzleLibrary}";
			if (File.Exists(filePath))
			{
				ErrorDialog_FileAlreadyExists.IsOpen = true;
				return;
			}

			var newInstance = new PuzzleLibraryBindableSource
			{
				Name = content.LibraryName ?? "",
				Author = content.LibraryAuthor ?? "",
				FileId = fileId,
				Description = content.LibraryDescription ?? "",
				Tags = content.LibraryTags ?? [],
				Puzzles = [],
				PuzzlesCount = 0
			};

			File.WriteAllText(filePath, JsonSerializer.Serialize(newInstance, SerializerOptions));

			_puzzleLibraries.Insert(^1, newInstance);
		}
	}

	private async void LoadFromTextMenuFlyoutItem_ClickAsync(object sender, RoutedEventArgs e) => await LoadFileAsync();

	private async void LoadFromCsvMenuFlyoutItem_ClickAsync(object sender, RoutedEventArgs e) => await LoadFileAsync();
}
