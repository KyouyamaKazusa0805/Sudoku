namespace SudokuStudio.Views.Pages.Analyze;

/// <summary>
/// Defines the solving path page.
/// </summary>
public sealed partial class SolvingPath : Page, IAnalyzeTabPage, INotifyPropertyChanged
{
	/// <summary>
	/// Indicates the analysis result.
	/// </summary>
	[NotifyBackingField(DoNotEmitPropertyChangedEventTrigger = true)]
	[NotifyCallback(nameof(AnalysisResultSetterAfter))]
	private LogicalSolverResult? _analysisResult;

	/// <summary>
	/// Indicates the tooltip display kind.
	/// </summary>
	[NotifyBackingField]
	private StepTooltipDisplayKind _stepTooltipDisplayKind = StepTooltipDisplayKind.TechniqueName
		| StepTooltipDisplayKind.DifficultyRating
		| StepTooltipDisplayKind.TechniqueIndex
		| StepTooltipDisplayKind.SimpleDescription;


	/// <summary>
	/// Initializes a <see cref="SolvingPath"/> instance.
	/// </summary>
	public SolvingPath() => InitializeComponent();


	/// <inheritdoc/>
	public AnalyzePage BasePage { get; set; } = null!;


	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;


	private void AnalysisResultSetterAfter(LogicalSolverResult? value)
		=> SolvingPathList.ItemsSource = value is null ? null : SolvingPathStepCollection.Create(value);


	private void ListViewItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
	{
		if (sender is not ListViewItem { Tag: SolvingPathStep step } item)
		{
			return;
		}

		Dialog_StepInfo.Target = item;
		TextBlockBindable.SetInlines(StepInfoLabel, createInlines(step, StepTooltipDisplayKind));

		Dialog_StepInfo.IsOpen = true;


		static IEnumerable<Inline> createInlines(SolvingPathStep s, StepTooltipDisplayKind displayKind)
		{
			var (index, _, step) = s;
			var result = new List<Inline>();

			if (displayKind.Flags(StepTooltipDisplayKind.TechniqueName))
			{
				result.Add(new Run { Text = GetString("AnalyzePage_TechniqueName") }.SingletonSpan<Bold>());
				result.Add(new LineBreak());
				result.Add(new Run { Text = step.Name });
			}

			if (displayKind.Flags(StepTooltipDisplayKind.TechniqueIndex))
			{
				f();

				result.Add(new Run { Text = GetString("AnalyzePage_TechniqueIndex") }.SingletonSpan<Bold>());
				result.Add(new LineBreak());
				result.Add(new Run { Text = (index + 1).ToString() });
			}

			if (displayKind.Flags(StepTooltipDisplayKind.DifficultyRating))
			{
				f();

				result.Add(new Run { Text = GetString("AnalyzePage_TechniqueDifficultyRating") }.SingletonSpan<Bold>());
				result.Add(new LineBreak());
				result.Add(new Run { Text = step.Difficulty.ToString("0.0") });
			}

			if (displayKind.Flags(StepTooltipDisplayKind.SimpleDescription))
			{
				f();

				result.Add(new Run { Text = GetString("AnalyzePage_SimpleDescription") }.SingletonSpan<Bold>());
				result.Add(new LineBreak());
				result.Add(new Run { Text = step.ToString()! });
			}

			return result;


			void f()
			{
				if (result.Count != 0)
				{
					result.Add(new LineBreak());
					result.Add(new LineBreak());
				}
			}
		}
	}

	private void ListViewItem_Tapped(object sender, TappedRoutedEventArgs e)
	{
		if ((this, sender) is not ({ BasePage.SudokuPane: var pane }, ListViewItem { Tag: SolvingPathStep(_, var stepGrid, _) }))
		{
			return;
		}

		pane.SetPuzzle(stepGrid, clearStack: true, clearAnalyzeTabData: false);
	}
}

/// <include file='../../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// Creates a <see cref="Bold"/> ionstance with a singleton value of <see cref="Run"/>.
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
