namespace SudokuStudio.Views.Pages;

/// <summary>
/// Defines a new page that stores a set of controls to analyze a sudoku grid.
/// </summary>
[DependencyProperty<bool>("IsAnalyzerLaunched", Accessibility = GeneralizedAccessibility.Internal, DocSummary = "Indicates whether the analyzer is launched.")]
[DependencyProperty<bool>("IsGathererLaunched", Accessibility = GeneralizedAccessibility.Internal, DocSummary = "Indicates whether the gatherer is launched.")]
[DependencyProperty<bool>("GeneratorIsNotRunning", DefaultValue = true, Accessibility = GeneralizedAccessibility.Internal, DocSummary = "Indicates whether the generator is not running currently.")]
[DependencyProperty<int>("CurrentViewIndex", DefaultValue = -1, Accessibility = GeneralizedAccessibility.Internal, DocSummary = "Indicates the current index of the view of property <see cref=\"VisualUnit.Views\"/> displayed.")]
[DependencyProperty<double>("ProgressPercent", Accessibility = GeneralizedAccessibility.Internal, DocSummary = "Indicates the progress percent value.")]
[DependencyProperty<LogicalSolverResult>("AnalysisResultCache", IsNullable = true, Accessibility = GeneralizedAccessibility.Internal, DocSummary = "Indicates the anlaysis result cache.")]
[DependencyProperty<VisualUnit>("VisualUnit", IsNullable = true, Accessibility = GeneralizedAccessibility.Internal, DocSummary = "Indicates the visual unit.")]
public sealed partial class AnalyzePage : Page
{
	/// <summary>
	/// Defines a key-value pair of functions that is used for routing hotkeys.
	/// </summary>
	private Dictionary<Hotkey, Action> _hotkeyFunctions;

	/// <summary>
	/// The navigating data.
	/// </summary>
	private Dictionary<Predicate<NavigationViewItemBase>, Type> _navigatingData;


	/// <summary>
	/// Initializes an <see cref="AnalyzePage"/> instance.
	/// </summary>
	public AnalyzePage()
	{
		InitializeComponent();
		InitializeFields();
		LoadInitialGrid();
	}


	/// <summary>
	/// Provides with an event that is triggered when failed to open a file.
	/// </summary>
	public event OpenFileFailedEventHandler? OpenFileFailed;

	/// <summary>
	/// Provides with an event that is triggered when failed to save a file.
	/// </summary>
	public event SaveFileFailedEventHandler? SaveFileFailed;


	/// <summary>
	/// To clear all possible data from the analyze tabs.
	/// </summary>
	internal void ClearAnalyzeTabsData()
	{
		foreach (var tabPage in AnalyzeTabs.TabItems.OfType<TabViewItem>())
		{
			if (tabPage is { Content: IAnalyzeTabPage subTabPage })
			{
				subTabPage.AnalysisResult = null;
			}
		}
	}

	/// <summary>
	/// To update values via the specified <see cref="LogicalSolverResult"/> instance.
	/// </summary>
	/// <param name="analysisResult">The analysis result instance.</param>
	/// <seealso cref="LogicalSolverResult"/>
	internal void UpdateAnalysisResult(LogicalSolverResult analysisResult)
	{
		foreach (var tabPage in AnalyzeTabs.TabItems.OfType<TabViewItem>())
		{
			if (tabPage is { Content: IAnalyzeTabPage subTabPage })
			{
				subTabPage.AnalysisResult = analysisResult;
			}
		}
	}

	/// <summary>
	/// Copies for sudoku puzzle text.
	/// </summary>
	internal void CopySudokuGridText()
	{
		if (!IsSudokuPaneFocused())
		{
			return;
		}

		if (SudokuPane.Puzzle is var puzzle and ({ IsUndefined: true } or { IsEmpty: true }))
		{
			return;
		}

		var dataPackage = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
		dataPackage.SetText(puzzle.ToString(SusserFormat.Full));

		Clipboard.SetContent(dataPackage);
	}

