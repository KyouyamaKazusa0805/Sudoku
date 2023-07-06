namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents a <see cref="ListView"/> control that displays for <see cref="StepSearcher"/> instances.
/// </summary>
/// <seealso cref="StepSearcher"/>
[DependencyProperty<ObservableCollection<StepSearcherInfo>>("StepSearchers", DocSummary = "Indicates the step searchers.")]
public sealed partial class StepSearcherListView : UserControl
{
	/// <summary>
	/// Initializes a <see cref="StepSearcherListView"/> instance.
	/// </summary>
	public StepSearcherListView() => InitializeComponent();


	/// <summary>
	/// Indicates the event triggered when an item is selected.
	/// </summary>
	public event StepSearcherListViewItemSelectedEventHandler? ItemSelected;


	private void MainListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
	{
		if (e is not { Data: var dataPackage, Items: [StepSearcherInfo stepSearcherInfo] })
		{
			return;
		}

		dataPackage.SetText(stepSearcherInfo.ToString());
		dataPackage.RequestedOperation = DataPackageOperation.Move;
	}

	private void MainListView_DragOver(object sender, DragEventArgs e) => e.AcceptedOperation = DataPackageOperation.Move;

	private void MainListView_DragEnter(object sender, DragEventArgs e) => e.DragUIOverride.IsGlyphVisible = false;

	private async void MainListView_DropAsync(object sender, DragEventArgs e)
	{
		if (sender is not ListView { Name: nameof(MainListView) } target)
		{
			return;
		}

		if (!e.DataView.Contains(StandardDataFormats.Text))
		{
			return;
		}

		var def = e.GetDeferral();
		var s = await e.DataView.GetTextAsync();

		var capture = StepSearcherSerializationDataStringPattern().Match(s);
		if (capture is not { Groups: [{ Value: var isEnabledRaw }, { Value: var nameRaw }, { Value: var typeNameRaw }] })
		{
			return;
		}

		var instance = new StepSearcherInfo { IsEnabled = bool.Parse(isEnabledRaw), Name = nameRaw, TypeName = typeNameRaw };

		// Important step: Drag & drop behavior cannot get the target element's index.
		// We should get the target insertion index via cursor point.
		var index = e.GetIndexViaCursorPoint(target);

		StepSearchers.Insert(index, instance);
		StepSearchers.RemoveWhen(item => item == instance);

		e.AcceptedOperation = DataPackageOperation.Move;
		def.Complete();
	}


	[GeneratedRegex("""IsEnabled\s*=\s*([Tt]rue|[Ff]alse),\s*Name\s*=\s*([^,]+),\s*TypeName\s*=\s*(\w+)""", RegexOptions.Compiled, 5000)]
	private static partial Regex StepSearcherSerializationDataStringPattern();


	private void MainListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		=> ItemSelected?.Invoke(this, new((StepSearcherInfo)MainListView.SelectedItem));
}
