namespace Nano.CustomControls;

/// <summary>
/// Defines a custom control that holds a sudoku grid.
/// </summary>
public sealed partial class SudokuPane : UserControl
{
	/// <summary>
	/// Initializes a <see cref="SudokuPane"/> instance.
	/// </summary>
	public SudokuPane()
	{
		InitializeComponent();
		InitializeControls();
	}


	/// <summary>
	/// Indicates the event that will be triggered when the current sudoku grid information
	/// has been changed its status.
	/// </summary>
	public event GridChangedEventHandler? GridChanged;


	/// <summary>
	/// To initialize the controls. The method is only used by the constructor <see cref="SudokuPane()"/>.
	/// </summary>
	/// <seealso cref="SudokuPane()"/>
	private void InitializeControls()
	{
		// Initializes '_Grid'.
		for (int i = 0; i < 27; i++)
		{
			_GridBase.RowDefinitions.Add(new());
			_GridBase.ColumnDefinitions.Add(new());
		}
	}
}
