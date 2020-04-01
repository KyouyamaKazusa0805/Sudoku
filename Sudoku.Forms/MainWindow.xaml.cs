using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Data.Stepping;
using Sudoku.Drawing;
using Sudoku.Drawing.Extensions;
using Sudoku.Drawing.Layers;
using Sudoku.Forms.Drawing.Layers;
using Sudoku.Forms.Extensions;
using Sudoku.Solving;
using Sudoku.Solving.Manual;
using PointConverter = Sudoku.Drawing.PointConverter;
using SudokuGrid = Sudoku.Data.Grid;
using WPoint = System.Windows.Point;

namespace Sudoku.Forms
{
	/// <summary>
	/// Interaction logic for <c>MainWindow.xaml</c>.
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// Internal layer collection.
		/// </summary>
		private readonly LayerCollection _layerCollection = new LayerCollection();


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


		/// <summary>
		/// Indicates the analysis result after solving of the current grid.
		/// </summary>
		private AnalysisResult? _analyisResult = null;

		/// <summary>
		/// The grid.
		/// </summary>
		private UndoableGrid _puzzle = new UndoableGrid(SudokuGrid.Empty);

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
		private IReadOnlyGrid _initialPuzzle = null!;


		/// <include file='../GlobalDocComments.xml' path='comments/defaultConstructor'/>
		public MainWindow() => InitializeComponent();


		/// <summary>
		/// Indicates the name of this solution.
		/// </summary>
		private string SolutionName
		{
			get
			{
				return Assembly
					.GetExecutingAssembly()
					.GetCustomAttribute<AssemblyProductAttribute>()
					is AssemblyProductAttribute attr
					? attr.Product
					: "Sunnie's Sudoku Solution";
			}
		}

		/// <summary>
		/// Indicates the version.
		/// </summary>
		private string Version
		{
			get
			{
				return Assembly
					.GetExecutingAssembly()
					.GetName()
					.Version
					.NullableToString("Unknown version");
			}
		}

		/// <summary>
		/// Indicates the puzzle, which is equivalent to <see cref="_puzzle"/>,
		/// but add the auto-update value layer.
		/// </summary>
		/// <seealso cref="_puzzle"/>
		private UndoableGrid Puzzle
		{
			set
			{
				_layerCollection.Add(
					new ValueLayer(
						_pointConverter, Settings.ValueScale, Settings.CandidateScale,
						Settings.GivenColor, Settings.ModifiableColor, Settings.CandidateColor,
						Settings.GivenFontName, Settings.ModifiableFontName,
						Settings.CandidateFontName, _puzzle = value, Settings.ShowCandidates));
				_layerCollection.Remove(typeof(ViewLayer).Name);
				_initialPuzzle = value.Clone();

				GC.Collect();
			}
		}

		/// <summary>
		/// Indicates the settings used.
		/// </summary>
		public Settings Settings { get; private set; } = null!;


