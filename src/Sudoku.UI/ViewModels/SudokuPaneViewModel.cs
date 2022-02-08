using Sudoku.UI.Models;
using Sudoku.UI.Views.Controls;
using Windows.Foundation;

namespace Sudoku.UI.ViewModels;

/// <summary>
/// Indicates the view model that binds with the <see cref="SudokuPane"/> instances.
/// </summary>
/// <seealso cref="SudokuPane"/>
internal sealed class SudokuPaneViewModel : NotificationObject
{
	private double _size;
	private double _outsideOffset;
	private Grid _grid;


	/// <summary>
	/// Indicates the size of the pane.
	/// </summary>
	public double Size
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _size;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_size == value)
			{
				return;
			}

			SetProperty(ref _size, value);
			SizeChanged?.Invoke(this, new());
		}
	}

	/// <summary>
	/// Indicates the outside offset.
	/// </summary>
	public double OutsideOffset
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _outsideOffset;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_outsideOffset == value)
			{
				return;
			}

			SetProperty(ref _outsideOffset, value);
			OutsideOffsetChanged?.Invoke(this, new());
		}
	}

	/// <summary>
	/// Indicates the grid used.
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

			SetProperty(ref _grid, value);
			GridChanged?.Invoke(this, new(value));
		}
	}


	/// <summary>
	/// Defines an event that triggers when the property <see cref="Size"/> value is changed.
	/// </summary>
	/// <seealso cref="Size"/>
	public event RoutedEventHandler? SizeChanged;

	/// <summary>
	/// Defines an event that triggers when the property <see cref="OutsideOffset"/> value is changed.
	/// </summary>
	/// <seealso cref="OutsideOffset"/>
	public event RoutedEventHandler? OutsideOffsetChanged;

	/// <summary>
	/// Indicates the event to be triggered when the property <see cref="Grid"/> value is changed.
	/// </summary>
	public event GridChangedEventHandler? GridChanged;


	/// <summary>
	/// Gets the first point value of the horizontal border line.
	/// </summary>
	/// <param name="i">The index of the line.</param>
	/// <param name="weight">The weight of the division operation. The value can only be 3, 9 or 27.</param>
	/// <returns>The first point value of the horizontal border line.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Point HorizontalBorderLinePoint1(int i, int weight) =>
		new(OutsideOffset + i * (Size - 2 * OutsideOffset) / weight, OutsideOffset);

	/// <summary>
	/// Gets the second point value of the horizontal border line.
	/// </summary>
	/// <param name="i">The index of the line.</param>
	/// <param name="weight">The weight of the division operation. The value can only be 3, 9 or 27.</param>
	/// <returns>The second point value of the horizontal border line.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Point HorizontalBorderLinePoint2(int i, int weight) =>
		new(OutsideOffset + i * (Size - 2 * OutsideOffset) / weight, Size - OutsideOffset);

	/// <summary>
	/// Gets the first point value of the vertical border line.
	/// </summary>
	/// <param name="i">The index of the line.</param>
	/// <param name="weight">The weight of the division operation. The value can only be 3, 9 or 27.</param>
	/// <returns>The first point value of the horizontal border line.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Point VerticalBorderLinePoint1(int i, int weight) =>
		new(OutsideOffset, OutsideOffset + i * (Size - 2 * OutsideOffset) / weight);

	/// <summary>
	/// Gets the second point value of the vertical border line.
	/// </summary>
	/// <param name="i">The index of the line.</param>
	/// <param name="weight">The weight of the division operation. The value can only be 3, 9 or 27.</param>
	/// <returns>The second point value of the horizontal border line.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Point VerticalBorderLinePoint2(int i, int weight) =>
		new(Size - OutsideOffset, OutsideOffset + i * (Size - 2 * OutsideOffset) / weight);
}
