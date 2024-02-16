namespace SudokuStudio.Views.Pages;

/// <summary>
/// Represents library page.
/// </summary>
public sealed partial class LibraryPage : Page
{
	/// <summary>
	/// Initializes a <see cref="LibraryPage"/> instance.
	/// </summary>
	public LibraryPage() => InitializeComponent();


	private async void AddOnePuzzleItem_ClickAsync(object sender, RoutedEventArgs e)
	{
		if (sender is not MenuFlyoutItem { Tag: MenuFlyout { Target: GridViewItem { Content: LibraryBindableSource { LibraryInfo: var lib } } } })
		{
			return;
		}

		var dialog = new ContentDialog
		{
			XamlRoot = XamlRoot,
			Title = ResourceDictionary.Get("LibraryPage_AddOnePuzzleDialogTitle", App.CurrentCulture),
			DefaultButton = ContentDialogButton.Primary,
			IsPrimaryButtonEnabled = true,
			PrimaryButtonText = ResourceDictionary.Get("LibraryPage_AddOnePuzzleDialogSure", App.CurrentCulture),
			CloseButtonText = ResourceDictionary.Get("LibraryPage_AddOnePuzzleDialogCancel", App.CurrentCulture),
			Content = new AddOnePuzzleDialogContent()
		};
		if (await dialog.ShowAsync() != ContentDialogResult.Primary)
		{
			return;
		}

		var text = ((AddOnePuzzleDialogContent)dialog.Content).TextCodeInput.Text;
		if (!Grid.TryParse(text, out _))
		{
			return;
		}

		await lib.AppendPuzzleAsync(text);
	}

	private async void AddListItem_ClickAsync(object sender, RoutedEventArgs e)
	{
		if (sender is not MenuFlyoutItem
			{
				Tag: MenuFlyout
				{
					Target: GridViewItem
					{
						Content: LibraryBindableSource
						{
							LibraryInfo: var lib
						} instance
					}
				}
			})
		{
			return;
		}

		var fop = new FileOpenPicker();
		fop.Initialize(this);
		fop.ViewMode = PickerViewMode.Thumbnail;
		fop.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
		fop.AddFileFormat(FileFormats.Text);
		fop.AddFileFormat(FileFormats.PlainText);

		if (await fop.PickSingleFileAsync() is not { Path: var filePath })
		{
			return;
		}

		instance.IsActive = true;

		await lib.AppendPuzzlesAsync(File.ReadLinesAsync(filePath));

		instance.IsActive = false;
	}

	private async void RemoveDuplicatePuzzlesItem_ClickAsync(object sender, RoutedEventArgs e)
	{
		if (sender is not MenuFlyoutItem
			{
				Tag: MenuFlyout
				{
					Target: GridViewItem
					{
						Content: LibraryBindableSource
						{
							LibraryInfo: var lib
						} instance
					}
				}
			})
		{
			return;
		}

		instance.IsActive = true;

		await lib.RemoveDuplicatePuzzlesAsync();

		instance.IsActive = false;
	}

#if false
	private void VisitItem_Click(object sender, RoutedEventArgs e)
	{
	}
#endif

	private void LibrariesDisplayer_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (sender is not GridView { ItemsPanelRoot.Children: var children })
		{
			return;
		}