		/// <inheritdoc/>
		protected override void OnInitialized(EventArgs e)
		{
			// Call the base method.
			base.OnInitialized(e);

			// Define shortcuts.
			AddShortCut(Key.C, ModifierKeys.Control, MenuItemEditCopy_Click);
			AddShortCut(Key.H, ModifierKeys.Control, MenuItemGenerateHardPattern_Click);
			AddShortCut(Key.O, ModifierKeys.Control, MenuItemFileOpen_Click);
			AddShortCut(Key.P, ModifierKeys.Control, MenuItemFileGetSnapshot_Click);
			AddShortCut(Key.S, ModifierKeys.Control, MenuItemFileSave_Click);
			AddShortCut(Key.V, ModifierKeys.Control, MenuItemEditPaste_Click);
			AddShortCut(Key.Y, ModifierKeys.Control, MenuItemEditRedo_Click);
			AddShortCut(Key.Z, ModifierKeys.Control, MenuItemEditUndo_Click);
			AddShortCut(Key.F4, ModifierKeys.Alt, MenuItemFileQuit_Click);
			AddShortCut(Key.F9, ModifierKeys.None, MenuItemAnalyzeSolve_Click);
			AddShortCut(Key.OemTilde, ModifierKeys.Control, MenuItemEditFix_Click);
			AddShortCut(Key.N, ModifierKeys.Control | ModifierKeys.Shift, MenuItemEditClear_Click);
			AddShortCut(Key.C, ModifierKeys.Control | ModifierKeys.Shift, MenuItemEditCopyCurrentGrid_Click);
			AddShortCut(Key.OemTilde, ModifierKeys.Control | ModifierKeys.Shift, MenuItemEditUnfix_Click);

			// Initializes some controls.
			Title = $"{SolutionName} Ver {Version}";

			// Load configurations if worth.
			LoadConfig();

			// Then initialize the layer collection and point converter
			// for later utility.
			_pointConverter = new PointConverter((float)_imageGrid.Width, (float)_imageGrid.Height);
			_layerCollection.Add(new BackLayer(_pointConverter, Settings.BackgroundColor));
			_layerCollection.Add(
				new GridLineLayer(
					_pointConverter, Settings.GridLineWidth, Settings.GridLineColor));
			_layerCollection.Add(
				new BlockLineLayer(
					_pointConverter, Settings.BlockLineWidth, Settings.BlockLineColor));
			_layerCollection.Add(
				new ValueLayer(
					_pointConverter, Settings.ValueScale, Settings.CandidateScale,
					Settings.GivenColor, Settings.ModifiableColor, Settings.CandidateColor,
					Settings.GivenFontName, Settings.ModifiableFontName,
					Settings.CandidateFontName, _puzzle, Settings.ShowCandidates));

			UpdateControls();
		}

		/// <inheritdoc/>
		protected override void OnClosing(CancelEventArgs e)
		{
			if (Settings.AskWhileQuitting
				&& MessageBox.Show("Are you sure to quit?", "Info", MessageBoxButton.YesNo) == MessageBoxResult.No)
			{
				e.Cancel = true;
				return;
			}

			SaveConfig();

			base.OnClosing(e);
		}

		/// <inheritdoc/>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			// Get all cases for being pressed keys.
			if (e.Key.IsDigit())
			{
				// Input.
				var pt = Mouse.GetPosition(_imageGrid);
				if (IsPointOutOfRange(_imageGrid, pt))
				{
					e.Handled = true;
					return;
				}

				_puzzle[_pointConverter.GetCellOffset(pt.ToDPointF())] =
					e.Key.IsDigitUpsideAlphabets() ? e.Key - Key.D1 : e.Key - Key.NumPad1;

				UpdateUndoRedoControls();
				UpdateImageGrid();
			}
			else if (e.Key.IsArrow() && _focusedCells.Count == 1)
			{
				// Move the focused cell.
				int cell = _focusedCells.SetAt(0);
				_focusedCells.Clear();
				_focusedCells.Add(e.Key switch
				{
					Key.Up => cell - 9 < 0 ? cell + 72 : cell - 9,
					Key.Down => cell + 9 >= 81 ? cell - 72 : cell + 9,
					Key.Left => cell - 1 < 0 ? cell + 8 : cell - 1,
					Key.Right => (cell + 1) % 81,
					_ => throw Throwing.ImpossibleCase
				});

				_layerCollection.Add(
					new FocusLayer(_pointConverter, _focusedCells, Settings.FocusedCellColor));

				UpdateImageGrid();
			}
			else if (e.Key == Key.Space)
			{
				// View the intersection.
				_previewMap = _focusedCells;
				_focusedCells = new GridMap(
					_focusedCells.Offsets, GridMap.InitializeOption.ProcessPeersWithoutItself);

				_layerCollection.Add(
					new FocusLayer(_pointConverter, _focusedCells, Settings.FocusedCellColor));

				UpdateImageGrid();
			}
			else if (e.Key == Key.Tab)
			{
				// Move to next box row.
				int cell = _focusedCells.IsEmpty ? 0 : _focusedCells.SetAt(0);
				_focusedCells.Clear();
				_focusedCells.Add((cell + 3) % 81);

				_layerCollection.Add(
					new FocusLayer(_pointConverter, _focusedCells, Settings.FocusedCellColor));

				UpdateImageGrid();
			}
			else if (e.Key == Key.Escape)
			{
				// Clear focused cells.
				_focusedCells.Clear();
				_layerCollection.Remove(typeof(FocusLayer).Name);

				UpdateImageGrid();
			}

