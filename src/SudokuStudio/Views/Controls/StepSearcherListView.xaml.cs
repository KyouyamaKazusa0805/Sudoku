using System.Collections.ObjectModel;
using System.Text.Json;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sudoku.Analytics;
using SudokuStudio.ComponentModel;
using SudokuStudio.Configuration;
using SudokuStudio.Interaction;
using Windows.ApplicationModel.DataTransfer;

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
		if (e is { Data: var dataPackage, Items: [StepSearcherInfo stepSearcherInfo] })
		{
			dataPackage.SetText(JsonSerializer.Serialize(stepSearcherInfo));
			dataPackage.RequestedOperation = DataPackageOperation.Move;
		}
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
		if (JsonSerializer.Deserialize<StepSearcherInfo>(await e.DataView.GetTextAsync()) is not { TypeName: var typeName } instance
			|| StepSearcherPool.GetStepSearchers(typeName, false)[0].Metadata.IsFixed)
		{
			return;
		}

		// Important step: Drag & drop behavior cannot get the target element's index.
		// We should get the target insertion index via cursor point.
		var index = e.GetIndexViaCursorPoint(target);

		StepSearchers.Insert(index, instance);
		StepSearchers.RemoveWhen(item => item == instance);

		e.AcceptedOperation = DataPackageOperation.Move;
		def.Complete();
	}


	private void MainListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		=> ItemSelected?.Invoke(this, new((StepSearcherInfo)MainListView.SelectedItem));
}
