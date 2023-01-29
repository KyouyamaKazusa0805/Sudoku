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
		new PieSeries<double>(),
		new PieSeries<double>(),
		new PieSeries<double>(),
		new PieSeries<double>(),
		new PieSeries<double>(),
		new PieSeries<double>()
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
			var iterationVariableCopied = i; // Lambda should not directly capture iteration variables.

			var element = (PieSeries<double>)_difficultyLevelProportion[i];
			element.Values = new ObservableCollection<double> { i == 0 ? 100 : 0 };
			element.DataLabelsSize = 12;
			element.DataLabelsFormatter =
				chartPoint => dataLabelFormatter(
					chartPoint,
					iterationVariableCopied switch // Here we cannot use 'i switch' because here is inside a lambda; otherwise 'i' always be 6.
					{
						0 => GetString("_DifficultyLevel_Easy"),
						1 => GetString("_DifficultyLevel_Moderate"),
						2 => GetString("_DifficultyLevel_Hard"),
						3 => GetString("_DifficultyLevel_Fiendish"),
						4 => GetString("_DifficultyLevel_Nightmare"),
						5 => GetString("_DifficultyLevel_Other")
					}
				);
			element.DataLabelsPosition = PolarLabelsPosition.Outer;
			element.DataLabelsPaint = new SolidColorPaint
			{
				Color = SKColors.Black,
				SKTypeface = SKFontManager.Default.MatchCharacter('\u6c49') // '\u6c49': Chinese character "Han" (e.g. "Han Yu" - Chinese)
			};
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

		static string dataLabelFormatter(ChartPoint<double, DoughnutGeometry, LabelGeometry> p, string difficultyLevelName)
			=> $"{difficultyLevelName}{GetString("_Token_Colon")}{(int)p.PrimaryValue}/{(int)p.StackedValue!.Total} ({p.StackedValue.Share:P2})";

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
}
