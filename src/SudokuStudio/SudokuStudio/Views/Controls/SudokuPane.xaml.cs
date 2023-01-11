namespace SudokuStudio.Views.Controls;

/// <summary>
/// Defines a sudoku pane control.
/// </summary>
public sealed partial class SudokuPane : UserControl, INotifyPropertyChanged
{
	/// <inheritdoc cref="ValueFontScale"/>
	private double _valueFontScale = 1.0;

	/// <inheritdoc cref="PencilmarkFontScale"/>	
	private double _pencilmarkFontScale = 0.33;

	/// <inheritdoc cref="CoordinateLabelFontScale"/>
	private double _coordinateLabelFontScale = 0.4;

	/// <inheritdoc cref="SelectedCell"/>
	private int _selectedCell;

	/// <inheritdoc cref="CoordinateLabelDisplayKind"/>
	private CoordinateLabelDisplayKind _coordinateLabelDisplayKind;

	/// <inheritdoc cref="GivenColor"/>
	private Color _givenColor = Colors.Black;

	/// <inheritdoc cref="ModifiableColor"/>
	private Color _modifiableColor = Colors.Blue;

	/// <inheritdoc cref="PencilmarkColor"/>
	private Color _pencilmarkColor = new() { A = 255, R = 100, G = 100, B = 100 };

	/// <inheritdoc cref="CoordinateLabelColor"/>
	private Color _coordinateLabelColor = new() { A = 255, R = 100, G = 100, B = 100 };

	/// <inheritdoc cref="BorderColor"/>
	private Color _borderColor = Colors.Black;

	/// <inheritdoc cref="Puzzle"/>
	private Grid _puzzle = Grid.Empty;

	/// <inheritdoc cref="ValueFont"/>
	private FontFamily _valueFont = new("Tahoma");

	/// <inheritdoc cref="PencilmarkFont"/>
	private FontFamily _pencilmarkFont = new("Tahoma");

	/// <inheritdoc cref="CoordinateLabelFont"/>
	private FontFamily _coordinateLabelFont = new("Tahoma");

	/// <summary>
	/// The easy entry to visit children <see cref="SudokuPaneCell"/> instances. This field contains 81 elements,
	/// indicating controls being displayed as 81 cells in a sudoku grid respectively.
	/// </summary>
	private SudokuPaneCell[] _children;


	/// <summary>
	/// Initializes a <see cref="SudokuPane"/> instance.
	/// </summary>
	public SudokuPane()
	{
		InitializeComponent();
		InitializeChildrenControls();
		UpdateCellData(_puzzle);
	}


	/// <summary>
	/// Indicates the font scale of value digits (given or modifiable ones). The value should generally be below 1.0.
	/// </summary>
	public double ValueFontScale
	{
		get => _valueFontScale;

		set
		{
			if (_valueFontScale.NearlyEquals(value, 1E-2))
			{
				return;
			}

			_valueFontScale = value;

			PropertyChanged?.Invoke(this, new(nameof(ValueFontScale)));
		}
	}

	/// <summary>
	/// Indicates the font scale of pencilmark digits (candidates). The value should generally be below 1.0.
	/// </summary>
	public double PencilmarkFontScale
	{
		get => _pencilmarkFontScale;

		set
		{
			if (_pencilmarkFontScale.NearlyEquals(value, 1E-2))
			{
				return;
			}

			_pencilmarkFontScale = value;

			PropertyChanged?.Invoke(this, new(nameof(PencilmarkFontScale)));
		}
	}

	/// <summary>
	/// Indicates the coordinate label font scale.
	/// </summary>
	public double CoordinateLabelFontScale
	{
		get => _coordinateLabelFontScale;

		set
		{
			if (_coordinateLabelFontScale.NearlyEquals(value, 1E-2))
			{
				return;
			}

			_coordinateLabelFontScale = value;

			PropertyChanged?.Invoke(this, new(nameof(CoordinateLabelFontScale)));
		}
	}

	/// <summary>
	/// Indicates the currently selected cell.
	/// </summary>
	public int SelectedCell
	{
		get => _selectedCell;

		set
		{
			if (_selectedCell == value)
			{
				return;
			}

			_selectedCell = Clamp(value, -1, 80);

			PropertyChanged?.Invoke(this, new(nameof(SelectedCell)));
		}
	}

	/// <summary>
	/// Indicates the displaying kind of coordinate labels.
	/// </summary>
	/// <remarks>
	/// For more information please visit <see cref="Controls.CoordinateLabelDisplayKind"/>.
	/// </remarks>
	/// <seealso cref="Controls.CoordinateLabelDisplayKind"/>
	public CoordinateLabelDisplayKind CoordinateLabelDisplayKind
	{
		get => _coordinateLabelDisplayKind;

		set
		{
			if (_coordinateLabelDisplayKind == value)
			{
				return;
			}

			_coordinateLabelDisplayKind = value;

			PropertyChanged?.Invoke(this, new(nameof(CoordinateLabelDisplayKind)));
		}
	}

	/// <summary>
	/// Indicates the given color.
	/// </summary>
	public Color GivenColor
	{
		get => _givenColor;

		set
		{
			if (_givenColor == value)
			{
				return;
			}

			_givenColor = value;

			PropertyChanged?.Invoke(this, new(nameof(GivenColor)));
		}
	}

