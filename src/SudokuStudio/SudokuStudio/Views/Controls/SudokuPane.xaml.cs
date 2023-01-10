namespace SudokuStudio.Views.Controls;

/// <summary>
/// Defines a sudoku pane control.
/// </summary>
public sealed partial class SudokuPane : UserControl, INotifyPropertyChanged
{
	private readonly SudokuPaneCell[] _children = new SudokuPaneCell[81];

	private double _valueFontScale = 1.0;

	private double _pencilmarkFontScale = 0.33;

	private int _selectedCell;

	private CoordinateLabelDisplayKind _coordinateLabelDisplayKind;

	private Color _givenColor = Colors.Black;

	private Color _modifiableColor = new() { A = 255, R = 0, G = 0, B = 255 };

	private Color _pencilmarkColor = new() { A = 255, R = 100, G = 100, B = 100 };

	private Grid _puzzle;

	private FontFamily _valueFont = new("Tahoma");

	private FontFamily _pencilmarkFont = new("Tahoma");


	/// <summary>
	/// Initializes a <see cref="SudokuPane"/> instance.
	/// </summary>
	public SudokuPane() => InitializeComponent();


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
	/// Indicates the selected cell.
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
	/// Indicates the coordinate label displaying kind.
	/// </summary>
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
	/// Indicates the approximately-measured width and height value of a cell.
	/// </summary>
	internal double ApproximateCellWidth => ((Width + Height) / 2 - 100 - (4 << 1)) / 10;


	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;


	/// <summary>
	/// To initialize <see cref="GridCellData"/> values via the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	private void UpdateCellData(scoped in Grid grid)
	{
		for (var i = 0; i < 81; i++)
		{
			var cellControl = _children[i];
			cellControl.CandidatesMask = grid.GetCandidates(i);
			cellControl.CellStatus = grid.GetStatus(i);
		}
	}


	private void UserControl_Loaded(object sender, RoutedEventArgs e)
	{
		for (var i = 0; i < 81; i++)
		{
			var cellControl = new SudokuPaneCell { CellIndex = i, BasePane = this };

			GridLayout.SetRow(cellControl, i / 9 + 2);
			GridLayout.SetColumn(cellControl, i % 9 + 2);

			MainGrid.Children.Add(cellControl);
			_children[i] = cellControl;
		}
	}
}
