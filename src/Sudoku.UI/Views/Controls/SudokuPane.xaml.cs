namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Defines a user control that interacts with a sudoku grid.
/// </summary>
public sealed partial class SudokuPane : UserControl, INotifyPropertyChanged
{
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
	/// Indicates whether the control is loading at the first time.
	/// </summary>
	private bool _isFirstLoading = true;

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
	/// Indicates the cell being focused.
	/// </summary>
	/// <remarks>
	/// The variable is used for the following members:
	/// <list type="bullet">
	/// <item><see cref="OnPointerMoved(PointerRoutedEventArgs)"/></item>
	/// <item><see cref="OnKeyDown(KeyRoutedEventArgs)"/></item>
	/// </list>
	/// </remarks>
	private int _cell;

	/// <summary>
	/// Indicates the candidate being focused.
	/// </summary>
	/// <remarks>
	/// The variable is used for the following members:
	/// <list type="bullet">
	/// <item><see cref="OnPointerMoved(PointerRoutedEventArgs)"/></item>
	/// <item><see cref="OnKeyDown(KeyRoutedEventArgs)"/></item>
	/// </list>
	/// </remarks>
	private int _candidate;


	/// <summary>
	/// Initializes a <see cref="SudokuPane"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SudokuPane() => InitializeComponent();


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
	/// Indicates the current cell focused. The default value is -1.
	/// </summary>
	public int CurrentCell { get; internal set; } = -1;

	/// <summary>
	/// Indicates the number of the total undo steps.
	/// </summary>
	public int UndoStepsCount => GetSudokuGridViewModel().UndoStepsCount;

	/// <summary>
	/// Indicates the number of the total redo steps.
	/// </summary>
	public int RedoStepsCount => GetSudokuGridViewModel().RedoStepsCount;

	/// <summary>
	/// Gets or sets the current used grid. If you just want to get the value of the grid, please use
	/// the property <see cref="GridRef"/> instead.
	/// </summary>
	/// <seealso cref="GridRef"/>
	public Grid Grid
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => GetSudokuGridViewModel().Grid;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => GetSudokuGridViewModel().Grid = value;
	}

	/// <summary>
	/// Gets the reference of the grid. The property returns by reference in order to copy the reference instead
	/// of the instance itself to provide optimization on memory allocations.
	/// </summary>
	internal ref readonly Grid GridRef
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ref GetSudokuGridViewModel().GridRef;
	}


	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;

	/// <summary>
	/// Indicates an event that is triggered when a file is successfully received using drag and drop operation.
	/// </summary>
	public event EventHandler<object?>? SuccessfullyReceivedDroppedFile;

	/// <summary>
	/// Indicates an event that is triggered when a file is failed to be received using drag and drop operation.
	/// </summary>
	public event EventHandler<object?>? FailedReceivedDroppedFile;

	/// <summary>
	/// Indicates an event that is triggered when a file is failed to be loaded because the file is empty.
	/// </summary>
	public event EventHandler<object?>? DroppedFileIsEmpty;

	/// <summary>
	/// Indicates an event that is triggered when a file is failed to be loaded because the file is larger than 1KB.
	/// </summary>
	public event EventHandler<object?>? DroppedFileIsTooLarge;


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
	/// To fix the grid, to change all modifiable values to given ones.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FixGrid() => GetSudokuGridViewModel().FixGrid();

	/// <summary>
	/// To unfix the grid, to change all given values to modifiable ones.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void UnfixGrid() => GetSudokuGridViewModel().UnfixGrid();

	/// <summary>
	/// To reset the grid, to revert the grid to the initial status.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ResetGrid() => GetSudokuGridViewModel().ResetGrid();

	/// <summary>
	/// To replace the grid with the new one, in order to make the current operation undoable.
	/// </summary>
	/// <param name="grid">The grid to be replaced with.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ReplaceGridUndoable(in Grid grid) => GetSudokuGridViewModel().ReplaceGrid(grid);

	/// <summary>
	/// Mask all value cells.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Mask() => GetSudokuGridViewModelNullable()?.Mask();

	/// <summary>
	/// Unmask all value cells.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Unmask() => GetSudokuGridViewModelNullable()?.Unmask();

	/// <summary>
	/// Shows the candidates.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ShowCandidates() => GetSudokuGridViewModel().UserShowCandidates = true;

	/// <summary>
	/// Hides the candidates.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void HideCandidates() => GetSudokuGridViewModel().UserShowCandidates = false;

