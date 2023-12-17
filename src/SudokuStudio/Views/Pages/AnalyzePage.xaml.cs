using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.SourceGeneration;
using System.Text.Json;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Sudoku.Analytics;
using Sudoku.Analytics.Metadata;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Rendering.Nodes;
using Sudoku.Text;
using SudokuStudio.BindableSource;
using SudokuStudio.Collection;
using SudokuStudio.ComponentModel;
using SudokuStudio.Configuration;
using SudokuStudio.Input;
using SudokuStudio.Interaction;
using SudokuStudio.Rendering;
using SudokuStudio.Storage;
using SudokuStudio.Views.Controls;
using SudokuStudio.Views.Pages.Analyze;
using SudokuStudio.Views.Pages.ContentDialogs;
using SudokuStudio.Views.Pages.Operation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.ViewManagement;
using WinRT;
using static Sudoku.SolutionWideReadOnlyFields;
using static SudokuStudio.ProjectWideConstants;
using static SudokuStudio.Strings.StringsAccessor;
using SolvingPath = SudokuStudio.Views.Pages.Analyze.SolvingPath;

namespace SudokuStudio.Views.Pages;

/// <summary>
/// Defines a new page that stores a set of controls to analyze a sudoku grid.
/// </summary>
[DependencyProperty<bool>("IsAnalyzerLaunched", Accessibility = Accessibility.Internal, DocSummary = "Indicates whether the analyzer is launched.")]
[DependencyProperty<bool>("IsGathererLaunched", Accessibility = Accessibility.Internal, DocSummary = "Indicates whether the gatherer is launched.")]
[DependencyProperty<bool>("IsGeneratorLaunched", Accessibility = Accessibility.Internal, DocSummary = "Indicates whether the generator is launched.")]
[DependencyProperty<double>("ProgressPercent", Accessibility = Accessibility.Internal, DocSummary = "Indicates the progress percent value.")]
[DependencyProperty<int>("CurrentViewIndex", DefaultValue = -1, Accessibility = Accessibility.Internal, DocSummary = "Indicates the current index of the view of property <see cref=\"global::Sudoku.Rendering.IRenderable.Views\"/> displayed.")]
[DependencyProperty<int>("SelectedColorIndex", DefaultValue = -1, Accessibility = Accessibility.Internal, DocSummary = "Indicates the selected color index.")]
[DependencyProperty<string>("BabaGroupNameInput?", Accessibility = Accessibility.Internal, DocSummary = "Indicates the input character that is used as a baba group variable.")]
[DependencyProperty<DrawingMode>("SelectedMode", DefaultValue = DrawingMode.Cell, Accessibility = Accessibility.Internal, DocSummary = "Indicates the selected drawing mode.")]
[DependencyProperty<Inference>("LinkKind", DefaultValue = Inference.Strong, Accessibility = Accessibility.Internal, DocSummary = "Indicates the link type.")]
[DependencyProperty<AnalyzerResult>("AnalysisResultCache?", Accessibility = Accessibility.Internal, DocSummary = "Indicates the analysis result cache.")]
[DependencyProperty<ColorPalette>("UserDefinedPalette", Accessibility = Accessibility.Internal, DocSummary = "Indicates the user-defined colors.")]
[DependencyProperty<IRenderable>("VisualUnit?", Accessibility = Accessibility.Internal, DocSummary = "Indicates the visual unit.")]
public sealed partial class AnalyzePage : Page
{
	[Default]
	private static readonly ColorPalette UserDefinedPaletteDefaultValue = ((App)Application.Current).Preference.UIPreferences.UserDefinedColorPalette;


	/// <summary>
	/// <para>Indicates the previously selected candidate.</para>
	/// <para>
	/// This field can record the previous data in a pair of clicking operation.
	/// For example, constructing a chain, we should click twice, for the head and tail of the chain.
	/// If we click at the second time, this field will be set the candidate having clicked at the first time.
	/// </para>
	/// </summary>
	internal Candidate? _previousSelectedCandidate;

	/// <summary>
	/// Indicates the cancellation token source used by analyzing-related operations.
	/// </summary>
	internal CancellationTokenSource? _ctsForAnalyzingRelatedOperations;

	/// <summary>
	/// Defines a user-defined view that will be used when <see cref="SudokuPane.CurrentPaneMode"/> is <see cref="PaneMode.Drawing"/>.
	/// </summary>
	/// <seealso cref="SudokuPane.CurrentPaneMode"/>
	/// <seealso cref="PaneMode.Drawing"/>
	internal ViewUnitBindableSource? _userColoringView = new();

	/// <summary>
	/// The internal puzzle libraries.
	/// </summary>
	internal ObservableCollection<PuzzleLibraryBindableSource>? _puzzleLibraries;

	/// <summary>
	/// Indicates the tab routing data.
	/// </summary>
	private List<AnalyzeTabPageBindableSource> _tabsRoutingData;

