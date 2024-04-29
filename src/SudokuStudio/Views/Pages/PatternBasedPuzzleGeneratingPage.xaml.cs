namespace SudokuStudio.Views.Pages;

/// <summary>
/// Represents a page that generates for pattern-based puzzles.
/// </summary>
[DependencyProperty<bool>("IsGeneratorLaunched", Accessibility = Accessibility.Internal, DocSummary = "Indicates whether the generator is running.")]
[DependencyProperty<double>("ProgressPercent", Accessibility = Accessibility.Internal, DocSummary = "Indicates the progress.")]
[DependencyProperty<Digit>("MissingDigit", Accessibility = Accessibility.Internal, DocSummary = "Indicates the missing digit.")]
[DependencyProperty<CellMap>("SelectedCells", Accessibility = Accessibility.Internal, DocSummary = "Indicates the selected cells.")]
public sealed partial class PatternBasedPuzzleGeneratingPage : Page
{
	/// <summary>
	/// Defines a user-defined view that will be used.
	/// </summary>
	/// <seealso cref="SudokuPane.CurrentPaneMode"/>
	private readonly ViewUnitBindableSource _userColoringView = new();


	/// <summary>
	/// Indicates the number of generating and generated puzzles.
	/// </summary>
	private int _generatingCount, _generatingFilteredCount;

	/// <summary>
	/// Indicates the cancellation token source for generating operations.
	/// </summary>
	private CancellationTokenSource? _ctsForGeneratingOperations;

	/// <summary>
	/// Defines a key-value pair of functions that is used for routing hotkeys.
	/// </summary>
	private (Hotkey Hotkey, Action Action)[] _hotkeyFunctions;


	/// <summary>
	/// Initializes a <see cref="PatternBasedPuzzleGeneratingPage"/> instance.
	/// </summary>
	public PatternBasedPuzzleGeneratingPage()
	{
		InitializeComponent();
		InitializeFields();
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
	/// Try to initialize field <see cref="_hotkeyFunctions"/>.
	/// </summary>
	/// <seealso cref="_hotkeyFunctions"/>
	[MemberNotNull(nameof(_hotkeyFunctions))]
	private void InitializeFields()
		=> _hotkeyFunctions = [
			(new(VirtualKey.C, VirtualKeyModifiers.Control), () => CopySudokuGridText()),
			(new(VirtualKey.C, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift), async () => await CopySudokuGridControlAsSnapshotAsync()),
			(new(VirtualKey.V, VirtualKeyModifiers.Control), async () => await PasteCodeToSudokuGridAsync()),
		];

	/// <summary>
	/// Copy the text.
	/// </summary>
	private void CopySudokuGridText()
	{
		var dataPackage = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
		dataPackage.SetText(new BitStatusCellMapConverter().Converter(SelectedCells));
		Clipboard.SetContent(dataPackage);
	}

	/// <summary>
	/// Copy snapshot.
	/// </summary>
	private async Task CopySudokuGridControlAsSnapshotAsync()
	{
		if (SudokuPane.FocusState == FocusState.Unfocused)
		{
			return;
		}

		// Creates the stream to store the output image data.
		var stream = new InMemoryRandomAccessStream();
		await OnSavingOrCopyingSudokuPanePictureSimpleAsync(stream);

		// Copies the data to the data package.
		var dataPackage = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
		var streamRef = RandomAccessStreamReference.CreateFromStream(stream);
		dataPackage.SetBitmap(streamRef);

		// Copies to the clipboard.
		Clipboard.SetContent(dataPackage);
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
	private async Task OnSavingOrCopyingSudokuPanePictureSimpleAsync<T>(T obj) where T : class
	{
		if (((App)Application.Current).Preference.UIPreferences.TransparentBackground)
		{
			var color = App.CurrentTheme switch { ApplicationTheme.Light => Colors.White, _ => Colors.Black };
			SudokuPane.MainGrid.Background = new SolidColorBrush(color);
		}

		await SudokuPane.RenderToAsync(obj);

		if (((App)Application.Current).Preference.UIPreferences.TransparentBackground)
		{
			SudokuPane.MainGrid.Background = null;
		}
	}

	/// <summary>
	/// Paste the grid.
	/// </summary>
	/// <returns></returns>
	private async Task PasteCodeToSudokuGridAsync()
	{
		if (SudokuPane.FocusState != FocusState.Unfocused
			&& Clipboard.GetContent() is var dataPackageView
			&& dataPackageView.Contains(StandardDataFormats.Text)
			&& await dataPackageView.GetTextAsync() is var targetText)
		{
			SelectedCells = new BitStatusCellMapParser().Parser(targetText);
		}
	}


	[Callback]
	private static void MissingDigitPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is not (PatternBasedPuzzleGeneratingPage { MissingDigitSelector: var selector }, { NewValue: Digit newValue }))
		{
			return;
		}

		var control = selector.Items.First(item => item.Tag is Digit d && d == newValue);
		selector.SelectedItem = control;
	}

