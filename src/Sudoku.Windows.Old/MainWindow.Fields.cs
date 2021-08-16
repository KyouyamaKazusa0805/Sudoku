using Sudoku.Data.Stepping;
using PointConverter = Sudoku.Drawing.PointConverter;

namespace Sudoku.Windows;

partial class MainWindow
{
#if AUTHOR_RESERVED && DEBUG
	/// <summary>
	/// Indicates the cache path.
	/// </summary>
	/// <remarks>
	/// <i>Please don't re-order the two fields in this <c>#if</c> block.</i>
	/// </remarks>
	private static readonly string CachePath = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
		"Sudoku_Temp"
	);

	/// <summary>
	/// Indicates the temporary file path.
	/// </summary>
	/// <remarks>
	/// <i>Please don't re-order the two fields in this <c>#if</c> block.</i>
	/// </remarks>
	private static readonly string TempFilePath = Path.Combine(CachePath, "TempFilling.cache");
#endif


#if AUTHOR_RESERVED && DEBUG
	/// <summary>
	/// Indicates whether the program will automatically check the local path, and imports the info
	/// all the time.
	/// </summary>
	private readonly bool _enableDynamicChecking;
#endif

	/// <summary>
	/// The custom view.
	/// </summary>
	private readonly MutableView _view = new();

	/// <summary>
	/// The JSON serializer options. It'll be initialized in <see cref="MainWindow()"/>.
	/// </summary>
	/// <seealso cref="MainWindow()"/>
	private readonly JsonSerializerOptions _serializerOptions = CreateDefaultJsonSerializerOptionsInstance();


	/// <summary>
	/// The current custom drawing mode. The values are:
	/// <list type="table">
	/// <item>
	/// <term>-1</term>
	/// <description>None selected.</description>
	/// </item>
	/// <item>
	/// <term>0</term>
	/// <description>Drawing cells.</description>
	/// </item>
	/// <item>
	/// <term>1</term>
	/// <description>Drawing candidates.</description>
	/// </item>
	/// <item>
	/// <term>2</term>
	/// <description>Drawing regions.</description>
	/// </item>
	/// <item>
	/// <term>3</term>
	/// <description>Drawing links.</description>
	/// </item>
	/// </list>
	/// </summary>
	private int _customDrawingMode = -1;

	/// <summary>
	/// Indicates the start candidate that is used for drawing a chain.
	/// </summary>
	private int _startCand = -1;

	/// <summary>
	/// The current view index.
	/// </summary>
	private int _currentViewIndex = -1;

	/// <summary>
	/// Indicates the current color chosen (used in coloring mode).
	/// See <see cref="Settings.PaletteColors"/> for more. If the value is
	/// <see cref="int.MinValue"/>, the current color is unavailable.
	/// </summary>
	/// <seealso cref="Settings.PaletteColors"/>
	/// <seealso cref="int.MinValue"/>
	private int _currentColor = int.MinValue;

	/// <summary>
	/// Indicates the database of puzzles used current.
	/// </summary>
	private string? _database;

	/// <summary>
	/// Indicates the puzzles text loaded.
	/// </summary>
	private string[]? _puzzlesText;

	/// <summary>
	/// Indicates the current right click position, which is used for
	/// check the cell (set and delete a candidate from a grid using context menu).
	/// </summary>
	private WPoint _currentRightClickPos;

	/// <summary>
	/// The map of selected cells while drawing regions.
	/// </summary>
	private Cells _selectedCellsWhileDrawingRegions = Cells.Empty;

	/// <summary>
	/// Indicates all focused cells.
	/// </summary>
	private Cells _focusedCells = Cells.Empty;

	/// <summary>
	/// The preview map. This field is only used for
	/// <see cref="OnKeyDown(KeyEventArgs)"/> and <see cref="OnKeyUp(KeyEventArgs)"/>.
	/// </summary>
	/// <seealso cref="OnKeyDown(KeyEventArgs)"/>
	/// <seealso cref="OnKeyUp(KeyEventArgs)"/>
	private Cells? _previewMap;

	/// <summary>
	/// The initial grid.
	/// </summary>
	private SudokuGrid _initialPuzzle = SudokuGrid.Undefined;

	/// <summary>
	/// Indicates an recognition instance.
	/// </summary>
	private RecognitionServiceProvider? _recognition;

	/// <summary>
	/// Indicates the analysis result after solved of the current grid.
	/// </summary>
	/// <remarks>
	/// <para>This field is used for give each step after solved the puzzle.</para>
	/// <para>
	/// If you changed the width of the <see cref="_tabControlInfo"/> and its width is <c>0</c>
	/// or below <c>0</c>, the result won't be shown on the tab control.
	/// </para>
	/// </remarks>
	private AnalysisResult? _analyisResult;

	/// <summary>
	/// The current technique information.
	/// </summary>
	/// <remarks>This field is used for rendering a <see cref="View"/>.</remarks>
	/// <seealso cref="View"/>
	private StepInfo? _currentStepInfo;

	/// <summary>
	/// Indicates the current target painter.
	/// </summary>
	private GridPainter _currentPainter = null!;

	/// <summary>
	/// The grid.
	/// </summary>
	private UndoableGrid _puzzle = SudokuGrid.Empty;

	/// <summary>
	/// Indicates the internal manual solver.
	/// This field is mutable.
	/// </summary>
	private ManualSolver _manualSolver = null!;

	/// <summary>
	/// The point converter.
	/// </summary>
	private PointConverter _pointConverter = null!;
}
