using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Extensions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
using Sudoku.Data;
using Sudoku.Painting;
using Sudoku.UI.Data;

namespace Sudoku.UI.Controls
{
	/// <summary>
	/// Indicates a sudoku panel that used for drawing a sudoku grid.
	/// </summary>
	public sealed partial class SudokuPanel : UserControl
	{
		/// <summary>
		/// Indicates the stack that uses for undoing or redoing a step.
		/// </summary>
		private readonly Stack<SudokuGrid> _undoStack = new(), _redoStack = new();


		/// <summary>
		/// Initializes a <see cref="SudokuPanel"/> instance with the default instantiation behavior.
		/// </summary>
		public SudokuPanel() => InitializeComponent();


		/// <summary>
		/// Indicates the preferences used.
		/// </summary>
		public PagePreferences Preferences { get; set; } = new();

		/// <summary>
		/// Indicates the grid painter instance.
		/// </summary>
		public GridPainter GridPainter { get; set; } = null!;


		/// <summary>
		/// Indicates an event triggered when undid a step.
		/// </summary>
		public event UndoEventHandler? Undo;

		/// <summary>
		/// Indicates an event triggered when redid a step.
		/// </summary>
		public event RedoEventHandler? Redo;


		/// <summary>
		/// Raises the event <see cref="Undo"/>.
		/// </summary>
		/// <seealso cref="Undo"/>
		private void UndoGrid()
		{
			if (_undoStack.Count == 0)
			{
				return;
			}

			var grid = _undoStack.Pop();
			_redoStack.Push(grid);
			Repaint();

			Undo?.Invoke(grid);
		}

		/// <summary>
		/// Raises the event <see cref="Redo"/>
		/// </summary>
		/// <see cref="Redo"/>
		private void RedoGrid()
		{
			if (_redoStack.Count == 0)
			{
				return;
			}

			var grid = _redoStack.Pop();
			_undoStack.Push(grid);
			Repaint();

			Redo?.Invoke(grid);
		}

		/// <summary>
		/// Initialize <see cref="GridPainter"/>.
		/// </summary>
		/// <seealso cref="GridPainter"/>
		[MemberNotNull(nameof(GridPainter))]
		private void InitializeGridPainter()
		{
			GridPainter = new(new(Width, Height), Preferences) { Grid = SudokuGrid.Empty };
			Repaint();
		}

		/// <summary>
		/// Repaint the grid using the <see cref="Shape"/> controls.
		/// </summary>
		private void Repaint()
		{
			var shapeControls = GridPainter.Create();
			var controls = MainGrid.Children;
			controls.Clear();
			controls.AddRange(shapeControls);
		}


		/// <summary>
		/// Triggers when the sudoku panel is loaded.
		/// </summary>
		/// <param name="sender">The object to trigger the event.</param>
		/// <param name="e">The event arguments provided.</param>
		private void SudokuPanel_Loaded(object sender, RoutedEventArgs e) => InitializeGridPainter();
	}
}
