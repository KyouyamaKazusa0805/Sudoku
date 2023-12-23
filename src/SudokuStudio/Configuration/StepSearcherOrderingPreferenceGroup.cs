namespace SudokuStudio.Configuration;

/// <summary>
/// Represents with preference items that is used by <see cref="Analyzer"/>, for the ordering of <see cref="StepSearcher"/>s.
/// </summary>
/// <seealso cref="Analyzer"/>
/// <seealso cref="StepSearcher"/>
[DependencyProperty<ObservableCollection<StepSearcherInfo>>("StepSearchersOrder")]
public sealed partial class StepSearcherOrderingPreferenceGroup : PreferenceGroup
{
	[Default]
	private static readonly ObservableCollection<StepSearcherInfo> StepSearchersOrderDefaultValue = new(
		from searcher in StepSearcherPool.BuiltInStepSearchers
		select new StepSearcherInfo
		{
			IsEnabled = searcher.RunningArea.Flags(StepSearcherRunningArea.Searching),
			Name = searcher.ToString(),
			TypeName = searcher.GetType().Name
		}
	);


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

		analyzer.WithStepSearchers([.. from s in stepSearchers from stepSearcher in s.CreateStepSearchers() select stepSearcher]);
	}
}
