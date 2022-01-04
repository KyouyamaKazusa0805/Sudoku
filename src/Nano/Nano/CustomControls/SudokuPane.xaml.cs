namespace Nano.CustomControls;

/// <summary>
/// Defines a custom control that holds a sudoku grid.
/// </summary>
public partial class SudokuPane : UserControl
{
	/// <summary>
	/// Indicates the inner grid used.
	/// </summary>
	private SudokuGrid _grid;


	/// <summary>
	/// Initializes a <see cref="SudokuPane"/> instance.
	/// </summary>
	public SudokuPane()
	{
		InitializeComponent();
		initializeControls();
		initializeEvents();


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void initializeControls() => ShapePool = new(Preference, _GridBase);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void initializeEvents()
		{
			GridChanging += OnGridChanging;
			GridChanged += OnGridChanged;
		}
	}


	/// <summary>
	/// Indicates the sudoku grid.
	/// </summary>
	/// <returns>The sudoku grid got.</returns>
	/// <value>The value to replace with.</value>
	public SudokuGrid Grid
	{
		get => _grid;

		set
		{
			GridChanging?.Invoke(this, new(_grid));

			_grid = value;

			GridChanged?.Invoke(this, new(value));
		}
	}

	/// <summary>
	/// Indicates the preferences.
	/// </summary>
	public Preference Preference { get; } = new();

	/// <summary>
	/// Indicates the shape pool.
	/// </summary>
	public GridShapePool ShapePool { get; private set; } = null!;


	/// <summary>
	/// Indicates the event that will be triggered when the current sudoku grid information
	/// will be changed in a second.
	/// </summary>
	public event GridChangingEventHandler? GridChanging;

	/// <summary>
	/// Indicates the event that will be triggered when the current sudoku grid information
	/// has been changed its status.
	/// </summary>
	public event GridChangedEventHandler? GridChanged;


	/// <summary>
	/// The default method that is triggered and invoked if the grid is changing.
	/// </summary>
	/// <param name="sender">The object to trigger the event.</param>
	/// <param name="e">The data provided on this trigger.</param>
	protected virtual void OnGridChanging(object? sender, GridChangingEventArgs e)
	{
	}

	/// <summary>
	/// The default method that is triggered and invoked if the grid is changed.
	/// </summary>
	/// <param name="sender">The object to trigger the event.</param>
	/// <param name="e">The data provided on this trigger.</param>
	protected virtual void OnGridChanged(object? sender, GridChangedEventArgs e) =>
		ShapePool.RefreshCandidates(e.NewGrid);
}