	/// <summary>
	/// To determine whether the current application view is in an unsnapped state.
	/// </summary>
	/// <returns>The <see cref="bool"/> value indicating that.</returns>
	internal bool EnsureUnsnapped(bool isFileSaving)
	{
		/**
			<see cref="FileOpenPicker"/> APIs will not work if the application is in a snapped state.
			If an app wants to show a <see cref="FileOpenPicker"/> while snapped, it must attempt to unsnap first.
		*/
		var unsnapped = ApplicationView.Value != ApplicationViewState.Snapped || ApplicationView.TryUnsnap();
		if (!unsnapped)
		{
			if (isFileSaving)
			{
				SaveFileFailed?.Invoke(this, new(SaveFileFailedReason.UnsnappingFailed));
			}
			else
			{
				OpenFileFailed?.Invoke(this, new(OpenFileFailedReason.UnsnappingFailed));
			}
		}

		return unsnapped;
	}

	/// <summary>
	/// Copy the snapshot of the sudoku grid control, to the clipboard.
	/// </summary>
	/// <returns>
	/// The typical <see langword="await"/>able instance that holds the task to copy the snapshot.
	/// </returns>
	/// <remarks>
	/// The code is referenced from
	/// <see href="https://github.com/microsoftarchive/msdn-code-gallery-microsoft/blob/21cb9b6bc0da3b234c5854ecac449cb3bd261f29/Official%20Windows%20Platform%20Sample/XAML%20render%20to%20bitmap%20sample/%5BC%23%5D-XAML%20render%20to%20bitmap%20sample/C%23/Scenario2.xaml.cs#L120">here</see>
	/// and
	/// <see href="https://github.com/microsoftarchive/msdn-code-gallery-microsoft/blob/21cb9b6bc0da3b234c5854ecac449cb3bd261f29/Official%20Windows%20Platform%20Sample/XAML%20render%20to%20bitmap%20sample/%5BC%23%5D-XAML%20render%20to%20bitmap%20sample/C%23/Scenario2.xaml.cs#L182">here</see>.
	/// </remarks>
	internal async Task CopySudokuGridControlAsSnapshotAsync()
	{
		if (!IsSudokuPaneFocused())
		{
			return;
		}

		// Creates the stream to store the output image data.
		var stream = new InMemoryRandomAccessStream();

		// Gets the snapshot of the control.
		await SudokuPane.RenderToAsync(stream);

		// Copies the data to the data package.
		var dataPackage = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
		var streamRef = RandomAccessStreamReference.CreateFromStream(stream);
		dataPackage.SetBitmap(streamRef);

		// Copies to the clipboard.
		Clipboard.SetContent(dataPackage);
	}

	/// <summary>
	/// Pastes the text, to the clipboard.
	/// </summary>
	/// <returns>
	/// The typical <see langword="await"/>able instance that holds the task to paste the text.
	/// </returns>
	internal async Task PasteCodeToSudokuGridAsync()
	{
		if (!IsSudokuPaneFocused())
		{
			return;
		}

		var dataPackageView = Clipboard.GetContent();
		if (!dataPackageView.Contains(StandardDataFormats.Text))
		{
			return;
		}

		var gridStr = await dataPackageView.GetTextAsync();
		if (!Grid.TryParse(gridStr, out var grid))
		{
			return;
		}

		SudokuPane.Puzzle = grid;
	}

	/// <summary>
	/// Open a file.
	/// </summary>
	/// <returns>A task that handles the operation.</returns>
	internal async Task OpenFileInternalAsync()
	{
		if (!EnsureUnsnapped(false))
		{
			return;
		}

		var fop = new FileOpenPicker();

		var window = ((App)Application.Current).WindowManager.GetWindowForElement(this);
		var hWnd = WindowNative.GetWindowHandle(window);
		InitializeWithWindow.Initialize(fop, hWnd);

		fop.ViewMode = PickerViewMode.Thumbnail;
		fop.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
		fop.FileTypeFilter.Add(CommonFileExtensions.Text);
		fop.FileTypeFilter.Add(CommonFileExtensions.PlainText);

		if (await fop.PickSingleFileAsync() is not { } file)
		{
			return;
		}

		await LoadPuzzleCoreAsync(file);
	}

