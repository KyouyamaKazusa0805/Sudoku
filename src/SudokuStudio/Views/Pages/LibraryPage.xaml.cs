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

		var fileId = Path.GetFileNameWithoutExtension(filePath);
		var libFilePath = $@"{CommonPaths.PuzzleLibrariesFolder}\{fileId}{FileExtensions.PuzzleLibrary}";
		if (File.Exists(libFilePath))
		{
			ErrorDialog_FileAlreadyExists.IsOpen = true;
			return false;
		}

		var fileExtension = Path.GetExtension(filePath);
		if (fileExtension == FileExtensions.PuzzleLibrary)
		{
			var content = await File.ReadAllTextAsync(filePath);
			PuzzleLibraryBindableSource instance;
			try
			{
				instance = JsonSerializer.Deserialize<PuzzleLibraryBindableSource>(content, SerializerOptions)
					?? throw new JsonException("Invalid value.");
			}
			catch (JsonException)
			{
				ErrorDialog_FileIsInvalid.IsOpen = true;
				return false;
			}

			File.WriteAllText(libFilePath, JsonSerializer.Serialize(instance, SerializerOptions));

			_puzzleLibraries.Insert(^1, instance);
			return true;
		}
		else
		{
			var contentDialog = new ContentDialog
			{
				XamlRoot = XamlRoot,
				IsPrimaryButtonEnabled = true,
				Style = (Style)Application.Current.Resources["DefaultContentDialogStyle"]!,
				CloseButtonText = GetString("LibraryPage_Close"),
				Content = new LibraryLoadOrAddContent
				{
					IsAdding = false,
					FilePath = libFilePath,
					FileId = fileId,
					LibraryName = fileId,
					LibraryDescription = string.Format(GetString("LibraryPage_DescriptionDefaultValue"), fileId)
				},
				DefaultButton = ContentDialogButton.Primary,
				PrimaryButtonText = GetString("LibraryPage_LoadOrAddingButtonText")
			};
			var result = await contentDialog.ShowAsync();
			if (result != ContentDialogResult.Primary)
			{
				return false;
			}

			var validPuzzles = new List<Grid>();
			await foreach (var line in File.ReadLinesAsync(filePath))
			{
				if (fileExtension is FileExtensions.PlainText or FileExtensions.CommaSeparated && Grid.TryParse(line, out var grid))
				{
					validPuzzles.Add(grid);
				}
			}

			var content = (LibraryLoadOrAddContent)contentDialog.Content;
			var instance = new PuzzleLibraryBindableSource
			{
				Name = content.LibraryName ?? "",
				Author = content.LibraryAuthor ?? "",
				Description = content.LibraryDescription ?? "",
				FileId = fileId,
				Tags = content.LibraryTags ?? [],
				Puzzles = [.. validPuzzles],
				PuzzlesCount = validPuzzles.Count
			};

			File.WriteAllText(libFilePath, JsonSerializer.Serialize(instance, SerializerOptions));

			_puzzleLibraries.Insert(^1, instance);
			return true;
		}
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
			Content = new LibraryLoadOrAddContent { IsAdding = true },
			DefaultButton = ContentDialogButton.Primary,
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

	private async void AppendPuzzlesMenuFlyoutItem_ClickAsync(object sender, RoutedEventArgs e)
	{
		if (sender is not MenuFlyoutItem
			{
				Tag: MenuFlyout
				{
					Target: GridViewItem
					{
						Content: PuzzleLibraryBindableSource { FilePath: var originalFilePath, Puzzles: var puzzles }
					}
				}
			})
		{
			return;
		}

		if (!EnsureUnsnapped())
		{
			return;
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
			return;
		}

		var newPuzzles = default(List<Grid>);
		switch (Path.GetExtension(filePath))
		{
			case FileExtensions.PlainText or FileExtensions.CommaSeparated:
			{
				newPuzzles = [];
				await foreach (var line in File.ReadLinesAsync(filePath))
				{
					if (Grid.TryParse(line, out var puzzle))
					{
						newPuzzles.Add(puzzle);
					}
				}

				break;
			}
			case FileExtensions.PuzzleLibrary:
			{
				var json = await File.ReadAllTextAsync(filePath);
				var instance = JsonSerializer.Deserialize<PuzzleLibraryBindableSource>(json, SerializerOptions);
				if (instance is not { Puzzles: { Length: var newPuzzlesCount } tempPuzzles })
				{
					ErrorDialog_FileIsInvalid.IsOpen = true;
					return;
				}

				newPuzzles = new(tempPuzzles);
				break;
			}
		}
		if (newPuzzles is null)
		{
			return;
		}

		var originalJson = await File.ReadAllTextAsync(originalFilePath);
		var originalInstance = JsonSerializer.Deserialize<PuzzleLibraryBindableSource>(originalJson, SerializerOptions)!;

		var index = -1;
		for (var i = 0; i < _puzzleLibraries.Count; i++)
		{
			if (_puzzleLibraries[i].FileId == originalInstance.FileId)
			{
				index = i;
				break;
			}
		}
		if (index == -1)
		{
			return;
		}

		var newInstance = new PuzzleLibraryBindableSource(originalInstance, [.. puzzles, .. newPuzzles]);
		var resultJson = JsonSerializer.Serialize(newInstance, SerializerOptions);
		await File.WriteAllTextAsync(originalFilePath, resultJson);

		_puzzleLibraries[index] = newInstance;
	}
}
