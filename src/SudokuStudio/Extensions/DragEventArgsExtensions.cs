namespace Microsoft.UI.Xaml;

/// <summary>
/// Provides with extension methods on <see cref="DragEventArgs"/>.
/// </summary>
/// <seealso cref="DragEventArgs"/>
public static class DragEventArgsExtensions
{
	/// <summary>
	/// Infers the index of the element to be removed via specified event data provider instance of type <see cref="DragEventArgs"/>.
	/// </summary>
	/// <param name="this">The drag-drop event data provider instance.</param>
	/// <param name="target">The target list view.</param>
	/// <returns>The index of the element to be removed.</returns>
	public static int GetIndexViaCursorPoint<T>(this DragEventArgs @this, T target) where T : ItemsControl
	{
		var (pos, index) = (@this.GetPosition(target.ItemsPanelRoot), 0);
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

			// If the drop position is more than half-way down the item being dropped on top of,
			// increment the insertion index so the dropped item is inserted below instead of above the item being dropped on top of.
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