	/// <summary>
	/// Save a file.
	/// </summary>
	/// <returns>A task that handles the operation.</returns>
	internal async Task SaveFileInternalAsync()
	{
		if (!EnsureUnsnapped(true))
		{
			return;
		}

		var fsp = new FileSavePicker();

		var window = ((App)Application.Current).WindowManager.GetWindowForElement(this);
		var hWnd = WindowNative.GetWindowHandle(window);
		InitializeWithWindow.Initialize(fsp, hWnd);

		fsp.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
		fsp.SuggestedFileName = GetString("Sudoku");
		fsp.FileTypeChoices.Add(GetString("FileExtension_TextDescription"), new[] { CommonFileExtensions.Text });
		fsp.FileTypeChoices.Add(GetString("FileExtension_PlainTextDescription"), new[] { CommonFileExtensions.PlainText });
		fsp.FileTypeChoices.Add(GetString("FileExtension_Picture"), new[] { CommonFileExtensions.PortablePicture });

		if (await fsp.PickSaveFileAsync() is not { Path: var filePath } file)
		{
			return;
		}

		switch (SystemPath.GetExtension(filePath))
		{
			case CommonFileExtensions.PlainText:
			{
				SudokuPlainTextFileHandler.Write(filePath, SudokuPane.Puzzle);
				break;
			}
			case CommonFileExtensions.Text:
			{
				SudokuFileHandler.Write(
					filePath,
					new GridSerializationData[] { new() { GridString = SusserFormat.Full.ToString(SudokuPane.Puzzle) } }
				);
				break;
			}
			case CommonFileExtensions.PortablePicture:
			{
				await SudokuPane.RenderToAsync(file);
				break;
			}
		}
	}

	/// <summary>
	/// Save a file via grid formatters.
	/// </summary>
	/// <param name="gridFormatters">The grid formatters.</param>
	/// <returns>A task that handles the operation.</returns>
	internal async Task<bool> SaveFileInternalAsync(ArrayList gridFormatters)
	{
		if (!EnsureUnsnapped(true))
		{
			return false;
		}

		var fsp = new FileSavePicker();

		var window = ((App)Application.Current).WindowManager.GetWindowForElement(this);
		var hWnd = WindowNative.GetWindowHandle(window);
		InitializeWithWindow.Initialize(fsp, hWnd);

		fsp.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
		fsp.SuggestedFileName = GetString("Sudoku");
		fsp.FileTypeChoices.Add(GetString("FileExtension_TextDescription"), new[] { CommonFileExtensions.Text });
		fsp.FileTypeChoices.Add(GetString("FileExtension_PlainTextDescription"), new[] { CommonFileExtensions.PlainText });
		fsp.FileTypeChoices.Add(GetString("FileExtension_Picture"), new[] { CommonFileExtensions.PortablePicture });

		if (await fsp.PickSaveFileAsync() is not { Path: var filePath } file)
		{
			return false;
		}

		var grid = SudokuPane.Puzzle;
		switch (SystemPath.GetExtension(filePath))
		{
			case CommonFileExtensions.PlainText:
			{
				await File.WriteAllTextAsync(
					filePath,
					string.Join("\r\n\r\n", from gridFormatter in gridFormatters select ((IGridFormatter)gridFormatter).ToString(grid))
				);

				break;
			}
			case CommonFileExtensions.Text:
			{
				SudokuFileHandler.Write(
					filePath,
					(
						from gridFormatter in gridFormatters
						select ((IGridFormatter)gridFormatter).ToString(grid) into gridString
						select new GridSerializationData { GridString = gridString }
					).ToArray()
				);

				break;
			}
			case CommonFileExtensions.PortablePicture:
			{
				await SudokuPane.RenderToAsync(file);
				break;
			}
		}

		return true;
	}

