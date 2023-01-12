namespace SudokuStudio.Views.Controls;

/// <summary>
/// Defines a cell displayed in a <see cref="SudokuPane"/>.
/// </summary>
/// <seealso cref="SudokuPane"/>
public sealed partial class SudokuPaneCell : UserControl, INotifyPropertyChanged
{
	/// <inheritdoc cref="CandidatesMask"/>
	private short _candidatesMask = 511;

	/// <inheritdoc cref="CellStatus"/>
	private CellStatus _status = CellStatus.Empty;


	/// <summary>
	/// Initializes a <see cref="SudokuPaneCell"/> instance.
	/// </summary>
	public SudokuPaneCell() => InitializeComponent();


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
	/// Indicates the base pane.
	/// </summary>
	public SudokuPane BasePane { get; set; } = null!;

	/// <summary>
	/// Indicates the cell index.
	/// </summary>
	internal int CellIndex { get; init; }


	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;


	private void UserControl_PointerEntered(object sender, PointerRoutedEventArgs e) => BasePane.SelectedCell = CellIndex;

	private void UserControl_PointerExited(object sender, PointerRoutedEventArgs e) => BasePane.SelectedCell = -1;

	private void TextBlock_PointerEntered(object sender, PointerRoutedEventArgs e) => BasePane.SelectedCell = CellIndex;

	private void TextBlock_PointerExited(object sender, PointerRoutedEventArgs e) => BasePane.SelectedCell = CellIndex;

	private void TextBlock_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
	{
		if (sender is not TextBlock { Text: var text } || !int.TryParse(text, out var originalDigit) || originalDigit is not (>= 1 and <= 9))
		{
			return;
		}

		if (BasePane is not { Puzzle: var modified, SelectedCell: var cell and not -1 })
		{
			return;
		}

		var digit = originalDigit - 1;
		if ((modified.GetCandidates(cell) >> digit & 1) == 0)
		{
			return;
		}

		if (modified.GetStatus(cell) != CellStatus.Empty)
		{
			return;
		}

		modified[cell] = digit;

		BasePane.Puzzle = modified;
	}
}
