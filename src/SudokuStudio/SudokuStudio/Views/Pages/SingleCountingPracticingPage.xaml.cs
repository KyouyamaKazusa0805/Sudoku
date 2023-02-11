namespace SudokuStudio.Views.Pages;

/// <summary>
/// Represents a page that provides with practise tool to allow you practicing counting logic for technique Naked Single and Full House.
/// </summary>
[DependencyProperty<int>("SelectedMode", DefaultValue = -1)]
[DependencyProperty<int>("TestedPuzzlesCount", DefaultValue = 10)]
public sealed partial class SingleCountingPracticingPage : Page, INotifyPropertyChanged
{
	/// <summary>
	/// Indicates the maximum possible supported number of puzzles.
	/// </summary>
	private const int MaxPuzzlesCountSupported = 100;


	/// <summary>
	/// Defines a timer instance.
	/// </summary>
	private readonly Stopwatch _stopwatch = new();

	/// <summary>
	/// Indicates whether the game is running.
	/// </summary>
	[NotifyBackingField(Accessibility = GeneralizedAccessibility.Internal)]
	[NotifyCallback]
	private bool _isRunning;

	/// <summary>
	/// Indicates the puzzles last.
	/// </summary>
	private int _currentPuzzleIndex = -1;

	/// <summary>
	/// Indicates the target result data.
	/// </summary>
	private List<int> _targetResultData;

	/// <summary>
	/// Indicates the answered data.
	/// </summary>
	private List<(int Candidate, TimeSpan TimeSpan)> _answeredData;


	/// <summary>
	/// Initializes a <see cref="SingleCountingPracticingPage"/> instance.
	/// </summary>
	public SingleCountingPracticingPage()
	{
		InitializeComponent();
		InitializeEvents();
		InitializeFields();
	}


	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;


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
		var final = Generator.Generate((GeneratingMode)(SelectedMode + 1), out var targetCandidate);
		SudokuPane.Puzzle = final;
		_targetResultData[_currentPuzzleIndex] = targetCandidate;
	}

	private void IsRunningSetterAfter(bool value)
	{
		if (value)
		{
			_stopwatch.Start();
			_currentPuzzleIndex = 0;
		}
		else
		{
			_stopwatch.Stop();
			_currentPuzzleIndex = -1;

			var correctCount = _answeredData.CountWithIndex(_targetResultData, static (a, b) => a.Candidate == b, TestedPuzzlesCount);
			var totalTimeSpan = _answeredData[TestedPuzzlesCount - 1].TimeSpan;
			ResultDataDisplayer.Text = string.Format(
				GetString("SingleCountingPracticingPage_ResultDisplayLabel"),
				totalTimeSpan,
				TestedPuzzlesCount,
				totalTimeSpan / TestedPuzzlesCount,
				(double)correctCount / TestedPuzzlesCount,
				correctCount,
				TestedPuzzlesCount
			);
		}

		_targetResultData.Refresh(MaxPuzzlesCountSupported);
		_answeredData.Refresh(MaxPuzzlesCountSupported);
	}


	private void SudokuPane_DigitInput(SudokuPane sender, DigitInputEventArgs e)
	{
		if (e is not { Candidate: var candidate and not -1 })
		{
			return;
		}

		var elapsed = _stopwatch.Elapsed;
		_answeredData[_currentPuzzleIndex] =
			(candidate, _currentPuzzleIndex == 0 ? elapsed : elapsed - _answeredData[_currentPuzzleIndex - 1].TimeSpan);

		if (++_currentPuzzleIndex >= TestedPuzzlesCount)
		{
			IsRunning = false;
		}
		else
		{
			GeneratePuzzle();
		}
	}

	private void StartButton_Click(object sender, RoutedEventArgs e)
	{
		IsRunning = true;

		GeneratePuzzle();
	}

	private void Page_Loaded(object sender, RoutedEventArgs e) => SelectModeComboxBox.SelectedIndex = 0;
}

/// <include file='../../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
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
	public static int CountWithIndex<T1, T2>(this List<T1?> @this, List<T2?> other, Func<T1?, T2?, bool> predicate, int count)
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
	public static Grid Generate(GeneratingMode mode, out int targetCandidate)
	{
		Debug.Assert(Enum.IsDefined(mode));

		if (!IsPow2((int)mode))
		{
			mode = Rng.Next(0, 1000) < 500 ? GeneratingMode.FullHouseOnly : GeneratingMode.NakedSingleOnly;
		}

		scoped var digits = (stackalloc[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 });

		// Shuffle digits.
		for (var currentShuffleRound = 0; currentShuffleRound < 10; currentShuffleRound++)
		{
			var a = Rng.Next(0, 9);
			var b = Rng.Next(0, 9) is var t && t == a ? (t + 1) % 9 : t;
			(digits[a], digits[b]) = (digits[b], digits[a]);
		}

		var result = Grid.Empty;
		if (mode == GeneratingMode.FullHouseOnly)
		{
			var house = Rng.Next(0, 27);
			var cell = HouseCells[house][Rng.Next(0, 9)];

			var i = 0;
			foreach (var otherCell in HousesMap[house] - cell)
			{
				result[otherCell] = digits[i++];
			}

			targetCandidate = cell * 9 + digits[^1];
		}
		else
		{
			var cell = Rng.Next(0, 81);
			scoped var peers = new Span<int>(Peers[cell]);

			for (var currentShuffleRound = 0; currentShuffleRound < 15; currentShuffleRound++)
			{
				var a = Rng.Next(0, 20);
				var b = Rng.Next(0, 20) is var t && t == a ? (t + 1) % 20 : t;
				(peers[a], peers[b]) = (peers[b], peers[a]);
			}

			var cells = CellMap.Empty;
			foreach (var index in stackalloc[] { 0, 2, 4, 7, 9, 11, 14, 16, 18 })
			{
				cells.Add(peers[index]);
			}

			var i = 0;
			foreach (var otherCell in cells - cells[Rng.Next(0, 9)])
			{
				result[otherCell] = digits[i++];
			}

			targetCandidate = cell * 9 + digits[^1];
		}

		result.Fix();
		return result;
	}
}

/// <summary>
/// Indicates the generating mode.
/// </summary>
[Flags]
file enum GeneratingMode : int
{
	/// <summary>
	/// Indicates the mode is none.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the generator generates the puzzle that relies on full houses.
	/// </summary>
	FullHouseOnly = 1,

	/// <summary>
	/// Indicates the generator generates the puzzle that relies on naked singles.
	/// </summary>
	NakedSingleOnly = 1 << 1
}
