namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about analyze tab pages.
/// </summary>
internal static class AnalyzeConversion
{
	public static bool GetIsEnabled(Grid grid) => grid is { IsSolved: false, SolutionGrid.IsUndefined: false };

	public static bool GetAnalyzerButtonIsEnabled(bool isGeneratorLaunched) => !isGeneratorLaunched;

	public static bool GetProgressRingIsIntermediate(bool isAnalyzerLaunched, bool isGathererLaunched, bool isGeneratorLaunched)
		=> (isAnalyzerLaunched, isGathererLaunched, isGeneratorLaunched) switch { (_, _, true) => true, _ => false };

	public static bool GetProgressRingIsActive(bool isAnalyzerLaunched, bool isGathererLaunched, bool isGeneratorLaunched)
		=> (isAnalyzerLaunched, isGathererLaunched, isGeneratorLaunched) switch { (_, _, true) => false, _ => true };

	public static int GetViewPipsPagerPageCount(IRenderable? renderable) => renderable?.Views?.Length ?? 0;

	public static int GetCurrentViewIndexForViewPipsPager(int currentIndex) => currentIndex;

	public static double GetWidth_HodokuRatingText(bool showing) => showing ? 40 : 0;

	public static double GetWidth_SudokuExplainerText(bool showing) => showing ? 60 : 0;

	public static string GetEliminationString(Step step) => step.Options.Converter.ConclusionConverter(step.Conclusions);

	public static string GetDifficultyRatingText(Step step) => step.Difficulty.ToString("0.0");

	public static string GetDifficultyRatingText_Hodoku(Step step)
		=> HodokuCompatibility.GetDifficultyRating(step.Code, out _) is { } r ? r.ToString() : string.Empty;

	public static string GetDifficultyRatingText_SudokuExplainer(Step step)
		=> SudokuExplainerCompatibility.GetDifficultyRatingRange(step.Code) switch
		{
			({ IsRange: false, Min: var d }, _) => $"{d:0.0}",
			({ IsRange: true, Min: var d1, Max: var d2 }, _) => $"{d1:0.0}-{d2:0.0}",
			(_, { IsRange: false, Min: var d }) => $"{d:0.0}",
			(_, { IsRange: true, Min: var d1, Max: var d2 }) => $"{d1:0.0}-{d2:0.0}",
			_ => string.Empty
		};

	public static string GetIndexText(SolvingPathStepBindableSource step) => (step.Index + 1).ToString();

	public static string GetViewIndexDisplayerString(IRenderable? visualUnit, int currentIndex)
		=> visualUnit?.Views?.Length is { } length ? $"{currentIndex + 1}/{length}" : "0/0";

	public static string GetName(Step step) => step.GetName(App.CurrentCulture);

	public static string GetSimpleString(Step step) => step.ToSimpleString(App.CurrentCulture);

	public static Thickness GetMargin_HodokuRating(bool showing) => showing ? new(12, 0, 0, 0) : new();

	public static Thickness GetMargin_SudokuExplainerRating(bool showing) => showing ? new(12, 0, 0, 0) : new();

	public static Visibility GetProgressRingVisibility(bool isAnalyzerLaunched, bool isGathererLaunched, bool isGeneratorLaunched)
		=> isAnalyzerLaunched || isGathererLaunched || isGeneratorLaunched ? Visibility.Visible : Visibility.Collapsed;

	public static Visibility GetAnalyzeTabsVisibility(bool isAnalyzerLaunched, bool isGathererLaunched, bool isGeneratorLaunched)
		=> isAnalyzerLaunched || isGathererLaunched || isGeneratorLaunched ? Visibility.Collapsed : Visibility.Visible;

	public static Visibility GetDifficultyRatingVisibility(bool showDifficultyRating)
		=> showDifficultyRating ? Visibility.Visible : Visibility.Collapsed;

	public static Visibility GetSummaryTableVisibility(IEnumerable itemsSource)
		=> itemsSource is null || itemsSource.None() ? Visibility.Collapsed : Visibility.Visible;

	public static Visibility GetSolvingPathListVisibility(object itemsSource)
		=> itemsSource switch { SolvingPathStepCollection and not [] => Visibility.Visible, _ => Visibility.Collapsed };

	public static Visibility GetViewPipsPagerVisibility(IRenderable? renderable)
		=> renderable switch { { Views.Length: >= 2 } => Visibility.Visible, _ => Visibility.Collapsed };

	public static Visibility GetEnglishNameTextBlockVisibility()
		=> ((App)Application.Current).Preference.AnalysisPreferences.AlsoDisplayEnglishNameOfStep ? Visibility.Visible : Visibility.Collapsed;

