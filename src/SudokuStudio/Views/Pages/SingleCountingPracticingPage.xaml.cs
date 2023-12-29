namespace SudokuStudio.Views.Pages;

/// <summary>
/// Represents a page that provides with practise tool to allow you practicing counting logic for technique Naked Single and Full House.
/// </summary>
[DependencyProperty<bool>("IsRunning", Accessibility = Accessibility.Internal, DocSummary = "Indicates whether the game is running.")]
[DependencyProperty<int>("SelectedMode", DefaultValue = -1)]
[DependencyProperty<int>("TestedPuzzlesCount", DefaultValue = 10)]
public sealed partial class SingleCountingPracticingPage : Page
{
	/// <summary>
	/// Indicates the maximum possible supported number of puzzles.
	/// </summary>
	private const int MaxPuzzlesCountSupported = 100;


	/// <summary>
	/// The internal sync root.
	/// </summary>
	private static readonly object SyncRootOnChangingPuzzles = new();


	/// <summary>
	/// Defines a timer instance.
	/// </summary>
	private readonly Stopwatch _stopwatch = new();

	/// <summary>
	/// Indicates the puzzles last.
	/// </summary>
	private volatile int _currentPuzzleIndex = -1;

	/// <summary>
	/// Indicates the target result data.
	/// </summary>
	private List<Candidate> _targetResultData;

	/// <summary>
	/// Indicates the answered data.
	/// </summary>
	private List<(Candidate Candidate, TimeSpan TimeSpan)> _answeredData;


	/// <summary>
	/// Initializes a <see cref="SingleCountingPracticingPage"/> instance.
	/// </summary>
	public SingleCountingPracticingPage()
	{
		InitializeComponent();
		InitializeEvents();
		InitializeFields();
	}


	/// <summary>
	/// Initializes for fields.
	/// </summary>
	[MemberNotNull(nameof(_targetResultData), nameof(_answeredData))]
	private void InitializeFields()
	{
		_targetResultData = new(MaxPuzzlesCountSupported);
		_answeredData = new(MaxPuzzlesCountSupported);
		_targetResultData.Refresh(MaxPuzzlesCountSupported);
		_answeredData.Refresh(MaxPuzzlesCountSupported);
	}

	/// <summary>
	/// Initializes for events.
	/// </summary>
	private void InitializeEvents() => SudokuPane.DigitInput += SudokuPane_DigitInput;

	/// <summary>
	/// Try to generate a new puzzle.
	/// </summary>
	private void GeneratePuzzle()
	{
		var final = Generator.Generate((GeneratingMode)SelectedMode, out var targetCandidate);
		SudokuPane.Puzzle = final;
		_targetResultData[_currentPuzzleIndex] = targetCandidate;
	}


	[Callback]
	private static void IsRunningPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is not (SingleCountingPracticingPage { TestedPuzzlesCount: var testedCount } page, { NewValue: bool value }))
		{
			return;
		}

		lock (SyncRootOnChangingPuzzles)
		{
			if (value)
			{
				page._stopwatch.Start();
				page._currentPuzzleIndex = 0;
				page.ResultDataDisplayer.Text = string.Empty;
			}
			else
			{
				page._stopwatch.Stop();
				page._currentPuzzleIndex = -1;

				var correctCount = page._answeredData.CountWithSameIndex(page._targetResultData, (a, b) => a.Candidate == b, testedCount);
				var totalTimeSpan = page._answeredData[testedCount - 1].TimeSpan;
				page.ResultDataDisplayer.Text = string.Format(
					GetString("SingleCountingPracticingPage_ResultDisplayLabel"),
					totalTimeSpan,
					testedCount,
					totalTimeSpan / testedCount,
					(double)correctCount / testedCount,
					correctCount,
					testedCount
				);
			}

			page._targetResultData.Refresh(MaxPuzzlesCountSupported);
			page._answeredData.Refresh(MaxPuzzlesCountSupported);
		}
	}


	private void SudokuPane_DigitInput(SudokuPane sender, DigitInputEventArgs e)
	{
		lock (SyncRootOnChangingPuzzles)
		{
			if (!IsRunning || _currentPuzzleIndex >= TestedPuzzlesCount)
			{
				return;
			}

			if (e is not { Candidate: var candidate and not -1 })
			{
				return;
			}

			var elapsed = _stopwatch.Elapsed;
			_answeredData[_currentPuzzleIndex] = (
				candidate,
				_currentPuzzleIndex == 0 ? elapsed : elapsed - _answeredData[_currentPuzzleIndex - 1].TimeSpan
			);

			if (++_currentPuzzleIndex >= TestedPuzzlesCount)
			{
				IsRunning = false;
			}
			else
			{
				GeneratePuzzle();
			}
		}
	}

	private void StartButton_Click(object sender, RoutedEventArgs e)
	{
		IsRunning = true;

		GeneratePuzzle();
	}

	private void Page_Loaded(object sender, RoutedEventArgs e) => SelectModeComboxBox.SelectedIndex = 0;

	private void SudokuPane_Loaded(object sender, RoutedEventArgs e)
		=> ((App)Application.Current).CoverSettingsToSudokuPaneViaApplicationTheme(SudokuPane);
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// Counts up the number of instances that satisfies the specified condition, with the specified instance as the reference,
	/// using index from another collection.
	/// </summary>
	/// <typeparam name="T1">The type of elements from the current collection.</typeparam>
	/// <typeparam name="T2">The type of elements from the other collection.</typeparam>
	/// <param name="this">The collection.</param>
	/// <param name="other">The other collection.</param>
	/// <param name="predicate">The condition.</param>
	/// <param name="count">The number of elements.</param>
	/// <returns>An <see cref="int"/> result.</returns>
	public static int CountWithSameIndex<T1, T2>(this List<T1?> @this, List<T2?> other, Func<T1?, T2?, bool> predicate, int count)
		where T1 : notnull
		where T2 : notnull
	{
		Debug.Assert(@this.Count == other.Count);

		var result = 0;
		for (var i = 0; i < count; i++)
		{
			if (predicate(@this[i], other[i]))
			{
				result++;
			}
		}

		return result;
	}

	/// <summary>
	/// Refresh the collection.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The collection.</param>
	/// <param name="count">The number of elements to be created.</param>
	public static void Refresh<T>(this List<T?> @this, int count) where T : notnull
	{
		@this.Clear();
		for (var i = 0; i < count; i++)
		{
			@this.Add(default);
		}
	}
}

