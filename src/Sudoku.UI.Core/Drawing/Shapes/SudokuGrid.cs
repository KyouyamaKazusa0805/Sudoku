namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a sudoku grid.
/// </summary>
public sealed class SudokuGrid : DrawingElement
{
	/// <summary>
	/// Indicates the possible cell indices.
	/// </summary>
	private static readonly int[] CellIndices = Enumerable.Range(0, 81).ToArray();

	/// <summary>
	/// Indicates the digit labels.
	/// </summary>
	private static readonly int[] Digits = Enumerable.Range(0, 9).ToArray();


	/// <summary>
	/// Indicates the inner grid layout control.
	/// </summary>
	private readonly GridLayout _gridLayout = new GridLayout()
		.WithRowDefinitionsCount(9)
		.WithColumnDefinitionsCount(9)
		.WithHorizontalAlignment(HorizontalAlignment.Center)
		.WithVerticalAlignment(VerticalAlignment.Center);

	/// <summary>
	/// Indicates the cell digits.
	/// </summary>
	private readonly CellDigit[] _cellDigits = new CellDigit[81];

	/// <summary>
	/// Indicates the candidate digits.
	/// </summary>
	private readonly CandidateDigit[] _candidateDigits = new CandidateDigit[81];

	/// <summary>
	/// Indicates the stacks to store the undoing and redoing steps.
	/// </summary>
	private readonly Stack<Grid> _undoSteps = new(), _redoSteps = new();

	/// <summary>
	/// Indicates the focused cell rectangle.
	/// </summary>
	private readonly Rectangle _focusedRectangle = null!;

	/// <summary>
	/// Indicates the rectangles displaying for peers of the focused cell.
	/// </summary>
	private readonly Rectangle[] _peerFocusedRectangle = new Rectangle[20];

	/// <summary>
	/// Indicates the cell view node shapes.
	/// </summary>
	private readonly CellViewNodeShape[] _cellViewNodeShapes = new CellViewNodeShape[81];

	/// <summary>
	/// Indicates the candidate view node shapes.
	/// </summary>
	private readonly CandidateViewNodeShape[] _candidateViewNodeShapes = new CandidateViewNodeShape[81];

	/// <summary>
	/// Indicates the user preference used.
	/// </summary>
	private readonly IDrawingPreference _preference = null!;

	/// <summary>
	/// Indicates whether the current mode is mask mode.
	/// </summary>
	private bool _isMaskMode;

	/// <summary>
	/// Indicates whether the current grid pane shows candidates regardless of the value in the preference file.
	/// </summary>
	private bool _showsCandidates;

	/// <summary>
	/// Indicates the pane size.
	/// </summary>
	private double _paneSize;

	/// <summary>
	/// Indicates the outside offset.
	/// </summary>
	private double _outsideOffset;

	/// <summary>
	/// Indicates the focused cell.
	/// </summary>
	private int _focusedCell;

	/// <summary>
	/// Indicates the inner grid.
	/// </summary>
	private Grid _grid;

	/// <summary>
	/// Indicates the views displayed.
	/// </summary>
	private View[]? _views;

#if AUTHOR_FEATURE_CELL_MARKS
	/// <summary>
	/// Indicates the cell marks.
	/// The value won't be <see langword="null"/> if the field <see cref="AllowMarkups"/> is <see langword="true"/>.
	/// </summary>
	private CellMark[]? _cellMarks;
#endif

#if AUTHOR_FEATURE_CANDIDATE_MARKS
	/// <summary>
	/// Indicates the candidate marks.
	/// The value won't be <see langword="null"/> if the field <see cref="AllowMarkups"/> is <see langword="true"/>.
	/// </summary>
	private CandidateMark[]? _candidateMarks;
#endif


	/// <summary>
	/// Indicates whether allow users drawing.
	/// </summary>
#if AUTHOR_FEATURE_CELL_MARKS || AUTHOR_FEATURE_CANDIDATE_MARKS
	[MemberNotNullWhen(
		true
#if AUTHOR_FEATURE_CELL_MARKS
		,
		nameof(_cellMarks)
#endif
#if AUTHOR_FEATURE_CANDIDATE_MARKS
		,
		nameof(_candidateMarks)
#endif
	)]
#endif
	public required bool AllowMarkups { get; init; }

