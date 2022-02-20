using Sudoku.Diagnostics.CodeAnalysis;
using Sudoku.UI.Drawing.Shapes;

namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Defines a user control that interacts with a sudoku grid.
/// </summary>
public sealed partial class SudokuPane : UserControl
{
	#region Fields
	/// <summary>
	/// Indicates the delta that is used for checking whether two <see cref="double"/> values are same
	/// or their difference is below to the delta value.
	/// </summary>
	private const double Epsilon = 1E-2;


	/// <summary>
	/// Indicates the inner collection that stores the drawing elements, and also influences the controls
	/// displaying in the window.
	/// </summary>
	private readonly DrawingElementBag _drawingElements = new();

	/// <summary>
	/// Indicates the user preferences.
	/// </summary>
	/// <!--Wait for new function that allows serializations or deserializations.-->
	private readonly UserPreference _userPreference = new();

	/// <summary>
	/// Indicates the size that the current pane is, which is the backing field
	/// of the property <see cref="Size"/>.
	/// </summary>
	/// <seealso cref="Size"/>
	private double _size;

	/// <summary>
	/// Indicates the outside offset value, which is the backing field
	/// of the property <see cref="OutsideOffset"/>.
	/// </summary>
	/// <seealso cref="OutsideOffset"/>
	private double _outsideOffset;

	/// <summary>
	/// Indicates the grid used, which is the backing field of the property <see cref="Grid"/>.
	/// </summary>
	/// <seealso cref="Grid"/>
	private Grid _grid = Grid.Empty;
	#endregion


	#region Constructors
	/// <summary>
	/// Initializes a <see cref="SudokuPane"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SudokuPane() => InitializeComponent();
	#endregion


	#region Properties
	/// <summary>
	/// Gets or sets the size of the pane.
	/// </summary>
	public double Size
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _size;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_size.NearlyEquals(value, Epsilon))
			{
				return;
			}

			_size = value;
			foreach (var drawingElement in _drawingElements.OfType<CellLine, BlockLine>())
			{
				drawingElement.DynamicAssign(
					instance =>
					{
						instance.OutsideOffset = OutsideOffset;
						instance.PaneSize = value;
					}
				);
			}
		}
	}

	/// <summary>
	/// Gets or sets the outside offset to the view model.
	/// </summary>
	public double OutsideOffset
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _outsideOffset;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_outsideOffset.NearlyEquals(value, Epsilon))
			{
				return;
			}

			_outsideOffset = value;
			foreach (var drawingElement in _drawingElements.OfType<CellLine, BlockLine>())
			{
				drawingElement.DynamicAssign(
					instance =>
					{
						instance.OutsideOffset = value;
						instance.PaneSize = Size;
					}
				);
			}
		}
	}

	/// <summary>
	/// Gets or sets the current grid used.
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
			_drawingElements.OfType<SudokuGrid>().Single().Grid = value;
		}
	}
	#endregion


	#region Normal instance methods
	/// <summary>
	/// Gets the reference of the sudoku grid.
	/// </summary>
	/// <returns>The reference of the sudoku grid.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref readonly Grid GetGridByReference() => ref _grid;
	#endregion


	#region Delegated methods
	/// <summary>
	/// Triggers when the current control is loaded.
	/// </summary>
	/// <param name="sender">The object to trigger the event. The instance is always itself.</param>
	/// <param name="e">The event arguments provided.</param>
	private void SudokuPane_Loaded([IsDiscard] object sender, [IsDiscard] RoutedEventArgs e)
	{
		// Initializes the outside border if worth.
		if (_userPreference.OutsideBorderWidth != 0 && OutsideOffset != 0)
		{
			_drawingElements.Add(
				new OutsideRectangle(
					_userPreference.OutsideBorderColor,
					Size,
					_userPreference.OutsideBorderWidth
				)
			);
		}

		// Initializes block border lines.
		for (byte i = 0; i < 4; i++)
		{
			_drawingElements.Add(
				new BlockLine(
					_userPreference.BlockBorderColor,
					_userPreference.BlockBorderWidth,
					Size,
					OutsideOffset,
					i
				)
			);
			_drawingElements.Add(
				new BlockLine(
					_userPreference.BlockBorderColor,
					_userPreference.BlockBorderWidth,
					Size,
					OutsideOffset,
					(byte)(i + 4)
				)
			);
		}

		// Initializes cell border lines.
		for (byte i = 0; i < 10; i++)
		{
			if (i % 3 == 0)
			{
				// Skips the overlapping lines.
				continue;
			}

			_drawingElements.Add(
				new CellLine(
					_userPreference.CellBorderColor,
					_userPreference.CellBorderWidth,
					Size, OutsideOffset,
					i
				)
			);
			_drawingElements.Add(
				new CellLine(
					_userPreference.CellBorderColor,
					_userPreference.CellBorderWidth,
					Size, OutsideOffset,
					(byte)(i + 10)
				)
			);
		}

		// TODO: Initializes candidate border lines if worth.

		// Initializes the sudoku grid.
		_drawingElements.Add(
			new SudokuGrid(
				_grid,
				_userPreference.ShowCandidates,
				Size,
				OutsideOffset,
				_userPreference.GivenColor,
				_userPreference.ModifiableColor,
				_userPreference.CandidateColor,
				_userPreference.ValueFontName,
				_userPreference.CandidateFontName,
				_userPreference.ValueFontSize,
				_userPreference.CandidateFontSize
			)
		);

		// Add them into the control collection.
		foreach (var control in from drawingElement in _drawingElements select drawingElement.GetControl())
		{
			_cCanvasMain.Children.Add(control);
		}
	}
	#endregion
}
