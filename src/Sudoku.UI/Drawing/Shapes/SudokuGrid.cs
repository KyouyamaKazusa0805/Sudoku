namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a sudoku grid.
/// </summary>
public sealed class SudokuGrid : DrawingElement
{
	/// <summary>
	/// Indicates the inner grid.
	/// </summary>
	private Grid _grid;


	/// <summary>
	/// 
	/// </summary>
	public SudokuGrid(in Grid grid)
	{
		_grid = grid;
	}


	/// <summary>
	/// Gets or sets the grid.
	/// </summary>
	public Grid Grid
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _grid;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_grid == value)
			{
				return;
			}

			_grid = value;
		}
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] DrawingElement? other) =>
		other is SudokuGrid comparer && _grid == comparer._grid;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override UIElement GetControl() => throw new NotImplementedException();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => _grid.GetHashCode();
}
