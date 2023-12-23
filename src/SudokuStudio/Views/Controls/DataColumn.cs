// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents for a data column.
/// </summary>
[TemplatePart(Name = nameof(PART_ColumnSizer), Type = typeof(ContentSizer))]
[DependencyProperty<bool>("CanResize", DocSummary = "Gets or sets whether the column can be resized by the user.")]
[DependencyProperty<GridLength>("DesiredWidth", DocSummary = "Gets or sets the desired width of the column upon initialization. Defaults to a <see cref=\"global::Microsoft.UI.Xaml.GridLength\"/> of 1 <see cref=\"global::Microsoft.UI.Xaml.GridUnitType.Star\"/>.")]
public partial class DataColumn : ContentControl
{
	[Default]
	private static readonly GridLength DesiredWidthDefaultValue = GridLength.Auto;


	[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
	private ContentSizer? PART_ColumnSizer;

	private WeakReference<DataTable>? _parent;


	/// <summary>
	/// Initalizes a <see cref="DataColumn"/> instance.
	/// </summary>
	public DataColumn() => DefaultStyleKey = typeof(DataColumn);


	/// <summary>
	/// Gets or sets the width of the largest child contained within the visible <see cref="DataRow"/>s of the <see cref="DataTable"/>.
	/// </summary>
	internal double MaxChildDesiredWidth { get; set; }

	/// <summary>
	/// Gets or sets the internal copy of the <see cref="DesiredWidth"/> property to be used in calculations, this gets manipulated in Auto-Size mode.
	/// </summary>
	internal GridLength CurrentWidth { get; private set; }


	/// <inheritdoc/>
	protected override void OnApplyTemplate()
	{
		if (PART_ColumnSizer is not null)
		{
			PART_ColumnSizer.TargetControl = null;
			PART_ColumnSizer.ManipulationDelta -= PART_ColumnSizer_ManipulationDelta;
			PART_ColumnSizer.ManipulationCompleted -= PART_ColumnSizer_ManipulationCompleted;
		}

		PART_ColumnSizer = GetTemplateChild(nameof(PART_ColumnSizer)) as ContentSizer;

		if (PART_ColumnSizer is not null)
		{
			PART_ColumnSizer.TargetControl = this;
			PART_ColumnSizer.ManipulationDelta += PART_ColumnSizer_ManipulationDelta;
			PART_ColumnSizer.ManipulationCompleted += PART_ColumnSizer_ManipulationCompleted;
		}

		// Get DataTable parent weak reference for when we manipulate columns.
		var parent = this.FindAscendant<DataTable>();
		if (parent is not null)
		{
			_parent = new(parent);
		}

		base.OnApplyTemplate();
	}

	private void ColumnResizedByUserSizer()
	{
		// Update our internal representation to be our size now as a fixed value.
		CurrentWidth = new(ActualWidth);

		// Notify the rest of the table to update
		if (_parent?.TryGetTarget(out var parent) is true && parent is not null)
		{
			parent.ColumnResized();
		}
	}


	[Callback]
	private static void DesiredWidthPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		// If the developer updates the size of the column, update our internal copy
		if (d is DataColumn col)
		{
			col.CurrentWidth = col.DesiredWidth;
		}
	}


	private void PART_ColumnSizer_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e) => ColumnResizedByUserSizer();

	private void PART_ColumnSizer_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) => ColumnResizedByUserSizer();
}