#if AUTHOR_FEATURE_CELL_MARKS
	/// <summary>
	/// Sets the cell mark at the specified cell index.
	/// </summary>
	/// <param name="cellIndex">The cell index.</param>
	/// <param name="shapeKind">The shape kind you want to set.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetCellMark(int cellIndex, ShapeKind shapeKind)
		=> GetSudokuGridViewModel().SetCellMark(cellIndex, shapeKind);

	/// <summary>
	/// Expands the cell mark at the specified cell to all cells in the specified house.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="houseType">The house type.</param>
	public void DiffuseCellMark(int cell, HouseType houseType)
	{
		var cellMarks = GetSudokuGridViewModel().GetCellMarks();
		var shapeKind = cellMarks[cell].ShapeKind;
		if (shapeKind == ShapeKind.None)
		{
			// The cell doesn't contain any cell marks.
			return;
		}

		bool coverOldShapeWhenDiffused = ((App)Application.Current).UserPreference.__CoverOldShapeWhenDiffused;
		foreach (int currentCell in HouseMaps[cell.ToHouseIndex(houseType)])
		{
			if (currentCell == cell)
			{
				// Skips for the current cell.
				continue;
			}

			if (!coverOldShapeWhenDiffused && cellMarks[currentCell].ShapeKind != ShapeKind.None)
			{
				// Skips for the cell that is filled by a certain shape.
				continue;
			}

			SetCellMark(currentCell, shapeKind);
		}
	}

	/// <summary>
	/// Clears the cell mark at the specified cell index.
	/// </summary>
	/// <param name="cell">The cell index.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ClearCellMark(int cell) => SetCellMark(cell, ShapeKind.None);
#endif

#if AUTHOR_FEATURE_CANDIDATE_MARKS
	/// <summary>
	/// Sets the candidate mark at the specified candidate index.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	/// <param name="shapeKind">The shape kind.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetCandidateMark(int cell, int digit, ShapeKind shapeKind)
		=> GetSudokuGridViewModel().SetCandidateMark(cell, digit, shapeKind);

	/// <summary>
	/// Expands the candidate mark at the specified cell and the digit to all candidates in the specified house.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	/// <param name="houseType">The house type.</param>
	public void DiffuseCandidateMark(int cell, int digit, HouseType houseType)
	{
		var candidateMarks = GetSudokuGridViewModel().GetCandidateMarks();
		var shapeKind = candidateMarks[cell].GetShapeKind(digit);
		if (shapeKind == ShapeKind.None)
		{
			// The candidate doesn't contain any candidate marks.
			return;
		}

		bool coverOldShapeWhenDiffused = ((App)Application.Current).UserPreference.__CoverOldShapeWhenDiffused;
		var grid = Grid;
		foreach (int currentCell in HouseMaps[cell.ToHouseIndex(houseType)])
		{
			if (currentCell == cell)
			{
				// Skips for the current cell.
				continue;
			}

			if (grid.Exists(currentCell, digit) is not true)
			{
				// Skips for the cell that doesn't contain the digit.
				continue;
			}

			if (!coverOldShapeWhenDiffused && candidateMarks[currentCell].GetShapeKind(digit) != ShapeKind.None)
			{
				// Skips for the candidate that is filled by a certain shape.
				continue;
			}

			SetCandidateMark(currentCell, digit, shapeKind);
		}
	}

	/// <summary>
	/// Clears the candidate mark at the specified candidate index.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ClearCandidateMark(int cell, int digit) => SetCandidateMark(cell, digit, ShapeKind.None);
#endif