	/// <summary>
	/// The internal logic to load a puzzle. This operation can be called after dropping files
	/// or picking files from <see cref="FileOpenPicker"/>.
	/// </summary>
	/// <param name="file">The <see cref="StorageFile"/> instance.</param>
	/// <returns>The task instance that handles this operation.</returns>
	/// <seealso cref="FileOpenPicker"/>
	internal async Task LoadPuzzleCoreAsync(StorageFile? file)
	{
		if (file is null)
		{
			return;
		}

		var filePath = file.Path;
		switch (new FileInfo(filePath).Length)
		{
			case 0:
			{
				OpenFileFailed?.Invoke(this, new(OpenFileFailedReason.FileIsEmpty));
				return;
			}
			case > 1024:
			{
				OpenFileFailed?.Invoke(this, new(OpenFileFailedReason.FileIsTooLarge));
				return;
			}
			default:
			{
				switch (SystemPath.GetExtension(filePath))
				{
					case CommonFileExtensions.PlainText:
					{
						var content = await FileIO.ReadTextAsync(file);
						if (string.IsNullOrWhiteSpace(content))
						{
							OpenFileFailed?.Invoke(this, new(OpenFileFailedReason.FileIsEmpty));
							return;
						}

						if (!Grid.TryParse(content, out var g))
						{
							OpenFileFailed?.Invoke(this, new(OpenFileFailedReason.FileCannotBeParsed));
							return;
						}

						SudokuPane.Puzzle = g;
						break;
					}
					case CommonFileExtensions.Text:
					{
						switch (SudokuFileHandler.Read(filePath))
						{
							case [{ GridString: var str }]:
							{
								if (!Grid.TryParse(str, out var g))
								{
									OpenFileFailed?.Invoke(this, new(OpenFileFailedReason.FileCannotBeParsed));
									return;
								}

								SudokuPane.Puzzle = g;
								break;
							}
							default:
							{
								OpenFileFailed?.Invoke(this, new(OpenFileFailedReason.FileCannotBeParsed));
								return;
							}
						}

						break;
					}
				}

				break;
			}
		}
	}

	/// <inheritdoc/>
	protected override void OnKeyDown(KeyRoutedEventArgs e)
	{
		base.OnKeyDown(e);

		// This method routes the hotkeys.
		var modifierStatus = Keyboard.GetModifierStatusForCurrentThread();
		foreach (var ((modifiers, key), action) in _hotkeyFunctions)
		{
			if (modifierStatus == modifiers && e.Key == key)
			{
				action();
				break;
			}
		}

		e.Handled = false;
	}

	/// <summary>
	/// Try to initialize field <see cref="_hotkeyFunctions"/> and <see cref="_navigatingData"/>.
	/// </summary>
	/// <seealso cref="_hotkeyFunctions"/>
	/// <seealso cref="_navigatingData"/>
	[MemberNotNull(nameof(_hotkeyFunctions), nameof(_navigatingData))]
	private void InitializeFields()
	{
		_hotkeyFunctions = new()
		{
			{ new(VirtualKeyModifiers.Control, VirtualKey.Z), SudokuPane.UndoStep },
			{ new(VirtualKeyModifiers.Control, VirtualKey.Y), SudokuPane.RedoStep },
			{ new(VirtualKeyModifiers.Control, VirtualKey.C), CopySudokuGridText },
			{
				new(VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift, VirtualKey.C),
				async () => await CopySudokuGridControlAsSnapshotAsync()
			},
			{ new(VirtualKeyModifiers.Control, VirtualKey.V), async () => await PasteCodeToSudokuGridAsync() },
			{ new(VirtualKeyModifiers.Control, VirtualKey.O), async () => await OpenFileInternalAsync() },
			{ new(VirtualKeyModifiers.Control, VirtualKey.S), async () => await SaveFileInternalAsync() },
			{ new((VirtualKey)189), SetPreviousView }, // Minus sign
			{ new((VirtualKey)187), SetNextView }, // Equals sign
			{ new(VirtualKey.Home), SetHomeView },
			{ new(VirtualKey.End), SetEndView },
			{ new(VirtualKey.Escape), ClearView }
		};
		_navigatingData = new()
		{
			{ container => container == BasicOperationBar, typeof(BasicOperation) },
			{ container => container == AttributeCheckingOperationBar, typeof(AttributeCheckingOperation) },
			{ container => container == PrintingOperationBar, typeof(PrintingOperation) },
			{ container => container == ShuffleOperationBar, typeof(ShuffleOperation) }
		};
	}

