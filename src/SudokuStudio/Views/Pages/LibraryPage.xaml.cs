using System.Collections.ObjectModel;
using System.Text.Json;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using SudokuStudio.BindableSource;
using SudokuStudio.Interaction;
using SudokuStudio.Storage;
using SudokuStudio.Views.Pages.ContentDialogs;
using Windows.Storage.Pickers;
using Windows.System;
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
	internal static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

	/// <summary>
	/// The internal flyout showing options.
	/// </summary>
	private static readonly FlyoutShowOptions FlyoutShowOptions = new() { Placement = FlyoutPlacementMode.Auto };


	/// <summary>
	/// Indicates the puzzle library.
	/// </summary>
	private readonly ObservableCollection<PuzzleLibraryBindableSource> _puzzleLibraries = PuzzleLibraryBindableSource.LocalPuzzleLibraries(true);


	/// <summary>
	/// Initializes a <see cref="LibraryPage"/> instance.
	/// </summary>
	public LibraryPage() => InitializeComponent();


	/// <summary>
	/// Loads a file from local.
	/// </summary>
	/// <returns>A <see cref="bool"/> result indicating whether the operation is success.</returns>
	private async Task<bool> LoadFileAsync()
	{
		if (!App.EnsureUnsnapped())
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
				Content = new LibraryInfoDisplayerContent
				{
					Mode = LibraryDataUpdatingMode.Load,
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

			var content = (LibraryInfoDisplayerContent)contentDialog.Content;
			var instance = new PuzzleLibraryBindableSource
			{
				Name = content.LibraryName ?? "",
				Author = content.LibraryAuthor ?? "",
				Description = content.LibraryDescription ?? "",
				FileId = fileId,
				Tags = [.. content.LibraryTags],
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
			((MenuFlyout)selectedControl.ContextFlyout!).ShowAt(selectedControl, FlyoutShowOptions);
		}
		else
		{
			App.GetMainWindow(this).NavigateToPage<LibraryPuzzleDetailsPage, PuzzleLibraryBindableSource>(clickedSource);
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
			Content = new LibraryInfoDisplayerContent { Mode = LibraryDataUpdatingMode.Add },
			DefaultButton = ContentDialogButton.Primary,
			PrimaryButtonText = GetString("LibraryPage_LoadOrAddingButtonText")
		};
		if (await contentDialog.ShowAsync() == ContentDialogResult.Primary)
		{
			var content = (LibraryInfoDisplayerContent)contentDialog.Content;
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
				Tags = [.. content.LibraryTags],
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

		if (!App.EnsureUnsnapped())
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

		var index = _puzzleLibraries.FindIndex(c => c.FileId == originalInstance.FileId);
		if (index == -1)
		{
			return;
		}

		var newInstance = new PuzzleLibraryBindableSource(originalInstance, [.. puzzles, .. newPuzzles]);
		var resultJson = JsonSerializer.Serialize(newInstance, SerializerOptions);
		await File.WriteAllTextAsync(originalFilePath, resultJson);

		_puzzleLibraries[index] = newInstance;
	}

	private void DeleteLibraryMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
	{
		if (sender is not MenuFlyoutItem { Tag: MenuFlyout { Target: GridViewItem { Content: PuzzleLibraryBindableSource { FilePath: var filePath } source } } })
		{
			return;
		}

		if (!File.Exists(filePath))
		{
			return;
		}

		var index = _puzzleLibraries.FindIndex(c => c.FileId == source.FileId);
		if (index == -1)
		{
			return;
		}

		File.Delete(filePath);
		_puzzleLibraries.RemoveAt(index);
	}

	private void ClearLibraryMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
	{
		if (sender is not MenuFlyoutItem { Tag: MenuFlyout { Target: GridViewItem { Content: PuzzleLibraryBindableSource source } } })
		{
			return;
		}

		var index = _puzzleLibraries.FindIndex(c => c.FileId == source.FileId);
		if (index == -1)
		{
			return;
		}

		_puzzleLibraries[index] = new(_puzzleLibraries[index], []);
	}

	private async void RemoveInvalidPuzzlesMenuFlyoutItem_ClickAsync(object sender, RoutedEventArgs e)
	{
		if (sender is not MenuFlyoutItem { Tag: MenuFlyout { Target: GridViewItem { Content: PuzzleLibraryBindableSource { FilePath: var filePath } source } } })
		{
			return;
		}

		var index = _puzzleLibraries.FindIndex(c => c.FileId == source.FileId);
		if (index == -1)
		{
			return;
		}

		var origianlInstance = _puzzleLibraries[index];
		var newInstance = new PuzzleLibraryBindableSource(origianlInstance, [.. getValidGrids(origianlInstance.Puzzles)]);
		var resultJson = JsonSerializer.Serialize(newInstance, SerializerOptions);
		await File.WriteAllTextAsync(filePath, resultJson);

		_puzzleLibraries[index] = newInstance;


		static Grid[] getValidGrids(Grid[] originalPuzzles)
		{
			var newPuzzles = new List<Grid>(originalPuzzles.Length);
			foreach (ref readonly var puzzle in originalPuzzles.AsReadOnlySpan())
			{
				if (puzzle.IsValid)
				{
					newPuzzles.Add(puzzle);
				}
			}

			return [.. newPuzzles];
		}
	}

	private async void UpdateLibraryInfoMenuFlyoutItem_ClickAsync(object sender, RoutedEventArgs e)
	{
		if (sender is not MenuFlyoutItem
			{
				Tag: MenuFlyout
				{
					Target: GridViewItem
					{
						Content: PuzzleLibraryBindableSource
						{
							Name: var libraryName,
							Author: var libraryAuthor,
							Description: var libraryDescription,
							Tags: var libraryTags,
							FilePath: var filePath,
							FileId: var fileId,
							Puzzles: var puzzles,
							PuzzlesCount: var puzzlesCount
						} source
					}
				}
			})
		{
			return;
		}

		var index = _puzzleLibraries.FindIndex(c => c.FileId == source.FileId);
		if (index == -1)
		{
			return;
		}

		var contentDialog = new ContentDialog
		{
			XamlRoot = XamlRoot,
			IsPrimaryButtonEnabled = true,
			Style = (Style)Application.Current.Resources["DefaultContentDialogStyle"]!,
			CloseButtonText = GetString("LibraryPage_Close"),
			Content = new LibraryInfoDisplayerContent
			{
				Mode = LibraryDataUpdatingMode.Update,
				LibraryName = libraryName,
				LibraryAuthor = libraryAuthor,
				LibraryDescription = libraryDescription,
				LibraryTags = new(libraryTags),
				FileId = fileId
			},
			DefaultButton = ContentDialogButton.Primary,
			PrimaryButtonText = GetString("LibraryPage_LoadOrAddingButtonText")
		};
		if (await contentDialog.ShowAsync() == ContentDialogResult.Primary)
		{
			var content = (LibraryInfoDisplayerContent)contentDialog.Content;
			if (content.FileId is not { Length: not 0 } newFileId)
			{
				ErrorDialog_FileIdCannotBeEmpty.IsOpen = true;
				return;
			}

			var newFilePath = fileId == newFileId ? filePath : $@"{CommonPaths.PuzzleLibrariesFolder}\{newFileId}{FileExtensions.PuzzleLibrary}";
			if (filePath != newFilePath && File.Exists(newFilePath))
			{
				ErrorDialog_FileAlreadyExists.IsOpen = true;
				return;
			}

			var newInstance = new PuzzleLibraryBindableSource
			{
				Name = content.LibraryName ?? "",
				Author = content.LibraryAuthor ?? "",
				FileId = newFileId,
				Description = content.LibraryDescription ?? "",
				Tags = [.. content.LibraryTags],
				Puzzles = puzzles,
				PuzzlesCount = puzzlesCount
			};

			if (fileId != newFileId)
			{
				// Special case: If a user has modified the file ID, the local path would be also changed. We should rename a file.
				File.Move(filePath, newFilePath);
			}

			File.WriteAllText(newFilePath, JsonSerializer.Serialize(newInstance, SerializerOptions));

			_puzzleLibraries.RemoveAt(index);
			_puzzleLibraries.Insert(index, newInstance);
		}
	}

	private async void AppendANewPuzzleMenuFlyoutItem_ClickAsync(object sender, RoutedEventArgs e)
	{
		if (sender is not MenuFlyoutItem
			{
				Tag: MenuFlyout
				{
					Target: GridViewItem
					{
						Content: PuzzleLibraryBindableSource
						{
							FilePath: var filePath,
							FileId: var fileId
						} source
					}
				}
			})
		{
			return;
		}

		var index = _puzzleLibraries.FindIndex(c => c.FileId == source.FileId);
		if (index == -1)
		{
			return;
		}

		var contentDialog = new ContentDialog
		{
			XamlRoot = XamlRoot,
			IsPrimaryButtonEnabled = true,
			Style = (Style)Application.Current.Resources["DefaultContentDialogStyle"]!,
			CloseButtonText = GetString("LibraryPage_Close"),
			Content = new LibraryInfoDisplayerContent { Mode = LibraryDataUpdatingMode.AddOne },
			DefaultButton = ContentDialogButton.Primary,
			PrimaryButtonText = GetString("LibraryPage_LoadOrAddingButtonText")
		};
		if (await contentDialog.ShowAsync() == ContentDialogResult.Primary)
		{
			var content = (LibraryInfoDisplayerContent)contentDialog.Content;
			if (content.AppendingPuzzleTextCode is not (var textCode and not (null or [])))
			{
				return;
			}

			if (!Grid.TryParse(textCode, out var validGrid))
			{
				ErrorDialog_TextCodeIsInvalid.IsOpen = true;
				return;
			}

			var instance = _puzzleLibraries[index];
			var newInstance = new PuzzleLibraryBindableSource(instance, [.. instance.Puzzles, validGrid]);
			File.WriteAllText(filePath, JsonSerializer.Serialize(newInstance, SerializerOptions));

			_puzzleLibraries.RemoveAt(index);
			_puzzleLibraries.Insert(index, newInstance);
		}
	}
}