	/// <summary>
	/// <para>Indicates whether the grid displays for candidates.</para>
	/// <para>
	/// This property will also change the value in the user preference. If you want to temporarily change
	/// the value, use <see cref="UserShowCandidates"/> instead.
	/// </para>
	/// </summary>
	/// <seealso cref="UserShowCandidates"/>
	public bool ShowCandidates
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _preference.ShowCandidates;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_preference.ShowCandidates == value || _isMaskMode)
			{
				return;
			}

			_preference.ShowCandidates = value;
			Array.ForEach(_candidateDigits, candidateDigit => candidateDigit.ShowCandidates = value);
		}
	}

	/// <summary>
	/// <para>Indicates whether the grid displays for candidates.</para>
	/// <para>
	/// This property will temporarily change the state of the displaying candidates. The property doesn't
	/// modify the user preference. If you want to modify the user preference value, use <see cref="ShowCandidates"/>
	/// instead.
	/// </para>
	/// </summary>
	/// <seealso cref="ShowCandidates"/>
	public bool UserShowCandidates
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _showsCandidates;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_showsCandidates == value || _isMaskMode)
			{
				return;
			}

			_showsCandidates = value;
			Array.ForEach(_candidateDigits, candidateDigit => candidateDigit.UserShowCandidates = value);
		}
	}

	/// <summary>
	/// Indicates whether the current mode is mask mode.
	/// </summary>
	public bool IsMaskMode
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _isMaskMode;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_isMaskMode == value)
			{
				return;
			}

			var a = Mask;
			var b = Unmask;
			(value ? a : b)();
		}
	}

	/// <summary>
	/// Gets or sets the outside offset.
	/// </summary>
	public required double OutsideOffset
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _outsideOffset;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_outsideOffset.NearlyEquals(value))
			{
				return;
			}

			_outsideOffset = value;
			_gridLayout.Padding = new(value);
		}
	}

	/// <summary>
	/// Gets or sets the pane size.
	/// </summary>
	public required double PaneSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _paneSize;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_paneSize.NearlyEquals(value))
			{
				return;
			}

			_paneSize = value;
			_gridLayout.Width = value;
			_gridLayout.Height = value;
		}
	}

	/// <summary>
	/// Indicates the current view index.
	/// </summary>
	public int ViewIndex { get; set; } = -1;

	/// <summary>
	/// Indicates the focused cell used.
	/// </summary>
	public int FocusedCell
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _focusedCell;

		set
		{
			if (_focusedCell == value)
			{
				return;
			}

			if (_preference.PeerFocusingMode == PeerFocusingMode.None)
			{
				return;
			}

			_focusedCell = value;
			switch (value, _preference.PeerFocusingMode)
			{
				case (-1, _):
				{
					_focusedCell = value;

					_focusedRectangle.Visibility = Visibility.Collapsed;
					Array.ForEach(_peerFocusedRectangle, CommonMethods.HideControl);

					break;
				}
				case (_, PeerFocusingMode.FocusedCellAndPeerCells):
				{
					for (int i = 0; i < 20; i++)
					{
						var rectangle = _peerFocusedRectangle[i];
						int cell = Peers[value][i];

						rectangle.Visibility = Visibility.Visible;
						GridLayout.SetRow(rectangle, cell / 9);
						GridLayout.SetColumn(rectangle, cell % 9);
					}

					goto Previous;
				}
#pragma warning disable IDE0055
				case (_, PeerFocusingMode.FocusedCell):
				Previous:
				{
					_focusedRectangle.Visibility = Visibility.Visible;
					GridLayout.SetRow(_focusedRectangle, value / 9);
					GridLayout.SetColumn(_focusedRectangle, value % 9);

					break;
				}
#pragma warning restore IDE0055
			}
		}
	}

	/// <summary>
	/// Gets or sets the grid. If you want to get the inner sudoku grid puzzle instance,
	/// we suggest you use the property <see cref="GridRef"/> instead of using the accessor
	/// <see cref="Grid"/>.<see langword="get"/> because that property (i.e. <see cref="GridRef"/>) copies by reference.
	/// </summary>
	/// <seealso cref="GridRef"/>
	public Grid Grid
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _grid;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			// If the current mode is mask mode, we should skip the operation and do nothing.
			if (_isMaskMode)
			{
				return;
			}

			// Set the new grid and update the view.
			_grid = value;

			// Clear the wrong status for all cell digits and candidate digits.
			Array.ForEach(_cellDigits, static cellDigit => cellDigit.IsGiven = false);
			Array.ForEach(_candidateDigits, static candidateDigit => candidateDigit.WrongDigitMask = 0);

			// Update the view.
			UpdateView();