	/// <summary>
	/// Load initial grid.
	/// </summary>
	private void LoadInitialGrid()
	{
		if (((App)Application.Current).FirstGrid is { } grid)
		{
			SudokuPane.Puzzle = grid;
			((App)Application.Current).FirstGrid = null; // Maybe not necessary...
		}
	}

	/// <summary>
	/// An outer-layered method to switching pages. This method can be used by both
	/// <see cref="CommandBarView_ItemInvoked"/> and <see cref="CommandBarView_SelectionChanged"/>.
	/// </summary>
	/// <param name="container">The container.</param>
	/// <seealso cref="CommandBarView_ItemInvoked"/>
	/// <seealso cref="CommandBarView_SelectionChanged"/>
	private void SwitchingPage(NavigationViewItemBase container)
	{
		foreach (var (predicate, type) in _navigatingData)
		{
			if (predicate(container))
			{
				NavigateToPage(type);
			}
		}
	}

	/// <summary>
	/// Try to navigate to the target page.
	/// </summary>
	/// <param name="pageType">The target page type.</param>
	private void NavigateToPage(Type pageType)
	{
		if (CommandBarFrame.SourcePageType != pageType)
		{
			CommandBarFrame.Navigate(pageType, this, App.DefaultNavigationTransitionInfo);
		}
	}

	/// <summary>
	/// Sets the previous view.
	/// </summary>
	private void SetPreviousView()
	{
		if (!IsSudokuPaneFocused())
		{
			return;
		}

		if (VisualUnit is { Views.Length: not 0 } && CurrentViewIndex - 1 >= 0)
		{
			CurrentViewIndex--;
		}
	}

	/// <summary>
	/// Sets the next view.
	/// </summary>
	private void SetNextView()
	{
		if (!IsSudokuPaneFocused())
		{
			return;
		}

		if (VisualUnit is { Views.Length: var length and not 0 } && CurrentViewIndex + 1 < length)
		{
			CurrentViewIndex++;
		}
	}

	/// <summary>
	/// Sets the home view.
	/// </summary>
	private void SetHomeView()
	{
		if (!IsSudokuPaneFocused())
		{
			return;
		}

		if (VisualUnit is { Views.Length: not 0 })
		{
			CurrentViewIndex = 0;
		}
	}

	/// <summary>
	/// Sets the end view.
	/// </summary>
	private void SetEndView()
	{
		if (!IsSudokuPaneFocused())
		{
			return;
		}

		if (VisualUnit is { Views.Length: var length and not 0 })
		{
			CurrentViewIndex = length - 1;
		}
	}

	/// <summary>
	/// Skips to the specified index of the view.
	/// </summary>
	/// <param name="viewIndex">The view index.</param>
	private void SkipToSpecifiedViewIndex(int viewIndex)
	{
		if (VisualUnit is not { Views.Length: var length } || length <= viewIndex)
		{
			return;
		}

		CurrentViewIndex = viewIndex;
	}

	/// <summary>
	/// Clear the visual unit data.
	/// </summary>
	private void ClearView()
	{
		if (!IsSudokuPaneFocused())
		{
			return;
		}

		if (VisualUnit is { Views.Length: not 0 })
		{
			VisualUnit = null;
		}
	}

	/// <summary>
	/// Checks whether the pane is focused.
	/// </summary>
	/// <returns>A <see cref="bool"/> result.</returns>
	private bool IsSudokuPaneFocused() => SudokuPane.FocusState != FocusState.Unfocused;


