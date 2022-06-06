namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a sudoku grid.
/// </summary>
[AutoOverridesGetHashCode(nameof(TypeIdentifier), nameof(_grid))]
public sealed partial class SudokuGrid : DrawingElement
{
	/// <summary>
	/// Indicates the inner grid layout control.
	/// </summary>
	private readonly GridLayout _gridLayout;

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
	/// Indicates the callback method that invokes when the undoing and redoing steps are updated.
	/// </summary>
	private readonly Action? _undoRedoStepsUpdatedCallback;

	/// <summary>
	/// Indicates the user preference used.
	/// </summary>
	private readonly IDrawingPreference _preference;

	/// <summary>
	/// Indicates the pane size.
	/// </summary>
	private double _paneSize;

	/// <summary>
	/// Indicates the outside offset.
	/// </summary>
	private double _outsideOffset;

	/// <summary>
	/// Indicates the inner grid.
	/// </summary>
	private Grid _grid;


	/// <summary>
	/// Initializes a <see cref="SudokuGrid"/> instance via the details.
	/// </summary>
	/// <param name="preference">The user preference instance.</param>
	/// <param name="paneSize">The pane size.</param>
	/// <param name="outsideOffset">The outside offset.</param>
	/// <param name="elementUpdatedCallback">
	/// The callback method that triggers when the inner undo-redo steps are updated.
	/// </param>
	public SudokuGrid(
		IDrawingPreference preference, double paneSize, double outsideOffset, Action? elementUpdatedCallback) :
		this(Grid.Empty, preference, paneSize, outsideOffset, elementUpdatedCallback)
	{
	}

	/// <summary>
	/// Initializes a <see cref="SudokuGrid"/> instance via the details.
	/// </summary>
	/// <param name="grid">The <see cref="Grid"/> instance.</param>
	/// <param name="preference">The user preference.</param>
	/// <param name="paneSize">The pane size.</param>
	/// <param name="outsideOffset">The outside offset.</param>
	/// <param name="elementUpdatedCallback">
	/// The callback method that triggers when the inner undo-redo steps are updated.
	/// </param>
	public SudokuGrid(
		in Grid grid, IDrawingPreference preference, double paneSize, double outsideOffset,
		Action? elementUpdatedCallback)
	{
		(_preference, _grid, _paneSize, _outsideOffset, _gridLayout, _undoRedoStepsUpdatedCallback) = (
			preference,
			grid,
			paneSize,
			outsideOffset,
			initializeGridLayout(paneSize, outsideOffset),
			elementUpdatedCallback
		);

		// Initializes values.
		initializeValues();

		// Then initialize other items.
		UpdateView();


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static GridLayout initializeGridLayout(double paneSize, double outsideOffset)
		{
			var result = new GridLayout
			{
				Width = paneSize,
				Height = paneSize,
				Padding = new(outsideOffset),
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};
			for (int i = 0; i < 9; i++)
			{
				result.RowDefinitions.Add(new());
				result.ColumnDefinitions.Add(new());
			}

			return result;
		}

		void initializeValues()
		{
			for (int i = 0; i < 81; i++)
			{
				ref var p = ref _cellDigits[i];
				p = new(preference);
				var control1 = p.GetControl();
				GridLayout.SetRow(control1, i / 9);
				GridLayout.SetColumn(control1, i % 9);
				_gridLayout.Children.Add(control1);

				ref var q = ref _candidateDigits[i];
				q = new(preference);
				var control2 = q.GetControl();
				GridLayout.SetRow(control2, i / 9);
				GridLayout.SetColumn(control2, i % 9);
				_gridLayout.Children.Add(control2);
			}
		}
	}


	/// <summary>
	/// Indicates whether the grid displays for candidates.
	/// </summary>
	public bool ShowCandidates
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _preference.ShowCandidates;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_preference.ShowCandidates == value)
			{
				return;
			}

			_preference.ShowCandidates = value;
			Array.ForEach(_candidateDigits, candidateDigit => candidateDigit.ShowCandidates = value);
		}
	}

	/// <summary>
	/// Gets or sets the outside offset.
	/// </summary>
	public double OutsideOffset
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
	public double PaneSize
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
			_undoRedoStepsUpdatedCallback?.Invoke();
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

	private string TypeIdentifier => nameof(SudokuGrid);


	/// <summary>
	/// To undo a step.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Undo()
	{
		_redoSteps.Push(_grid);

		var previousStep = _undoSteps.Pop();
		_grid = previousStep;

		_undoRedoStepsUpdatedCallback?.Invoke();

		UpdateView();
	}

	/// <summary>
	/// To redo a step.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Redo()
	{
		_undoSteps.Push(_grid);

		var nextStep = _redoSteps.Pop();
		_grid = nextStep;

		_undoRedoStepsUpdatedCallback?.Invoke();

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
		// Stores the previous grid status to the undo stack.
		AddStep(_grid);

		// To re-compute candidates if the current cell is modifiable.
		if (digit != -1 && _grid.GetStatus(cell) == CellStatus.Modifiable)
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
		if (digit == -1)
		{
			// Skips the invalid data.
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
	public void ReplaceGrid(in Grid grid)
	{
		// Stores the previous grid status to the undo stack.
		AddStep(_grid);

		// Update the grid and view.
		_grid = grid;
		UpdateView();
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] DrawingElement? other)
		=> other is SudokuGrid comparer && _grid == comparer._grid;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override GridLayout GetControl() => _gridLayout;

	/// <summary>
	/// Adds the specified step into the collection.
	/// </summary>
	/// <param name="grid">The step to be added.</param>
	private void AddStep(in Grid grid)
	{
		_undoSteps.Push(_grid);
		_grid = grid;

		_redoSteps.Clear();

		_undoRedoStepsUpdatedCallback?.Invoke();
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
}
