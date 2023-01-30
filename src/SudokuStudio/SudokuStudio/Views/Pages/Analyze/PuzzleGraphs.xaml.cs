using LiveChartsCore.SkiaSharpView.Drawing;

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
			Stroke = new SolidColorPaint { Color = SKColors.SkyBlue, StrokeThickness = 1 }
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
			Stroke = new SolidColorPaint { Color = SKColors.SkyBlue, StrokeThickness = 1 },
			DataLabelsSize = 12,
			DataLabelsPosition = PolarLabelsPosition.End,
			DataLabelsPaint = new SolidColorPaint { Color = SKColors.Black },
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

	/// <summary>
	/// Difficulty distribution sections.
	/// </summary>
	internal Section<SkiaSharpDrawingContext>[] DifficultyDistributionSections { get; set; } = new RectangularSection[]
	{
		new RectangularSection
		{
			Yi = 3.4,
			Yj = 3.4,
			Stroke = new SolidColorPaint
			{
				Color = DifficultyLevelConversion.GetBackgroundRawColor(DifficultyLevel.Moderate).AsSKColor(),
				StrokeThickness = 1
			}
		},
		new RectangularSection
		{
			Yi = 4.8,
			Yj = 4.8,
			Stroke = new SolidColorPaint
			{
				Color = DifficultyLevelConversion.GetBackgroundRawColor(DifficultyLevel.Hard).AsSKColor(),
				StrokeThickness = 1
			}
		},
		new RectangularSection
		{
			Yi = 5.9,
			Yj = 5.9,
			Stroke = new SolidColorPaint
			{
				Color = DifficultyLevelConversion.GetBackgroundRawColor(DifficultyLevel.Fiendish).AsSKColor(),
				StrokeThickness = 1
			}
		},
		new RectangularSection
		{
			Yi = 8.7,
			Yj = 8.7,
			Stroke = new SolidColorPaint
			{
				Color = DifficultyLevelConversion.GetBackgroundRawColor(DifficultyLevel.Nightmare).AsSKColor(),
				StrokeThickness = 1
			}
		},
		new RectangularSection
		{
			Yi = 12.0,
			Yj = 12.0,
			Stroke = new SolidColorPaint
			{
				Color = DifficultyLevelConversion.GetBackgroundRawColor(DifficultyLevel.Unknown).AsSKColor(),
				StrokeThickness = 1
			}
		}
	};

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
			element.Fill = new SolidColorPaint(
				i switch
				{
					0 => getColor(DifficultyLevel.Easy),
					1 => getColor(DifficultyLevel.Moderate),
					2 => getColor(DifficultyLevel.Hard),
					3 => getColor(DifficultyLevel.Fiendish),
					4 => getColor(DifficultyLevel.Nightmare),
					5 => getColor(DifficultyLevel.Unknown)
				}
			);
		}


		static string dataLabelFormatter(ChartPoint<double, DoughnutGeometry, LabelGeometry> p, string difficultyLevelName)
			=> p switch
			{
				{ StackedValue.Share: 0 } => string.Empty,
				{ StackedValue.Share: var percent, PrimaryValue: var a, StackedValue.Total: var b } when !percent.NearlyEquals(0, 1E-2)
					=> $"{difficultyLevelName}{GetString("_Token_Colon")}{(int)a}/{(int)b} ({percent:P2})",
				_ => string.Empty
			};

		static SKColor getColor(DifficultyLevel difficultyLevel) => DifficultyLevelConversion.GetBackgroundRawColor(difficultyLevel).AsSKColor();
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
			coll[0] = getExerciziability(value);
			coll[1] = getRarity(value);
			coll[2] = getDirectability(value);
		}

		PropertyChanged?.Invoke(this, new(nameof(PuzzleArgumentsPolar)));


		static double getExerciziability(LogicalSolverResult value)
		{
			var techniqueUsedDic = new Dictionary<Technique, List<IStep>>();
			foreach (var step in value)
			{
				if (step is { TechniqueCode: var technique, DifficultyLevel: not DifficultyLevel.Easy }
					&& !techniqueUsedDic.TryAdd(technique, new()))
				{
					techniqueUsedDic[technique].Add(step);
				}
			}

			if (techniqueUsedDic.Count == 1 && techniqueUsedDic.First().Value.Count == 1)
			{
				return 100;
			}

			var total = 300;
			foreach (var (technique, steps) in techniqueUsedDic)
			{
				foreach (var step in steps)
				{
					total -= step.DifficultyLevel switch
					{
						DifficultyLevel.Moderate => 1,
						DifficultyLevel.Hard => 2,
						DifficultyLevel.Fiendish => 4,
						DifficultyLevel.Nightmare => 8,
						_ => 0
					};
				}
			}

			return (int)(Clamp(total, 0, 300) / 3D);
		}

		static double getRarity(LogicalSolverResult value)
		{
			var total = 300;
			foreach (var step in value)
			{
				total -= step.Rarity switch
				{
					Rarity.Always or Rarity.ReplacedByOtherTechniques or Rarity.OnlyForSpecialPuzzles => 0,
					Rarity.HardlyEver => 1,
					Rarity.Seldom => 2,
					Rarity.Sometimes => 4,
					Rarity.Often => 8,
					_ => 0
				};
			}

			return (int)(Clamp(total, 0, 300) / 3D);
		}

		static double getDirectability(LogicalSolverResult value)
		{
			var total = 300D;

			foreach (var step in value)
			{
				total -= step switch
				{
					{ TechniqueCode: Technique.NakedSingle } => .5,
					{ TechniqueCode: Technique.HiddenSingleColumn or Technique.HiddenSingleRow } => .25,
					{ DifficultyLevel: DifficultyLevel.Easy } => 0,
					{ DifficultyLevel: DifficultyLevel.Moderate } => 1,
					{ DifficultyLevel: DifficultyLevel.Hard } => 3,
					{ DifficultyLevel: DifficultyLevel.Fiendish } => 6,
					{ DifficultyLevel: DifficultyLevel.Nightmare } => 10,
					_ => 10
				};
			}

			return (int)(Clamp(total, 0, 300) / 3);
		}
	}
}
