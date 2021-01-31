using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Extensions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
using Sudoku.Data;
using Sudoku.Data.Stepping;
using Sudoku.Painting;

namespace Sudoku.UI.Controls
{
	/// <summary>
	/// Indicates a sudoku panel that used for drawing a sudoku grid.
	/// </summary>
	public sealed partial class SudokuPanel : UserControl
	{
		/// <summary>
		/// Initializes a <see cref="SudokuPanel"/> instance with the default instantiation behavior.
		/// </summary>
		public SudokuPanel() => InitializeComponent();


		/// <summary>
		/// Indicates the grid painter instance.
		/// </summary>
		public GridPainter GridPainter { get; set; } = null!;


		/// <summary>
		/// Initialize <see cref="GridPainter"/>.
		/// </summary>
		/// <seealso cref="GridPainter"/>
		[MemberNotNull(nameof(GridPainter))]
		private void InitializeGridPainter()
		{
			GridPainter = new(new(Width, Height), SudokuGrid.Empty);

			var shapeControls = GridPainter.Create();
			Paint(shapeControls);
		}

		/// <summary>
		/// Paint the <see cref="Shape"/> controls.
		/// </summary>
		private void Paint(IReadOnlyCollection<Shape> shapeControls)
		{
			MainGrid.Children.Clear();
			MainGrid.Children.AddRange(shapeControls);
		}


		/// <summary>
		/// Triggers when the sudoku panel is loaded.
		/// </summary>
		/// <param name="sender">The object to trigger the event.</param>
		/// <param name="e">The event arguments provided.</param>
		private void SudokuPanel_Loaded(object sender, RoutedEventArgs e) => InitializeGridPainter();
	}
}