	[Callback]
	private static void SelectedCellsPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is not (PatternBasedPuzzleGeneratingPage { _userColoringView: var view, SudokuPane: var pane } page, { NewValue: CellMap newValue }))
		{
			return;
		}

		var (a, r, g, b) = App.CurrentTheme switch
		{
			ApplicationTheme.Light => (Color)page.Resources["SelectedCellColorLight"]!,
			_ => (Color)page.Resources["SelectedCellColorDark"]!
		};
		view.View.Clear();
		view.View.AddRange(
			from cell in newValue
			select new CellViewNode(new ColorColorIdentifier(a, r, g, b), cell)
			{
				RenderingMode = RenderingMode.BothDirectAndPencilmark
			}
		);

		pane.ViewUnit = null;
		pane.ViewUnit = view;
	}


	private void SudokuPane_Loaded(object sender, RoutedEventArgs e)
		=> ((App)Application.Current).CoverSettingsToSudokuPaneViaApplicationTheme(SudokuPane);

	private void SudokuPane_ActualThemeChanged(FrameworkElement sender, object args)
		=> ((App)Application.Current).CoverSettingsToSudokuPaneViaApplicationTheme(SudokuPane);

	private void SudokuPane_Clicked(SudokuPane sender, GridClickedEventArgs e)
	{
		if (e is not { Cell: var cell and not -1 })
		{
			return;
		}

		var newValue = SelectedCells;
		newValue.Toggle(cell);
		SelectedCells = newValue;
	}

	private void SudokuPane_DigitInput(SudokuPane sender, DigitInputEventArgs e) => e.Cancel = true;

	private async void GeneratingButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		var pattern = SelectedCells;
		var missingDigit = MissingDigit;
		using var cts = new CancellationTokenSource();
		try
		{
			(_ctsForGeneratingOperations, IsGeneratorLaunched, _generatingCount, _generatingFilteredCount) = (cts, true, 0, 0);
			if (await Task.Run(() => taskEntry(cts.Token)) is { IsUndefined: false } grid)
			{
				SudokuPane.Puzzle = grid;
			}
		}
		catch (OperationCanceledException)
		{
		}
		finally
		{
			_ctsForGeneratingOperations = null;
			IsGeneratorLaunched = false;
		}


		Grid taskEntry(CancellationToken cancellationToken)
		{
#if FULL_IMPL
			var generator = new PatternBasedPuzzleGenerator(in pattern, missingDigit);
			var progress = new SelfReportingProgress<GeneratorProgress>(progressReporter);
			while (true)
			{
				var grid = generator.Generate(cancellationToken: cancellationToken);
				return grid;
			ReportState:
				progress.Report(create<GeneratorProgress>(ref _generatingCount, _generatingFilteredCount));
				cancellationToken.ThrowIfCancellationRequested();
			}


			static T create<T>(ref int generatingCount, int generatingFilteredCount)
				where T : struct, IEquatable<T>, IProgressDataProvider<T>
				=> T.Create(++generatingCount, generatingFilteredCount);

			void progressReporter(GeneratorProgress progress)
			{
				DispatcherQueue.TryEnqueue(dispatchingHandler);


				void dispatchingHandler()
					=> AnalyzeStepSearcherNameLabel.Text = ((IProgressDataProvider<GeneratorProgress>)progress).ToDisplayString();
			}
#else
			var generator = new PatternBasedPuzzleGenerator(in pattern, missingDigit);
			return generator.Generate(cancellationToken: cancellationToken);
#endif
		}
	}

	private void SelectorBar_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
		=> MissingDigit = (Digit)sender.SelectedItem.Tag!;

	private void Page_Loaded(object sender, RoutedEventArgs e) => MissingDigit = -1;

	private void CancelOperationButton_Click(object sender, RoutedEventArgs e) => _ctsForGeneratingOperations?.Cancel();

	private void CopyButton_Click(object sender, RoutedEventArgs e) => CopySudokuGridText();

	private async void CopyPictureButton_ClickAsync(object sender, RoutedEventArgs e) => await CopySudokuGridControlAsSnapshotAsync();

	private async void PasteButton_ClickAsync(object sender, RoutedEventArgs e) => await PasteCodeToSudokuGridAsync();

	private void Dialog_AreYouSureToReturnToEmpty_ActionButtonClick(TeachingTip sender, object args)
	{
		SudokuPane.Puzzle = Grid.Empty;
		SudokuPane.ViewUnit = null;
		SelectedCells = [];
		Dialog_AreYouSureToReturnToEmpty.IsOpen = false;
	}

	private void ClearButton_Click(object sender, RoutedEventArgs e) => Dialog_AreYouSureToReturnToEmpty.IsOpen = true;
}