			GC.Collect();
		}

		/// <inheritdoc/>
		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);

			if (e.Key == Key.Space && !(_previewMap is null))
			{
				_focusedCells = _previewMap.Value;

				_layerCollection.Add(
					new FocusLayer(_pointConverter, _focusedCells, Settings.FocusedCellColor));

				UpdateImageGrid();
			}
		}

		/// <summary>
		/// Repaint the <see cref="_imageGrid"/> to show the newer grid image.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void UpdateImageGrid()
		{
			var bitmap = new Bitmap((int)_imageGrid.Width, (int)_imageGrid.Height);
			_layerCollection.IntegrateTo(bitmap);
			_imageGrid.Source = bitmap.ToImageSource();

			GC.Collect();
		}

		/// <summary>
		/// Save configurations if worth.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void LoadConfig(string path = "configurations.scfg")
		{
			Settings = new Settings();
			if (File.Exists(path))
			{
				var fs = new FileStream(path, FileMode.Open);
				try
				{
					Settings = (Settings)new BinaryFormatter().Deserialize(fs);
				}
				catch
				{
					// Do nothing.
				}
				finally
				{
					fs.Close();
				}
			}
			else
			{
				Settings.CoverBy(Settings.DefaultSetting);
			}
		}

		/// <summary>
		/// Save configurations.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SaveConfig(string path = "configurations.scfg")
		{
			var fs = new FileStream(path, FileMode.Create);
			try
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(fs, Settings);
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					$"The configuration file cannot be saved due to exception throws:{Environment.NewLine}{ex.Message}",
					"Warning");
			}
			finally
			{
				fs.Close();
			}
		}

		/// <summary>
		/// Bind a shortcut to a method (mounted to an event) to execute.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="modifierKeys">The modifiers.</param>
		/// <param name="executed">The execution.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AddShortCut(Key key, ModifierKeys modifierKeys, ExecutedRoutedEventHandler executed)
		{
			var routedCommand = new RoutedCommand();
			routedCommand.InputGestures.Add(new KeyGesture(key, modifierKeys));
			CommandBindings.Add(new CommandBinding(routedCommand, executed));
		}

		/// <summary>
		/// The internal copy method to process the operation of copying value to clipboard.
		/// </summary>
		/// <param name="format">
		/// The grid format. If the value is <see langword="null"/>, it will call the method
		/// <see cref="SudokuGrid.ToString(string?, IFormatProvider?)"/>; otherwise,
		/// <see cref="SudokuGrid.ToString(string)"/>.
		/// </param>
		/// <seealso cref="SudokuGrid.ToString(string)"/>
		/// <seealso cref="SudokuGrid.ToString(string?, IFormatProvider?)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void InternalCopy(string? format)
		{
			try
			{
				// Even though the value is null, the formatter can be processed.
				Clipboard.SetText(_puzzle.ToString(format!));
			}
			catch (ArgumentNullException ex)
			{
				MessageBox.Show(
					$"Cannot save text to clipboard due to:{Environment.NewLine}{ex.Message}",
					"Warning");
			}
		}

		/// <summary>
		/// To update the control status.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void UpdateControls()
		{
			_menuItemOptionsShowCandidates.IsChecked = Settings.ShowCandidates;
			_menuItemAnalyzeSeMode.IsChecked = Settings.SeMode;
			_menuItemAnalyzeFastSearch.IsChecked = Settings.FastSearch;
			_menuItemAnalyzeCheckGurthSymmetricalPlacement.IsChecked = Settings.CheckGurthSymmetricalPlacement;
			_menuItemAnalyzeShowFullHouses.IsChecked = Settings.EnableFullHouse;
			_menuItemAnalyzeShowLastDigits.IsChecked = Settings.EnableLastDigit;
			_menuItemAnalyzeOptimizeApplyingOrder.IsChecked = Settings.OptimizedApplyingOrder;
			_menuItemAnalyzeUseCalculationPriority.IsChecked = Settings.UseCalculationPriority;
			_menuItemAnalyzeCheckConclusionValidityAfterSearched.IsChecked = Settings.CheckConclusionValidityAfterSearched;

			_manualSolver = Settings.MainManualSolver;

			//_comboBoxDifficulty.ItemsSource = EnumEx.GetValues<DifficultyLevel>();
			_comboBoxSymmetry.ItemsSource = EnumEx.GetValues<SymmetricalType>();

			UpdateImageGrid();
		}

		/// <summary>
		/// To load a puzzle with a specified possible puzzle string.
		/// </summary>
		/// <param name="puzzleStr">The puzzle string.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void LoadPuzzle(string puzzleStr)
		{
			try
			{
				Puzzle = new UndoableGrid(SudokuGrid.Parse(puzzleStr));

				_menuItemUndo.IsEnabled = _menuItemRedo.IsEnabled = false;
				UpdateImageGrid();
			}
			catch (ArgumentException)
			{
				MessageBox.Show("The specified puzzle is invalid.", "Warning");
			}
		}

		/// <summary>
		/// To check whether the specified point is out of range of a control.
		/// </summary>
		/// <param name="control">The control.</param>
		/// <param name="point">The point.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool IsPointOutOfRange(FrameworkElement control, WPoint point)
		{
			var (x, y) = point;
			return x < 0 || x > control.Width || y < 0 || y > control.Height;
		}

		/// <summary>
		/// Update undo and redo controls.
		/// </summary>
		private void UpdateUndoRedoControls()
		{
			_imageUndoIcon.Source =
				new BitmapImage(
					new Uri(
						$"Resources/ImageIcon-Undo{((_menuItemUndo.IsEnabled = _puzzle.HasUndoSteps) ? string.Empty : "Disable")}.png",
						UriKind.Relative));
			_imageRedoIcon.Source =
				new BitmapImage(
					new Uri(
						$"Resources/ImageIcon-Redo{((_menuItemRedo.IsEnabled = _puzzle.HasRedoSteps) ? string.Empty : "Disable")}.png",
						UriKind.Relative));
		}

		/// <summary>
		/// Disable generating controls.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DisableGeneratingControls()
		{
			_textBoxInfo.Text = "Generating...";
			_menuItemFileOpen.IsEnabled = false;
			_menuItemGenerateHardPattern.IsEnabled = false;
			_menuItemEditPaste.IsEnabled = false;
			_menuItemEditFix.IsEnabled = false;
			_menuItemEditUnfix.IsEnabled = false;
			_menuItemEditReset.IsEnabled = false;
			_menuItemEditClear.IsEnabled = false;
			_menuItemGenerateWithSymmetry.IsEnabled = false;
			_menuItemAnalyzeSolve.IsEnabled = false;
			_menuItemAnalyzeExport.IsEnabled = false;
			_buttonFindAllSteps.IsEnabled = false;
			_imageGeneratingIcon.IsEnabled = false;
			_imageSolve.IsEnabled = false;
			_comboBoxSymmetry.IsEnabled = false;
			_comboBoxMode.IsEnabled = false;

			UpdateUndoRedoControls();
		}

		/// <summary>
		/// Enable generating controls.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnableGeneratingControls()
		{
			_textBoxInfo.ClearValue(TextBox.TextProperty);
			_menuItemFileOpen.IsEnabled = true;
			_menuItemGenerateHardPattern.IsEnabled = true;
			_menuItemEditPaste.IsEnabled = true;
			_menuItemEditFix.IsEnabled = true;
			_menuItemEditUnfix.IsEnabled = true;
			_menuItemEditReset.IsEnabled = true;
			_menuItemEditClear.IsEnabled = true;
			_menuItemGenerateWithSymmetry.IsEnabled = true;
			_menuItemAnalyzeSolve.IsEnabled = true;
			_menuItemAnalyzeExport.IsEnabled = true;
			_buttonFindAllSteps.IsEnabled = true;
			_imageGeneratingIcon.IsEnabled = true;
			_imageSolve.IsEnabled = true;
			_comboBoxMode.IsEnabled = true;
			_comboBoxSymmetry.IsEnabled = true;

			UpdateUndoRedoControls();
		}

		/// <summary>
		/// Disable solving controls.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DisableSolvingControls()
		{
			_menuItemFileOpen.IsEnabled = false;
			_menuItemGenerateHardPattern.IsEnabled = false;
			_menuItemEditPaste.IsEnabled = false;
			_menuItemEditFix.IsEnabled = false;
			_menuItemEditUnfix.IsEnabled = false;
			_menuItemEditReset.IsEnabled = false;
			_menuItemEditClear.IsEnabled = false;
			_menuItemGenerateWithSymmetry.IsEnabled = false;
			_menuItemAnalyzeSolve.IsEnabled = false;
			_menuItemAnalyzeShowFullHouses.IsEnabled = false;
			_menuItemAnalyzeShowLastDigits.IsEnabled = false;
			_menuItemAnalyzeSeMode.IsEnabled = false;
			_menuItemAnalyzeFastSearch.IsEnabled = false;
			_menuItemAnalyzeCheckConclusionValidityAfterSearched.IsEnabled = false;
			_menuItemAnalyzeCheckGurthSymmetricalPlacement.IsEnabled = false;
			_menuItemAnalyzeOptimizeApplyingOrder.IsEnabled = false;
			_menuItemAnalyzeUseCalculationPriority.IsEnabled = false;
			_menuItemAnalyzeExport.IsEnabled = false;
			_buttonFindAllSteps.IsEnabled = false;
			_imageGeneratingIcon.IsEnabled = false;
			_imageSolve.IsEnabled = false;
			_comboBoxSymmetry.IsEnabled = false;
			_comboBoxMode.IsEnabled = false;

			UpdateUndoRedoControls();
		}

		/// <summary>
		/// Enable solving controls.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnableSolvingControls()
		{
			_textBoxInfo.ClearValue(TextBox.TextProperty);
			_menuItemFileOpen.IsEnabled = true;
			_menuItemGenerateHardPattern.IsEnabled = true;
			_menuItemEditPaste.IsEnabled = true;
			_menuItemEditFix.IsEnabled = true;
			_menuItemEditUnfix.IsEnabled = true;
			_menuItemEditReset.IsEnabled = true;
			_menuItemEditClear.IsEnabled = true;
			_menuItemGenerateWithSymmetry.IsEnabled = true;
			_menuItemAnalyzeSolve.IsEnabled = true;
			_menuItemAnalyzeShowFullHouses.IsEnabled = true;
			_menuItemAnalyzeShowLastDigits.IsEnabled = true;
			_menuItemAnalyzeSeMode.IsEnabled = true;
			_menuItemAnalyzeFastSearch.IsEnabled = true;
			_menuItemAnalyzeCheckConclusionValidityAfterSearched.IsEnabled = true;
			_menuItemAnalyzeCheckGurthSymmetricalPlacement.IsEnabled = true;
			_menuItemAnalyzeOptimizeApplyingOrder.IsEnabled = true;
			_menuItemAnalyzeUseCalculationPriority.IsEnabled = true;
			_menuItemAnalyzeExport.IsEnabled = true;
			_buttonFindAllSteps.IsEnabled = true;
			_imageGeneratingIcon.IsEnabled = true;
			_imageSolve.IsEnabled = true;
			_comboBoxMode.IsEnabled = true;
			_comboBoxSymmetry.IsEnabled = true;

			UpdateUndoRedoControls();
		}

		/// <summary>
		/// Clear item sources when generated.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ClearItemSourcesWhenGeneratedOrSolving()
		{
			_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);
			_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);
			_listBoxTechniques.ClearValue(ItemsControl.ItemsSourceProperty);

			SwitchTabItemWhenGeneratedOrSolving();
		}

		/// <summary>
		/// Switch <see cref="TabItem"/>s when generated or solving.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SwitchTabItemWhenGeneratedOrSolving() => _tabControlInfo.SelectedIndex = 0;
	}
}
