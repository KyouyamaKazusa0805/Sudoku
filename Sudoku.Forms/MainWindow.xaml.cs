using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Input;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Data.Stepping;
using Sudoku.Drawing;
using Sudoku.Drawing.Extensions;
using Sudoku.Drawing.Layers;
using Sudoku.Forms.Extensions;
using Sudoku.Solving;
using Sudoku.Solving.Manual;
using PointConverter = Sudoku.Drawing.PointConverter;
using SudokuGrid = Sudoku.Data.Grid;
using w = System.Windows;

namespace Sudoku.Forms
{
	/// <summary>
	/// Interaction logic for <c>MainWindow.xaml</c>.
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// Indicates the internal manual solver.
		/// </summary>
		private readonly ManualSolver _manualSolver = new ManualSolver();

		/// <summary>
		/// Internal layer collection.
		/// </summary>
		private readonly LayerCollection _layerCollection = new LayerCollection();

		/// <summary>
		/// Indicates all focused cells.
		/// </summary>
		private readonly IList<int> _focusedCells = new List<int>();


		/// <summary>
		/// Indicates the analysis result after solving of the current grid.
		/// </summary>
		private AnalysisResult? _analyisResult = null;

		/// <summary>
		/// The grid.
		/// </summary>
		private UndoableGrid _puzzle = new UndoableGrid(SudokuGrid.Empty.Clone());

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
			get => _puzzle;
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
			AddShortCut(Key.O, ModifierKeys.Control, MenuItemFileOpen_Click);
			AddShortCut(Key.S, ModifierKeys.Control, MenuItemFileSave_Click);
			AddShortCut(Key.F4, ModifierKeys.Alt, MenuItemFileQuit_Click);
			AddShortCut(Key.F9, ModifierKeys.None, MenuItemAnalyzeSolve_Click); ;
			AddShortCut(Key.OemTilde, ModifierKeys.Control, MenuItemEditFix_Click);
			AddShortCut(Key.OemTilde, ModifierKeys.Control | ModifierKeys.Shift, MenuItemEditUnfix_Click);
			AddShortCut(Key.Z, ModifierKeys.Control, MenuItemEditUndo_Click);
			AddShortCut(Key.Y, ModifierKeys.Control, MenuItemEditRedo_Click);
			AddShortCut(Key.C, ModifierKeys.Control, MenuItemEditCopy_Click);
			AddShortCut(Key.C, ModifierKeys.Control | ModifierKeys.Shift, MenuItemEditCopyCurrentGrid_Click);
			AddShortCut(Key.V, ModifierKeys.Control, MenuItemEditPaste_Click);

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
					Settings.CandidateFontName, Puzzle, Settings.ShowCandidates));

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
				// Get the current cell.
				var pt = Mouse.GetPosition(_imageGrid);
				if (IsPointOutOfRange(_imageGrid, pt))
				{
					e.Handled = true;
					return;
				}

				Puzzle[_pointConverter.GetCellOffset(pt.ToDPointF())] =
					e.Key.IsDigitUpsideAlphabets() ? e.Key - Key.D1 : e.Key - Key.NumPad1;

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
				if (format is null)
				{
					Clipboard.SetText(Puzzle.ToString(null, null));
				}
				else
				{
					Clipboard.SetText(Puzzle.ToString(format));
				}
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
			_menuItemCheckGurthSymmetricalPlacement.IsChecked = Settings.CheckGurthSymmetricalPlacement;
			_menuItemDisableSlowTechniques.IsChecked = Settings.DisableSlowTechniques;
			_menuItemShowFullHouses.IsChecked = Settings.EnableFullHouse;
			_menuItemShowLastDigits.IsChecked = Settings.EnableLastDigit;
			_menuItemOptimizeApplyingOrder.IsChecked = Settings.OptimizedApplyingOrder;
			_menuItemUseCalculationPriority.IsChecked = Settings.UseCalculationPriority;
			_menuItemCheckConclusionValidityAfterSearched.IsChecked = Settings.CheckConclusionValidityAfterSearched;

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
		private bool IsPointOutOfRange(FrameworkElement control, w::Point point)
		{
			var (x, y) = point;
			return x < 0 || x > control.Width || y < 0 || y > control.Height;
		}

		/// <summary>
		/// Disable generating controls.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DisableGeneratingControls()
		{
			_textBoxInfo.Text = "Generating...";
			_menuItemGenerateHardPattern.IsEnabled = false;
			_menuItemAnalyzeSolve.IsEnabled = false;
			_menuItemEditPaste.IsEnabled = false;
			_menuItemEditFix.IsEnabled = false;
			_menuItemEditUnfix.IsEnabled = false;
			_menuItemEditReset.IsEnabled = false;
			_menuItemExport.IsEnabled = false;
		}

		/// <summary>
		/// Enable generating controls.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnableGeneratingControls()
		{
			_textBoxInfo.ClearValue(w::Controls.TextBox.TextProperty);
			_menuItemGenerateHardPattern.IsEnabled = true;
			_menuItemAnalyzeSolve.IsEnabled = true;
			_menuItemEditPaste.IsEnabled = true;
			_menuItemEditFix.IsEnabled = true;
			_menuItemEditUnfix.IsEnabled = true;
			_menuItemEditReset.IsEnabled = true;
			_menuItemExport.IsEnabled = true;
		}

		/// <summary>
		/// Disable solving controls.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DisableSolvingControls()
		{
			_textBoxInfo.Text = "Solving, please wait. During solving you can do some other work...";
			_menuItemAnalyzeSolve.IsEnabled = false;
			_menuItemGenerateHardPattern.IsEnabled = false;
			_menuItemEditPaste.IsEnabled = false;
			_menuItemEditFix.IsEnabled = false;
			_menuItemEditUnfix.IsEnabled = false;
			_menuItemEditReset.IsEnabled = false;
			_menuItemShowFullHouses.IsEnabled = false;
			_menuItemShowLastDigits.IsEnabled = false;
			_menuItemAnalyzeSeMode.IsEnabled = false;
			_menuItemAnalyzeFastSearch.IsEnabled = false;
			_menuItemCheckConclusionValidityAfterSearched.IsEnabled = false;
			_menuItemCheckGurthSymmetricalPlacement.IsEnabled = false;
			_menuItemDisableSlowTechniques.IsEnabled = false;
			_menuItemOptimizeApplyingOrder.IsEnabled = false;
			_menuItemUseCalculationPriority.IsEnabled = false;
			_menuItemExport.IsEnabled = false;
		}

		/// <summary>
		/// Enable solving controls.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnableSolvingControls()
		{
			_textBoxInfo.ClearValue(w::Controls.TextBox.TextProperty);
			_menuItemAnalyzeSolve.IsEnabled = true;
			_menuItemGenerateHardPattern.IsEnabled = true;
			_menuItemEditPaste.IsEnabled = true;
			_menuItemEditFix.IsEnabled = true;
			_menuItemEditUnfix.IsEnabled = true;
			_menuItemEditReset.IsEnabled = true;
			_menuItemShowFullHouses.IsEnabled = true;
			_menuItemShowLastDigits.IsEnabled = true;
			_menuItemAnalyzeSeMode.IsEnabled = true;
			_menuItemAnalyzeFastSearch.IsEnabled = true;
			_menuItemCheckConclusionValidityAfterSearched.IsEnabled = true;
			_menuItemCheckGurthSymmetricalPlacement.IsEnabled = true;
			_menuItemDisableSlowTechniques.IsEnabled = true;
			_menuItemOptimizeApplyingOrder.IsEnabled = true;
			_menuItemUseCalculationPriority.IsEnabled = true;
			_menuItemExport.IsEnabled = true;
		}
	}
}
