using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Stepping;
using Sudoku.Drawing;
using Sudoku.Recognitions;
using Sudoku.Solving;
using Sudoku.Solving.Manual;
using Grid = Sudoku.Data.Grid;
using WPoint = System.Windows.Point;

namespace Sudoku.Windows
{
	partial class MainWindow
	{
		/// <summary>
		/// The custom view.
		/// </summary>
		/// <remarks>
		/// This field is used for the methods <see cref="ImageGrid_MouseLeftButtonDown"/>,
		/// <see cref="ImageGrid_MouseRightButtonUp"/> and <see cref="ButtonCellReset_Click"/>.
		/// </remarks>
		/// <seealso cref="ImageGrid_MouseLeftButtonDown"/>
		/// <seealso cref="ImageGrid_MouseRightButtonUp"/>
		/// <seealso cref="ButtonCellReset_Click"/>
		private readonly MutableView _view = new();


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
		/// The current view index.
		/// </summary>
		private int _currentViewIndex = -1;

		/// <summary>
		/// Indicates the current color chosen (used in coloring mode).
		/// See <see cref="WindowsSettings.PaletteColors"/> for more. If the value is
		/// <see cref="int.MinValue"/>, the current color is unavailable.
		/// </summary>
		/// <seealso cref="WindowsSettings.PaletteColors"/>
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
		private GridMap _selectedCellsWhileDrawingRegions = GridMap.Empty;

		/// <summary>
		/// Indicates all focused cells.
		/// </summary>
		private GridMap _focusedCells = GridMap.Empty;

		/// <summary>
		/// The preview map. This field is only used for
		/// <see cref="OnKeyDown(KeyEventArgs)"/> and <see cref="OnKeyUp(KeyEventArgs)"/>.
		/// </summary>
		/// <seealso cref="OnKeyDown(KeyEventArgs)"/>
		/// <seealso cref="OnKeyUp(KeyEventArgs)"/>
		private GridMap? _previewMap;

#if SUDOKU_RECOGNIZING
		/// <summary>
		/// Indicates an recognition instance.
		/// </summary>
		private RecognitionServiceProvider? _recognition;
#endif

		/// <summary>
		/// Indicates the analysis result after solved of the current grid.
		/// </summary>
		/// <remarks>
		/// <para>This field is used for give each step after solved the puzzle.</para>
		/// <para>
		/// If you changed the width of the <see cref="_tabControlInfo"/> and its width is <c>0</c>
		/// or below <c>0</c>, the result will not be shown on the tab control.
		/// </para>
		/// </remarks>
		private AnalysisResult? _analyisResult;

		/// <summary>
		/// The current technique information.
		/// </summary>
		/// <remarks>This field is used for rendering a <see cref="View"/>.</remarks>
		/// <seealso cref="View"/>
		private TechniqueInfo? _currentTechniqueInfo;

		/// <summary>
		/// Indicates the current target painter.
		/// </summary>
		private GridPainter _currentPainter = null!;

		/// <summary>
		/// The grid.
		/// </summary>
		private UndoableGrid _puzzle = new(Grid.Empty);

		/// <summary>
		/// Indicates the internal manual solver.
		/// This field is mutable.
		/// </summary>
		private ManualSolver _manualSolver = null!;

		/// <summary>
		/// The point converter.
		/// </summary>
		private PointConverter _pointConverter = null!;

		/// <summary>
		/// The initial grid.
		/// </summary>
		private Grid _initialPuzzle = null!;

		/// <summary>
		/// The steps searched. This field stores the previous group that searched before.
		/// </summary>
		private IEnumerable<IGrouping<string, TechniqueInfo>>? _cacheAllSteps;
	}
}