#if AUTHOR_FEATURE_CELL_MARKS || AUTHOR_FEATURE_CANDIDATE_MARKS
	/// <summary>
	/// Try to get drawing data.
	/// </summary>
	/// <returns>The drawing data.</returns>
	public string GetDrawingData()
	{
		var sudokuGrid = GetSudokuGridViewModel();

#if AUTHOR_FEATURE_CELL_MARKS
		var cellMarks = sudokuGrid.GetCellMarks();
		var listOfCellMarks = new List<CellMarkInfo>(81);
#endif
#if AUTHOR_FEATURE_CANDIDATE_MARKS
		var candidateMarks = sudokuGrid.GetCandidateMarks();
		var listOfCandidateMarks = new List<CandidateMarkInfo>(729);

		var up = ((App)Application.Current).UserPreference;
#endif
		for (int cellIndex = 0; cellIndex < 81; cellIndex++)
		{
#if AUTHOR_FEATURE_CELL_MARKS
			var cellMark = cellMarks[cellIndex];
			var cellShapeKind = cellMark.ShapeKind;
			if (cellShapeKind != ShapeKind.None)
			{
				listOfCellMarks.Add(new() { CellIndex = cellIndex, ShapeKindRawValue = (int)cellShapeKind });
			}
#endif

#if AUTHOR_FEATURE_CANDIDATE_MARKS
			var candidateMark = candidateMarks[cellIndex];
			for (int digit = 0; digit < 9; digit++)
			{
				var candidateShapeKind = candidateMark.GetShapeKind(digit);
				if (candidateShapeKind != ShapeKind.None)
				{
					listOfCandidateMarks.Add(
						new()
						{
							CellIndex = cellIndex,
							DigitIndex = digit,
							ShapeKindRawValue = (int)candidateShapeKind
						}
					);
				}
			}
#endif
		}

		return JsonSerializer.Serialize(
			new DrawingData
			{
#if AUTHOR_FEATURE_CELL_MARKS
				CellData = listOfCellMarks,
#endif
#if AUTHOR_FEATURE_CANDIDATE_MARKS
				CandidateData = listOfCandidateMarks,
#endif
				ShowCandidates = sudokuGrid.UserShowCandidates,
				GridRawValue = Grid.ToString("#")
			},
			CommonSerializerOptions.CamelCasing
		);
	}