/// <summary>
/// The local puzzle generator.
/// </summary>
file static class Generator
{
	/// <summary>
	/// A random number generator.
	/// </summary>
	private static readonly Random Rng = new();


	/// <summary>
	/// Try to generate a new puzzle.
	/// </summary>
	/// <param name="mode">The mode.</param>
	/// <param name="targetCandidate">Indicates the target candidate as the result.</param>
	/// <returns>The result.</returns>
	public static Grid Generate(GeneratingMode mode, out Candidate targetCandidate)
	{
		if (mode == GeneratingMode.Both)
		{
			mode = Rng.Next(0, 1000) < 500 ? GeneratingMode.FullHouse : GeneratingMode.NakedSingle;
		}

		scoped var digits = (Span<Cell>)[0, 1, 2, 3, 4, 5, 6, 7, 8];

		// Shuffle digits.
		for (var currentShuffleRound = 0; currentShuffleRound < 10; currentShuffleRound++)
		{
			var a = Rng.Next(0, 9);
			var b = Rng.Next(0, 9) is var t && t == a ? (t + 1) % 9 : t;
			(digits[a], digits[b]) = (digits[b], digits[a]);
		}

		var result = Grid.Empty;
		switch (mode)
		{
			case GeneratingMode.House5 or GeneratingMode.FullHouse:
			{
				var house = mode == GeneratingMode.House5 ? Rng.Next(0, 3) * 9 + 4 : Rng.Next(0, 27);
				var cell = HouseCells[house][Rng.Next(0, 9)];

				var i = 0;
				foreach (var otherCell in HousesMap[house] - cell)
				{
					result.SetDigit(otherCell, digits[i++]);
				}

				targetCandidate = cell * 9 + digits[^1];
				break;
			}
			case GeneratingMode.NakedSingle:
			{
				var cell = Rng.Next(0, 81);
				scoped var peers = new Span<Cell>(Peers[cell]);

				for (var currentShuffleRound = 0; currentShuffleRound < 15; currentShuffleRound++)
				{
					var a = Rng.Next(0, 20);
					var b = Rng.Next(0, 20) is var t && t == a ? (t + 1) % 20 : t;
					(peers[a], peers[b]) = (peers[b], peers[a]);
				}

				var cells = CellMap.Empty;
				foreach (var index in (0, 2, 4, 7, 9, 11, 14, 16, 18))
				{
					cells.Add(peers[index]);
				}

				var i = 0;
				foreach (var otherCell in cells - cells[Rng.Next(0, 9)])
				{
					result.SetDigit(otherCell, digits[i++]);
				}

				targetCandidate = cell * 9 + digits[^1];
				break;
			}
			default:
			{
				throw new ArgumentException("The mode is invalid, it may not be defined in the enumeration type.", nameof(mode));
			}
		}

		return result.FixedGrid;
	}
}

/// <summary>
/// Indicates the generating mode.
/// </summary>
file enum GeneratingMode
{
	/// <summary>
	/// Indicates the generator generates the puzzle that only uses row/column/block 5.
	/// </summary>
	House5 = 0,

	/// <summary>
	/// Indicates the generator generates the puzzle that relies on full houses.
	/// </summary>
	FullHouse = 1,

	/// <summary>
	/// Indicates the generator generates the puzzle that relies on naked singles.
	/// </summary>
	NakedSingle = 2,

	/// <summary>
	/// Indicates both <see cref="FullHouse"/> and <see cref="NakedSingle"/>.
	/// </summary>
	/// <seealso cref="FullHouse"/>
	/// <seealso cref="NakedSingle"/>
	Both = 3
}
