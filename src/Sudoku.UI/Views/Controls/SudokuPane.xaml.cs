using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Sudoku.Diagnostics.CodeAnalysis;

namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Defines a user control that interacts with a sudoku grid.
/// </summary>
public sealed partial class SudokuPane : UserControl
{
	/// <summary>
	/// Initializes a <see cref="SudokuPane"/> instance.
	/// </summary>
	public SudokuPane() => InitializeComponent();


	private void InitializeGrid(double blockStrokeWidth, double cellStrokeWidth)
	{
		// Initializes row and column definitions.
		for (int i = 0; i < 27; i++)
		{
			_cGridMain.RowDefinitions.Add(new());
			_cGridMain.ColumnDefinitions.Add(new());
		}

		// Initializes block border lines.
		var blockThicknesses = new Thickness[,]
		{
			{
				new(0, blockStrokeWidth, 0, blockStrokeWidth / 2),
				new(0, blockStrokeWidth / 2, 0, blockStrokeWidth / 2),
				new(0, blockStrokeWidth / 2, 0, blockStrokeWidth)
			},
			{
				new(blockStrokeWidth, 0, blockStrokeWidth / 2, 0),
				new(blockStrokeWidth / 2, 0, blockStrokeWidth / 2, 0),
				new(blockStrokeWidth / 2, 0, blockStrokeWidth, 0)
			}
		};
		var borderBrush = new SolidColorBrush(Colors.Black);
		for (int i = 0; i < 3; i++)
		{
			_cGridMain.Children.Add(
				new Border
				{
					BorderThickness = blockThicknesses[0, i],
					BorderBrush = borderBrush,
					Tag = "Block border lines"
				}.WithGridRow(i * 9).WithGridRowSpan(9).WithGridColumnSpan(27)
			);
			_cGridMain.Children.Add(
				new Border
				{
					BorderThickness = blockThicknesses[1, i],
					BorderBrush = borderBrush,
					Tag = "Block border lines"
				}.WithGridColumn(i * 9).WithGridRowSpan(27).WithGridColumnSpan(9)
			);
		}

		// Initializes cell border lines.
		var cellThicknesses = new Thickness[,]
		{
			{
				new(0, cellStrokeWidth, 0, cellStrokeWidth / 2),
				new(0, cellStrokeWidth / 2, 0, cellStrokeWidth / 2),
				new(0, cellStrokeWidth / 2, 0, cellStrokeWidth)
			},
			{
				new(cellStrokeWidth, 0, cellStrokeWidth / 2, 0),
				new(cellStrokeWidth / 2, 0, cellStrokeWidth / 2, 0),
				new(cellStrokeWidth / 2, 0, cellStrokeWidth, 0)
			}
		};
		for (int i = 0; i < 9; i++)
		{
			int index = i switch { 0 => 0, 8 => 2, _ => 1 };
			_cGridMain.Children.Add(
				new Border
				{
					BorderThickness = cellThicknesses[0, index],
					BorderBrush = borderBrush,
					Tag = "Cell border lines"
				}.WithGridRow(i * 3).WithGridRowSpan(3).WithGridColumnSpan(27)
			);
			_cGridMain.Children.Add(
				new Border
				{
					BorderThickness = cellThicknesses[1, index],
					BorderBrush = borderBrush,
					Tag = "Cell border lines"
				}.WithGridColumn(i * 3).WithGridRowSpan(27).WithGridColumnSpan(3)
			);
		}
	}

	/// <summary>
	/// Triggers when the current control is loaded.
	/// </summary>
	/// <param name="sender">The object to trigger the event. The instance is always itself.</param>
	/// <param name="e">The event arguments provided.</param>
	private void SudokuPane_Loaded([IsDiscard] object sender, [IsDiscard] RoutedEventArgs e) =>
		InitializeGrid(4, 1);
}
