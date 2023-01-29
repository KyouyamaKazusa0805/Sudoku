namespace SudokuStudio.Views.Pages.Analyze;

/// <summary>
/// Defines a tab page that displays for graphs that describes the difficulty and analysis data of a puzzle.
/// </summary>
public sealed partial class PuzzleGraphs : Page, IAnalyzeTabPage, INotifyPropertyChanged
{
	/// <inheritdoc cref="IAnalyzeTabPage.AnalysisResult"/>
	[NotifyBackingField(DoNotEmitPropertyChangedEventTrigger = true)]
	[NotifyCallback(nameof(AnalysisResultSetterAfter))]
	private LogicalSolverResult? _analysisResult;

	/// <summary>
	/// Indicates the difficulty distribution values.
	/// </summary>
	[NotifyBackingField(ComparisonMode = EqualityComparisonMode.ObjectReference)]
	private ObservableCollection<ISeries> _difficultyDistribution = new()
	{
		new LineSeries<double>
		{
			Values = new ObservableCollection<double>(),
			GeometrySize = 0,
			Fill = null,
			GeometryStroke = null,
			IsHoverable = false
		}
	};

	/// <summary>
	/// Indicates the difficulty level proportion values.
	/// </summary>
	[NotifyBackingField(ComparisonMode = EqualityComparisonMode.ObjectReference)]
	private ObservableCollection<ISeries> _difficultyLevelProportion = new()
	{
		new PieSeries<double>
		{
			Values = new ObservableCollection<double> { 100 },
			Name = GetString("_DifficultyLevel_Easy"),
			DataLabelsFormatter = DataLabelFormatter,
			IsHoverable = false
		},
		new PieSeries<double>
		{
			Values = new ObservableCollection<double> { 0 },
			Name = GetString("_DifficultyLevel_Moderate"),
			DataLabelsFormatter = DataLabelFormatter,
			IsHoverable = false
		},
		new PieSeries<double>
		{
			Values = new ObservableCollection<double> { 0 },
			Name = GetString("_DifficultyLevel_Hard"),
			DataLabelsFormatter = DataLabelFormatter,
			IsHoverable = false
		},
		new PieSeries<double>
		{
			Values = new ObservableCollection<double> { 0 },
			Name = GetString("_DifficultyLevel_Fiendish"),
			DataLabelsFormatter = DataLabelFormatter,
			IsHoverable = false
		},
		new PieSeries<double>
		{
			Values = new ObservableCollection<double> { 0 },
			Name = GetString("_DifficultyLevel_Nightmare"),
			DataLabelsFormatter = DataLabelFormatter,
			IsHoverable = false
		},
		new PieSeries<double>
		{
			Values = new ObservableCollection<double> { 0 },
			Name = GetString("_DifficultyLevel_Unknown"),
			DataLabelsFormatter = DataLabelFormatter,
			IsHoverable = false
		}
	};


	/// <summary>
	/// Initializes a <see cref="PuzzleGraphs"/> instance.
	/// </summary>
	public PuzzleGraphs()
	{
		InitializeComponent();
		InitializeFields();
	}


	/// <inheritdoc/>
	public AnalyzePage BasePage { get; set; } = null!;


	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;



	/// <summary>
	/// Initializes for fields.
	/// </summary>
	private void InitializeFields()
	{
		for (var i = 0; i < _difficultyLevelProportion.Count; i++)
		{
			var element = (PieSeries<double>)_difficultyLevelProportion[i];
			element.Fill = new SolidColorPaint(
				i switch
				{
					0 => c(getColor(DifficultyLevel.Easy)),
					1 => c(getColor(DifficultyLevel.Moderate)),
					2 => c(getColor(DifficultyLevel.Hard)),
					3 => c(getColor(DifficultyLevel.Fiendish)),
					4 => c(getColor(DifficultyLevel.Nightmare)),
					5 => c(getColor(DifficultyLevel.Unknown))
				}
			);
		}


		static Color getColor(DifficultyLevel difficultyLevel) => DifficultyLevelConversion.GetBackgroundRawColor(difficultyLevel);

		static SKColor c(Color color) => new(color.R, color.G, color.B, color.A);
	}

	private void AnalysisResultSetterAfter(LogicalSolverResult? value)
	{
		UpdateForDifficultyDistribution(value);
		UpdateForDifficultyLevelProportion(value);
	}

	/// <summary>
	/// Update data for property <see cref="DifficultyDistribution"/>.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <seealso cref="DifficultyDistribution"/>
	private void UpdateForDifficultyDistribution(LogicalSolverResult? value)
	{
		var coll = (ObservableCollection<double>)DifficultyDistribution[0].Values!;
		coll.Clear();

		if (value is not null)
		{
			coll.AddRange(from step in value select (double)step.Difficulty);
		}

		PropertyChanged?.Invoke(this, new(nameof(DifficultyDistribution)));
	}

	/// <summary>
	/// Update data for property <see cref="DifficultyLevelProportion"/>.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <seealso cref="DifficultyLevelProportion"/>
	private void UpdateForDifficultyLevelProportion(LogicalSolverResult? value)
	{
		for (var i = 0; i < DifficultyLevelProportion.Count; i++)
		{
			var element = DifficultyLevelProportion[i];
			((ObservableCollection<double>)element.Values!)[0] = i == 0 ? 100 : 0;
		}

		if (value is not null)
		{
			foreach (var (difficultyLevel, count) in
				from step in value
				let dl = step.DifficultyLevel
				let targetDifficultyLevel = dl is DifficultyLevel.Unknown or DifficultyLevel.LastResort ? DifficultyLevel.Unknown : dl
				group step by targetDifficultyLevel into stepsGroupedByDifficultyLevel
				let dl = stepsGroupedByDifficultyLevel.Key
				let count = stepsGroupedByDifficultyLevel.Count(step => counterPredicate(step, dl))
				select (DifficultyLevel: dl, Count: count))
			{
				var index = difficultyLevel switch
				{
					DifficultyLevel.Easy => 0,
					DifficultyLevel.Moderate => 1,
					DifficultyLevel.Hard => 2,
					DifficultyLevel.Fiendish => 3,
					DifficultyLevel.Nightmare => 4,
					DifficultyLevel.Unknown => 5
				};

				((ObservableCollection<double>)DifficultyLevelProportion[index].Values!)[0] = count;
			}
		}

		PropertyChanged?.Invoke(this, new(nameof(DifficultyLevelProportion)));


		static bool counterPredicate(IStep step, DifficultyLevel key)
			=> key switch
			{
				DifficultyLevel.Unknown or DifficultyLevel.LastResort
					=> step.DifficultyLevel is DifficultyLevel.Unknown or DifficultyLevel.LastResort,
				_ => step.DifficultyLevel == key
			};
	}


	private static string DataLabelFormatter(ChartPoint<double, DoughnutGeometry, LabelGeometry> p)
		=> $"{(int)p.PrimaryValue}/{(int)p.StackedValue!.Total} ({p.StackedValue.Share:P2})";
}