#endif

	/// <inheritdoc/>
	protected override void OnPointerMoved(PointerRoutedEventArgs e)
	{
		base.OnPointerMoved(e);

		var position = e.GetCurrentPoint(this).Position;
		_cell = PointConversions.GetCell(position, Size, OutsideOffset);
		_candidate = PointConversions.GetCandidate(position, Size, OutsideOffset);
		if (GetSudokuGridViewModelNullable() is { } sudokuGrid)
		{
			sudokuGrid.FocusedCell = _cell;
		}
	}

	/// <inheritdoc/>
	protected override void OnKeyDown(KeyRoutedEventArgs e)
	{
		base.OnKeyDown(e);

		// Here we just suppose a mode to fill with value in a cell:
		//     1. Gets the mouse position, and then get the position of the cell.
		//     2. Gets the pressed key from the argument 'e'.
		//     3. Fill the cell with the value.
		if (_cell == -1)
		{
			e.Handled = true;
			return;
		}

		var pressedData = ModifierKeyDownData.FromCurrentState();
		switch (e.Key)
		{
			case var key and (>= Key.Number0 and <= Key.Number9 or Key.Back):
			{
				int digit = key - Key.Number0 - 1;
				switch (pressedData)
				{
#if AUTHOR_FEATURE_CANDIDATE_MARKS
					case (true, true, false): // Control + Shift.
					{
						d(_candidate, key == Key.Back ? 0 : key - Key.Number0);
						break;
					}
#endif
#if AUTHOR_FEATURE_CELL_MARKS
					case (true, false, false): // Control.
					{
						c(_cell, key == Key.Back ? -1 : digit);
						break;
					}
#endif
					case (false, true, false) when key != Key.Back: // Shift.
					{
						EliminateDigit(_cell, digit);
						break;
					}
					case (false, false, false):
					{
						MakeDigit(_cell, key == Key.Back ? -1 : digit);
						break;
					}
				}

				break;
			}
			case var key and (>= Key.NumberPad0 and <= Key.NumberPad9 or Key.Back):
			{
				int digit = key - Key.NumberPad0 - 1;
				switch (pressedData)
				{
#if AUTHOR_FEATURE_CANDIDATE_MARKS
					case (true, true, false): // Control + Shift.
					{
						d(_candidate, key == Key.Back ? 0 : key - Key.NumberPad0);
						break;
					}
#endif
#if AUTHOR_FEATURE_CELL_MARKS
					case (true, false, false): // Control.
					{
						c(_cell, key == Key.Back ? -1 : digit);
						break;
					}
#endif
					case (false, true, false) when key != Key.Back: // Shift.
					{
						EliminateDigit(_cell, digit);
						break;
					}
					case (false, false, false):
					{
						MakeDigit(_cell, key == Key.Back ? -1 : digit);
						break;
					}
				}

				break;
			}
#if AUTHOR_FEATURE_CELL_MARKS || AUTHOR_FEATURE_CANDIDATE_MARKS
			case var key and (Key.Up or Key.Down or Key.Left or Key.Right):
			{
				switch (pressedData)
				{
#if AUTHOR_FEATURE_CANDIDATE_MARKS
					case (true, true, false): // Control + Shift.
					{
						var houseType = key is Key.Up or Key.Down ? HouseType.Column : HouseType.Row;
						DiffuseCandidateMark(_candidate / 9, _candidate % 9, houseType);

						break;
					}
#endif
#if AUTHOR_FEATURE_CELL_MARKS
					case (true, false, false): // Control.
					{
						var houseType = key is Key.Up or Key.Down ? HouseType.Column : HouseType.Row;
						DiffuseCellMark(_cell, houseType);

						break;
					}
#endif
				}

				break;
			}
#endif
			default: // Other cases.
			{
				// Here we must set the value to 'false', in order to process keyboard accelerators.
				// If we set 'e.Handled = true', the keyboard accelerators in the base page won't be triggered.
				e.Handled = false;
				return;
			}


#if AUTHOR_FEATURE_CELL_MARKS
			void c(int cell, int shapeKindIndex) => SetCellMark(cell, (ShapeKind)(byte)(shapeKindIndex + 1));
#endif
#if AUTHOR_FEATURE_CANDIDATE_MARKS
			void d(int candidate, int shapeKindIndex)
			{
				if (shapeKindIndex >= CandidateMark.SupportedShapes.Length)
				{
					return;
				}

				if (Grid.Exists(candidate) is not true)
				{
					return;
				}

				SetCandidateMark(candidate / 9, candidate % 9, CandidateMark.SupportedShapes[shapeKindIndex]);
			}
#endif
		}
	}

	/// <inheritdoc/>
	protected override void OnRightTapped(RightTappedRoutedEventArgs e)
	{
		base.OnRightTapped(e);

		// Gets the mouse point and converts to the real cell value.
		var point = e.GetPosition(this);
		int cell = PointConversions.GetCell(point, Size, OutsideOffset);

		// Checks whether the cell value is valid.
		if (cell == -1)
		{
			return;
		}

		// Sets the tag with the cell value, in order to get the details by other methods later.
		CurrentCell = cell;

		// Try to create the menu flyout and show the items.
		for (int i = 0; i < 9; i++)
		{
			bool digitExists = Grid.Exists(cell, i) is true;
			((Button)FindName($"_cButtonMake{i + 1}")).IsEnabled = digitExists;
			((Button)FindName($"_cButtonDelete{i + 1}")).IsEnabled = digitExists;
		}
	}

	/// <summary>
	/// Loads the grid if the program is opened via opening a file.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void LoadFirstGridIfWorth()
	{
		switch (((App)Application.Current).InitialInfo)
		{
			case { FirstGrid: { } grid } initialInfo:
			{
				// Sets the value.
				Grid = grid;

				// Remove the value to avoid re-triggering.
				initialInfo.FirstGrid = null;

				break;
			}
#if AUTHOR_FEATURE_CELL_MARKS || AUTHOR_FEATURE_CANDIDATE_MARKS
			case { DrawingDataRawValue: { } rawDataValue } initialInfo:
			{
				// Handles the raw data value.
				if (
					JsonSerializer.Deserialize<DrawingData>(rawDataValue, CommonSerializerOptions.CamelCasing) is not (
						var gridRawValue,
						var showCandidates
#if AUTHOR_FEATURE_CELL_MARKS
						,
						var cellData
#endif
#if AUTHOR_FEATURE_CANDIDATE_MARKS
						,
						var candidateData
#endif
					)
				)
				{
					break;
				}

				if (!Grid.TryParse(gridRawValue, out var grid))
				{
					break;
				}

				var a = ShowCandidates;
				var b = HideCandidates;

				Grid = grid;
				(showCandidates ? a : b)();

#if AUTHOR_FEATURE_CELL_MARKS
				if (cellData.Count != 0)
				{
					foreach (var (cellIndex, shapeKindRaw) in cellData)
					{
						SetCellMark(cellIndex, (ShapeKind)(byte)shapeKindRaw);
					}
				}
#endif

#if AUTHOR_FEATURE_CANDIDATE_MARKS
				if (candidateData.Count != 0)
				{
					foreach (var (cellIndex, digitIndex, shapeKindRaw) in candidateData)
					{
						SetCandidateMark(cellIndex, digitIndex, (ShapeKind)(byte)shapeKindRaw);
					}
				}
#endif

				// Remove the value to avoid re-triggering.
				initialInfo.DrawingDataRawValue = null;

				break;
			}
#endif
		}
	}

	/// <summary>
	/// Makes the loading operation completed, sets the field <see cref="_isFirstLoading"/>
	/// to <see langword="false"/> value.
	/// </summary>
	/// <seealso cref="SudokuPane_Loaded(object, RoutedEventArgs)"/>
	/// <seealso cref="_isFirstLoading"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void MakeLoadingOperationCompleted() => _isFirstLoading = false;

	/// <summary>
	/// Initializes <see cref="OutsideRectangle"/> instances, and append
	/// into the collection <see cref="_drawingElements"/>.
	/// </summary>
	/// <param name="up">The user preference instance.</param>
	/// <seealso cref="OutsideRectangle"/>
	/// <seealso cref="_drawingElements"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void InitializeOutsideRectangle(Preference up)
		=> _drawingElements.Add(
			new OutsideRectangle
			{
				PaneSize = Size,
				StrokeColor = up.OutsideBorderColor,
				FillColor = up.GridBackgroundFillColor,
				StrokeThickness = up.OutsideBorderWidth
			}
		);

	/// <summary>
	/// Initializes <see cref="CandidateLine"/>, <see cref="CellLine"/> and <see cref="BlockLine"/> instances,
	/// and append into the collection <see cref="_drawingElements"/>.
	/// </summary>
	/// <param name="up">The user preference instance.</param>
	/// <seealso cref="CandidateLine"/>
	/// <seealso cref="CellLine"/>
	/// <seealso cref="BlockLine"/>
	/// <seealso cref="_drawingElements"/>
	private void InitializeBorderLines(Preference up)
	{
		if (up.ShowCandidateBorderLines)
		{
			for (byte i = 0; i < 28; i++)
			{
				if (i % 3 == 0)
				{
					// Skips the overlapping lines.
					continue;
				}

				_drawingElements.Add(
					new CandidateLine
					{
						Order = i,
						PaneSize = Size,
						OutsideOffset = OutsideOffset,
						StrokeColor = up.CandidateBorderColor,
						StrokeThickness = up.CandidateBorderWidth
					}
				);
				_drawingElements.Add(
					new CandidateLine
					{
						Order = (byte)(i + 28),
						PaneSize = Size,
						OutsideOffset = OutsideOffset,
						StrokeColor = up.CandidateBorderColor,
						StrokeThickness = up.CandidateBorderWidth
					}
				);
			}
		}

		for (byte i = 0; i < 10; i++)
		{
			if (i % 3 == 0)
			{
				// Skips the overlapping lines.
				continue;
			}

			_drawingElements.Add(
				new CellLine
				{
					Order = i,
					PaneSize = Size,
					OutsideOffset = OutsideOffset,
					StrokeColor = up.CellBorderColor,
					StrokeThickness = up.CellBorderWidth
				}
			);
			_drawingElements.Add(
				new CellLine
				{
					Order = (byte)(i + 10),
					PaneSize = Size,
					OutsideOffset = OutsideOffset,
					StrokeColor = up.CellBorderColor,
					StrokeThickness = up.CellBorderWidth
				}
			);
		}

		for (byte i = 0; i < 4; i++)
		{
			_drawingElements.Add(
				new BlockLine
				{
					Order = i,
					PaneSize = Size,
					OutsideOffset = OutsideOffset,
					StrokeColor = up.BlockBorderColor,
					StrokeThickness = up.BlockBorderWidth
				}
			);
			_drawingElements.Add(
				new BlockLine
				{
					Order = (byte)(i + 4),
					PaneSize = Size,
					OutsideOffset = OutsideOffset,
					StrokeColor = up.BlockBorderColor,
					StrokeThickness = up.BlockBorderWidth
				}
			);
		}
	}

	/// <summary>
	/// Initializes <see cref="SudokuGrid"/> instances, and append into the collection <see cref="_drawingElements"/>.
	/// </summary>
	/// <param name="up">The user preference instance.</param>
	/// <seealso cref="SudokuGrid"/>
	/// <seealso cref="_drawingElements"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void InitializeSudokuGrid(Preference up)
		=> _drawingElements.Add(
			new SudokuGrid
			{
				AllowMarkups = true,
				Preference = up,
				PaneSize = Size,
				OutsideOffset = OutsideOffset,
				Grid = Grid.Empty,
				UndoRedoStepsUpdatedCallback = () =>
				{
					PropertyChanged?.Invoke(this, new(nameof(UndoStepsCount)));
					PropertyChanged?.Invoke(this, new(nameof(RedoStepsCount)));
				},
				FocusedCell = -1
			}
		);

	/// <summary>
	/// Adds the controls into the canvas.
	/// </summary>
	private void AddIntoCanvas()
	{
		foreach (var control in from drawingElement in _drawingElements select drawingElement.GetControl())
		{
			_cCanvasMain.Children.Add(control);
		}
	}

	/// <summary>
	/// Gets the <see cref="SudokuGrid"/> instance as the view model.
	/// </summary>
	/// <returns>The <see cref="SudokuGrid"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private SudokuGrid GetSudokuGridViewModel() => _drawingElements.OfType<SudokuGrid>().Single();

	/// <summary>
	/// Gets the <see cref="SudokuGrid"/> instance as the view model, and return <see langword="null"/>
	/// if the collection doesn't contain any <see cref="SudokuGrid"/> elements.
	/// </summary>
	/// <returns>
	/// The <see cref="SudokuGrid"/> instance, or <see langword="null"/>
	/// if the collection doesn't contain any <see cref="SudokuGrid"/> elements.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private SudokuGrid? GetSudokuGridViewModelNullable() => _drawingElements.OfType<SudokuGrid>().FirstOrDefault();


	/// <summary>
	/// Triggers when the pane is loaded.
	/// </summary>
	/// <param name="sender">The object to trigger the event. The instance is always itself.</param>
	/// <param name="e">The event arguments provided.</param>
	private void SudokuPane_Loaded(object sender, RoutedEventArgs e)
	{
		// If the initialization operation is not the first time, we should skip the initialization.
		if (!_isFirstLoading)
		{
			return;
		}

		// Extracts the values from resource dictionary.
		// The variable extraction is required in multi-threading.
		string phase1Str = R["LoadingSeparateParts"]!;
		string phase2Str = R["LoadingSudokuGrid"]!;

		// Initialize controls.
#if false
		_cLoading.Visibility = Visibility.Visible;
		var up = ((App)Application.Current).UserPreference;

		_cLoadingText.Text = phase1Str;

		InitializeOutsideRectangle(up);
		InitializeBorderLines(up);

		_cLoadingText.Text = phase2Str;

		InitializeSudokuGrid(up);

		AddIntoCanvas();
		LoadFirstGridIfWorth();
		MakeLoadingOperationCompleted();

		_cLoading.Visibility = Visibility.Collapsed;
#else
		var up = ((App)Application.Current).UserPreference;

		InitializeOutsideRectangle(up);
		InitializeBorderLines(up);
		InitializeSudokuGrid(up);
		AddIntoCanvas();
		LoadFirstGridIfWorth();
		MakeLoadingOperationCompleted();
#endif
	}

	/// <summary>
	/// Triggers when the mouse is moved into the current control.
	/// </summary>
	/// <param name="sender">The object to trigger the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void SudokuPane_PointerEntered(object sender, PointerRoutedEventArgs e)
		// Set the focus to the control in order to trigger the 'KeyDown' event.
		=> Focus(FocusState.Programmatic);

	/// <summary>
	/// Triggers when a thing is dragging over the control.
	/// </summary>
	/// <param name="sender">The object to trigger the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void SudokuPane_DragOver(object sender, DragEventArgs e)
	{
		e.AcceptedOperation = DataPackageOperation.Copy;
		e.DragUIOverride.Caption = R["DropSudokuFileHere"]!;
		e.DragUIOverride.IsCaptionVisible = true;
		e.DragUIOverride.IsContentVisible = true;
	}

	/// <summary>
	/// Triggers when a thing is dropping over the control.
	/// </summary>
	/// <param name="sender">The object to trigger the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private async void SudokuPane_DropAsync(object sender, DragEventArgs e)
	{
		if (e.DataView is not { } dataView)
		{
			return;
		}

		if (!dataView.Contains(StandardDataFormats.StorageItems))
		{
			return;
		}

		switch (await dataView.GetStorageItemsAsync())
		{
#pragma warning disable IDE0055
			case [StorageFolder folder] when await folder.GetFilesAsync(CommonFileQuery.DefaultQuery, 0, 2) is 
			[
				StorageFile
				{
					FileType: CommonFileExtensions.Text or CommonFileExtensions.Sudoku,
					Path: var path
				}
			]:
#pragma warning restore IDE0055
			{
				await handleSudokuFile(path);

				break;
			}
			case [StorageFile { FileType: CommonFileExtensions.Text or CommonFileExtensions.Sudoku, Path: var path }]:
			{
				await handleSudokuFile(path);

				break;
			}


			async Task handleSudokuFile(string path)
			{
				switch (new FileInfo(path).Length)
				{
					case 0:
					{
						DroppedFileIsEmpty?.Invoke(this, null);
						FailedReceivedDroppedFile?.Invoke(this, null);
						break;
					}
					case > 1024:
					{
						DroppedFileIsTooLarge?.Invoke(this, null);
						FailedReceivedDroppedFile?.Invoke(this, null);
						break;
					}
					default:
					{
						string content = await SioFile.ReadAllTextAsync(path);
						if (string.IsNullOrWhiteSpace(content))
						{
							return;
						}

						if (!Grid.TryParse(content, out var grid))
						{
							return;
						}

						Grid = grid;

						SuccessfullyReceivedDroppedFile?.Invoke(this, null);
						break;
					}
				}
			}
		}
	}

	/// <summary>
	/// Triggers when the target <see cref="MenuFlyoutItem"/> is clicked.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void MakeOrDeleteMenuItem_Click(object sender, RoutedEventArgs e)
	{
		if (sender is not Button { Tag: string s } || !int.TryParse(s, out int possibleDigit))
		{
			return;
		}

		var (digit, action) = possibleDigit switch
		{
			> 0 => (possibleDigit - 1, MakeDigit),
			< 0 => (~possibleDigit, EliminateDigit),
			_ => default((int, Action<int, int>?))
		};
		action!(CurrentCell, digit);
	}
}