	/// <summary>
	/// Indicates the modifiable color.
	/// </summary>
	public Color ModifiableColor
	{
		get => _modifiableColor;

		set
		{
			if (_modifiableColor == value)
			{
				return;
			}

			_modifiableColor = value;

			PropertyChanged?.Invoke(this, new(nameof(ModifiableColor)));
		}
	}

	/// <summary>
	/// Indicates the pencilmark color.
	/// </summary>
	public Color PencilmarkColor
	{
		get => _pencilmarkColor;

		set
		{
			if (_pencilmarkColor == value)
			{
				return;
			}

			_pencilmarkColor = value;

			PropertyChanged?.Invoke(this, new(nameof(PencilmarkColor)));
		}
	}

	/// <summary>
	/// Indicates the coordinate label color.
	/// </summary>
	public Color CoordinateLabelColor
	{
		get => _coordinateLabelColor;

		set
		{
			if (_coordinateLabelColor == value)
			{
				return;
			}

			_coordinateLabelColor = value;

			PropertyChanged?.Invoke(this, new(nameof(CoordinateLabelColor)));
		}
	}

	/// <summary>
	/// Indicates the border color.
	/// </summary>
	public Color BorderColor
	{
		get => _borderColor;

		set
		{
			if (_borderColor == value)
			{
				return;
			}

			_borderColor = value;

			PropertyChanged?.Invoke(this, new(nameof(BorderColor)));
		}
	}

	/// <summary>
	/// Indicates the target grid puzzle.
	/// </summary>
	public Grid Puzzle
	{
		get => _puzzle;

		set
		{
			if (_puzzle == value)
			{
				return;
			}

			_puzzle = value;

			UpdateCellData(value);

			PropertyChanged?.Invoke(this, new(nameof(Puzzle)));
		}
	}

	/// <summary>
	/// Indicates the value font.
	/// </summary>
	public FontFamily ValueFont
	{
		get => _valueFont;

		set
		{
			if (_valueFont == value)
			{
				return;
			}

			_valueFont = value;

			PropertyChanged?.Invoke(this, new(nameof(ValueFont)));
		}
	}

	/// <summary>
	/// Indicates the candidate font.
	/// </summary>
	public FontFamily PencilmarkFont
	{
		get => _pencilmarkFont;

		set
		{
			if (_pencilmarkFont == value)
			{
				return;
			}

			_pencilmarkFont = value;

			PropertyChanged?.Invoke(this, new(nameof(PencilmarkFont)));
		}
	}

	/// <summary>
	/// Indicates the coordinate label font.
	/// </summary>
	public FontFamily CoordinateLabelFont
	{
		get => _coordinateLabelFont;

		set
		{
			if (_coordinateLabelFont == value)
			{
				return;
			}

			_coordinateLabelFont = value;

			PropertyChanged?.Invoke(this, new(nameof(CoordinateLabelFont)));
		}
	}

	/// <summary>
	/// Indicates the approximately-measured width and height value of a cell.
	/// </summary>
	internal double ApproximateCellWidth => ((Width + Height) / 2 - 100 - (4 << 1)) / 10;


	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;


	/// <summary>
	/// To initialize children controls for <see cref="_children"/>.
	/// </summary>
	[MemberNotNull(nameof(_children))]
	private void InitializeChildrenControls()
	{
		_children = new SudokuPaneCell[81];
		for (var i = 0; i < 81; i++)
		{
			var cellControl = new SudokuPaneCell { CellIndex = i, BasePane = this };

			GridLayout.SetRow(cellControl, i / 9 + 2);
			GridLayout.SetColumn(cellControl, i % 9 + 2);

			MainGrid.Children.Add(cellControl);
			_children[i] = cellControl;
		}
	}

	/// <summary>
	/// To initialize <see cref="GridCellData"/> values via the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	private void UpdateCellData(scoped in Grid grid)
	{
		for (var i = 0; i < 81; i++)
		{
			var cellControl = _children[i];
			cellControl.CellStatus = grid.GetStatus(i);
			cellControl.CandidatesMask = grid.GetCandidates(i);
		}
	}

	private void UserControl_Loaded(object sender, RoutedEventArgs e) => Focus(FocusState.Programmatic);

	private void UserControl_KeyDown(object sender, KeyRoutedEventArgs e)
	{
		/// Please note that the parent control may use globalized hotkeys to control some behaviors.
		/// If <c>e.Handled</c> is not set <see langword="false"/> value before exited parent <c>KeyDown</c> method,
		/// this method will not be triggered and executed.
		switch (Keyboard.GetModifierStatusForCurrentThread(), SelectedCell, Keyboard.GetInputDigit(e.Key))
		{
			case (_, not (>= 0 and < 81), _):
			case (_, var cell, _) when Puzzle.GetStatus(cell) == CellStatus.Given:
			case (_, _, -2):
			{
				return;
			}
			case ((false, false, false, _), var cell, -1):
			{
				var modified = Puzzle;
				modified[cell] = -1;

				Puzzle = modified;

				break;
			}
			case ((false, true, false, _), var cell, var digit) when Puzzle.Exists(cell, digit) is true:
			{
				var modified = Puzzle;
				modified[cell, digit] = false;

				Puzzle = modified;

				break;
			}
			case ((false, false, false, _), var cell, var digit) when !Puzzle.DupliateWith(cell, digit):
			{
				var modified = Puzzle;
				if (Puzzle.GetStatus(cell) == CellStatus.Modifiable)
				{
					// Temporarily re-compute candidates.
					modified[cell] = -1;
				}

				modified[cell] = digit;
				Puzzle = modified;

				break;
			}
		}
	}
}