		foreach (var child in children)
		{
			if (child is not GridViewItem { Content: LibraryBindableSource source, ContextFlyout: var flyout })
			{
				continue;
			}

			if (!ReferenceEquals(source, (LibraryBindableSource)e.ClickedItem))
			{
				continue;
			}

			flyout.ShowAt(child, new() { Placement = FlyoutPlacementMode.Auto });
			break;
		}
	}

	private void ClearPuzzlesItem_Click(object sender, RoutedEventArgs e)
	{
		if (sender is not MenuFlyoutItem { Tag: MenuFlyout { Target: GridViewItem { Content: LibraryBindableSource { LibraryInfo: var lib } } } })
		{
			return;
		}

		lib.ClearPuzzles();
	}

	private void DeleteLibraryItem_Click(object sender, RoutedEventArgs e)
	{
		if (sender is not MenuFlyoutItem { Tag: MenuFlyout { Target: GridViewItem { Content: LibraryBindableSource { LibraryInfo: var lib } } } })
		{
			return;
		}

		lib.Delete();

		var p = (ObservableCollection<LibraryBindableSource>)LibrariesDisplayer.ItemsSource;
		for (var i = 0; i < p.Count; i++)
		{
			var libraryBindableSource = p[i];
			if (libraryBindableSource.LibraryInfo == lib)
			{
				((ObservableCollection<LibraryBindableSource>)LibrariesDisplayer.ItemsSource).RemoveAt(i);
				return;
			}
		}
	}

	private async void PropertiesItem_ClickAsync(object sender, RoutedEventArgs e)
	{
		if (sender is not MenuFlyoutItem { Tag: MenuFlyout { Target: GridViewItem { Content: LibraryBindableSource { LibraryInfo: var lib } } } })
		{
			return;
		}

		var dialog = new ContentDialog
		{
			XamlRoot = XamlRoot,
			Title = ResourceDictionary.Get("LibraryPage_LibraryPropertiesDialogTitle", App.CurrentCulture),
			DefaultButton = ContentDialogButton.Close,
			IsPrimaryButtonEnabled = false,
			CloseButtonText = ResourceDictionary.Get("LibraryPage_LibraryPropertiesDialogClose", App.CurrentCulture),
			Content = new LibraryPropertiesDialogContent
			{
				LibraryName = lib.Name ?? LibraryBindableSource.NameDefaultValue,
				LibraryAuthor = lib.Author ?? LibraryBindableSource.AuthorDefaultValue,
				LibraryDescription = lib.Description ?? LibraryBindableSource.DescriptionDefaultValue,
				LibraryLastModifiedTime = lib.LastModifiedTime,
				LibraryInfo = lib
			}
		};
		await dialog.ShowAsync();
	}

	private async void ModifyPropertiesItem_ClickAsync(object sender, RoutedEventArgs e)
	{
		if (sender is not MenuFlyoutItem { Tag: MenuFlyout { Target: GridViewItem { Content: LibraryBindableSource { LibraryInfo: var lib } } } })
		{
			return;
		}

		var dialog = new ContentDialog
		{
			XamlRoot = XamlRoot,
			Title = ResourceDictionary.Get("LibraryPage_ModifyPropertiesDialogTitle", App.CurrentCulture),
			DefaultButton = ContentDialogButton.Primary,
			IsPrimaryButtonEnabled = true,
			PrimaryButtonText = ResourceDictionary.Get("LibraryPage_ModifyPropertiesDialogSure", App.CurrentCulture),
			CloseButtonText = ResourceDictionary.Get("LibraryPage_ModifyPropertiesDialogCancel", App.CurrentCulture),
			Content = new LibraryModifyPropertiesDialogContent
			{
				LibraryName = lib.Name,
				LibraryAuthor = lib.Author,
				LibraryDescription = lib.Description,
				LibraryTags = [.. lib.Tags ?? []]
			}
		};
		if (await dialog.ShowAsync() != ContentDialogResult.Primary)
		{
			return;
		}

		// Replace with the original dictionary set to refresh UI.
		var content = (LibraryModifyPropertiesDialogContent)dialog.Content;
		var newInstance = new LibraryBindableSource
		{
			FileId = lib.FileId,
			Name = (lib.Name = content.LibraryName is var name and not (null or "") ? name : null) switch
			{
				{ } finalName => finalName,
				_ => LibraryBindableSource.NameDefaultValue
			},
			Author = (lib.Author = content.LibraryAuthor is var author and not (null or "") ? author : null) switch
			{
				{ } finalAuthor => finalAuthor,
				_ => LibraryBindableSource.AuthorDefaultValue
			},
			Description = (lib.Description = content.LibraryDescription is var description and not (null or "") ? description : null) switch
			{
				{ } finalDescription => finalDescription,
				_ => LibraryBindableSource.DescriptionDefaultValue
			},
			Tags = (lib.Tags = content.LibraryTags is { Count: not 0 } tags ? [.. tags] : null) switch
			{
				{ } finalTags => finalTags,
				_ => []
			}
		};

		var p = (ObservableCollection<LibraryBindableSource>)LibrariesDisplayer.ItemsSource;
		for (var i = 0; i < p.Count; i++)
		{
			var libraryBindableSource = p[i];
			if (libraryBindableSource.LibraryInfo == lib)
			{
				((ObservableCollection<LibraryBindableSource>)LibrariesDisplayer.ItemsSource)[i] = newInstance;
				return;
			}
		}
	}

	private async void AddLibraryButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		var dialog = new ContentDialog
		{
			XamlRoot = XamlRoot,
			Title = ResourceDictionary.Get("LibraryPage_AddLibraryDialogTitle", App.CurrentCulture),
			DefaultButton = ContentDialogButton.Primary,
			IsPrimaryButtonEnabled = true,
			PrimaryButtonText = ResourceDictionary.Get("LibraryPage_AddLibraryDialogSure", App.CurrentCulture),
			CloseButtonText = ResourceDictionary.Get("LibraryPage_AddLibraryDialogCancel", App.CurrentCulture),
			Content = new AddLibraryDialogContent()
		};
		if (await dialog.ShowAsync() != ContentDialogResult.Primary)
		{
			return;
		}

		// Update UI.
		var content = (AddLibraryDialogContent)dialog.Content;
		if (!content.IsNameValidAsFileId)
		{
			return;
		}

		var libraryCreated = new Library(CommonPaths.Library, content.FileId);
		libraryCreated.Initialize();

		((ObservableCollection<LibraryBindableSource>)LibrariesDisplayer.ItemsSource).Add(
			new()
			{
				FileId = libraryCreated.FileId,
				Name = (libraryCreated.Name = content.LibraryName is var name and not (null or "") ? name : null) switch
				{
					{ } finalName => finalName,
					_ => LibraryBindableSource.NameDefaultValue
				},
				Author = (libraryCreated.Author = content.LibraryAuthor is var author and not (null or "") ? author : null) switch
				{
					{ } finalAuthor => finalAuthor,
					_ => LibraryBindableSource.AuthorDefaultValue
				},
				Description = (libraryCreated.Description = content.LibraryDescription is var description and not (null or "") ? description : null) switch
				{
					{ } finalDescription => finalDescription,
					_ => LibraryBindableSource.DescriptionDefaultValue
				},
				Tags = (libraryCreated.Tags = content.LibraryTags is { Count: not 0 } tags ? [.. tags] : null) switch
				{
					{ } finalTags => finalTags,
					_ => []
				}
			}
		);
	}

	private async void LoadLibraryFileButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		var fop = new FileOpenPicker();
		fop.Initialize(this);
		fop.ViewMode = PickerViewMode.Thumbnail;
		fop.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
		fop.AddFileFormat(FileFormats.Text);
		fop.AddFileFormat(FileFormats.PlainText);

		if (await fop.PickSingleFileAsync() is not { Path: var filePath })
		{
			return;
		}

		var fileName = io::Path.GetFileNameWithoutExtension(filePath);
		var lib = new Library(CommonPaths.Library, fileName);
		lib.Initialize();
		lib.Name = fileName;

		var source = new LibraryBindableSource { IsActive = true, FileId = lib.FileId };
		((ObservableCollection<LibraryBindableSource>)LibrariesDisplayer.ItemsSource).Add(source);
		((App)Application.Current).Libraries.Add(lib);

		await lib.AppendPuzzlesAsync(File.ReadLinesAsync(filePath));

		source.IsActive = false;
	}
}
