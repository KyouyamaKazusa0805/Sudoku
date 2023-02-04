namespace SudokuStudio.Views.Pages.Analyze;

/// <summary>
/// Defines a tab page that displays for graphs that describes the difficulty and analysis data of a puzzle.
/// </summary>
public sealed partial class PuzzleGraphs : Page, IAnalyzeTabPage, INotifyPropertyChanged
{
	/// <summary>
	/// Indicates the "Han" character.
	/// </summary>
	private const char HanCharacter = '\u6c49';


	/// <inheritdoc cref="IAnalyzeTabPage.AnalysisResult"/>
	[NotifyBackingField(DisableEventTrigger = true)]
	[NotifyCallback]
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
			Stroke = new SolidColorPaint { Color = SKColors.SkyBlue, StrokeThickness = 1.5F }
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
	/// Indicates the multiple arguments that describes the current puzzle.
	/// </summary>
	[NotifyBackingField(ComparisonMode = EqualityComparisonMode.ObjectReference)]
	private ObservableCollection<ISeries> _puzzleArgumentsPolar = new()
	{
		new PolarLineSeries<double>
		{
			Values = new ObservableCollection<double> { 0, 0, 0, 100 },
			LineSmoothness = 0,
			GeometrySize = 0,
			Fill = new SolidColorPaint { Color = SKColors.SkyBlue.WithAlpha(96) },
			GeometryStroke = null,
			Stroke = null,
			DataLabelsSize = 12,
			DataLabelsPosition = PolarLabelsPosition.End,
			DataLabelsPaint = new SolidColorPaint { Color = SKColors.Black },
		}
	};

	/// <summary>
	/// Difficulty distribution sections.
	/// </summary>
	[NotifyBackingField(ComparisonMode = EqualityComparisonMode.ObjectReference)]
	private ObservableCollection<Section<SkiaSharpDrawingContext>> _difficultyDistributionSections = new()
	{
		new RectangularSection { Yi = 2.4, Yj = 2.4 },
		new RectangularSection { Yi = 3.8, Yj = 3.8 },
		new RectangularSection { Yi = 4.9, Yj = 4.9 },
		new RectangularSection { Yi = 7.7, Yj = 7.7 },
		new RectangularSection { Yi = 11.0, Yj = 11.0 }
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

	/// <summary>
	/// Difficulty distribution axes X.
	/// </summary>
	internal ICartesianAxis[] DifficultyDistributionAxesX { get; set; } = new ICartesianAxis[]
	{
		new Axis { Name = GetString("AnalyzePage_DifficultyDistributionXLabel"), LabelsPaint = null, NamePaint = DefaultNameLabelPaint }
	};

	/// <summary>
	/// Difficulty distribution axes Y.
	/// </summary>
	internal ICartesianAxis[] DifficultyDistributionAxesY { get; set; } = new ICartesianAxis[]
	{
		new Axis { Name = GetString("AnalyzePage_DifficultyDistributionYLabel"), LabelsPaint = null, NamePaint = DefaultNameLabelPaint }
	};

	/// <summary>
	/// Radius axes.
	/// </summary>
	internal IPolarAxis[] RadiusAxes { get; set; } = new IPolarAxis[] { new PolarAxis { LabelsPaint = null } };

	/// <summary>
	/// Polar axes.
	/// </summary>
	internal IPolarAxis[] PolarAxes { get; set; } = new IPolarAxis[]
	{
		new PolarAxis
		{
			Name = GetString("AnalyzePage_ArgumentsPolarScoreName"),
			NamePaint = DefaultNameLabelPaint,
			LabelsRotation = LiveCharts.TangentAngle,
			LabelsPaint = DefaultNameLabelPaint,
			LabelsBackground = LvcColor.Empty,
			Labels = new[]
			{
				GetString("AnalyzePage_PuzzleExerciziability"),
				GetString("AnalyzePage_PuzzleRarity"),
				GetString("AnalyzePage_PuzzleDirectability"),
				GetString("AnalyzePage_MaxValueLegend")
			}
		}
	};


	/// <summary>
	/// Default name or label <see cref="Paint"/> instance.
	/// </summary>
	private static Paint DefaultNameLabelPaint
		=> new SolidColorPaint { Color = SKColors.Black, SKTypeface = SKFontManager.Default.MatchCharacter(HanCharacter) };


	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;


	/// <summary>
	/// Initializes for fields.
	/// </summary>
	private void InitializeFields()
	{
		for (var i = 0; i < _difficultyLevelProportion.Count; i++)
		{
			var i2 = i;
			var element = (PieSeries<double>)_difficultyLevelProportion[i];
			element.Values = new ObservableCollection<double> { i == 0 ? 100 : 0 };
			element.DataLabelsSize = 12;
			element.DataLabelsFormatter =
				chartPoint => dataLabelFormatter(
					chartPoint,
					i2 switch // Here we cannot use variable 'i' because here is inside a lambda; otherwise 'i' always be 6.
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
			element.DataLabelsPaint = DefaultNameLabelPaint;
		}


		static string dataLabelFormatter(ChartPoint<double, DoughnutGeometry, LabelGeometry> p, string difficultyLevelName)
			=> p switch
			{
				{ StackedValue.Share: 0 } => string.Empty,
				{ StackedValue.Share: var percent, PrimaryValue: var a, StackedValue.Total: var b } when !percent.NearlyEquals(0, 1E-2)
					=> $"{difficultyLevelName}{GetString("_Token_Colon")}{(int)a}/{(int)b} ({percent:P2})",
				_ => string.Empty
			};
	}

	private void AnalysisResultSetterAfter(LogicalSolverResult? value)
	{
		UpdateForDifficultyDistribution(value);
		UpdateForDifficultyLevelProportion(value);
		UpdatePuzzleArgumentsPolar(value);
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
			coll.AddRange(from step in value select (double)(step.Difficulty - 1.0M));
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
			((ObservableCollection<double>)DifficultyLevelProportion[i].Values!)[0] = i == 0 ? 100 : 0;
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

	/// <summary>
	/// Update data for property <see cref="PuzzleArgumentsPolar"/>.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <seealso cref="PuzzleArgumentsPolar"/>
	private void UpdatePuzzleArgumentsPolar(LogicalSolverResult? value)
	{
		var coll = (ObservableCollection<double>)PuzzleArgumentsPolar[0].Values!;
		coll[0] = coll[1] = coll[2] = 0;

		if (value is not null)
		{
			var rater = new Rater(value);

			coll[0] = rater.Exerciziability;
			coll[1] = rater.Rarity;
			coll[2] = rater.Directability;
		}

		PropertyChanged?.Invoke(this, new(nameof(PuzzleArgumentsPolar)));
	}


	private void Page_Loaded(object sender, RoutedEventArgs e)
	{
		var fields = Enum.GetValues<DifficultyLevel>();
		for (var i = 0; i < DifficultyDistributionSections.Count; i++)
		{
			DifficultyDistributionSections[i].Stroke = new SolidColorPaint
			{
				Color = DifficultyLevelConversion.GetBackgroundRawColor(fields[2..][i]).AsSKColor(),
				StrokeThickness = 1
			};
		}

		for (var i = 0; i < DifficultyLevelProportion.Count; i++)
		{
			((PieSeries<double>)DifficultyLevelProportion[i]).Fill = new SolidColorPaint(getColor(fields[1..][i]));
		}


		static SKColor getColor(DifficultyLevel level) => DifficultyLevelConversion.GetBackgroundRawColor(level).AsSKColor();
	}
}
