using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
#if SUDOKU_RECOGNIZING
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
#endif
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Sudoku.Data;
using Sudoku.Data.Stepping;
using Sudoku.Drawing;
using Sudoku.Drawing.Extensions;
using Sudoku.Drawing.Layers;
using Sudoku.Extensions;
#if SUDOKU_RECOGNIZING
using Sudoku.Recognitions;
#endif
using Sudoku.Solving;
using Sudoku.Solving.Manual;
using Sudoku.Windows.Drawing.Layers;
using Sudoku.Windows.Extensions;
using static Sudoku.Data.GridMap.InitializeOption;
using static Sudoku.Solving.ConclusionType;
using static Sudoku.Windows.Constants.Processing;
using PointConverter = Sudoku.Drawing.PointConverter;
using SudokuGrid = Sudoku.Data.Grid;
using WPoint = System.Windows.Point;

namespace Sudoku.Windows
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
		/// The custom view.
		/// </summary>
		private readonly MutableView _view = new MutableView();


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
		[SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "<Pending>")]
		private RecognitionServiceProvider? _recognition;
#endif

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

#if SUDOKU_RECOGNIZING
			// Then initialize for recognizer.
			try
			{
				_recognition = new RecognitionServiceProvider();
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					$"Cannot to calculate{NewLine}" +
					$"  Source: {ex.Source}{NewLine}" +
					$"  Message: {ex.Message}",
					"Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
#endif

			// Define shortcuts.
			AddShortCut(Key.C, ModifierKeys.Control, null, MenuItemEditCopy_Click);
			AddShortCut(Key.H, ModifierKeys.Control, _menuItemGenerateHardPattern, MenuItemGenerateHardPattern_Click);
			AddShortCut(Key.O, ModifierKeys.Control, _menuItemFileOpen, MenuItemFileOpen_Click);
			AddShortCut(Key.P, ModifierKeys.Control, null, MenuItemFileGetSnapshot_Click);
			AddShortCut(Key.S, ModifierKeys.Control, null, MenuItemFileSave_Click);
			AddShortCut(Key.V, ModifierKeys.Control, _menuItemEditPaste, MenuItemEditPaste_Click);
			AddShortCut(Key.Y, ModifierKeys.Control, _menuItemEditRedo, MenuItemEditRedo_Click);
			AddShortCut(Key.Z, ModifierKeys.Control, _menuItemEditUndo, MenuItemEditUndo_Click);
			AddShortCut(Key.F5, ModifierKeys.Control, _menuItemEditRecomputeCandidates, MenuItemEditRecomputeCandidates_Click);
			AddShortCut(Key.OemTilde, ModifierKeys.Control, _menuItemEditFix, MenuItemEditFix_Click);
			AddShortCut(Key.F9, ModifierKeys.None, _menuItemAnalyzeSolve, MenuItemAnalyzeSolve_Click);
			AddShortCut(Key.F10, ModifierKeys.None, _menuItemAnalyzeGetSolution, MenuItemAnalyzeGetSolution_Click);
			AddShortCut(Key.F4, ModifierKeys.Alt, null, MenuItemFileQuit_Click);
			AddShortCut(Key.N, ModifierKeys.Control | ModifierKeys.Shift, _menuItemEditClear, MenuItemEditClear_Click);
			AddShortCut(Key.C, ModifierKeys.Control | ModifierKeys.Shift, null, MenuItemEditCopyCurrentGrid_Click);
			AddShortCut(Key.OemTilde, ModifierKeys.Control | ModifierKeys.Shift, _menuItemEditUnfix, MenuItemEditUnfix_Click);

			Title = $"{SolutionName} Ver {SolutionVersion}";

			LoadConfigIfWorth();
			InitializePointConverterAndLayers();
			LoadDatabaseIfWorth();
			UpdateControls();
		}

		/// <inheritdoc/>
		protected override void OnClosing(CancelEventArgs e)
		{
			// Ask when worth.
			if (Settings.AskWhileQuitting
				&& MessageBox.Show("Are you sure to quit?", "Info", MessageBoxButton.YesNo) == MessageBoxResult.No)
			{
				e.Cancel = true;
				return;
			}

			// Save configuration.
			SaveConfig();

#if SUDOKU_RECOGNIZING
			// Dispose the instance.
			// If the service provider is not initialized, this value will be null.
			_recognition?.Dispose();
#endif

			GC.Collect();

			base.OnClosing(e);

#if SUDOKU_RECOGNIZING
			if (_recognition?.ToolIsInitialized ?? false)
			{
				// If you don't use this feature, the program will not need to use
				// this method to KILL itself... KILL... sounds terrible and dangerous, isn't it?
				// To be honest, I don't know why the program fails to exit... The background
				// threads still running after base close method executed completely. If you
				// know the detail of Emgu.CV, please tell me, thx!
				Process.GetCurrentProcess().Kill();
			}
#endif
		}

		/// <inheritdoc/>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			// Get all cases for being pressed keys.
			if (e.Key.IsDigit())
			{
				var pt = Mouse.GetPosition(_imageGrid);
				if (IsPointOutOfRange(_imageGrid, pt))
				{
					e.Handled = true;
					return;
				}

				// Input or eliminate a digit.
				if (Keyboard.Modifiers == ModifierKeys.Shift)
				{
					// Eliminate a digit.
					_puzzle[
						_pointConverter.GetCellOffset(pt.ToDPointF()),
						e.Key.IsDigitUpsideAlphabets() ? e.Key - Key.D1 : e.Key - Key.NumPad1] = true;
				}
				else if (Keyboard.Modifiers == ModifierKeys.None)
				{
					// Input a digit.
					_puzzle[_pointConverter.GetCellOffset(pt.ToDPointF())] =
						e.Key.IsDigitUpsideAlphabets() ? e.Key - Key.D1 : e.Key - Key.NumPad1;
				}

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

				_layerCollection.Add(new FocusLayer(_pointConverter, _focusedCells, Settings.FocusedCellColor));

				UpdateImageGrid();
			}
			else if (e.Key == Key.Space)
			{
				// View the intersection.
				_previewMap = _focusedCells;
				_focusedCells = new GridMap(_focusedCells, ProcessPeersWithoutItself);

				_layerCollection.Add(new FocusLayer(_pointConverter, _focusedCells, Settings.FocusedCellColor));

				UpdateImageGrid();
			}
			else if (e.Key == Key.Tab)
			{
				// Move to next box row.
				int cell = _focusedCells.IsEmpty ? 0 : _focusedCells.SetAt(0);
				_focusedCells.Clear();
				_focusedCells.Add((cell + 3) % 81);

				_layerCollection.Add(new FocusLayer(_pointConverter, _focusedCells, Settings.FocusedCellColor));

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

				_layerCollection.Add(new FocusLayer(_pointConverter, _focusedCells, Settings.FocusedCellColor));

				UpdateImageGrid();
			}
		}

		/// <summary>
		/// Load database if worth.
		/// </summary>
		private void LoadDatabaseIfWorth()
		{
			if (Settings.CurrentPuzzleDatabase is null
				|| MessageBox.Show(
					"You have used a database at the previous time you use the program. " +
					"Do you want to load now?",
					"Info", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
			{
				return;
			}

			if (!File.Exists(Settings.CurrentPuzzleDatabase))
			{
				MessageBox.Show("File is missing... Load failed >_<", "Warning");

				Settings.CurrentPuzzleDatabase = null;
				Settings.CurrentPuzzleNumber = -1;

				return;
			}

			using var sr = new StreamReader(_database = Settings.CurrentPuzzleDatabase);
			_puzzlesText = sr.ReadToEnd().Split(Splitter, StringSplitOptions.RemoveEmptyEntries);

			int current = Settings.CurrentPuzzleNumber;
			int max = _puzzlesText.Length;
			LoadPuzzle(_puzzlesText[current].TrimEnd(Splitter));
			_labelPuzzleNumber.Content = $"{current + 1}/{max}";
			_textBoxJumpTo.IsEnabled = true;
			UpdateDatabaseControls(current != 0, current != 0, current != max - 1, current != max - 1);
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
		/// Update database controls.
		/// </summary>
		/// <param name="first">
		/// Indicates the next operation will set the property
		/// <see cref="UIElement.IsEnabled"/> of <see cref="_buttonFirst"/> at once.
		/// </param>
		/// <param name="prev">
		/// Indicates the next operation will set the property
		/// <see cref="UIElement.IsEnabled"/> of <see cref="_buttonPrev"/> at once.
		/// </param>
		/// <param name="next">
		/// Indicates the next operation will set the property
		/// <see cref="UIElement.IsEnabled"/> of <see cref="_buttonNext"/> at once.
		/// </param>
		/// <param name="last">
		/// Indicates the next operation will set the property
		/// <see cref="UIElement.IsEnabled"/> of <see cref="_buttonLast"/> at once.
		/// </param>
		/// <seealso cref="UIElement.IsEnabled"/>
		private void UpdateDatabaseControls(bool first, bool prev, bool next, bool last)
		{
			_buttonFirst.IsEnabled = first;
			_buttonPrev.IsEnabled = prev;
			_buttonNext.IsEnabled = next;
			_buttonLast.IsEnabled = last;
		}

		/// <summary>
		/// Save configurations if worth.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void LoadConfigIfWorth(string path = "configurations.scfg")
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
					MessageBox.Show("Failed to load the settings.", "Info");
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
					$"The configuration file cannot be saved due to exception throws:{NewLine}{ex.Message}",
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
		/// <param name="matchControl">
		/// The matching control. The hot-key can be executed if and only if this control
		/// is enabled, in other words, the <see cref="UIElement.IsEnabled"/>
		/// is <see langword="true"/>.
		/// </param>
		/// <param name="executed">The execution.</param>
		/// <seealso cref="UIElement.IsEnabled"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AddShortCut(
			Key key, ModifierKeys modifierKeys, UIElement? matchControl,
			ExecutedRoutedEventHandler executed)
		{
			var routedCommand = new RoutedCommand();
			routedCommand.InputGestures.Add(new KeyGesture(key, modifierKeys));
			CommandBindings.Add(
				new CommandBinding(
					routedCommand,
					(sender, e) =>
					{
						if (matchControl?.IsEnabled ?? true)
						{
							executed(sender, e);
						}
					}));
		}

		/// <summary>
		/// The internal copy method to process the operation of copying value to clipboard.
		/// </summary>
		/// <param name="format">The grid format.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void InternalCopy(string format)
		{
			try
			{
				// This may throw exception.
				//Clipboard.SetText(_puzzle.ToString(format));
				Clipboard.SetDataObject(_puzzle.ToString(format));
			}
			catch (ArgumentNullException ex)
			{
				MessageBox.Show(
					$"Cannot save text to clipboard due to:{NewLine}{ex.Message}",
					"Warning");
			}
			catch (COMException ex) when (ex.HResult == unchecked((int)2147746256))
			{
				MessageBox.Show(
					"Your clipboard is unavailable now, " +
					"because the program is running for generating or solving." +
					"Please close this program or wait for finishing and try later.",
					"Info");
			}
		}

		/// <summary>
		/// To update the control status.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void UpdateControls()
		{
#if !SUDOKU_RECOGNIZING
			_menuItemFileLoadPicture.IsEnabled = false;
#endif

			_menuItemOptionsShowCandidates.IsChecked = Settings.ShowCandidates;
			_menuItemAnalyzeSeMode.IsChecked = Settings.AnalyzeDifficultyStrictly;
			_menuItemAnalyzeFastSearch.IsChecked = Settings.FastSearch;
			_menuItemAnalyzeCheckGurthSymmetricalPlacement.IsChecked = Settings.CheckGurthSymmetricalPlacement;
			_menuItemAnalyzeShowFullHouses.IsChecked = Settings.EnableFullHouse;
			_menuItemAnalyzeShowLastDigits.IsChecked = Settings.EnableLastDigit;
			_menuItemAnalyzeOptimizeApplyingOrder.IsChecked = Settings.OptimizedApplyingOrder;
			_menuItemAnalyzeUseCalculationPriority.IsChecked = Settings.UseCalculationPriority;
			_menuItemAnalyzeCheckConclusionValidityAfterSearched.IsChecked = Settings.CheckConclusionValidityAfterSearched;

			_buttonCellColor1.Background = new SolidColorBrush(Settings.Color1.ToWColor());
			_buttonCellColor2.Background = new SolidColorBrush(Settings.Color2.ToWColor());
			_buttonCellColor3.Background = new SolidColorBrush(Settings.Color3.ToWColor());
			_buttonCellColor4.Background = new SolidColorBrush(Settings.Color4.ToWColor());
			_buttonCellColor5.Background = new SolidColorBrush(Settings.Color5.ToWColor());
			_buttonCellColor6.Background = new SolidColorBrush(Settings.Color6.ToWColor());
			_buttonCellColor7.Background = new SolidColorBrush(Settings.Color7.ToWColor());
			_buttonCellColor8.Background = new SolidColorBrush(Settings.Color8.ToWColor());
			_buttonCellColor9.Background = new SolidColorBrush(Settings.Color9.ToWColor());
			_buttonCellColor10.Background = new SolidColorBrush(Settings.Color10.ToWColor());
			_buttonCellColor11.Background = new SolidColorBrush(Settings.Color11.ToWColor());
			_buttonCellColor12.Background = new SolidColorBrush(Settings.Color12.ToWColor());
			_buttonCellColor13.Background = new SolidColorBrush(Settings.Color13.ToWColor());
			_buttonCellColor14.Background = new SolidColorBrush(Settings.Color14.ToWColor());
			_buttonCandidateColor1.Background = new SolidColorBrush(Settings.Color1.ToWColor());
			_buttonCandidateColor2.Background = new SolidColorBrush(Settings.Color2.ToWColor());
			_buttonCandidateColor3.Background = new SolidColorBrush(Settings.Color3.ToWColor());
			_buttonCandidateColor4.Background = new SolidColorBrush(Settings.Color4.ToWColor());
			_buttonCandidateColor5.Background = new SolidColorBrush(Settings.Color5.ToWColor());
			_buttonCandidateColor6.Background = new SolidColorBrush(Settings.Color6.ToWColor());
			_buttonCandidateColor7.Background = new SolidColorBrush(Settings.Color7.ToWColor());
			_buttonCandidateColor8.Background = new SolidColorBrush(Settings.Color8.ToWColor());
			_buttonCandidateColor9.Background = new SolidColorBrush(Settings.Color9.ToWColor());
			_buttonCandidateColor10.Background = new SolidColorBrush(Settings.Color10.ToWColor());
			_buttonCandidateColor11.Background = new SolidColorBrush(Settings.Color11.ToWColor());
			_buttonCandidateColor12.Background = new SolidColorBrush(Settings.Color12.ToWColor());
			_buttonCandidateColor13.Background = new SolidColorBrush(Settings.Color13.ToWColor());
			_buttonCandidateColor14.Background = new SolidColorBrush(Settings.Color14.ToWColor());

			_manualSolver = Settings.MainManualSolver;

			//_comboBoxDifficulty.ItemsSource = EnumEx.GetValues<DifficultyLevel>();
			_comboBoxSymmetry.ItemsSource =
				from field in EnumEx.GetValues<SymmetryType>()
				select new PrimaryElementTuple<string, SymmetryType>(
					typeof(SymmetryType)
						.GetField(Enum.GetName(typeof(SymmetryType), field)!)!
						.GetCustomAttribute<NameAttribute>()!
						.Name, field);
			_comboBoxSymmetry.SelectedIndex = Settings.GeneratingSymmetryModeComboBoxSelectedIndex;
			_comboBoxMode.SelectedIndex = Settings.GeneratingModeComboBoxSelectedIndex;
			SwitchOnGeneratingComboBoxesDisplaying();

			UpdateImageGrid();
		}

		/// <summary>
		/// Switch on displaying view of generating combo boxes.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SwitchOnGeneratingComboBoxesDisplaying()
		{
			switch (Settings.GeneratingModeComboBoxSelectedIndex)
			{
				case 0:
				{
					_comboBoxSymmetry.IsEnabled = true;
					_comboBoxBackdoorFilteringDepth.IsEnabled = false;
					break;
				}
				case 1:
				{
					_comboBoxSymmetry.IsEnabled = false;
					_comboBoxBackdoorFilteringDepth.IsEnabled = true;
					break;
				}
			}
		}

		/// <summary>
		/// Initializes point converter and layer instances.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void InitializePointConverterAndLayers()
		{
			_pointConverter = new PointConverter((float)_imageGrid.Width, (float)_imageGrid.Height);
			_layerCollection.Add(new BackLayer(_pointConverter, Settings.BackgroundColor));
			_layerCollection.Add(
				new GridLineLayer(_pointConverter, Settings.GridLineWidth, Settings.GridLineColor));
			_layerCollection.Add(
				new BlockLineLayer(_pointConverter, Settings.BlockLineWidth, Settings.BlockLineColor));
			_layerCollection.Add(
				new ValueLayer(
					_pointConverter, Settings.ValueScale, Settings.CandidateScale,
					Settings.GivenColor, Settings.ModifiableColor, Settings.CandidateColor,
					Settings.GivenFontName, Settings.ModifiableFontName,
					Settings.CandidateFontName, _puzzle, Settings.ShowCandidates));
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
				Puzzle = new UndoableGrid(SudokuGrid.Parse(puzzleStr, Settings.PmGridCompatible));

				_menuItemEditUndo.IsEnabled = _menuItemEditRedo.IsEnabled = false;
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void UpdateUndoRedoControls()
		{
			_imageUndoIcon.Source =
				new BitmapImage(
					new Uri(
						$"Resources/ImageIcon-Undo{((_menuItemEditUndo.IsEnabled = _puzzle.HasUndoSteps) ? string.Empty : "Disable")}.png",
						UriKind.Relative));
			_imageRedoIcon.Source =
				new BitmapImage(
					new Uri(
						$"Resources/ImageIcon-Redo{((_menuItemEditRedo.IsEnabled = _puzzle.HasRedoSteps) ? string.Empty : "Disable")}.png",
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
			_menuItemFileOpenDatabase.IsEnabled = false;
			_menuItemOptionsSettings.IsEnabled = false;
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
			_comboBoxBackdoorFilteringDepth.IsEnabled = false;

			UpdateDatabaseControls(false, false, false, false);
			_textBoxJumpTo.IsEnabled = false;
			_labelPuzzleNumber.ClearValue(ContentProperty);

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
			_menuItemFileOpenDatabase.IsEnabled = true;
			_menuItemOptionsSettings.IsEnabled = true;
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
			_comboBoxBackdoorFilteringDepth.IsEnabled = true;

			UpdateUndoRedoControls();
		}

		/// <summary>
		/// Disable solving controls.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DisableSolvingControls()
		{
			_menuItemFileOpen.IsEnabled = false;
			_menuItemFileOpenDatabase.IsEnabled = false;
			_menuItemOptionsSettings.IsEnabled = false;
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
			_comboBoxBackdoorFilteringDepth.IsEnabled = false;

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
			_menuItemFileOpenDatabase.IsEnabled = true;
			_menuItemOptionsSettings.IsEnabled = true;
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
			_comboBoxBackdoorFilteringDepth.IsEnabled = true;

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

			SwitchOnTabItemWhenGeneratedOrSolving();
		}

		/// <summary>
		/// Switch <see cref="TabItem"/>s when generated or solving.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SwitchOnTabItemWhenGeneratedOrSolving() => _tabControlInfo.SelectedIndex = 0;

		/// <summary>
		/// Set a digit.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SetADigit(int cell, int digit)
		{
			_puzzle[cell] = digit;

			UpdateUndoRedoControls();
			UpdateImageGrid();
		}

		/// <summary>
		/// Delete a digit.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DeleteADigit(int cell, int digit)
		{
			_puzzle[cell, digit] = true;

			UpdateUndoRedoControls();
			UpdateImageGrid();
		}


		/// <summary>
		/// To check the validity of all conclusions.
		/// </summary>
		/// <param name="solution">The solution.</param>
		/// <param name="conclusions">The conclusions.</param>
		/// <returns>A <see cref="bool"/> indicating that.</returns>
		private static bool CheckConclusionsValidity(IReadOnlyGrid solution, IEnumerable<Conclusion> conclusions)
		{
			foreach (var (t, c, d) in conclusions)
			{
				int digit = solution[c];
				switch (t)
				{
					case Assignment when digit != d:
					case Elimination when digit == d:
					{
						return false;
					}
				}
			}

			return true;
		}
	}
}
