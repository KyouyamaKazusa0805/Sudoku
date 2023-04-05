namespace SudokuStudio.Configuration;

/// <summary>
/// Represents with preference items that is used by <see cref="Analyzer"/>, for the ordering of <see cref="StepSearcher"/>s.
/// </summary>
/// <seealso cref="Analyzer"/>
/// <seealso cref="StepSearcher"/>
[DependencyProperty<ObservableCollection<StepSearcherSerializationData>>("StepSearchersOrder")]
public sealed partial class StepSearcherOrderingPreferenceGroup : PreferenceGroup
{
	[DefaultValue]
	private static readonly ObservableCollection<StepSearcherSerializationData> StepSearchersOrderDefaultValue = new(
		from searcher in StepSearcherPool.Default(false)
		select new StepSearcherSerializationData
		{
			IsEnabled = searcher.RunningArea.Flags(StepSearcherRunningArea.Searching),
			Name = searcher.ToString(),
			TypeName = searcher.GetType().Name
		}
	);


	[Callback]
	private static void StepSearchersOrderPropertyCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
	{
		if (e is not { NewValue: ObservableCollection<StepSearcherSerializationData> stepSearchersData })
		{
			return;
		}

		if (((App)Application.Current).ProgramSolver is not { } programSolver)
		{
			return;
		}

		programSolver.WithStepSearchers(
			(
				from data in stepSearchersData
				from stepSearcher in data.CreateStepSearchers()
				select stepSearcher
			).ToArray()
		);
	}
}