#if AUTHOR_FEATURE_CELL_MARKS
			// Clears the cell marks.
			Array.ForEach(CellIndices, ClearCellMark);
#endif
#if AUTHOR_FEATURE_CANDIDATE_MARKS
			// Clears the candidate marks.
			Array.ForEach(CellIndices, cell => Array.ForEach(Digits, digit => ClearCandidateMark(cell, digit)));
#endif

			// The operation must clear two stacks, and trigger the handler '_undoRedoStepsUpdatedCallback'.
			_undoSteps.Clear();
			_redoSteps.Clear();
			UndoRedoStepsUpdatedCallback?.Invoke();
		}
	}

	/// <summary>
	/// Indicates the current view index.
	/// </summary>
	public View CurrentView => _views![ViewIndex];

	/// <summary>
	/// Indicates the current views.
	/// </summary>
	[DisallowNull]
	public View[]? Views
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _views;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_views = value;

			SetViewNodes(value[ViewIndex = 0]);
		}
	}

	/// <summary>
	/// Indicates the callback method that invokes when the undoing and redoing steps are updated.
	/// </summary>
	public required Action? UndoRedoStepsUpdatedCallback { get; init; }

	/// <summary>
	/// Indicates the user preference instance.
	/// </summary>
	public required IDrawingPreference Preference
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _preference;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[MemberNotNull(nameof(_preference), nameof(_focusedRectangle))]
		init
		{
			_preference = value;

			_focusedRectangle = new Rectangle()
				.WithFill(value.FocusedCellColor)
				.WithVisibility(Visibility.Collapsed)
				.WithGridLayout(row: 4, column: 4)
				.WithCanvasZIndex(-1);

			_showsCandidates = value.ShowCandidates;

			initializePeerRectangles(_peerFocusedRectangle, value);
			initializeValues(value);


			static void initializePeerRectangles(Rectangle[] rectangles, IDrawingPreference preference)
			{
				foreach (ref var rectangle in rectangles.EnumerateRef())
				{
					rectangle = new Rectangle()
						.WithFill(preference.PeersFocusedCellColor)
						.WithVisibility(Visibility.Collapsed)
						.WithGridLayout(row: 4, column: 4)
						.WithCanvasZIndex(-1);
				}
			}

			void initializeValues(IDrawingPreference preference)
			{
#if AUTHOR_FEATURE_CELL_MARKS || AUTHOR_FEATURE_CANDIDATE_MARKS
				if (AllowMarkups)
				{
#if AUTHOR_FEATURE_CELL_MARKS
					_cellMarks = new CellMark[81];
#endif
#if AUTHOR_FEATURE_CANDIDATE_MARKS
					_candidateMarks = new CandidateMark[81];
#endif
				}
#endif

				for (int i = 0; i < 81; i++)
				{
					ref var p = ref _cellDigits[i];
					p = new()
					{
						UserPreference = preference,
						IsGiven = false,
						IsMaskMode = false,
						Digit = byte.MaxValue
					};
					_gridLayout.Children.Add(p.GetControl().WithGridLayout(row: i / 9, column: i % 9));
					_gridLayout.Children.Add(p.GetMaskEllipseControl().WithGridLayout(row: i / 9, column: i % 9));

					ref var q = ref _candidateDigits[i];
					q = new()
					{
						UserPreference = preference,
						CandidateMask = 511,
						WrongDigitMask = 0,
						IsMaskMode = false
					};
					_gridLayout.Children.Add(q.GetControl().WithGridLayout(row: i / 9, column: i % 9));

					ref var r = ref _cellViewNodeShapes[i];
					r = new() { Preference = preference, IsVisible = false };
					_gridLayout.Children.Add(r.GetControl().WithGridLayout(row: i / 9, column: i % 9));

					ref var s = ref _candidateViewNodeShapes[i];
					s = new() { Preference = preference };
					Array.ForEach(Digits, digit => _candidateViewNodeShapes[i].SetIsVisible(digit, false));
					_gridLayout.Children.Add(s.GetControl().WithGridLayout(row: i / 9, column: i % 9));

#if AUTHOR_FEATURE_CELL_MARKS
					if (AllowMarkups)
					{
						// Initializes for the cell marks.
						ref var cellMark = ref _cellMarks[i];
						cellMark = new(preference);
						foreach (var cellMarkControl in cellMark.GetControls())
						{
							_gridLayout.Children.Add(cellMarkControl.WithGridLayout(row: i / 9, column: i % 9));
						}
					}
#endif

#if AUTHOR_FEATURE_CANDIDATE_MARKS
					if (AllowMarkups)
					{
						// Initializes for the candidate marks.
						ref var candidateMark = ref _candidateMarks[i];
						candidateMark = new(preference);
						_gridLayout.Children.Add(candidateMark.GetControl().WithGridLayout(row: i / 9, column: i % 9));
					}
#endif

					// Initializes for the items to render the focusing elements.
					if (_focusedCell == i)
					{
						_gridLayout.Children.Add(
							_focusedRectangle
								.WithGridLayout(row: _focusedCell / 9, column: _focusedCell % 9)
						);

						for (int peerIndex = 0; peerIndex < 20; peerIndex++)
						{
							int peerCell = Peers[_focusedCell][peerIndex];
							_gridLayout.Children.Add(
								_peerFocusedRectangle[peerIndex]
									.WithGridLayout(row: peerCell / 9, column: peerCell % 9)
							);
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Indicates the number of available undoable steps.
	/// </summary>
	internal int UndoStepsCount => _undoSteps.Count;

	/// <summary>
	/// Indicates the number of available redoable steps.
	/// </summary>
	internal int RedoStepsCount => _redoSteps.Count;

	/// <summary>
	/// Gets the reference of the grid. The method is used for getting the grid instance by reference.
	/// </summary>
	internal ref readonly Grid GridRef
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ref _grid;
	}

	/// <inheritdoc/>
	protected override string TypeIdentifier => nameof(SudokuGrid);


	/// <summary>
	/// To undo a step.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Undo()
	{
		if (_isMaskMode || _undoSteps.Count == 0)
		{
			return;
		}

		_redoSteps.Push(_grid);

		var previousStep = _undoSteps.Pop();
		_grid = previousStep;

		UndoRedoStepsUpdatedCallback?.Invoke();

		UpdateView();
	}

	/// <summary>
	/// To redo a step.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Redo()
	{
		if (_isMaskMode || _redoSteps.Count == 0)
		{
			return;
		}

		_undoSteps.Push(_grid);

		var nextStep = _redoSteps.Pop();
		_grid = nextStep;

		UndoRedoStepsUpdatedCallback?.Invoke();

		UpdateView();
	}

	/// <summary>
	/// To make the specified cell fill the specified digit.
	/// </summary>
	/// <param name="cell">The cell that the conclusion is from.</param>
	/// <param name="digit">The digit made.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void MakeDigit(int cell, int digit)
	{
		if (_isMaskMode)
		{
			// If the current mode is mask mode, we should directly skip the operation.
			return;
		}

		if (_grid.GetStatus(cell) == CellStatus.Given)
		{
			// Skips the case that the cell is a given.
			return;
		}

		if (digit == -1)
		{
			// Skip the case that the user input 0 but the cell is currently empty.
			return;
		}

		// Stores the previous grid status to the undo stack.
		AddStep(_grid);

		// To re-compute candidates if the current cell is modifiable.
		if (_grid.GetStatus(cell) == CellStatus.Modifiable)
		{
			_grid[cell] = -1;
		}

		// Then set the new value.
		// Please note that the previous statement '_grid[cell] = -1' will re-compute candidates,
		// so it's not a redundant statement. For more information for the indexer,
		// please visit the member 'Grid.this[int].set'.
		// If you remove it, the candidates won't be re-calculated and cause a bug that the candidate
		// not being refreshed.
		_grid[cell] = digit;

		// To update the view.
		UpdateView();
	}

	/// <summary>
	/// To eliminate the specified digit from the specified cell.
	/// </summary>
	/// <param name="cell">The cell that the eliminated digit is from.</param>
	/// <param name="digit">The digit eliminated.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void EliminateDigit(int cell, int digit)
	{
		if (_isMaskMode)
		{
			// If the current mode is mask mode, we should directly skip the operation.
			return;
		}

		if (digit == -1)
		{
			// Skips the invalid data.
			return;
		}

		if (_grid.Exists(cell, digit) is not true)
		{
			// Skips the case that user removes a candidate that doesn't exist.
			return;
		}

		// Stores the previous grid status to the undo stack.
		AddStep(_grid);

		// Update the grid and view.
		_grid[cell, digit] = false;
		UpdateView();
	}

	/// <summary>
	/// To fix the grid, which means all modifiable digits will be changed their own status to given ones.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FixGrid()
	{
		// If the current mode is mask mode, we should directly skip the operation.
		if (_isMaskMode)
		{
			return;
		}

		// Stores the previous grid status to the undo stack.
		AddStep(_grid);

		// Update the grid and view.
		_grid.Fix();
		UpdateView();
	}

	/// <summary>
	/// To unfix the grid, which means all given digits will be changed their own status to modifiable ones.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void UnfixGrid()
	{
		// If the current mode is mask mode, we should directly skip the operation.
		if (_isMaskMode)
		{
			return;
		}

		// Stores the previous grid status to the undo stack.
		AddStep(_grid);

		// Update the grid and view.
		_grid.Unfix();
		UpdateView();
	}

	/// <summary>
	/// To reset the grid, which means all value having been filled into the grid as modifiable ones
	/// will be cleared.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ResetGrid()
	{
		// If the current mode is mask mode, we should directly skip the operation.
		if (_isMaskMode)
		{
			return;
		}

		// Stores the previous grid status to the undo stack.
		AddStep(_grid);

		// Update the grid and view.
		_grid.Reset();
		UpdateView();

#if AUTHOR_FEATURE_CELL_MARKS
		Array.ForEach(CellIndices, ClearCellMark);
#endif
#if AUTHOR_FEATURE_CANDIDATE_MARKS
		Array.ForEach(CellIndices, cell => Array.ForEach(Digits, digit => ClearCandidateMark(cell, digit)));
#endif
	}

	/// <summary>
	/// To replace with the new grid.
	/// </summary>
	/// <param name="grid">The grid to be replaced with.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ReplaceGrid(in Grid grid)
	{
		// If the current mode is mask mode, we should directly skip the operation.
		if (_isMaskMode)
		{
			return;
		}

		// Stores the previous grid status to the undo stack.
		AddStep(_grid);

		// Update the grid and view.
		_grid = grid;
		UpdateView();

#if AUTHOR_FEATURE_CELL_MARKS
		Array.ForEach(CellIndices, ClearCellMark);
#endif
#if AUTHOR_FEATURE_CANDIDATE_MARKS
		Array.ForEach(CellIndices, cell => Array.ForEach(Digits, digit => ClearCandidateMark(cell, digit)));
#endif
	}

	/// <summary>
	/// To mask the grid.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Mask()
	{
		_isMaskMode = true;
		Array.ForEach(_cellDigits, static element => element.IsMaskMode = true);
		Array.ForEach(_candidateDigits, static element => element.IsMaskMode = true);

#if AUTHOR_FEATURE_CELL_MARKS || AUTHOR_FEATURE_CANDIDATE_MARKS
		if (AllowMarkups)
		{
#if AUTHOR_FEATURE_CELL_MARKS
			Array.ForEach(_cellMarks, static element => element.ShowMark = false);
#endif
#if AUTHOR_FEATURE_CANDIDATE_MARKS
			Array.ForEach(Digits, i => Array.ForEach(_candidateMarks, element => element.SetShowMark(i, false)));
#endif
		}
#endif
	}

	/// <summary>
	/// To unmask the grid.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Unmask()
	{
		_isMaskMode = false;
		Array.ForEach(_cellDigits, static element => element.IsMaskMode = false);
		Array.ForEach(_candidateDigits, static element => element.IsMaskMode = false);

#if AUTHOR_FEATURE_CELL_MARKS || AUTHOR_FEATURE_CANDIDATE_MARKS
		if (AllowMarkups)
		{
#if AUTHOR_FEATURE_CELL_MARKS
			Array.ForEach(_cellMarks, static element => element.ShowMark = true);
#endif
#if AUTHOR_FEATURE_CANDIDATE_MARKS
			Array.ForEach(Digits, i => Array.ForEach(_candidateMarks, element => element.SetShowMark(i, true)));
#endif
		}
#endif
	}

	/// <summary>
	/// Sets the previous view.
	/// </summary>
	public void SetPreviousView()
	{
		if (ViewIndex - 1 < 0)
		{
			return;
		}

		if (_views is null)
		{
			return;
		}

		SetViewNodes(_views[--ViewIndex]);
	}

	/// <summary>
	/// Sets the next view.
	/// </summary>
	public void SetNextView()
	{
		if (_views is null)
		{
			return;
		}

		if (ViewIndex + 1 >= _views.Length)
		{
			return;
		}

		SetViewNodes(_views[++ViewIndex]);
	}

	/// <summary>
	/// Sets the cell view node.
	/// </summary>
	/// <param name="cellViewNode">The cell view node.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetCellViewNode(CellViewNode cellViewNode)
	{
		if (_isMaskMode)
		{
			return;
		}

		ref var current = ref _cellViewNodeShapes[cellViewNode.Cell];
		current.IsVisible = true;
		current.ColorIdentifier = cellViewNode.Identifier;
	}

	/// <summary>
	/// Sets the candidate view node.
	/// </summary>
	/// <param name="candidateViewNode">The candidate view node.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetCandidateViewNode(CandidateViewNode candidateViewNode)
	{
		if (_isMaskMode)
		{
			return;
		}

		int cell = candidateViewNode.Candidate / 9;
		int digit = candidateViewNode.Candidate % 9;
		ref var current = ref _candidateViewNodeShapes[cell];
		current.SetIsVisible(digit, true);
		current.SetIdentifier(digit, candidateViewNode.Identifier);
	}

	/// <summary>
	/// Clears all view nodes.
	/// </summary>
	public void ClearViewNodes()
	{
		if (_isMaskMode)
		{
			return;
		}

		// TODO: Sets other kinds of view nodes.
		Array.ForEach(_cellViewNodeShapes, shape => shape.IsVisible = false);
		Array.ForEach(_candidateViewNodeShapes, shape => Array.ForEach(Digits, digit => shape.SetIsVisible(digit, false)));
	}

	/// <summary>
	/// Clears the cell view node.
	/// </summary>
	/// <param name="cellIndex">The cell index.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ClearCellViewNode(int cellIndex)
	{
		if (_isMaskMode)
		{
			return;
		}

		_cellViewNodeShapes[cellIndex].IsVisible = false;
	}

	/// <summary>
	/// Clears the candidate view node.
	/// </summary>
	/// <param name="candidateIndex">The candidate index.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ClearCandidateViewNode(int candidateIndex)
	{
		if (_isMaskMode)
		{
			return;
		}

		_candidateViewNodeShapes[candidateIndex / 9].SetIsVisible(candidateIndex % 9, false);
	}

#if AUTHOR_FEATURE_CELL_MARKS
	/// <summary>
	/// Sets the mark shape at the specified cell index.
	/// </summary>
	/// <param name="cellIndex">The cell index.</param>
	/// <param name="shapeKind">
	/// The shape kind you want to assign. If the value is <see cref="ShapeKind.None"/>,
	/// the method will clear the displaying of the shape. In this case you can also call the method
	/// <see cref="ClearCellMark(int)"/>. They are same.
	/// </param>
	/// <seealso cref="ClearCellMark(int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetCellMark(int cellIndex, ShapeKind shapeKind)
	{
		if (_isMaskMode || !AllowMarkups)
		{
			return;
		}

		_cellMarks[cellIndex].ShapeKind = shapeKind;
	}

	/// <summary>
	/// Clears the mark shape at the specified cell index.
	/// </summary>
	/// <param name="cellIndex">The cell index.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ClearCellMark(int cellIndex)
	{
		if (_isMaskMode)
		{
			return;
		}

		SetCellMark(cellIndex, ShapeKind.None);
	}
#endif

#if AUTHOR_FEATURE_CANDIDATE_MARKS
	/// <summary>
	/// Sets the shape at the specified candidate index.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	/// <param name="shapeKind">The shape kind.</param>
	public void SetCandidateMark(int cell, int digit, ShapeKind shapeKind)
	{
		if (_isMaskMode || !AllowMarkups)
		{
			return;
		}

		_candidateMarks[cell].SetShapeKind(digit, shapeKind);
	}

	/// <summary>
	/// Clears the mark at the specified candidate index.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ClearCandidateMark(int cell, int digit) => SetCandidateMark(cell, digit, ShapeKind.None);
#endif

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] DrawingElement? other)
		=> other is SudokuGrid comparer && _grid == comparer._grid;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(TypeIdentifier, _grid.GetHashCode());

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override GridLayout GetControl() => _gridLayout;

#if AUTHOR_FEATURE_CELL_MARKS
	/// <summary>
	/// Try to get the inner field <c>_cellMarks</c>.
	/// </summary>
	/// <returns>The cell marks.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the current sudoku grid doesn't support drawing.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal CellMark[] GetCellMarks()
		=> _cellMarks ?? throw new InvalidOperationException("The current sudoku grid doesn't support drawing.");
#endif

#if AUTHOR_FEATURE_CANDIDATE_MARKS
	/// <summary>
	/// Try to get the inner field <c>_candidateMarks</c>.
	/// </summary>
	/// <returns>The candidate marks.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the current sudoku grid doesn't support drawing.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal CandidateMark[] GetCandidateMarks()
		=> _candidateMarks ?? throw new InvalidOperationException("The current sudoku grid doesn't support drawing.");
#endif

	/// <summary>
	/// Adds the specified step into the collection.
	/// </summary>
	/// <param name="grid">The step to be added.</param>
	private void AddStep(in Grid grid)
	{
		_undoSteps.Push(_grid);
		_grid = grid;

		_redoSteps.Clear();

		UndoRedoStepsUpdatedCallback?.Invoke();
	}

	/// <summary>
	/// To update the view via the current grid.
	/// </summary>
	private void UpdateView()
	{
		// Iterates on each cell.
		for (int i = 0; i < 81; i++)
		{
			// Checks the status of the cell.
			switch (_grid.GetStatus(i))
			{
				case CellStatus.Empty:
				{
					// Due to the empty cell, we should set the current cell value to byte.MaxValue
					// in order not to display the value in the view.
					_cellDigits[i].Digit = byte.MaxValue;

					// Gets the current candidate mask, and set the value in order to update the view.
					short candidateMask = _grid.GetCandidates(i);
					_candidateDigits[i].CandidateMask = candidateMask;

					// Checks the correctness of the candidates.
					// If a certain digit has been wrongly removed from the grid, we should display it
					// using a different color if enabled the delta view.
					if (_preference.EnableDeltaValuesDisplaying
						&& _grid.ResetGrid.Solution is { IsUndefined: false } solution)
					{
						// Checks the wrong digits.
						// Wrong digits are the correct digits in the solution but they have been eliminated.
						_candidateDigits[i].WrongDigitMask = (short)(511 & ~candidateMask & 1 << solution[i]);
					}

					break;
				}
				case var status and (CellStatus.Given or CellStatus.Modifiable):
				{
					// Gets the current digit input, and set the value in order to update the view.
					byte digit = (byte)_grid[i];
					_cellDigits[i].Digit = digit;

					// Due to the value cell, we should set the candidate mask to 0
					// in order not to display the value in the view.
					_cellDigits[i].IsGiven = status == CellStatus.Given;
					_candidateDigits[i].CandidateMask = 0;

					// Checks the correctness of the digit.
					// If the digit is wrong, we should display it using a different color
					// if enabled the delta view.
					if (_preference.EnableDeltaValuesDisplaying)
					{
						if (_grid.ResetGrid.Solution is { IsUndefined: false } solution)
						{
							// For unique-solution puzzle, we should check both duplicate digits
							// and wrong digits different with the solution.
							if (solution[i] != digit)
							{
								_cellDigits[i].IsGiven = null;
								_candidateDigits[i].WrongDigitMask = 0;
							}
						}
						else
						{
							// For multiple- and no- solution puzzle, we should check only duplicate digits.
							foreach (int cell in PeerMaps[i] - _grid.EmptyCells)
							{
								if (_grid[cell] == _grid[i] && _grid.GetStatus(i) != CellStatus.Given)
								{
									// Duplicate.
									// Here we should report the duplicate digit.
									_cellDigits[i].IsGiven = null;
									_candidateDigits[i].WrongDigitMask = 0;

									break;
								}
							}
						}
					}

					break;
				}
			}
		}
	}

	/// <summary>
	/// Sets the view instance.
	/// </summary>
	/// <param name="view">The view instance.</param>
	private void SetViewNodes(View view)
	{
		// TODO: Sets other kinds of view nodes.

		// Clears the current view.
		ClearViewNodes();

		// Rebuild the nodes.
		foreach (var viewNode in view)
		{
			switch (viewNode)
			{
				case CellViewNode cellViewNode:
				{
					SetCellViewNode(cellViewNode);
					break;
				}
				case CandidateViewNode candidateViewNode:
				{
					SetCandidateViewNode(candidateViewNode);
					break;
				}
			}
		}
	}
}
