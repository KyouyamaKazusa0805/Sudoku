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
	/// Indicates the house labels.
	/// </summary>
	private static readonly int[] Houses = Enumerable.Range(0, 27).ToArray();


	/// <summary>
	/// Indicates the inner grid layout control.
	/// </summary>
	private readonly GridLayout _gridLayout = new GridLayout()
		.WithRowDefinitionsCount(9)
		.WithColumnDefinitionsCount(9)
		.WithHorizontalAlignment(HorizontalAlignment.Center)
		.WithVerticalAlignment(VerticalAlignment.Center)
		.WithChildrenTransitions(
			new TransitionCollection()
				.Append(new EntranceThemeTransition())
		);

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
	/// Indicates the link view node shapes.
	/// </summary>
	private readonly List<LinkViewNodeShape> _linkViewNodeShapes = new();

	/// <summary>
	/// Indicates the unknown value view node shapes.
	/// </summary>
	private readonly UnknownValueViewNodeShape[] _unknownValueViewNodeShapes = new UnknownValueViewNodeShape[81];

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
	/// Indicates the house view node shapes.
	/// </summary>
	private HouseViewNodeShape _houseViewNodeShape = null!;

	/// <summary>
	/// Indicates the current displayable unit to be displayed.
	/// </summary>
	private IDisplayable? _displayableUnit;


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

			// The operation must clear two stacks, and trigger the handler '_undoRedoStepsUpdatedCallback'.
			_undoSteps.Clear();
			_redoSteps.Clear();
			UndoRedoStepsUpdatedCallback?.Invoke();
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
		[MemberNotNull(nameof(_preference), nameof(_focusedRectangle), nameof(_houseViewNodeShape))]
		init
		{
			_preference = value;

			_focusedRectangle = new Rectangle()
				.WithFill(value.FocusedCellColor)
				.WithVisibility(Visibility.Collapsed)
				.WithGridLayout(row: 4, column: 4)
				.WithCanvasZIndex(-1);

			_showsCandidates = value.ShowCandidates;

			_houseViewNodeShape = new() { Preference = value };
			Array.ForEach(Houses, house => _houseViewNodeShape.SetIsVisible(house, false));
			_gridLayout.Children.Add(
				_houseViewNodeShape.GetControl()
					.WithGridLayout(rowSpan: 9, columnSpan: 9)
			);

			initializePeerRectangles(_peerFocusedRectangle, value);
			initializeValues(value);


			static void initializePeerRectangles(Rectangle[] rectangles, IDrawingPreference preference)
			{
				foreach (ref scoped var rectangle in rectangles.EnumerateRef())
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
				for (int i = 0; i < 81; i++)
				{
					scoped ref var p = ref _cellDigits[i];
					p = new()
					{
						UserPreference = preference,
						IsGiven = false,
						IsMaskMode = false,
						Digit = byte.MaxValue
					};
					_gridLayout.Children.Add(p.GetControl().WithGridLayout(row: i / 9, column: i % 9));
					_gridLayout.Children.Add(p.GetMaskEllipseControl().WithGridLayout(row: i / 9, column: i % 9));

					scoped ref var q = ref _candidateDigits[i];
					q = new()
					{
						UserPreference = preference,
						CandidateMask = 511,
						WrongDigitMask = 0,
						IsMaskMode = false
					};
					_gridLayout.Children.Add(q.GetControl().WithGridLayout(row: i / 9, column: i % 9));

					scoped ref var r = ref _cellViewNodeShapes[i];
					r = new() { Preference = preference, IsVisible = false };
					_gridLayout.Children.Add(r.GetControl().WithGridLayout(row: i / 9, column: i % 9));

					scoped ref var s = ref _candidateViewNodeShapes[i];
					s = new() { Preference = preference };
					Array.ForEach(Digits, digit => _candidateViewNodeShapes[i].SetIsVisible(digit, false));
					_gridLayout.Children.Add(s.GetControl().WithGridLayout(row: i / 9, column: i % 9));

					scoped ref var u = ref _unknownValueViewNodeShapes[i];
					u = new() { Preference = preference };
					_gridLayout.Children.Add(u.GetControl().WithGridLayout(row: i / 9, column: i % 9));

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
	/// Indicates the current displayable unit. If assigning <see langword="null"/>,
	/// the displaying unit will be clear.
	/// </summary>
	public IDisplayable? DisplayableUnit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _displayableUnit;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_displayableUnit = value;

			if (value is null)
			{
				ClearViewNodes();
			}
			else
			{
				var view = value.Views[ViewIndex = 0];

				SetViewNodes(view);
				SetConclusion(view, value.Conclusions);
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
	}

	/// <summary>
	/// To replace with the new grid.
	/// </summary>
	/// <param name="grid">The grid to be replaced with.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ReplaceGrid(scoped in Grid grid)
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
	}

	/// <summary>
	/// Sets the previous view.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetPreviousView()
	{
		if (ViewIndex - 1 < 0)
		{
			return;
		}

		if (DisplayableUnit is not { Views: var views and not [] })
		{
			return;
		}

		ViewIndex--;
		SetViewNodes(views[ViewIndex]);
		SetConclusion(views[ViewIndex], DisplayableUnit.Conclusions);
	}

	/// <summary>
	/// Sets the next view.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetNextView()
	{
		if (DisplayableUnit is not { Views: { Length: var viewLength and not 0 } views })
		{
			return;
		}

		if (ViewIndex + 1 >= viewLength)
		{
			return;
		}

		ViewIndex++;
		SetViewNodes(views[ViewIndex]);
		SetConclusion(views[ViewIndex], DisplayableUnit.Conclusions);
	}

	/// <summary>
	/// Directly skips to the view at the specified index.
	/// </summary>
	/// <param name="index">The index.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SkipToViewIndex(int index)
	{
		if (_isMaskMode)
		{
			return;
		}

		if (DisplayableUnit is null)
		{
			return;
		}

		ViewIndex = index;
		SetViewNodes(DisplayableUnit.Views[ViewIndex]);
		SetConclusion(DisplayableUnit.Views[ViewIndex], DisplayableUnit.Conclusions);
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

		scoped ref var current = ref _cellViewNodeShapes[cellViewNode.Cell];
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
		scoped ref var current = ref _candidateViewNodeShapes[cell];
		current.SetIsVisible(digit, true);
		current.SetIdentifier(digit, candidateViewNode.Identifier);
	}

	/// <summary>
	/// Sets the house view node.
	/// </summary>
	/// <param name="houseViewNode">The house view node.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetHouseViewNode(HouseViewNode houseViewNode)
	{
		if (_isMaskMode)
		{
			return;
		}

		int house = houseViewNode.House;
		_houseViewNodeShape.SetIsVisible(house, true);
		_houseViewNodeShape.SetIdentifier(house, houseViewNode.Identifier);
	}

	/// <summary>
	/// Sets the unknown view node.
	/// </summary>
	/// <param name="unknownViewNode">The unknown view node.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetUnknownViewNode(UnknownViewNode unknownViewNode)
	{
		if (_isMaskMode)
		{
			return;
		}

		int cell = unknownViewNode.Cell;
		var @char = unknownViewNode.UnknownValueChar;
		_unknownValueViewNodeShapes[cell].UnknownCharacter = @char;
	}

	/// <summary>
	/// Sets the link view node.
	/// </summary>
	/// <param name="linkViewNodes">The link view nodes.</param>
	public void SetLinkViewNode(IList<LinkViewNode> linkViewNodes)
	{
		if (_isMaskMode)
		{
			return;
		}

		var linkShape = new LinkViewNodeShape
		{
			PaneSize = PaneSize,
			OutsideOffset = OutsideOffset,
			Preference = Preference,
			Conclusions = DisplayableUnit!.Conclusions,
			Nodes = linkViewNodes.ToArray()
		};

		_linkViewNodeShapes.Add(linkShape);

		_gridLayout.Children.Add(linkShape.GetControl());
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

		Array.ForEach(_cellViewNodeShapes, static s => s.IsVisible = false);
		Array.ForEach(_candidateViewNodeShapes, static s => Array.ForEach(Digits, d => s.SetIsVisible(d, false)));
		Array.ForEach(Houses, house => _houseViewNodeShape.SetIsVisible(house, false));
		_linkViewNodeShapes.ForEach(link => _gridLayout.Children.Remove(link.GetControl()));
		Array.ForEach(_unknownValueViewNodeShapes, static s => s.UnknownCharacter = (Utf8Char)'\0');
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

	/// <summary>
	/// Clears the house view node.
	/// </summary>
	/// <param name="houseIndex">The house index.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ClearHouseViewNode(int houseIndex)
	{
		if (_isMaskMode)
		{
			return;
		}

		_houseViewNodeShape.SetIsVisible(houseIndex, false);
	}

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

	/// <summary>
	/// Adds the specified step into the collection.
	/// </summary>
	/// <param name="grid">The step to be added.</param>
	private void AddStep(scoped in Grid grid)
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

		DisplayableUnit = null;
	}

	/// <summary>
	/// Sets the view instance.
	/// </summary>
	/// <param name="view">The view instance.</param>
	private void SetViewNodes(View view)
	{
		// Clears the current view.
		ClearViewNodes();

		// Rebuild the nodes.
		var linkViewNodes = new List<LinkViewNode>();
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
				case HouseViewNode houseViewNode:
				{
					SetHouseViewNode(houseViewNode);
					break;
				}
				case LinkViewNode linkViewNode:
				{
					linkViewNodes.Add(linkViewNode);
					break;
				}
				case UnknownViewNode unknownViewNode:
				{
					SetUnknownViewNode(unknownViewNode);
					break;
				}
			}
		}

		if (linkViewNodes.Count != 0)
		{
			SetLinkViewNode(linkViewNodes);
		}
	}

	/// <summary>
	/// Sets the conclusions. The conclusions will be displayed
	/// using also <see cref="CandidateViewNodeShape"/>s.
	/// </summary>
	/// <param name="currentView">The view to check whether a conclusion is a cannibalism.</param>
	/// <param name="conclusions">The conclusions.</param>
	/// <seealso cref="CandidateViewNodeShape"/>
	private void SetConclusion(View currentView, ImmutableArray<Conclusion> conclusions)
	{
		if (_isMaskMode)
		{
			return;
		}

		foreach (var conclusion in conclusions)
		{
			_ = conclusion is var (type, cell, digit) and var (_, candidate);

			scoped ref var current = ref _candidateViewNodeShapes[cell];

			current.SetIsVisible(digit, true);
			current.SetIdentifier(
				digit,
				(
					type == ConclusionType.Assignment
						? _preference.AssignmentColor
						: currentView.ConflictWith(candidate)
							? _preference.CannibalismColor
							: _preference.EliminationColor
				).AsIdentifier()
			);

			current.SetAnimation(digit);
		}
	}
}