	[Callback]
	private static void CurrentViewIndexPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is not (AnalyzePage { VisualUnit: var visualUnit } page, { NewValue: int value }))
		{
			return;
		}

		if (value == -1)
		{
			page.VisualUnit = null;
			return;
		}

		page.SudokuPane.ViewUnit = visualUnit switch
		{
			{ Conclusions: var conclusions, Views: [] } => new() { Conclusions = conclusions, View = View.Empty },
			{ Conclusions: var conclusions, Views: var views } => new() { Conclusions = conclusions, View = views[value] },
			_ => null
		};
	}

	[Callback]
	private static void VisualUnitPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is not (AnalyzePage page, { NewValue: var value and (null or VisualUnit _) }))
		{
			return;
		}

		page.CurrentViewIndex = value is VisualUnit ? 0 : -1;
	}


	private void CommandBarView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
		=> SwitchingPage(args.InvokedItemContainer);

	private void CommandBarView_Loaded(object sender, RoutedEventArgs e) => BasicOperationBar.IsSelected = true;

	private void CommandBarView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
		=> SwitchingPage(args.SelectedItemContainer);

	private void CommandBarFrame_Navigated(object sender, NavigationEventArgs e)
	{
		if (e is not { Content: IOperationProviderPage operationProvider, Parameter: AnalyzePage @this })
		{
			return;
		}

		operationProvider.BasePage = @this;
	}

	private void FixGridButton_Click(object sender, RoutedEventArgs e)
	{
		var modified = SudokuPane.Puzzle;
		modified.Fix();

		SudokuPane.Puzzle = modified;
	}

	private void UnfixGridButton_Click(object sender, RoutedEventArgs e)
	{
		var modified = SudokuPane.Puzzle;
		modified.Unfix();

		SudokuPane.Puzzle = modified;
	}

	private void SudokuPane_MouseWheelChanged(SudokuPane sender, SudokuPaneMouseWheelChangedEventArgs e)
	{
		if (VisualUnit is { Views.Length: var length and not 0 })
		{
			((Action)(e.IsClockwise ? SetNextView : SetPreviousView))();
		}
	}

	private void SudokuPane_GridUpdated(SudokuPane sender, GridUpdatedEventArgs e)
	{
		if (e.Behavior is GridUpdatedBehavior.Clear or GridUpdatedBehavior.Elimination
			or GridUpdatedBehavior.EliminationMultiple or GridUpdatedBehavior.Assignment)
		{
			ClearAnalyzeTabsData();
		}
	}

	private async void AnalyzeButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		var puzzle = SudokuPane.Puzzle;
		if (!puzzle.IsValid())
		{
			return;
		}

		AnalyzeButton.IsEnabled = false;
		ClearAnalyzeTabsData();
		IsAnalyzerLaunched = true;

		var textFormat = GetString("AnalyzePage_AnalyzerProgress");

		var solver = ((App)Application.Current).ProgramSolver;
		var analysisResult = await Task.Run(analyze);

		AnalyzeButton.IsEnabled = true;
		IsAnalyzerLaunched = false;

		switch (analysisResult)
		{
			case { IsSolved: true }:
			{
				UpdateAnalysisResult(analysisResult);
				AnalysisResultCache = analysisResult;

				break;
			}
#if false
			case
			{
				WrongStep: _,
				FailedReason: _,
				UnhandledException: WrongStepException { CurrentInvalidGrid: _ }
			}:
			{
				break;
			}
			case { FailedReason: _, UnhandledException: _ }:
			{
				break;
			}
#endif
		}


		void progressReportHandler(double percent)
		{
			DispatcherQueue.TryEnqueue(updatePercentValueCallback);


			void updatePercentValueCallback()
			{
				ProgressPercent = percent * 100;
				AnalyzeProgressLabel.Text = string.Format(textFormat!, percent);
			}
		}

		LogicalSolverResult analyze()
		{
			lock (App.SyncRoot)
			{
				return solver.Solve(puzzle, new Progress<double>(progressReportHandler));
			}
		}
	}
}
