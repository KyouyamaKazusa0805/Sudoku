namespace SudokuStudio.Configuration;

/// <summary>
/// Represents with preference items that is used by <see cref="LogicalSolver"/>, for the ordering of <see cref="IStepSearcher"/>s.
/// </summary>
/// <seealso cref="LogicalSolver"/>
/// <seealso cref="IStepSearcher"/>
[DependencyProperty<ObservableCollection<StepSearcherSerializationData>>("StepSearchersOrder")]
public sealed partial class StepSearcherOrderingPreferenceGroup : PreferenceGroup
{
	[DefaultValue]
	private static readonly ObservableCollection<StepSearcherSerializationData> StepSearchersOrderDefaultValue = new(
		from searcher in StepSearcherPool.DefaultCollection(false)
		select new StepSearcherSerializationData
		{
			IsEnabled = searcher.Options.EnabledArea.Flags(SearcherEnabledArea.Default) && !searcher.IsTemporarilyDisabled,
			Name = searcher.TypeResourceName,
			TypeName = searcher.TypeName
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

		programSolver.CustomSearcherCollection = (
			from data in stepSearchersData
			from stepSearcher in data.CreateStepSearchers()
			select stepSearcher
		).ToArray();
	}
}
