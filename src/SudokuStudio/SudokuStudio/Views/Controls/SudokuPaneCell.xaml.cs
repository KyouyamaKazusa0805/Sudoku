namespace SudokuStudio.Views.Controls;

/// <summary>
/// Defines a cell displayed in a <see cref="SudokuPane"/>.
/// </summary>
/// <seealso cref="SudokuPane"/>
public sealed partial class SudokuPaneCell : UserControl, INotifyPropertyChanged
{
	private short _candidatesMask = 511;

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

	private void UserControl_Loaded(object sender, RoutedEventArgs e) => BasePane ??= (SudokuPane)((GridLayout)Parent).Parent;

	private void TextBlock_PointerEntered(object sender, PointerRoutedEventArgs e) => BasePane.SelectedCell = CellIndex;

	private void TextBlock_PointerExited(object sender, PointerRoutedEventArgs e) => BasePane.SelectedCell = CellIndex;
}
