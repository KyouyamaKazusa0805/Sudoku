using Windows.UI;

namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a sudoku grid.
/// </summary>
public sealed class SudokuGrid : DrawingElement
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
	/// Indicates the stacks that allows undoing or redoing operations.
	/// </summary>
	private readonly Stack<Grid> _undoStack = new(), _redoStack = new();

	/// <summary>
	/// Indicates the status for displaying candidates.
	/// </summary>
	private bool _showCandidates;

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
	/// <param name="showCandidates">Whether the grid displays candidates.</param>
	/// <param name="paneSize">The pane size.</param>
	/// <param name="outsideOffset">The outside offset.</param>
	/// <param name="givenColor">The given text color.</param>
	/// <param name="modifiableColor">The modifiable text color.</param>
	/// <param name="candidateColor">The candidate text color.</param>
	/// <param name="valueFontName">The given or modifiable font name.</param>
	/// <param name="candidateFontName">The candidate font name.</param>
	/// <param name="valueFontSize">The value font size.</param>
	/// <param name="candidateFontSize">The candidate font size.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SudokuGrid(
		bool showCandidates, double paneSize, double outsideOffset,
		Color givenColor, Color modifiableColor, Color candidateColor,
		string valueFontName, string candidateFontName, double valueFontSize, double candidateFontSize) :
		this(
			Grid.Empty, showCandidates, paneSize, outsideOffset, givenColor, modifiableColor, candidateColor,
			valueFontName, candidateFontName, valueFontSize, candidateFontSize)
	{
	}

	/// <summary>
	/// Initializes a <see cref="SudokuGrid"/> instance via the details.
	/// </summary>
	/// <param name="grid">The <see cref="Grid"/> instance.</param>
	/// <param name="showCandidates">Whether the grid displays candidates.</param>
	/// <param name="paneSize">The pane size.</param>
	/// <param name="outsideOffset">The outside offset.</param>
	/// <param name="givenColor">The given text color.</param>
	/// <param name="modifiableColor">The modifiable text color.</param>
	/// <param name="candidateColor">The candidate text color.</param>
	/// <param name="valueFontName">The given or modifiable font name.</param>
	/// <param name="candidateFontName">The candidate font name.</param>
	/// <param name="valueFontSize">The value font size.</param>
	/// <param name="candidateFontSize">The candidate font size.</param>
	public SudokuGrid(
		in Grid grid, bool showCandidates, double paneSize, double outsideOffset,
		Color givenColor, Color modifiableColor, Color candidateColor,
		string valueFontName, string candidateFontName, double valueFontSize, double candidateFontSize)
	{
		_grid = grid;
		_showCandidates = showCandidates;
		_paneSize = paneSize;
		_outsideOffset = outsideOffset;
		_gridLayout = initializeGridLayout(paneSize, outsideOffset);

		// Initializes values.
		initializeValues(
			showCandidates, givenColor, modifiableColor, candidateColor,
			valueFontName, valueFontSize, candidateFontName, candidateFontSize);

		// Then initialize other items.
		UpdateView();


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static GridLayout initializeGridLayout(double paneSize, double outsideOffset) =>
			new GridLayout
			{
				Width = paneSize,
				Height = paneSize,
				Padding = new(outsideOffset),
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			}
			.WithRowDefinitionsCount(9)
			.WithColumnDefinitionsCount(9);

		void initializeValues(
			bool showCandidates, Color givenColor, Color modifiableColor, Color candidateColor,
			string valueFontName, double valueFontSize, string candidateFontName, double candidateFontSize)
		{
			for (int i = 0; i < 81; i++)
			{
				ref var p = ref _cellDigits[i];
				p = new(givenColor, modifiableColor, valueFontName, valueFontSize);
				_gridLayout.Children.Add(p.GetControl().WithGridLayout(row: i / 9, column: i % 9));

				ref var q = ref _candidateDigits[i];
				q = new(showCandidates, candidateFontName, candidateFontSize, candidateColor);
				_gridLayout.Children.Add(q.GetControl().WithGridLayout(row: i / 9, column: i % 9));
			}
		}
	}


	/// <summary>
	/// Indicates whether the grid displays for candidates.
	/// </summary>
	public bool ShowCandidates
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _showCandidates;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_showCandidates == value)
			{
				return;
			}

			_showCandidates = value;
			Array.ForEach(_candidateDigits, candidateDigit => candidateDigit.ShowDigits = value);
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
	/// Gets or sets the given or modifiable font size.
	/// </summary>
	public double ValueFontSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _cellDigits[0].FontSize;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => Array.ForEach(_cellDigits, cellDigit => cellDigit.FontSize = value);
	}

	/// <summary>
	/// Gets or sets the candidate font size.
	/// </summary>
	public double CandidateFontSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _candidateDigits[0].FontSize;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => Array.ForEach(_candidateDigits, candidateDIgit => candidateDIgit.FontSize = value);
	}

	/// <summary>
	/// Gets or sets the given or modifiable font name.
	/// </summary>
	public string ValueFontName
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _cellDigits[0].FontName;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => Array.ForEach(_cellDigits, cellDigit => cellDigit.FontName = value);
	}

	/// <summary>
	/// Gets or sets the candidate font name.
	/// </summary>
	public string CandidateFontName
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _candidateDigits[0].FontName;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => Array.ForEach(_candidateDigits, candidateDigit => candidateDigit.FontName = value);
	}

	/// <summary>
	/// Gets or sets the given color.
	/// </summary>
	public Color GivenColor
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _cellDigits[0].GivenColor;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => Array.ForEach(_cellDigits, cellDigit => cellDigit.GivenColor = value);
	}

	/// <summary>
	/// Gets or sets the modifiable color.
	/// </summary>
	public Color ModifiableColor
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _cellDigits[0].ModifiableColor;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => Array.ForEach(_cellDigits, cellDigit => cellDigit.ModifiableColor = value);
	}

	/// <summary>
	/// Gets or sets the candidate color.
	/// </summary>
	public Color CandidateColor
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _candidateDigits[0].Color;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => Array.ForEach(_candidateDigits, candidateDigit => candidateDigit.Color = value);
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
			_grid = value;
			UpdateView();
		}
	}

	/// <summary>
	/// Indicates the number of available undoable steps.
	/// </summary>
	internal int UndoStepsCount => _undoStack.Count;

	/// <summary>
	/// Indicates the number of available redoable steps.
	/// </summary>
	internal int RedoStepsCount => _redoStack.Count;


	/// <summary>
	/// Indicates the event that triggers when the undo stack is changed.
	/// </summary>
	public event RoutedEventHandler? UndoStackChanged;

	/// <summary>
	/// Indicates the event that triggers when the redo stack is changed.
	/// </summary>
	public event RoutedEventHandler? RedoStackChanged;


	/// <summary>
	/// To undo a step.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Undo()
	{
		if (_undoStack.TryPop(out var lastStep))
		{
			RaiseRedoStackChanged(_grid);

			Grid = lastStep;

			UndoStackChanged?.Invoke(this, new());
		}
	}

	/// <summary>
	/// To redo a step.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Redo()
	{
		if (_redoStack.TryPop(out var nextStep))
		{
			RaiseUndoStackChanged(_grid);

			Grid = nextStep;

			RedoStackChanged?.Invoke(this, new());
		}
	}

	/// <summary>
	/// To make the specified cell fill the specified digit.
	/// </summary>
	/// <param name="cell">The cell that the conclusion is from.</param>
	/// <param name="digit">The digit made.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void MakeDigit(int cell, int digit)
	{
		RaiseUndoStackChanged(_grid);

		_grid[cell] = digit;

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
		RaiseUndoStackChanged(_grid);

		_grid[cell, digit] = false;

		UpdateView();
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] DrawingElement? other) =>
		other is SudokuGrid comparer && _grid == comparer._grid;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(nameof(SudokuGrid), _grid.GetHashCode());

	/// <summary>
	/// Gets the inner grid instance by reference.
	/// </summary>
	/// <returns>The reference of the grid.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref Grid GetGridByReference() => ref _grid;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override GridLayout GetControl() => _gridLayout;

	/// <summary>
	/// Raises the event <see cref="UndoStackChanged"/> via the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <seealso cref="UndoStackChanged"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void RaiseUndoStackChanged(in Grid grid)
	{
		_undoStack.Push(grid);
		UndoStackChanged?.Invoke(this, new());
	}

	/// <summary>
	/// Raises the event <see cref="RedoStackChanged"/> via the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <seealso cref="RedoStackChanged"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void RaiseRedoStackChanged(in Grid grid)
	{
		_redoStack.Push(grid);
		RedoStackChanged?.Invoke(this, new());
	}

	/// <summary>
	/// To update the view via the current grid.
	/// </summary>
	private void UpdateView()
	{
		for (int i = 0; i < 81; i++)
		{
			switch (_grid.GetStatus(i))
			{
				case CellStatus.Empty:
				{
					_cellDigits[i].Digit = byte.MaxValue;
					_cellDigits[i].IsGiven = false;
					_candidateDigits[i].CandidateMask = _grid.GetCandidates(i);

					break;
				}
				case var status and (CellStatus.Given or CellStatus.Modifiable):
				{
					_cellDigits[i].Digit = (byte)_grid[i];
					_cellDigits[i].IsGiven = status == CellStatus.Given;
					_candidateDigits[i].CandidateMask = 0;

					break;
				}
			}
		}
	}
}
