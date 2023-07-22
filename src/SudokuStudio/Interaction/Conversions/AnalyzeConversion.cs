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

	public static string GetEliminationString(Step step) => ConclusionFormatter.Format(step.Conclusions, FormattingMode.Normal);

	public static string GetDifficultyRatingText(Step step) => step.Difficulty.ToString("0.0");

	public static string GetIndexText(SolvingPathStepBindableSource step) => (step.Index + 1).ToString();

	public static string GetViewIndexDisplayerString(IRenderable? visualUnit, int currentIndex)
		=> visualUnit?.Views?.Length is { } length ? $"{currentIndex + 1}/{length}" : "0/0";

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

	public static IEnumerable<Inline> GetInlinesOfTooltip(SolvingPathStepBindableSource s)
	{
		if (s is not
			{
				Index: var index,
				DisplayItems: var displayKind,
				Step:
				{
					Name: var name,
					Code: var technique,
					BaseDifficulty: var baseDifficulty,
					Difficulty: var difficulty,
					ExtraDifficultyCases: var cases
				} step
			})
		{
			throw new ArgumentException($"The argument '{nameof(s)}' is invalid.", nameof(s));
		}

		var result = new List<Inline>();

		if (displayKind.Flags(StepTooltipDisplayItems.TechniqueName))
		{
			result.Add(new Run { Text = GetString("AnalyzePage_TechniqueName") }.SingletonSpan<Bold>());
			result.Add(new LineBreak());
			result.Add(new Run { Text = name });
		}

		if (displayKind.Flags(StepTooltipDisplayItems.TechniqueIndex))
		{
			appendEmptyLinesIfNeed();

			result.Add(new Run { Text = GetString("AnalyzePage_TechniqueIndex") }.SingletonSpan<Bold>());
			result.Add(new LineBreak());
			result.Add(new Run { Text = (index + 1).ToString() });
		}

		if (displayKind.Flags(StepTooltipDisplayItems.Abbreviation))
		{
			appendEmptyLinesIfNeed();

			result.Add(new Run { Text = GetString("AnalyzePage_Abbreviation") }.SingletonSpan<Bold>());
			result.Add(new LineBreak());
			result.Add(new Run { Text = technique.GetAbbreviation() ?? GetString("AnalyzePage_None") });
		}

		if (displayKind.Flags(StepTooltipDisplayItems.Aliases))
		{
			appendEmptyLinesIfNeed();

			result.Add(new Run { Text = GetString("AnalyzePage_Aliases") }.SingletonSpan<Bold>());
			result.Add(new LineBreak());
			result.Add(
				new Run
				{
					Text = technique.GetAliases() is { } aliases and not []
						? string.Join(GetString("_Token_Comma"), aliases)
						: GetString("AnalyzePage_None")
				}
			);
		}

		if (displayKind.Flags(StepTooltipDisplayItems.DifficultyRating))
		{
			appendEmptyLinesIfNeed();

			result.Add(new Run { Text = GetString("AnalyzePage_TechniqueDifficultyRating") }.SingletonSpan<Bold>());
			result.Add(new LineBreak());
			result.Add(new Run { Text = difficulty.ToString("0.0") });
		}

		if (displayKind.Flags(StepTooltipDisplayItems.ExtraDifficultyCases))
		{
			appendEmptyLinesIfNeed();

			result.Add(new Run { Text = GetString("AnalyzePage_ExtraDifficultyCase") }.SingletonSpan<Bold>());
			result.Add(new LineBreak());

			switch (cases)
			{
				case { Length: not 0 }:
				{
					result.Add(new Run { Text = $"{GetString("AnalyzePage_BaseDifficulty")}{baseDifficulty:0.0}" });
					result.Add(new LineBreak());
					result.AddRange(appendExtraDifficultyCases(cases));

					break;
				}
				default:
				{
					result.Add(new Run { Text = GetString("AnalyzePage_None") });

					break;
				}
			}
		}

		if (displayKind.Flags(StepTooltipDisplayItems.SimpleDescription))
		{
			appendEmptyLinesIfNeed();

			result.Add(new Run { Text = GetString("AnalyzePage_SimpleDescription") }.SingletonSpan<Bold>());
			result.Add(new LineBreak());
			result.Add(new Run { Text = step.ToString() });
		}

		return result;

		static IEnumerable<Inline> appendExtraDifficultyCases(ExtraDifficultyCase[] cases)
		{
			for (var i = 0; i < cases.Length; i++)
			{
				var (name, value) = cases[i];
				var extraDifficultyName = StringsAccessor.GetString($"{nameof(ExtraDifficultyCaseNames)}_{name}");
				yield return new Run { Text = $"{extraDifficultyName}{Token("Colon")}+{value:0.0}" };

				if (i != cases.Length - 1)
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

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// Determines whether an <see cref="IEnumerable"/> collection has no elements in it.
	/// </summary>
	/// <param name="this">The element.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool None(this IEnumerable @this) => !@this.GetEnumerator().MoveNext();

	/// <summary>
	/// Creates a <see cref="Bold"/> instance with a singleton value of <see cref="Run"/>.
	/// </summary>
	/// <param name="this">The <see cref="Run"/> instance.</param>
	/// <returns>A <see cref="Bold"/> instance.</returns>
	public static T SingletonSpan<T>(this Run @this) where T : Span, new()
	{
		var result = new T();
		result.Inlines.Add(@this);

		return result;
	}
}
