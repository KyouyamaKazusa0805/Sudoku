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


	private void MainListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
	{
		if (e is not { Data: var dataPackage, Items: [StepSearcherInfo stepSearcherSerializationData] })
		{
			return;
		}

		dataPackage.SetText(stepSearcherSerializationData.ToString());
		dataPackage.RequestedOperation = DataPackageOperation.Move;
	}

	private void MainListView_DragOver(object sender, DragEventArgs e) => e.AcceptedOperation = DataPackageOperation.Move;

	private void MainListView_DragEnter(object sender, DragEventArgs e) => e.DragUIOverride.IsGlyphVisible = false;

	private async void MainListView_DropAsync(object sender, DragEventArgs e)
	{
		if (sender is not ListView { Name: "MainListView" } target)
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
		StepSearchers.RemoveIf(item => item == instance);

		e.AcceptedOperation = DataPackageOperation.Move;
		def.Complete();
	}


	[GeneratedRegex("""IsEnabled\s*=\s*([Tt]rue|[Ff]alse),\s*Name\s*=\s*([^,]+),\s*TypeName\s*=\s*(\w+)""", RegexOptions.Compiled, 5000)]
	private static partial Regex StepSearcherSerializationDataStringPattern();
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// Removes the specified element if the specified condition returns <see langword="true"/>.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The collection.</param>
	/// <param name="predicate">The condition that checks whether the element should be removed.</param>
	public static void RemoveIf<T>(this ObservableCollection<T> @this, Predicate<T> predicate)
	{
		for (var i = 0; i < @this.Count; i++)
		{
			if (predicate(@this[i]))
			{
				@this.RemoveAt(i);
				return;
			}
		}
	}

	/// <summary>
	/// Infers the index of the element to be removed via specified event data provider instance of type <see cref="DragEventArgs"/>.
	/// </summary>
	/// <param name="this">The drag-drop event data provider instance.</param>
	/// <param name="target">The target list view.</param>
	/// <returns>The index of the element to be removed.</returns>
	public static int GetIndexViaCursorPoint(this DragEventArgs @this, ListView target)
	{
		var pos = @this.GetPosition(target.ItemsPanelRoot);

		var index = 0;
		if (target.Items.Count != 0)
		{
			// Get a reference to the first item in the current list view.
			var sampleItem = (ListViewItem)target.ContainerFromIndex(0);

			// Adjust itemHeight for margins.
			var itemHeight = sampleItem.ActualHeight + sampleItem.Margin.Top + sampleItem.Margin.Bottom;

			// Find index based on dividing number of items by height of each item.
			index = Min(target.Items.Count - 1, (int)(pos.Y / itemHeight));

			// Find the item being dropped on top of.
			var targetItem = (ListViewItem)target.ContainerFromIndex(index);

			// If the drop position is more than half-way down the item being dropped on
			// top of, increment the insertion index so the dropped item is inserted
			// below instead of above the item being dropped on top of.
			var positionInItem = @this.GetPosition(targetItem);
			if (positionInItem.Y > itemHeight / 2)
			{
				index++;
			}

			// Don't go out of bounds.
			index = Min(target.Items.Count, index);
		}

		return index;
	}
}