	/// <summary>
	/// Defines a key-value pair of functions that is used for routing hotkeys.
	/// </summary>
	private List<(Hotkey Hotkey, Action Action)> _hotkeyFunctions;

	/// <summary>
	/// The navigating data.
	/// </summary>
	private List<(Func<NavigationViewItemBase, bool> PageChecker, Type PageType)> _navigatingData;


	/// <summary>
	/// Indicates whether the current page is launched for the first time. This value is <see langword="false"/> after switching pages.
	/// </summary>
	private static bool _isFirstLaunched = true;


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
		foreach (var pageData in (IEnumerable<AnalyzeTabPageBindableSource>)AnalyzeTabs.TabItemsSource)
		{
			if (pageData is { Page: IAnalyzeTabPage subTabPage })
			{
				subTabPage.AnalysisResult = null;
			}
		}
	}

	/// <summary>
	/// To update values via the specified <see cref="AnalyzerResult"/> instance.
	/// </summary>
	/// <param name="analyzerResult">The analysis result instance.</param>
	/// <seealso cref="AnalyzerResult"/>
	internal void UpdateAnalysisResult(AnalyzerResult analyzerResult)
	{
		foreach (var pageData in (IEnumerable<AnalyzeTabPageBindableSource>)AnalyzeTabs.TabItemsSource)
		{
			if (pageData is { Page: IAnalyzeTabPage subTabPage })
			{
				subTabPage.AnalysisResult = analyzerResult;
			}
		}
	}

	/// <summary>
	/// Copies for sudoku puzzle text.
	/// </summary>
	/// <param name="focusRequired">Indicates whether the focus is required. The default value is <see langword="true"/>.</param>
	internal void CopySudokuGridText(bool focusRequired = true)
	{
		if (focusRequired && !IsSudokuPaneFocused())
		{
			return;
		}

		if (SudokuPane.Puzzle is not { IsUndefined: false, IsEmpty: false } puzzle)
		{
			return;
		}

		var character = ((App)Application.Current).Preference.UIPreferences.EmptyCellCharacter;
		var dataPackage = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
		dataPackage.SetText(puzzle.ToString($"#{character}"));

		Clipboard.SetContent(dataPackage);
	}

	/// <summary>
	/// To determine whether the current application view is in an unsnapped state.
	/// </summary>
	/// <returns>The <see cref="bool"/> value indicating that.</returns>
	internal bool EnsureUnsnapped(bool isFileSaving)
	{
		// 'FileOpenPicker' APIs will not work if the application is in a snapped state.
		// If an app wants to show a 'FileOpenPicker' while snapped, it must attempt to unsnap first.
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
		await OnSavingOrCopyingSudokuPanePictureAsync(stream);

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
		if (IsSudokuPaneFocused()
			&& Clipboard.GetContent() is var dataPackageView
			&& dataPackageView.Contains(StandardDataFormats.Text)
			&& await dataPackageView.GetTextAsync() is var targetText
			&& Grid.TryParse(targetText, out var grid))
		{
			SudokuPane.Puzzle = grid;
		}
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
		fop.Initialize(this);
		fop.ViewMode = PickerViewMode.Thumbnail;
		fop.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
		fop.AddFileFormat(FileFormats.Text);
		fop.AddFileFormat(FileFormats.PlainText);

		if (await fop.PickSingleFileAsync() is not { } file)
		{
			return;
		}

		await LoadPuzzleCoreAsync(file);
	}

	/// <summary>
	/// Save a file via grid formatters.
	/// </summary>
	/// <param name="gridFormatters">The grid formatters. The default value is <see langword="null"/>.</param>
	/// <returns>A task that handles the operation.</returns>
	internal async Task<bool> SaveFileInternalAsync(IConceptConverter<Grid>[]? gridFormatters = null)
	{
		if (!EnsureUnsnapped(true))
		{
			return false;
		}

		var fsp = new FileSavePicker();
		fsp.Initialize(this);
		fsp.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
		fsp.SuggestedFileName = GetString("Sudoku");
		fsp.AddFileFormat(FileFormats.Text);
		fsp.AddFileFormat(FileFormats.PlainText);
		fsp.AddFileFormat(FileFormats.PortablePicture);

		if (await fsp.PickSaveFileAsync() is not { Path: var filePath } file)
		{
			return false;
		}

		if (SudokuPane is not { Puzzle: var grid, ViewUnit: var viewUnit, DisplayCandidates: var displayCandidates })
		{
			return false;
		}

		switch (Path.GetExtension(filePath))
		{
			case FileExtensions.PlainText:
			{
				if (gridFormatters is null)
				{
					SudokuPlainTextFileHandler.Write(filePath, grid);
				}
				else
				{
					await File.WriteAllTextAsync(
						filePath,
						string.Join("\r\n\r\n", [.. from formatter in gridFormatters select formatter.Converter(in grid)])
					);
				}
				break;
			}
			case FileExtensions.Text:
			{
				SudokuFileHandler.Write(
					filePath,
					gridFormatters is null
						? [
							new()
							{
								BaseGrid = grid,
								RenderableData = viewUnit switch
								{
									{ Conclusions: var conclusions, View: var view } => new() { Conclusions = conclusions, Views = [view] },
									_ => null
								},
								ShowCandidates = displayCandidates
							}
						]
						:
						from formatter in gridFormatters
						select new GridInfo
						{
							BaseGrid = grid,
							GridString = formatter.Converter(in grid),
							RenderableData = viewUnit switch
							{
								{ Conclusions: var conclusions, View: var view } => new() { Conclusions = conclusions, Views = [view] },
								_ => null
							},
							ShowCandidates = displayCandidates
						}
				);
				break;
			}
			case FileExtensions.PortablePicture:
			{
				await OnSavingOrCopyingSudokuPanePictureAsync(file);
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
			case > 1024 * 64:
			{
				OpenFileFailed?.Invoke(this, new(OpenFileFailedReason.FileIsTooLarge));
				return;
			}
			default:
			{
				switch (Path.GetExtension(filePath))
				{
					case FileExtensions.PlainText:
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
					case FileExtensions.Text:
					{
						switch (SudokuFileHandler.Read(filePath))
						{
							case [
								{
									BaseGrid: var g,
									GridString: var gridStr,
									ShowCandidates: var showCandidates,
									RenderableData: var nullableRenderableData
								}
							]:
							{
								SudokuPane.Puzzle = gridStr is not null && Grid.TryParse(gridStr, out var g2) ? g2 : g;
								SudokuPane.DisplayCandidates = showCandidates;

								if (nullableRenderableData is { } renderableData)
								{
									VisualUnit = renderableData;
								}

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
	protected override void OnNavigatedTo(NavigationEventArgs e)
	{
		if (e.Parameter is Grid targetGrid)
		{
			SudokuPane.Puzzle = targetGrid;
		}
	}

	/// <inheritdoc/>
	protected override void OnKeyDown(KeyRoutedEventArgs e)
	{
		base.OnKeyDown(e);

		// This method routes the hotkeys.
		var modifierState = Keyboard.GetModifierStateForCurrentThread();
		foreach (var ((key, modifiers), action) in _hotkeyFunctions)
		{
			if (modifierState == modifiers && e.Key == key)
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
	[MemberNotNull(nameof(_hotkeyFunctions), nameof(_navigatingData), nameof(_tabsRoutingData), nameof(_puzzleLibraries))]
	private void InitializeFields()
	{
		var thickness = new Thickness(10);
		_tabsRoutingData = [
			new()
			{
				Header = GetString("AnalyzePage_TechniquesTable"),
				IconSource = new SymbolIconSource { Symbol = Symbol.Flag },
				Page = new Summary { Margin = thickness, BasePage = this }
			},
			new()
			{
				Header = GetString("AnalyzePage_StepDetail"),
				IconSource = new SymbolIconSource { Symbol = Symbol.ShowResults },
				Page = new SolvingPath { Margin = thickness, BasePage = this }
			},
			new()
			{
				Header = GetString("AnalyzePage_AllStepsInCurrentGrid"),
				IconSource = new SymbolIconSource { Symbol = Symbol.Shuffle },
				Page = new GridGathering { Margin = thickness, BasePage = this }
			},
			new()
			{
				Header = GetString("AnalyzePage_Drawing"),
				IconSource = new SymbolIconSource { Symbol = Symbol.Edit },
				Page = new Drawing { Margin = thickness, BasePage = this }
			}
		];

		_hotkeyFunctions = [
			(new(VirtualKey.Z, VirtualKeyModifiers.Control), SudokuPane.UndoStep),
			(new(VirtualKey.Y, VirtualKeyModifiers.Control), SudokuPane.RedoStep),
			(new(VirtualKey.C, VirtualKeyModifiers.Control), () => CopySudokuGridText()),
			(new(VirtualKey.C, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift), async () => await CopySudokuGridControlAsSnapshotAsync()),
			(new(VirtualKey.V, VirtualKeyModifiers.Control), async () => await PasteCodeToSudokuGridAsync()),
			(new(VirtualKey.O, VirtualKeyModifiers.Control), async () => await OpenFileInternalAsync()),
			(new(VirtualKey.R, VirtualKeyModifiers.Control), UpdatePuzzleViaSolutionGrid),
			(new(VirtualKey.S, VirtualKeyModifiers.Control), async () => await SaveFileInternalAsync()),
			(new((VirtualKey)189), SetPreviousView),
			(new((VirtualKey)187), SetNextView),
			(new(VirtualKey.Home), SetHomeView),
			(new(VirtualKey.End), SetEndView),
			(new(VirtualKey.Escape), ClearView)
		];

		_navigatingData = [
			(container => container == BasicOperationBar, typeof(BasicOperation)),
			(container => container == AttributeCheckingOperationBar, typeof(AttributeCheckingOperation)),
			(container => container == PrintingOperationBar, typeof(PrintingOperation)),
			(container => container == ShuffleOperationBar, typeof(ShuffleOperation)),
			(container => container == GeneratingOperationBar, typeof(GeneratingOperation))
		];

		_puzzleLibraries = PuzzleLibraryBindableSource.LocalPuzzleLibraries(false);
	}

	/// <summary>
	/// Load initial grid.
	/// </summary>
	private void LoadInitialGrid()
	{
		if (((App)Application.Current).AppStartingGridInfo is
			{
				BaseGrid: var grid,
				GridString: _,
				ShowCandidates: var showCandidates,
				RenderableData: var renderableData
			})
		{
			SudokuPane.Puzzle = grid;
			VisualUnit = renderableData;
			SudokuPane.DisplayCandidates = showCandidates;

			((App)Application.Current).AppStartingGridInfo = null; // Maybe not necessary...
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
			CommandBarFrame.Navigate(pageType, this, DefaultNavigationTransitionInfo);
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
			SudokuPane.ViewUnit = null;
		}
	}

	/// <summary>
	/// Checks whether the pane is focused.
	/// </summary>
	/// <returns>A <see cref="bool"/> result.</returns>
	private bool IsSudokuPaneFocused() => SudokuPane.FocusState != FocusState.Unfocused;

	/// <summary>
	/// To update puzzle via solution grid.
	/// </summary>
	private void UpdatePuzzleViaSolutionGrid() => SudokuPane.UpdateGrid(SudokuPane.Puzzle.SolutionGrid);

	/// <summary>
	/// Try to update view unit.
	/// </summary>
	private void UpdateViewUnit()
	{
		SudokuPane.ViewUnit = null; // Change the reference to update view.
		SudokuPane.ViewUnit = _userColoringView;
	}

	private bool CheckCellNode(int index, GridClickedEventArgs e, ViewUnitBindableSource view)
	{
		switch (e)
		{
			case { Cell: var cell, MouseButton: MouseButton.Left }:
			{
				if (view.View.Find(node => node is CellViewNode { Cell: var c } && c == cell) is { } foundNode)
				{
					view.View.Remove(foundNode);
				}
				else
				{
					view.View.Add(new CellViewNode(index, cell) { RenderingMode = RenderingMode.BothDirectAndPencilmark });
				}

				UpdateViewUnit();

				break;
			}
		}

		return true;
	}

	private bool CheckCandidateNode(int index, GridClickedEventArgs e, ViewUnitBindableSource view)
	{
		switch (e)
		{
			case { Candidate: var candidate, MouseButton: MouseButton.Left }:
			{
				if (view.View.Find(node => node is CandidateViewNode { Candidate: var c } && c == candidate) is { } foundNode)
				{
					view.View.Remove(foundNode);
				}
				else
				{
					view.View.Add(new CandidateViewNode(index, candidate));
				}

				UpdateViewUnit();

				break;
			}
		}

		return true;
	}

	private bool CheckHouseNode(int index, GridClickedEventArgs e, ViewUnitBindableSource view)
	{
		switch (e)
		{
			case { Candidate: var candidate2 }:
			{
				if (_previousSelectedCandidate is not { } candidate1)
				{
					_previousSelectedCandidate = candidate2;
					return false;
				}

				var cell1 = candidate1 / 9;
				var cell2 = candidate2 / 9;
				if ((CellsMap[cell1] + cell2).CoveredHouses is not (var coveredHouses and not 0))
				{
					_previousSelectedCandidate = null;
					return true;
				}

				var house = coveredHouses.GetAllSets()[^1];
				if (view.View.Find(node => node is HouseViewNode { House: var h } && h == house) is { } foundNode)
				{
					view.View.Remove(foundNode);
				}
				else
				{
					view.View.Add(new HouseViewNode(index, house));
				}

				UpdateViewUnit();

				break;
			}
		}

		return true;
	}

	private bool CheckChuteNode(int index, GridClickedEventArgs e, ViewUnitBindableSource view)
	{
		switch (e)
		{
			case { Candidate: var candidate2 }:
			{
				if (_previousSelectedCandidate is not { } candidate1)
				{
					_previousSelectedCandidate = candidate2;
					return false;
				}

				var (mr1, mc1) = GridClickedEventArgs.GetChute(candidate1);
				var (mr2, mc2) = GridClickedEventArgs.GetChute(candidate2);
				if (mr1 == mr2)
				{
					if (view.View.Find(node => node is ChuteViewNode { ChuteIndex: var c } && c == mr1) is { } foundNode)
					{
						view.View.Remove(foundNode);
					}
					else
					{
						view.View.Add(new ChuteViewNode(index, mr1));
					}

					UpdateViewUnit();
					break;
				}

				if (mc1 == mc2)
				{
					if (view.View.Find(node => node is ChuteViewNode { ChuteIndex: var c } && c - 3 == mc1) is { } foundNode)
					{
						view.View.Remove(foundNode);
					}
					else
					{
						view.View.Add(new ChuteViewNode(index, mc1 + 3));
					}

					UpdateViewUnit();
					break;
				}

				break;
			}
		}

		return true;
	}

	private bool CheckLinkNode(GridClickedEventArgs e, ViewUnitBindableSource view)
	{
		switch (e)
		{
			case { Candidate: var candidate2 }:
			{
				if (_previousSelectedCandidate is not { } candidate1)
				{
					_previousSelectedCandidate = candidate2;
					return false;
				}

				var cell1 = candidate1 / 9;
				var cell2 = candidate2 / 9;
				var digit1 = candidate1 % 9;
				var digit2 = candidate2 % 9;
				if (view.View.Find(predicate) is { } foundNode)
				{
					view.View.Remove(foundNode);
				}
				else
				{
					var lt1 = new LockedTarget(candidate1 % 9, [candidate1 / 9]);
					var lt2 = new LockedTarget(candidate2 % 9, [candidate2 / 9]);
					view.View.Add(new LinkViewNode(default!, lt1, lt2, LinkKind)); // Link nodes don't use identifier to display colors.
				}

				UpdateViewUnit();

				break;


				bool predicate(ViewNode element)
					=> element switch
					{
						LinkViewNode { Start.Cells: [var c1], End.Cells: [var c2], Inference: Inference.Default }
							=> c1 == cell1 && c2 == cell2 || c2 == cell1 && c1 == cell2,
						LinkViewNode { Start: { Cells: [var c1], Digit: var d1 }, End: { Cells: [var c2], Digit: var d2 } }
							=> c1 == cell1 && c2 == cell2 && d1 == digit1 && d2 == digit2
							|| c2 == cell1 && c1 == cell2 && d2 == digit1 && d1 == digit2,
						_ => false
					};
			}
		}

		return true;
	}

	private bool CheckBabaGroupingNode(int index, GridClickedEventArgs e, ViewUnitBindableSource view)
	{
		TextBlock wrongHintControl() => ((Drawing)((AnalyzeTabPageBindableSource)AnalyzeTabs.SelectedItem).Page).InvalidInputInfoDisplayer;
		switch (BabaGroupNameInput, e, view)
		{
			case (null or [], _, _):
			{
				return true;
			}
			case ([var character], { Candidate: var candidate }, { View: var v }):
			{
				var cell = candidate / 9;
				if (v.Find(node => node is BabaGroupViewNode { Cell: var c } && c == cell) is { } foundNode)
				{
					v.Remove(foundNode);
				}
				else
				{
					var id = index != -1 ? (ColorIdentifier)index : new ColorColorIdentifier(0, 255, 255, 255);
					v.Add(new BabaGroupViewNode(id, cell, (Utf8Char)character, Grid.MaxCandidatesMask));
				}

				UpdateViewUnit();

				wrongHintControl().Visibility = Visibility.Collapsed;
				break;
			}
			default:
			{
				wrongHintControl().Visibility = Visibility.Visible;
				break;
			}
		}

		return true;
	}

	/// <summary>
	/// Produces a copying/saving operation for pictures from sudoku pane <see cref="SudokuPane"/>.
	/// </summary>
	/// <typeparam name="T">
	/// The type of the handling argument. This type should be <see cref="IRandomAccessStream"/> or <see cref="StorageFile"/>.
	/// </typeparam>
	/// <param name="obj">The argument.</param>
	/// <returns>A <see cref="Task"/> instance that contains details for the current asynchronous operation.</returns>
	/// <seealso cref="IRandomAccessStream"/>
	/// <seealso cref="StorageFile"/>
	private async Task OnSavingOrCopyingSudokuPanePictureAsync<T>(T obj) where T : class
	{
		if (((App)Application.Current).Preference.UIPreferences.TransparentBackground)
		{
			SudokuPane.MainGrid.Background = new SolidColorBrush(Colors.White);
		}

		var desiredSize = ((App)Application.Current).Preference.UIPreferences.DesiredPictureSizeOnSaving;
		var (originalWidth, originalHeight) = (SudokuPaneOutsideViewBox.Width, SudokuPaneOutsideViewBox.Height);
		(SudokuPaneOutsideViewBox.Width, SudokuPaneOutsideViewBox.Height) = (desiredSize, desiredSize);

		await SudokuPaneOutsideViewBox.RenderToAsync(obj);

		(SudokuPaneOutsideViewBox.Width, SudokuPaneOutsideViewBox.Height) = (originalWidth, originalHeight);

		if (((App)Application.Current).Preference.UIPreferences.TransparentBackground)
		{
			SudokuPane.MainGrid.Background = null;
		}
	}


	/// <summary>
	/// Try to change the value of the property <see cref="CurrentViewIndex"/>.
	/// </summary>
	/// <param name="page">The triggering page.</param>
	/// <param name="value">The index value set.</param>
	/// <seealso cref="CurrentViewIndex"/>
	private static void ChangeCurrentViewIndex(AnalyzePage page, int value)
	{
		var visualUnit = page.VisualUnit;

		if (value == -1)
		{
			page.VisualUnit = null;
			return;
		}

		page.SudokuPane.ViewUnit = visualUnit switch
		{
			{ Conclusions: var conclusions, Views: null or [] } => new() { Conclusions = conclusions, View = [] },
			{ Conclusions: var conclusions, Views: var views } => new() { Conclusions = conclusions, View = views[value] },
			_ => null
		};
	}

	[Callback]
	private static void CurrentViewIndexPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is not (AnalyzePage page, { NewValue: int value }))
		{
			return;
		}

		ChangeCurrentViewIndex(page, value);
	}

	[Callback]
	private static void VisualUnitPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is not (AnalyzePage page, { NewValue: var value and (null or IRenderable) }))
		{
			return;
		}

		var currentViewIndex = value is IRenderable ? 0 : -1;
		page.CurrentViewIndex = currentViewIndex;

		ChangeCurrentViewIndex(page, currentViewIndex);

		// A rescue. The code snippet is used for manually updating the pips pager and text block.
		page.ViewsSwitcher.Visibility = value is null ? Visibility.Collapsed : Visibility.Visible;
		page.ViewsCountDisplayer.Visibility = value is null ? Visibility.Collapsed : Visibility.Visible;
	}

	[Callback]
	private static void SelectedColorIndexPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is (AnalyzePage page, { NewValue: int value }))
		{
			page.SudokuPane.CurrentPaneMode = value == -1 ? PaneMode.Normal : PaneMode.Drawing;
		}
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

	private void FixGridButton_Click(object sender, RoutedEventArgs e) => SudokuPane.UpdateGrid(SudokuPane.Puzzle.FixedGrid);

	private void UnfixGridButton_Click(object sender, RoutedEventArgs e) => SudokuPane.UpdateGrid(SudokuPane.Puzzle.UnfixedGrid);

	private void SudokuPane_MouseWheelChanged(SudokuPane sender, SudokuPaneMouseWheelChangedEventArgs e)
	{
		if (VisualUnit is { Views.Length: not 0 })
		{
			((Action)(e.IsClockwise ? SetNextView : SetPreviousView))();
		}
	}

	private void SudokuPane_GridUpdated(SudokuPane sender, GridUpdatedEventArgs e)
	{
		if (e.Behavior is >= 0 and <= GridUpdatedBehavior.Clear)
		{
			ClearAnalyzeTabsData();
		}

		VisualUnit = null;
		_userColoringView = null;
		UpdateViewUnit();
	}

	private void SudokuPane_ReceivedDroppedFileSuccessfully(SudokuPane sender, ReceivedDroppedFileSuccessfullyEventArgs e)
	{
		if (e is not { FilePath: var filePath, GridInfo: var gridInfo })
		{
			return;
		}

		switch (Path.GetExtension(filePath), gridInfo)
		{
			case (FileExtensions.PlainText, { BaseGrid: var g }):
			{
				SudokuPane.Puzzle = g;
				break;
			}
			case (FileExtensions.Text, { BaseGrid: var g, RenderableData: { } visualUnit, ShowCandidates: var showCandidates }):
			{
				SudokuPane.Puzzle = g;
				SudokuPane.DisplayCandidates = showCandidates;
				VisualUnit = visualUnit;
				break;
			}
		}
	}

	private async void AnalyzeButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		var puzzle = SudokuPane.Puzzle;
		if (!puzzle.IsValid)
		{
			return;
		}

		AnalyzeButton.IsEnabled = false;
		ClearAnalyzeTabsData();
		IsAnalyzerLaunched = true;

		var textFormat = GetString("AnalyzePage_AnalyzerProgress");
		using var cts = new CancellationTokenSource();
		var analyzer = ((App)Application.Current).GetAnalyzerConfigured(SudokuPane);
		_ctsForAnalyzingRelatedOperations = cts;

		try
		{
			switch (await Task.Run(() =>
			{
				lock (AnalyzingRelatedSyncRoot)
				{
					return analyzer.Analyze(
						in puzzle,
						progress: new Progress<AnalyzerProgress>(
							progress => DispatcherQueue.TryEnqueue(
								() =>
								{
									var (stepSearcherName, percent) = progress;
									ProgressPercent = progress.Percent * 100;
									AnalyzeProgressLabel.Text = string.Format(textFormat, percent);
									AnalyzeStepSearcherNameLabel.Text = stepSearcherName;
								}
							)
						),
						cancellationToken: cts.Token
					);
				}
			}))
			{
				case { IsSolved: true } analyzerResult:
				{
					UpdateAnalysisResult(analyzerResult);
					AnalysisResultCache = analyzerResult;
					break;
				}
				case
				{
					WrongStep: { Views: var views, Conclusions: var conclusions } wrongStep,
					FailedReason: FailedReason.WrongStep,
					UnhandledException: WrongStepException { CurrentInvalidGrid: var invalidGrid }
				}:
				{
					await new ContentDialog
					{
						XamlRoot = XamlRoot,
						Style = (Style)Application.Current.Resources["DefaultContentDialogStyle"]!,
						Title = GetString("AnalyzePage_ErrorStepEncounteredTitle"),
						CloseButtonText = GetString("AnalyzePage_ErrorStepDialogCloseButtonText"),
						DefaultButton = ContentDialogButton.Close,
						Content = new ErrorStepDialogContent
						{
							ErrorStepGrid = invalidGrid,
							ErrorStepText = string.Format(GetString("AnalyzePage_ErrorStepDescription"), wrongStep),
							ViewUnit = new() { View = views?[0] ?? [], Conclusions = conclusions }
						}
					}.ShowAsync();

					break;
				}
				case { FailedReason: FailedReason.ExceptionThrown, UnhandledException: { } ex }:
				{
					await new ContentDialog
					{
						XamlRoot = XamlRoot,
						Style = (Style)Application.Current.Resources["DefaultContentDialogStyle"]!,
						Title = GetString("AnalyzePage_ExceptionThrownTitle"),
						CloseButtonText = GetString("AnalyzePage_ErrorStepDialogCloseButtonText"),
						DefaultButton = ContentDialogButton.Close,
						Content = new ExceptionThrownOnAnalyzingContent { ThrownException = ex }
					}.ShowAsync();

					break;
				}
			}
		}
		catch (TaskCanceledException)
		{
		}
		finally
		{
			_ctsForAnalyzingRelatedOperations = null;
			AnalyzeButton.IsEnabled = true;
			IsAnalyzerLaunched = false;
		}
	}

	private void SudokuPane_Clicked(SudokuPane sender, GridClickedEventArgs e)
	{
		switch (this, sender, e)
		{
			case ({ SelectedMode: var selectionMode, SelectedColorIndex: var index }, { CurrentPaneMode: PaneMode.Drawing }, { MouseButton: MouseButton.Left }):
			{
				makeColoring(selectionMode, index, _userColoringView ??= new());
				break;
			}
			case ({ SudokuPane: { DisableFlyout: false, Puzzle: var puzzle } }, _, { MouseButton: MouseButton.Right, Cell: var cell }):
			{
				openFlyout(in puzzle, cell);
				break;
			}
			default:
			{
				// Manually set focus for pane because user clicked a candidate, displayed using a text block, making the pane unfocused.
				defaultFocusToGrid();
				break;
			}
		}


		void makeColoring(DrawingMode selectionMode, int index, ViewUnitBindableSource tempView)
		{
			var condition = (selectionMode, index) switch
			{
				(DrawingMode.Cell, not -1) => CheckCellNode(index, e, tempView),
				(DrawingMode.Candidate, not -1) => CheckCandidateNode(index, e, tempView),
				(DrawingMode.House, not -1) => CheckHouseNode(index, e, tempView),
				(DrawingMode.Chute, not -1) => CheckChuteNode(index, e, tempView),
				(DrawingMode.Link, _) => CheckLinkNode(e, tempView),
				(DrawingMode.BabaGrouping, _) => CheckBabaGroupingNode(index, e, tempView),
				_ => true
			};
			if (condition)
			{
				_previousSelectedCandidate = null;
			}
		}

		void openFlyout(scoped ref readonly Grid puzzle, Cell cell)
		{
			var appBarButtons = MainMenuFlyout.SecondaryCommands.OfType<AppBarButton>();
			switch (puzzle.GetState(cell))
			{
				case CellState.Empty:
				{
					SudokuPane._temporarySelectedCell = cell;
					foreach (var element in appBarButtons)
					{
						element.IsEnabled = (puzzle.GetCandidates(cell) >> Math.Abs((Digit)element.Tag) - 1 & 1) != 0;
					}

					MainMenuFlyout.ShowAt(SudokuPane);
					break;
				}
				case CellState.Given or CellState.Modifiable:
				{
					SudokuPane._temporarySelectedCell = cell;
					foreach (var element in appBarButtons)
					{
						element.IsEnabled = false;
					}

					MainMenuFlyout.ShowAt(SudokuPane, new() { ShowMode = FlyoutShowMode.Transient });
					break;
				}
			}
		}

		void defaultFocusToGrid() => SudokuPane.Focus(FocusState.Programmatic);
	}

	private void SudokuPane_CandidatesDisplayingToggled(SudokuPane sender, CandidatesDisplayingToggledEventArgs e)
		=> ((App)Application.Current).Preference.UIPreferences.DisplayCandidates = sender.DisplayCandidates;

	private void SudokuPane_Caching(object sender, EventArgs e)
	{
		if (SudokuPane is not { Puzzle: var puzzle, ViewUnit: var viewUnit })
		{
			return;
		}

		// Set as cached values.
		// No matter whether the option 'AutoCachePuzzleAndView' is true or not,
		// here we should save them because in a same program thread we may use the values to recover it.
		((App)Application.Current).Preference.UIPreferences.LastGridPuzzle = puzzle;
		((App)Application.Current).Preference.UIPreferences.LastRenderable = viewUnit switch
		{
			ViewUnitBindableSource { View: var view, Conclusions: var conclusions } => new() { Conclusions = conclusions, Views = [view] },
			_ => null
		};
	}

	private void MainMenuFlyout_Closed(object sender, object e)
	{
		foreach (var element in MainMenuFlyout.SecondaryCommands.OfType<AppBarButton>())
		{
			element.Visibility = Visibility.Visible;
		}
	}

	private void SetOrDeleteButton_Click(object sender, RoutedEventArgs e)
	{
		if ((sender, SudokuPane) is (AppBarButton { Tag: Digit rawDigit }, { _temporarySelectedCell: var cell and not -1 }))
		{
			SudokuPane.SetOrDeleteDigit(cell, Math.Abs(rawDigit) - 1, rawDigit > 0);
		}
	}

	private void CopyButton_Click(object sender, RoutedEventArgs e) => CopySudokuGridText(false);

	private void CopyKindButton_Click(object sender, RoutedEventArgs e)
	{
		if (sender is not MenuFlyoutItem { Tag: int rawFormatFlag })
		{
			return;
		}

		var flag = (SudokuFormatFlags)rawFormatFlag;
		if (!Enum.IsDefined(flag))
		{
			return;
		}

		if (SudokuPane.Puzzle is not { IsUndefined: false, IsEmpty: false } puzzle)
		{
			return;
		}

		var dataPackage = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
		dataPackage.SetText(puzzle.ToString(flag.GetConverter()));

		Clipboard.SetContent(dataPackage);
	}

	private void CancelOperationButton_Click(object sender, RoutedEventArgs e) => _ctsForAnalyzingRelatedOperations?.Cancel();

	private void AutoSolveButton_Click(object sender, RoutedEventArgs e) => UpdatePuzzleViaSolutionGrid();

	private void Page_Loaded(object sender, RoutedEventArgs e)
	{
		// This method is created to solve the problem that WinUI cannot cache navigation view pages due to internal error.
		if (((App)Application.Current).Preference.UIPreferences.AutoCachePuzzleAndView && !_isFirstLaunched)
		{
			var pref = ((App)Application.Current).Preference.UIPreferences;

			SudokuPane.Puzzle = pref.LastGridPuzzle;
			SudokuPane.ViewUnit = pref.LastRenderable is ({ } conclusions, [var view, ..])
				? new() { Conclusions = conclusions, View = view }
				: null;
		}
		else if (_isFirstLaunched)
		{
			_isFirstLaunched = false;
		}
	}

	private async void SavePuzzleToLibraryAppBarButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		Debug.Assert(_puzzleLibraries is not null);

		var contentDialog = new ContentDialog
		{
			XamlRoot = XamlRoot,
			IsPrimaryButtonEnabled = true,
			Style = (Style)Application.Current.Resources["DefaultContentDialogStyle"]!,
			CloseButtonText = GetString("LibraryPage_Close"),
			Content = new AddToLibraryContent { PuzzleLibraries = _puzzleLibraries },
			DefaultButton = ContentDialogButton.Primary,
			PrimaryButtonText = GetString("LibraryPage_LoadOrAddingButtonText")
		};

		if (await contentDialog.ShowAsync() != ContentDialogResult.Primary)
		{
			return;
		}

		if (contentDialog.Content is not AddToLibraryContent { SelectedLibrary.FileId: var selectedFileId } content)
		{
			ErrorDialog_MustSelectAtLeastOneLibrary.IsOpen = true;
			return;
		}

		var index = _puzzleLibraries.FindIndex(lib => lib.FileId == selectedFileId);
		if (index == -1)
		{
			return;
		}

		var instance = _puzzleLibraries[index];
		var newInstance = new PuzzleLibraryBindableSource(instance, [.. instance.Puzzles, SudokuPane.Puzzle]);

		_puzzleLibraries[index] = newInstance;

		var json = JsonSerializer.Serialize(newInstance, LibraryPage.SerializerOptions);
		await File.WriteAllTextAsync(newInstance.FilePath, json);
	}
}
