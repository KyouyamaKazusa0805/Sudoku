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
	/// Initializes a <see cref="PatternBasedPuzzleGeneratingPage"/> instance.
	/// </summary>
	public PatternBasedPuzzleGeneratingPage() => InitializeComponent();


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
}
