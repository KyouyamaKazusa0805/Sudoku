// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// https://github.com/CommunityToolkit/Labs-Windows/blob/main/components/DataTable/src/DataTable/DataTable.cs

namespace SudokuStudio.Views.Controls;

/// <summary>
/// A <see cref="DataTable"/> is a <see cref="Panel"/> which lays out <see cref="DataColumn"/>s based on
/// their configured properties (akin to <see cref="ColumnDefinition"/>); similar to a <see cref="Grid"/> with a single row.
/// </summary>
[DependencyProperty<double>("ColumnSpacing", DocSummary = "Gets or sets the amount of space to place between columns within the table.")]
public partial class DataTable : Panel
{
	/// <summary>
	/// The internal field storing rows.
	/// </summary>
	/// <remarks>
	/// TODO: Check with Sergio if there's a better structure here, as I don't need a Dictionary like ConditionalWeakTable
	/// </remarks>
	internal HashSet<DataRow> Rows { get; private set; } = [];


	/// <summary>
	/// Called when column is re-sized.
	/// </summary>
	internal void ColumnResized()
	{
		InvalidateArrange();

		foreach (var row in Rows)
		{
			row.InvalidateArrange();
		}
	}

	/// <inheritdoc/>
	protected override Size MeasureOverride(Size availableSize)
	{
		var fixedWidth = 0D;
		var proportionalUnits = 0D;
		var autoSized = 0D;

		var maxHeight = 0D;

		var elements = from e in Children where e.Visibility == Visibility.Visible && e is DataColumn select e;

		// We only need to measure elements that are visible
		foreach (DataColumn column in elements)
		{
			if (column.CurrentWidth.IsStar)
			{
				proportionalUnits += column.DesiredWidth.Value;
			}
			else if (column.CurrentWidth.IsAbsolute)
			{
				fixedWidth += column.DesiredWidth.Value;
			}
		}

		// Add in spacing between columns to our fixed size allotment
		fixedWidth += (elements.Count() - 1) * ColumnSpacing;

		// TODO: Handle infinite width?
		var proportionalAmount = (availableSize.Width - fixedWidth) / proportionalUnits;

		foreach (DataColumn column in elements)
		{
			if (column.CurrentWidth.IsStar)
			{
				column.Measure(new(proportionalAmount * column.CurrentWidth.Value, availableSize.Height));
			}
			else if (column.CurrentWidth.IsAbsolute)
			{
				column.Measure(new(column.CurrentWidth.Value, availableSize.Height));
			}
			else
			{
				// TODO: Technically this is using 'Auto' on the Header content
				// What the developer probably intends is it to be adjusted based on the contents of the rows...
				// To enable this scenario, we'll need to actually measure the contents of the rows for that column
				// in DataRow and figure out the maximum size to report back and adjust here in some sort of hand-shake
				// for the layout process... (i.e. get the data in the measure step, use it in the arrange step here,
				// then invalidate the child arranges [don't re-measure and cause loop]...)

				// For now, we'll just use the header content as a guideline to see if things work.
				column.Measure(new(availableSize.Width - fixedWidth - autoSized, availableSize.Height));

				// Keep track of already 'allotted' space, use either the maximum child size (if we know it) or the header content
				autoSized += Max(column.DesiredSize.Width, column.MaxChildDesiredWidth);
			}

			maxHeight = Max(maxHeight, column.DesiredSize.Height);
		}

		return new(availableSize.Width, maxHeight);
	}

	/// <inheritdoc/>
	protected override Size ArrangeOverride(Size finalSize)
	{
		var fixedWidth = 0D;
		var proportionalUnits = 0D;
		var autoSized = 0D;

		var elements = from e in Children where e.Visibility == Visibility.Visible && e is DataColumn select e;

		// We only need to measure elements that are visible
		foreach (DataColumn column in elements)
		{
			if (column.CurrentWidth.IsStar)
			{
				proportionalUnits += column.CurrentWidth.Value;
			}
			else if (column.CurrentWidth.IsAbsolute)
			{
				fixedWidth += column.CurrentWidth.Value;
			}
			else
			{
				autoSized += Max(column.DesiredSize.Width, column.MaxChildDesiredWidth);
			}
		}

		// TODO: Handle infinite width?
		// TODO: This can go out of bounds or something around here when pushing a resized column to the right...
		var proportionalAmount = (finalSize.Width - fixedWidth - autoSized) / proportionalUnits;

		var width = 0D;
		var x = 0D;

		foreach (DataColumn column in elements)
		{
			if (column.CurrentWidth.IsStar)
			{
				width = proportionalAmount * column.CurrentWidth.Value;
				column.Arrange(new(x, 0, width, finalSize.Height));
			}
			else if (column.CurrentWidth.IsAbsolute)
			{
				width = column.CurrentWidth.Value;
				column.Arrange(new(x, 0, width, finalSize.Height));
			}
			else
			{
				// TODO: We use the comparison of sizes a lot, should we cache in the DataColumn itself?
				width = Max(column.DesiredSize.Width, column.MaxChildDesiredWidth);
				column.Arrange(new(x, 0, width, finalSize.Height));
			}

			x += width + ColumnSpacing;
		}

		return finalSize;
	}
}
