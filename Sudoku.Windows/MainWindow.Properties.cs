using System;
using Sudoku.Data.Stepping;
using Sudoku.Drawing;

namespace Sudoku.Windows
{
	partial class MainWindow
	{
		/// <summary>
		/// Indicates the puzzle, which is equivalent to <see cref="_puzzle"/>,
		/// but add the auto-update value layer.
		/// </summary>
		/// <remarks>This property is an <see langword="set"/>-only method in fact.</remarks>
		/// <value>The new grid.</value>
		/// <seealso cref="_puzzle"/>
		private UndoableGrid Puzzle
		{
			set
			{
				_currentPainter = new GridPainter(_pointConverter, Settings, _puzzle = value);
				_initialPuzzle = value.Clone();

				GC.Collect();
			}
		}

		/// <summary>
		/// Indicates the settings used.
		/// </summary>
		public Settings Settings { get; private set; } = null!;
	}
}