	public static IEnumerable<Inline> GetInlinesOfTooltip(SolvingPathStepBindableSource s)
	{
		if (s is not
			{
				Index: var index,
				DisplayItems: var displayKind,
				Step:
				{
					Code: var technique,
					BaseDifficulty: var baseDifficulty,
					Difficulty: var difficulty,
					ExtraDifficultyFactors: var cases
				} step
			})
		{
			throw new ArgumentException($"The argument '{nameof(s)}' is invalid.", nameof(s));
		}

		var result = new List<Inline>();

		if (displayKind.Flags(StepTooltipDisplayItems.TechniqueName))
		{
			result.Add(new Run { Text = ResourceDictionary.Get("AnalyzePage_TechniqueName", App.CurrentCulture) }.SingletonSpan<Bold>());
			result.Add(new LineBreak());
			result.Add(new Run { Text = step.GetName(App.CurrentCulture) });
		}

		if (displayKind.Flags(StepTooltipDisplayItems.TechniqueIndex))
		{
			appendEmptyLinesIfNeed();

			result.Add(new Run { Text = ResourceDictionary.Get("AnalyzePage_TechniqueIndex", App.CurrentCulture) }.SingletonSpan<Bold>());
			result.Add(new LineBreak());
			result.Add(new Run { Text = (index + 1).ToString() });
		}

		if (displayKind.Flags(StepTooltipDisplayItems.Abbreviation))
		{
			appendEmptyLinesIfNeed();

			result.Add(new Run { Text = ResourceDictionary.Get("AnalyzePage_Abbreviation", App.CurrentCulture) }.SingletonSpan<Bold>());
			result.Add(new LineBreak());
			result.Add(new Run { Text = technique.GetAbbreviation() ?? ResourceDictionary.Get("AnalyzePage_None", App.CurrentCulture) });
		}

		if (displayKind.Flags(StepTooltipDisplayItems.Aliases))
		{
			appendEmptyLinesIfNeed();

			result.Add(new Run { Text = ResourceDictionary.Get("AnalyzePage_Aliases", App.CurrentCulture) }.SingletonSpan<Bold>());
			result.Add(new LineBreak());
			result.Add(
				new Run
				{
					Text = technique.GetAliases(App.CurrentCulture) is { } aliases and not []
						? string.Join(ResourceDictionary.Get("_Token_Comma", App.CurrentCulture), aliases)
						: ResourceDictionary.Get("AnalyzePage_None", App.CurrentCulture)
				}
			);
		}

		if (displayKind.Flags(StepTooltipDisplayItems.DifficultyRating))
		{
			appendEmptyLinesIfNeed();

			result.Add(new Run { Text = ResourceDictionary.Get("AnalyzePage_TechniqueDifficultyRating", App.CurrentCulture) }.SingletonSpan<Bold>());
			result.Add(new LineBreak());
			result.Add(new Run { Text = difficulty.ToString("0.0") });
		}

		if (displayKind.Flags(StepTooltipDisplayItems.ExtraDifficultyCases))
		{
			appendEmptyLinesIfNeed();

			result.Add(new Run { Text = ResourceDictionary.Get("AnalyzePage_ExtraDifficultyCase", App.CurrentCulture) }.SingletonSpan<Bold>());
			result.Add(new LineBreak());

			switch (cases)
			{
				case { Length: not 0 }:
				{
					result.Add(new Run { Text = $"{ResourceDictionary.Get("AnalyzePage_BaseDifficulty", App.CurrentCulture)}{baseDifficulty:0.0}" });
					result.Add(new LineBreak());
					result.AddRange(appendExtraDifficultyFactors(cases));

					break;
				}
				default:
				{
					result.Add(new Run { Text = ResourceDictionary.Get("AnalyzePage_None", App.CurrentCulture) });

					break;
				}
			}
		}

		if (displayKind.Flags(StepTooltipDisplayItems.SimpleDescription))
		{
			appendEmptyLinesIfNeed();

			result.Add(new Run { Text = ResourceDictionary.Get("AnalyzePage_SimpleDescription", App.CurrentCulture) }.SingletonSpan<Bold>());
			result.Add(new LineBreak());
			result.Add(new Run { Text = step.ToString(App.CurrentCulture) });
		}

		return result;


		static IEnumerable<Inline> appendExtraDifficultyFactors(ExtraDifficultyFactor[] factors)
		{
			for (var i = 0; i < factors.Length; i++)
			{
				var factor = factors[i];
				var extraDifficultyName = factor.ToString(App.CurrentCulture);
				yield return new Run { Text = $"{extraDifficultyName}{ResourceDictionary.Get("_Token_Colon", App.CurrentCulture)}+{factor.Value:0.0}" };

				if (i != factors.Length - 1)
				{
					yield return new LineBreak();
				}
			}
		}

		void appendEmptyLinesIfNeed()
		{
			if (result.Count != 0)
			{
				result.Add(new LineBreak());
				result.Add(new LineBreak());
			}
		}
	}
}
