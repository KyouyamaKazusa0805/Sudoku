namespace SudokuStudio.Configuration;

/// <summary>
/// Represents with preference items that is used by <see cref="Analyzer"/>, for the ordering of <see cref="StepSearcher"/>s.
/// </summary>
/// <seealso cref="Analyzer"/>
/// <seealso cref="StepSearcher"/>
public sealed partial class StepSearcherOrderingPreferenceGroup : PreferenceGroup
{
	[Default]
	private static readonly ObservableCollection<StepSearcherInfo> StepSearchersOrderDefaultValue = new(
		(
			from searcher in StepSearcherPool.StepSearchers
			select new StepSearcherInfo
			{
				IsEnabled = searcher.RunningArea.HasFlag(StepSearcherRunningArea.Searching),
				Name = searcher.ToString(),
				TypeName = searcher.GetType().Name
			}
		).ToArray()
	);


	/// <summary>
	/// Indicates the order of step searchers.
	/// </summary>
	[AutoDependencyProperty]
	public partial ObservableCollection<StepSearcherInfo> StepSearchersOrder { get; set; }


	[Callback]
	private static void StepSearchersOrderPropertyCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
	{
		if (e is not { NewValue: ObservableCollection<StepSearcherInfo> stepSearchers })
		{
			return;
		}

		if (((App)Application.Current).Analyzer is not { } analyzer)
		{
			return;
		}

		analyzer.WithStepSearchers([.. from s in stepSearchers select s.CreateStepSearcher()]);
	}
}
