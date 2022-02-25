using System.ComponentModel;
using Microsoft.UI.Xaml.Input;
using Sudoku.Diagnostics.CodeAnalysis;
using Sudoku.UI.Drawing;
using Sudoku.UI.Drawing.Shapes;

namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Defines a user control that interacts with a sudoku grid.
/// </summary>
public sealed partial class SudokuPane : UserControl, INotifyPropertyChanged
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
	/// Indicates the current cell focused.
	/// </summary>
	public int CurrentCell { get; internal set; }

	/// <summary>
	/// Indicates the number of the total undo steps.
	/// </summary>
	public int UndoStepsCount => GetSudokuGridViewModel().UndoStepsCount;

	/// <summary>
	/// Indicates the number of the total redo steps.
	/// </summary>
	public int RedoStepsCount => GetSudokuGridViewModel().RedoStepsCount;

	/// <summary>
	/// Gets or sets the current used grid.
	/// </summary>
	public Grid Grid
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Obsolete($"Due to the prefromance, please use the method '{nameof(GridByReference)}' instead.", false)]
		get => GetSudokuGridViewModel().Grid;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => GetSudokuGridViewModel().Grid = value;
	}
	#endregion


	#region Events
	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;
	#endregion


	#region Normal instance methods
	/// <summary>
	/// Undo a step.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void UndoStep() => GetSudokuGridViewModel().Undo();

	/// <summary>
	/// Redo a step.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RedoStep() => GetSudokuGridViewModel().Redo();

	/// <summary>
	/// To make the cell fill the digit.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void MakeDigit(int cell, int digit) => GetSudokuGridViewModel().MakeDigit(cell, digit);

	/// <summary>
	/// To eliminate the digit from the grid.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void EliminateDigit(int cell, int digit) => GetSudokuGridViewModel().EliminateDigit(cell, digit);

	/// <summary>
	/// Gets or sets the current used grid by reference. The method will return by reference, in order to
	/// copy the reference instead of the instance itself, to optimize the memory allocation.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref readonly Grid GridByReference() => ref GetSudokuGridViewModel().GetGridByReference();

	/// <summary>
	/// Gets the <see cref="SudokuGrid"/> instance as the view model.
	/// </summary>
	/// <returns>The <see cref="SudokuGrid"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private SudokuGrid GetSudokuGridViewModel() => _drawingElements.OfType<SudokuGrid>().Single();
	#endregion


	#region Delegated methods
	/// <summary>
	/// Triggers when the current control is tapped via mouse or other devices, by the right clicking.
	/// </summary>
	/// <param name="sender">The object to trigger the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void UserControl_RightTapped(object sender, RightTappedRoutedEventArgs e)
	{
		// Checks the value.
		if (sender is not SudokuPane { Size: var size, OutsideOffset: var outsideOffset } pane)
		{
			goto MakeHandledBeTrueValue;
		}

		// Gets the mouse point and converts to the real cell value.
		var point = e.GetPosition(pane);
		int cell = PointConversions.GetCell(point, size, outsideOffset);

		// Checks whether the cell value is valid.
		if (cell == -1)
		{
			goto MakeHandledBeTrueValue;
		}

		// Sets the tag with the cell value, in order to get the details by other methods later.
		pane.CurrentCell = cell;

		// Try to create the menu flyout and show the items.
		for (int i = 0; i < 9; i++)
		{
			((MenuFlyoutItem)FindName($"_cButtonMake{i + 1}")).Visibility = getVisibilityViaCandidate(cell, i);
		}
		for (int i = 0; i < 9; i++)
		{
			((MenuFlyoutItem)FindName($"_cButtonDelete{i + 1}")).Visibility = getVisibilityViaCandidate(cell, i);
		}

	MakeHandledBeTrueValue:
		// If at an invalid status, just return the method and make the property 'e.Handled' be true value.
		e.Handled = true;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		Visibility getVisibilityViaCandidate(int cell, int i) =>
			Grid.Exists(cell, i) is true ? Visibility.Visible : Visibility.Collapsed;
	}

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
				new OutsideRectangle(_userPreference.OutsideBorderColor, Size, _userPreference.OutsideBorderWidth)
			);
		}

		// Initializes block border lines.
		for (byte i = 0; i < 4; i++)
		{
			_drawingElements.Add(
				new BlockLine(
					_userPreference.BlockBorderColor, _userPreference.BlockBorderWidth,
					Size, OutsideOffset, i
				)
			);
			_drawingElements.Add(
				new BlockLine(
					_userPreference.BlockBorderColor, _userPreference.BlockBorderWidth,
					Size, OutsideOffset, (byte)(i + 4)
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
					_userPreference.CellBorderColor, _userPreference.CellBorderWidth,
					Size, OutsideOffset, i
				)
			);
			_drawingElements.Add(
				new CellLine(
					_userPreference.CellBorderColor, _userPreference.CellBorderWidth,
					Size, OutsideOffset, (byte)(i + 10)
				)
			);
		}

		// TODO: Initializes candidate border lines if worth.

		// Initializes the sudoku grid.
		var sudokuGrid = new SudokuGrid(
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
		);
		sudokuGrid.UndoStackChanged += ([IsDiscard] _) => triggerPropertyChangedVia(nameof(UndoStepsCount));
		sudokuGrid.RedoStackChanged += ([IsDiscard] _) => triggerPropertyChangedVia(nameof(RedoStepsCount));

		_drawingElements.Add(sudokuGrid);

		// Add them into the control collection.
		foreach (var control in from drawingElement in _drawingElements select drawingElement.GetControl())
		{
			_cCanvasMain.Children.Add(control);
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void triggerPropertyChangedVia(string propertyName) => PropertyChanged?.Invoke(this, new(propertyName));
	}

	/// <summary>
	/// Triggers when the target <see cref="MenuFlyoutItem"/> is clicked.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void MakeOrDeleteMenuItem_Click(object sender, [IsDiscard] RoutedEventArgs e)
	{
		if (sender is not MenuFlyoutItem { Tag: string s } || !int.TryParse(s, out int possibleDigit))
		{
			return;
		}

		var (digit, action) = possibleDigit switch
		{
			> 0 => (possibleDigit - 1, MakeDigit),
			< 0 => (~possibleDigit, EliminateDigit), // '~a' is equi to '-a - 1'.
			_ => (default(int), default(Action<int, int>)!)
		};

		action(CurrentCell, digit);
	}
	#endregion
}
