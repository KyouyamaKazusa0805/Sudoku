using Sudoku.Diagnostics.CodeAnalysis;
using Sudoku.UI.Models;
using Sudoku.UI.Views.Controls;

namespace Sudoku.UI.ViewModels;

/// <summary>
/// Indicates the view model that binds with the <see cref="SudokuPane"/> instances.
/// </summary>
/// <seealso cref="SudokuPane"/>
internal sealed class SudokuPaneViewModel : NotificationObject
{
	/// <summary>
	/// Indicates the grid, which is the backing field of the property <see cref="Grid"/>.
	/// </summary>
	/// <seealso cref="Grid"/>
	private Grid _grid;


	/// <summary>
	/// Initializes a <see cref="SudokuPageViewModel"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SudokuPaneViewModel() => GridChanged += Grid_GridChanged;


	/// <summary>
	/// Gets or sets the current grid data.
	/// </summary>
	public Grid Grid
	{
		get => _grid;

		set
		{
			if (_grid == value)
			{
				return;
			}

			_grid = value;

			GridChanged?.Invoke(this, new(value));
		}
	}


	/// <summary>
	/// Indicates the event that is triggered by other members when the backing field of the property
	/// <see cref="Grid"/> has been changed its value.
	/// </summary>
	/// <seealso cref="Grid"/>
	public event GridChangedEventHandler? GridChanged;


	/// <summary>
	/// Triggers when the backing field of the property <see cref="Grid"/> has been changed its value.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void Grid_GridChanged([IsDiscard] object sender, GridChangedEventArgs e)
	{

	}
}
