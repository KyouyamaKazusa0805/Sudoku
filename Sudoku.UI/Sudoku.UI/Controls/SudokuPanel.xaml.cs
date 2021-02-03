using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Sudoku.Data;
using Sudoku.Painting;
using Sudoku.Painting.Extensions;
using Sudoku.UI.Data;
using Sudoku.UI.Extensions;
using Windows.Foundation;
using Windows.System;

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
		/// Indicates the pointer in this control.
		/// </summary>
		private Point _pointer;


		/// <summary>
		/// Initializes a <see cref="SudokuPanel"/> instance with the default instantiation behavior.
		/// </summary>
		public SudokuPanel() => InitializeComponent();


		/// <summary>
		/// Indicates the preferences used.
		/// </summary>
		public PagePreferences Preferences => ProgramData.BaseWindow.Preferences;

		/// <summary>
		/// Indicates the grid painter instance. This property can be <see langword="null"/>
		/// when the panel doesn't finish its initialization.
		/// </summary>
		public GridPainter Painter { get; set; } = null!;


		/// <summary>
		/// Indicates an event triggered when undid a step.
		/// </summary>
		public event UndoEventHandler? Undo;

		/// <summary>
		/// Indicates an event triggered when redid a step.
		/// </summary>
		public event RedoEventHandler? Redo;

		/// <summary>
		/// Indicates an event triggered when the grid data is changed.
		/// </summary>
		/// <remarks>
		/// This event can update the status of its relative controls. For example, an undo button
		/// should be updated when this event triggered.
		/// </remarks>
		public event EventHandler? GridDataChanged;


		/// <inheritdoc/>
		protected override async void OnKeyDown(KeyRoutedEventArgs e)
		{
			await i();

			async Task i()
			{
				switch (e.Key)
				{
					// Set or remove a digit.
					case var key when key.IsDigit(out var anchor):
					{
						var grid = Painter.Grid;
						int cell = GetCell(), digit = key - anchor;
						if (VirtualKey.Shift.IsDown())
						{
							RecordAnUndoStep(grid);

							RemoveCandidate(ref grid, cell, digit);

							await RepaintAsync();

							GridDataChanged?.Invoke(this, EventArgs.Empty);
						}
						else
						{
							RecordAnUndoStep(grid);

							SetCandidate(ref grid, cell, digit);

							await RepaintAsync();

							GridDataChanged?.Invoke(this, EventArgs.Empty);
						}

						break;
					}
				}
			}
		}

		/// <summary>
		/// Record a grid status (push into the undo stack).
		/// </summary>
		/// <param name="oldGrid">(<see langword="in"/> parameter) The old grid.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RecordAnUndoStep(in SudokuGrid oldGrid) => _undoStack.Push(oldGrid);

		/// <summary>
		/// Remove a candidate.
		/// </summary>
		/// <param name="grid">(<see langword="ref"/> parameter) The grid.</param>
		/// <param name="cell">The cell.</param>
		/// <param name="digit">The digit.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RemoveCandidate(ref SudokuGrid grid, int cell, int digit) => grid[cell, digit] = false;

		/// <summary>
		/// Set a candidate.
		/// </summary>
		/// <param name="grid">(<see langword="ref"/> parameter) The grid.</param>
		/// <param name="cell">The cell.</param>
		/// <param name="digit">The digit.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SetCandidate(ref SudokuGrid grid, int cell, int digit) => grid[cell] = digit;

		/// <summary>
		/// Try to get the cell the mouse pointer just pressed or down.
		/// </summary>
		/// <returns>The cell.</returns>
		/// <exception cref="Exception">
		/// Throws when the field <see cref="_pointer"/> keeps the default value.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int GetCell() =>
			_pointer is { X: 0, Y: 0 }
			? throw new Exception("The mouse point shouldn't be zero at the current scenario.")
			: Painter.Translator.GetCell(_pointer.ToDPointF());

		/// <summary>
		/// Try to get the candidate the mouse pointer just pressed or down.
		/// </summary>
		/// <returns>The candidate.</returns>
		/// <exception cref="Exception">
		/// Throws when the field <see cref="_pointer"/> keeps the default value.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int GetCandidate() =>
			_pointer is { X: 0, Y: 0 }
			? throw new Exception("The mouse point shouldn't be zero at the current scenario.")
			: Painter.Translator.GetCandidate(_pointer.ToDPointF());

		/// <summary>
		/// Raises <see cref="Undo"/>.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <seealso cref="Undo"/>
		private async ValueTask OnUndoingAsync(SudokuGrid grid)
		{
			// TODO: Update icons.

			await RepaintAsync();

			Undo?.Invoke(grid);
		}

		/// <summary>
		/// Raises <see cref="Redo"/>.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <seealso cref="Redo"/>
		private async ValueTask OnRedoingAsync(SudokuGrid grid)
		{
			// TODO: Update icons.

			await RepaintAsync();

			Redo?.Invoke(grid);
		}


		/// <summary>
		/// Raises the event <see cref="Undo"/>.
		/// </summary>
		/// <seealso cref="Undo"/>
		private async ValueTask UndoGridAsync()
		{
			if (_undoStack.Count == 0)
			{
				return;
			}

			var grid = _undoStack.Pop();
			_redoStack.Push(grid);

			await OnUndoingAsync(grid);
		}

		/// <summary>
		/// Raises the event <see cref="Redo"/>
		/// </summary>
		/// <see cref="Redo"/>
		private async ValueTask RedoGridAsync()
		{
			if (_redoStack.Count == 0)
			{
				return;
			}

			var grid = _redoStack.Pop();
			_undoStack.Push(grid);

			await OnRedoingAsync(grid);
		}

		/// <summary>
		/// Initialize <see cref="Painter"/>.
		/// </summary>
		/// <seealso cref="Painter"/>
		[MemberNotNull(nameof(Painter))]
		private async ValueTask InitializeGridPainterAsync()
		{
			Painter = new(new((float)Width, (float)Height), Preferences) { Grid = SudokuGrid.Empty };
			await RepaintAsync();
		}

		/// <summary>
		/// Repaint the grid.
		/// </summary>
		private async ValueTask RepaintAsync() => ImageGrid.Source = await Painter.Paint().ToImageSourceAsync();


		/// <summary>
		/// Triggers when the pointer is moved in <see cref="ImageGrid"/>.
		/// </summary>
		/// <param name="sender">The sender to trigger the event.</param>
		/// <param name="e">The event arguments provided.</param>
		/// <seealso cref="ImageGrid"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ImageGrid_PointerMoved(object sender, PointerRoutedEventArgs e) =>
			_pointer = e.GetCurrentPoint(ImageGrid).Position;

		/// <summary>
		/// Triggers when the pointer is exited from the control <see cref="ImageGrid"/>.
		/// </summary>
		/// <param name="sender">The sender to trigger the event.</param>
		/// <param name="e">The event arguments provided.</param>
		/// <seealso cref="ImageGrid"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ImageGrid_PointerExited(object sender, PointerRoutedEventArgs e) => _pointer = new();

		/// <summary>
		/// Triggers when the sudoku panel is loaded.
		/// </summary>
		/// <param name="sender">The object to trigger the event.</param>
		/// <param name="e">The event arguments provided.</param>
		private async void SudokuPanel_LoadedAsync(object sender, RoutedEventArgs e)
		{
			await i();

			async ValueTask i() => await InitializeGridPainterAsync();
		}
	}
}
