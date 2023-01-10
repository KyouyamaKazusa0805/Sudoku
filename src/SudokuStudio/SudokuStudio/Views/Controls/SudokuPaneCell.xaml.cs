namespace SudokuStudio.Views.Controls;

/// <summary>
/// Defines a cell displayed in a <see cref="SudokuPane"/>.
/// </summary>
/// <seealso cref="SudokuPane"/>
public sealed partial class SudokuPaneCell : UserControl, INotifyPropertyChanged
{
	public static readonly DependencyProperty ValueFontSizeProperty =
		DependencyProperty.Register(nameof(ValueFontSize), typeof(double), typeof(SudokuPaneCell), new(42.0));

	public static readonly DependencyProperty CandidateFontSizeProperty =
		DependencyProperty.Register(nameof(CandidateFontSize), typeof(double), typeof(SudokuPaneCell), new(14.0));


	private short _candidatesMask = 511;

	private CellStatus _status = CellStatus.Empty;


	/// <summary>
	/// Initializes a <see cref="SudokuPaneCell"/> instance.
	/// </summary>
	public SudokuPaneCell() => InitializeComponent();


	/// <summary>
	/// Indicates the size of value text.
	/// </summary>
	public double ValueFontSize
	{
		get => (double)GetValue(ValueFontSizeProperty);

		set => SetValue(ValueFontSizeProperty, value);
	}

	/// <summary>
	/// Indicates the size of candidate text.
	/// </summary>
	public double CandidateFontSize
	{
		get => (double)GetValue(CandidateFontSizeProperty);

		set => SetValue(CandidateFontSizeProperty, value);
	}

	/// <summary>
	/// Indicates the candidates mask.
	/// </summary>
	public short CandidatesMask
	{
		get => _candidatesMask;

		set
		{
			if (_candidatesMask == value)
			{
				return;
			}

			_candidatesMask = value;

			PropertyChanged?.Invoke(this, new(nameof(CandidatesMask)));
		}
	}

	/// <summary>
	/// Indicates the cell status.
	/// </summary>
	public CellStatus CellStatus
	{
		get => _status;

		set
		{
			if (_status == value)
			{
				return;
			}

			_status = value;

			PropertyChanged?.Invoke(this, new(nameof(CellStatus)));
		}
	}

	/// <summary>
	/// Indicates the cell index.
	/// </summary>
	internal int CellIndex { get; init; }


	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;
}
