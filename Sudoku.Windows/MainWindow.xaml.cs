using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Stepping;
using Sudoku.Drawing;
using Sudoku.Drawing.Extensions;
using Sudoku.Extensions;
using Sudoku.Solving;
using Sudoku.Solving.Manual;
using Sudoku.Windows.Constants;
using Sudoku.Windows.Extensions;
using static System.StringSplitOptions;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;
using static Sudoku.Windows.Constants.Processings;
using CoreResources = Sudoku.Windows.Resources;
using PointConverter = Sudoku.Drawing.PointConverter;
using SudokuGrid = Sudoku.Data.Grid;
using WPoint = System.Windows.Point;
#if SUDOKU_RECOGNIZING
using System.Diagnostics;
using Sudoku.Recognitions;
#endif

namespace Sudoku.Windows
{
	/// <summary>
	/// Interaction logic for <c>MainWindow.xaml</c>.
	/// </summary>
	public partial class MainWindow : Window
	{
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
		/// Indicates the analysis result after solving of the current grid.
		/// </summary>
		private AnalysisResult? _analyisResult;

		/// <summary>
		/// Indicates the current target painter.
		/// </summary>
		private GridPainter _currentPainter = null!;

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

		/// <summary>
		/// The steps searched. This field stores the previous group that searched before.
		/// </summary>
		private IEnumerable<IGrouping<string, TechniqueInfo>>? _cacheAllSteps;


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
				_currentPainter = new GridPainter(_pointConverter, Settings, _puzzle = value);
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

			// Load configurations.
			LoadConfigIfWorth();

			// Load language configuration.
			ChangeLanguage(Settings.LanguageCode ??= "en-us");

			// Prevent you opening the second same window.
			var mutex = new Mutex(true, LangSource["SolutionName"] as string, out bool mutexIsNew);
			if (mutexIsNew)
			{
				mutex.ReleaseMutex();
			}
			else
			{
				Messagings.OnlyOpenOneProgram();
				Environment.Exit(0);
			}

#if SUDOKU_RECOGNIZING
			// Then initialize for recognizer.
			try
			{
				_recognition = new RecognitionServiceProvider();
			}
			catch (Exception ex)
			{
				Messagings.FailedToLoadRecognitionTool(ex);
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
			AddShortCut(Key.F9, ModifierKeys.None, _menuItemAnalyzeAnalyze, MenuItemAnalyzeAnalyze_Click);
			AddShortCut(Key.F10, ModifierKeys.None, _menuItemAnalyzeSolve, MenuItemAnalyzeSolve_Click);
			AddShortCut(Key.F4, ModifierKeys.Alt, null, MenuItemFileQuit_Click);
			AddShortCut(Key.N, ModifierKeys.Control | ModifierKeys.Shift, _menuItemEditClear, MenuItemEditClear_Click);
			AddShortCut(Key.C, ModifierKeys.Control | ModifierKeys.Shift, null, MenuItemEditCopyCurrentGrid_Click);
			AddShortCut(Key.OemTilde, ModifierKeys.Control | ModifierKeys.Shift, _menuItemEditUnfix, MenuItemEditUnfix_Click);

			InitializePointConverterAndLayers();
			LoadDatabaseIfWorth();
			UpdateControls();
		}

		/// <inheritdoc/>
		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);

			_imageGrid.Height = _imageGrid.Width =
				Math.Min(_gridMain.ColumnDefinitions[0].ActualWidth, _gridMain.RowDefinitions[0].ActualHeight);
			Settings.GridSize = _gridMain.ColumnDefinitions[0].ActualWidth;
			_currentPainter.PointConverter = new PointConverter(_imageGrid.RenderSize);

			UpdateImageGrid();
		}

		/// <inheritdoc/>
		protected override void OnClosing(CancelEventArgs e)
		{
			// Ask when worth.
			if (Settings.AskWhileQuitting && Messagings.AskWhileQuitting() == MessageBoxResult.No)
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
			switch (e.Key)
			{
				case var key when key.IsDigit():
				{
					int cell = _pointConverter.GetCellOffset(Mouse.GetPosition(_imageGrid).ToDPointF());
					if (cell == -1)
					{
						return;
					}

					if (_puzzle.GetStatus(cell) == CellStatus.Given)
					{
						return;
					}

					int digit = e.Key.IsDigitUpsideAlphabets() ? e.Key - Key.D1 : e.Key - Key.NumPad1;
					switch (Keyboard.Modifiers)
					{
						case ModifierKeys.None:
						{
							// Input a digit.
							// Input or eliminate a digit.
							if (digit != -1 && PeerMaps[cell].Any(c => _puzzle[c] == digit))
							{
								// Input is invalid. We cannot let you fill this cell with this digit.
								return;
							}

							_puzzle[cell] = digit;
							if (digit != -1 && _puzzle.GetStatus(cell) == CellStatus.Modifiable)
							{
								// This cell can be modified with other digits.
								_puzzle.RecomputeCandidates();
							}

							break;
						}
						case ModifierKeys.Shift:
						{
							// Eliminate a digit.
							_puzzle[cell, digit] = true;

							break;
						}
					}

					UpdateUndoRedoControls();
					UpdateImageGrid();

					break;
				}
				case var key when key.IsArrow() && _focusedCells.Count == 1:
				{
					// Move the focused cell.
					int cell = _focusedCells.SetAt(0);
					_focusedCells.Clear();
					_focusedCells.Add(
						e.Key switch
						{
							Key.Up => cell - 9 < 0 ? cell + 72 : cell - 9,
							Key.Down => cell + 9 >= 81 ? cell - 72 : cell + 9,
							Key.Left => cell - 1 < 0 ? cell + 8 : cell - 1,
							Key.Right => (cell + 1) % 81,
							_ => throw Throwings.ImpossibleCase
						});

					_currentPainter.Grid = _puzzle;
					_currentPainter.FocusedCells = _focusedCells;

					UpdateImageGrid();

					break;
				}
				case Key.Space:
				{
					// View the intersection.
					_previewMap = _focusedCells;
					_focusedCells = _focusedCells.PeerIntersection;

					_currentPainter.Grid = _puzzle;
					_currentPainter.FocusedCells = _focusedCells;

					UpdateImageGrid();

					break;
				}
				case Key.Tab:
				{
					// Move to next box row.
					int cell = _focusedCells.IsEmpty ? 0 : _focusedCells.SetAt(0);
					_focusedCells.Clear();
					_focusedCells.Add((cell + 3) % 81);

					_currentPainter.Grid = _puzzle;
					_currentPainter.FocusedCells = _focusedCells;

					UpdateImageGrid();

					break;
				}
				case Key.Escape:
				{
					// Clear focused cells.
					_focusedCells.Clear();
					_currentPainter.Grid = _puzzle;
					_currentPainter.FocusedCells = null;

					UpdateImageGrid();

					break;
				}
			}

			GC.Collect();
		}

		/// <inheritdoc/>
		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);

			if (!(_previewMap is null) && e.Key == Key.Space)
			{
				_focusedCells = _previewMap.Value;

				_currentPainter.FocusedCells = _focusedCells;

				UpdateImageGrid();
			}
		}

		/// <summary>
		/// Load database if worth.
		/// </summary>
		private void LoadDatabaseIfWorth()
		{
			if (Settings.CurrentPuzzleDatabase is null
				|| Messagings.AskWhileLoadingAndCoveringDatabase() != MessageBoxResult.Yes)
			{
				return;
			}

			if (!File.Exists(Settings.CurrentPuzzleDatabase))
			{
				Messagings.FailedToLoadDatabase();

				Settings.CurrentPuzzleDatabase = null;
				Settings.CurrentPuzzleNumber = -1;

				return;
			}

			using var sr = new StreamReader(_database = Settings.CurrentPuzzleDatabase);
			_puzzlesText = sr.ReadToEnd().Split(Splitter, RemoveEmptyEntries);

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
			_imageGrid.Source = _currentPainter.Draw().ToImageSource();

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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
				FileStream? fs = null;
				try
				{
					fs = new FileStream(path, FileMode.Open);
					Settings = (Settings)new BinaryFormatter().Deserialize(fs);
				}
				catch
				{
					Messagings.FailedToLoadSettings();
				}
				finally
				{
					fs?.Close();
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
			FileStream? fs = null;
			try
			{
				fs = new FileStream(path, FileMode.Create);
				var formatter = new BinaryFormatter();
				formatter.Serialize(fs, Settings);
			}
			catch (Exception ex)
			{
				Messagings.FailedToSaveConfig(ex);
			}
			finally
			{
				fs?.Close();
			}
		}

		/// <summary>
		/// Bind a shortcut to a method (mounted to an event) to execute.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="modifierKeys">The modifiers.</param>
		/// <param name="matchControl">
		/// The matching control. The hot-key can be executed <b>if and only if</b> this control
		/// is enabled, in other words, the <see cref="UIElement.IsEnabled"/>
		/// is <see langword="true"/>.
		/// </param>
		/// <param name="executed">The execution.</param>
		/// <seealso cref="UIElement.IsEnabled"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AddShortCut(
			Key key, ModifierKeys modifierKeys, UIElement? matchControl, ExecutedRoutedEventHandler executed)
		{
			var routedCommand = new RoutedCommand();
			routedCommand.InputGestures.Add(new KeyGesture(key, modifierKeys));
			CommandBindings.Add(
				new CommandBinding(
					routedCommand,
					(sender, e) => { if (matchControl?.IsEnabled ?? true) executed(sender, e); }));
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
				Clipboard.SetDataObject(_puzzle.ToString(format));
				#region Obsolete code
				// This may throw exceptions being called while solving and generating puzzles.
				//Clipboard.SetText(_puzzle.ToString(format));
				#endregion
			}
			catch (ArgumentNullException ex)
			{
				Messagings.FailedToSaveToClipboardDueToArgumentNullException(ex);
			}
			catch (COMException ex) when (ex.HResult == unchecked((int)2147746256))
			{
				Messagings.FailedToSaveToClipboardDueToAsyncCalling();
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

#if !DEBUG
			_menuItemGenerateWithTechniqueFiltering.IsEnabled = false;
#endif

			_menuItemOptionsShowCandidates.IsChecked = Settings.ShowCandidates;
			_menuItemAnalyzeSeMode.IsChecked = Settings.MainManualSolver.AnalyzeDifficultyStrictly;
			_menuItemAnalyzeFastSearch.IsChecked = Settings.MainManualSolver.FastSearch;
			_menuItemAnalyzeCheckGurthSymmetricalPlacement.IsChecked = Settings.MainManualSolver.CheckGurthSymmetricalPlacement;
			_menuItemAnalyzeShowFullHouses.IsChecked = Settings.MainManualSolver.EnableFullHouse;
			_menuItemAnalyzeShowLastDigits.IsChecked = Settings.MainManualSolver.EnableLastDigit;
			_menuItemAnalyzeOptimizeApplyingOrder.IsChecked = Settings.MainManualSolver.OptimizedApplyingOrder;
			_menuItemAnalyzeUseCalculationPriority.IsChecked = Settings.MainManualSolver.UseCalculationPriority;
			_menuItemAnalyzeCheckConclusionValidityAfterSearched.IsChecked = Settings.MainManualSolver.CheckConclusionValidityAfterSearched;

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
			_buttonRegionColor1.Background = new SolidColorBrush(Settings.Color1.ToWColor());
			_buttonRegionColor2.Background = new SolidColorBrush(Settings.Color2.ToWColor());
			_buttonRegionColor3.Background = new SolidColorBrush(Settings.Color3.ToWColor());
			_buttonRegionColor4.Background = new SolidColorBrush(Settings.Color4.ToWColor());
			_buttonRegionColor5.Background = new SolidColorBrush(Settings.Color5.ToWColor());
			_buttonRegionColor6.Background = new SolidColorBrush(Settings.Color6.ToWColor());
			_buttonRegionColor7.Background = new SolidColorBrush(Settings.Color7.ToWColor());
			_buttonRegionColor8.Background = new SolidColorBrush(Settings.Color8.ToWColor());
			_buttonRegionColor9.Background = new SolidColorBrush(Settings.Color9.ToWColor());
			_buttonRegionColor10.Background = new SolidColorBrush(Settings.Color10.ToWColor());
			_buttonRegionColor11.Background = new SolidColorBrush(Settings.Color11.ToWColor());
			_buttonRegionColor12.Background = new SolidColorBrush(Settings.Color12.ToWColor());
			_buttonRegionColor13.Background = new SolidColorBrush(Settings.Color13.ToWColor());
			_buttonRegionColor14.Background = new SolidColorBrush(Settings.Color14.ToWColor());

			_manualSolver = Settings.MainManualSolver;

			_gridMain.ColumnDefinitions[0].Width = new GridLength(Settings.GridSize);

			_comboBoxSymmetry.SelectedIndex = Settings.GeneratingSymmetryModeComboBoxSelectedIndex;
			_comboBoxMode.SelectedIndex = Settings.GeneratingModeComboBoxSelectedIndex;
			_comboBoxDifficulty.SelectedIndex = Settings.GeneratingDifficultyLevelSelectedIndex;
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
					_comboBoxDifficulty.IsEnabled = false;
					break;
				}
				case 1:
				{
					_comboBoxSymmetry.IsEnabled = false;
					_comboBoxBackdoorFilteringDepth.IsEnabled = true;
					_comboBoxDifficulty.IsEnabled = true;
					break;
				}
			}
		}

		/// <summary>
		/// Initializes point converter and layer instances.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void InitializePointConverterAndLayers() =>
			_currentPainter =
				new GridPainter(
					_pointConverter = new PointConverter((float)_imageGrid.Width, (float)_imageGrid.Height),
					Settings)
				{
					Grid = _puzzle
				};

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
				Messagings.FailedToLoadPuzzle();
			}
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
						$@"Resources/ImageIcon-Undo{
							((_menuItemEditUndo.IsEnabled = _puzzle.HasUndoSteps) ? string.Empty : "Disable")
						}.png",
						UriKind.Relative));
			_imageRedoIcon.Source =
				new BitmapImage(
					new Uri(
						$@"Resources/ImageIcon-Redo{
							((_menuItemEditRedo.IsEnabled = _puzzle.HasRedoSteps) ? string.Empty : "Disable")
						}.png",
						UriKind.Relative));
		}

		/// <summary>
		/// Disable generating controls.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DisableGeneratingControls()
		{
			_cacheAllSteps = null;

			_textBoxInfo.Text = (string)LangSource["WhileGenerating"];
			_menuItemFileOpen.IsEnabled = false;
			_menuItemFileOpenDatabase.IsEnabled = false;
			_menuItemFileSave.IsEnabled = false;
			_menuItemFileSavePicture.IsEnabled = false;
			_menuItemFileSaveBatch.IsEnabled = false;
			_menuItemOptionsSettings.IsEnabled = false;
			_menuItemGenerateHardPattern.IsEnabled = false;
			_menuItemEditPaste.IsEnabled = false;
			_menuItemEditFix.IsEnabled = false;
			_menuItemEditUnfix.IsEnabled = false;
			_menuItemEditReset.IsEnabled = false;
			_menuItemEditClear.IsEnabled = false;
			_menuItemClearStack.IsEnabled = false;
			_menuItemGenerateWithSymmetry.IsEnabled = false;
			_menuItemAnalyzeAnalyze.IsEnabled = false;
			_menuItemAnalyzeSolve.IsEnabled = false;
			_menuItemAnalyzeExport.IsEnabled = false;
			_menuItemTransformMirrorAntidiagonal.IsEnabled = false;
			_menuItemTransformMirrorDiagonal.IsEnabled = false;
			_menuItemTransformMirrorLeftRight.IsEnabled = false;
			_menuItemTransformMirrorTopBotton.IsEnabled = false;
			_menuItemTransformRotateClockwise.IsEnabled = false;
			_menuItemTransformRotateCounterclockwise.IsEnabled = false;
			_menuItemTransformRotatePi.IsEnabled = false;
			_buttonFindAllSteps.IsEnabled = false;
			_imageGeneratingIcon.IsEnabled = false;
			_imageSolve.IsEnabled = false;
			_comboBoxSymmetry.IsEnabled = false;
			_comboBoxMode.IsEnabled = false;
			_comboBoxBackdoorFilteringDepth.IsEnabled = false;
			_comboBoxDifficulty.IsEnabled = false;
			_textBoxPathFilter.IsEnabled = false;

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
			_menuItemFileSave.IsEnabled = true;
			_menuItemFileSavePicture.IsEnabled = true;
			_menuItemFileSaveBatch.IsEnabled = true;
			_menuItemOptionsSettings.IsEnabled = true;
			_menuItemGenerateHardPattern.IsEnabled = true;
			_menuItemEditPaste.IsEnabled = true;
			_menuItemEditFix.IsEnabled = true;
			_menuItemEditUnfix.IsEnabled = true;
			_menuItemEditReset.IsEnabled = true;
			_menuItemEditClear.IsEnabled = true;
			_menuItemClearStack.IsEnabled = true;
			_menuItemGenerateWithSymmetry.IsEnabled = true;
			_menuItemAnalyzeSolve.IsEnabled = true;
			_menuItemAnalyzeAnalyze.IsEnabled = true;
			_menuItemAnalyzeExport.IsEnabled = true;
			_menuItemTransformMirrorAntidiagonal.IsEnabled = true;
			_menuItemTransformMirrorDiagonal.IsEnabled = true;
			_menuItemTransformMirrorLeftRight.IsEnabled = true;
			_menuItemTransformMirrorTopBotton.IsEnabled = true;
			_menuItemTransformRotateClockwise.IsEnabled = true;
			_menuItemTransformRotateCounterclockwise.IsEnabled = true;
			_menuItemTransformRotatePi.IsEnabled = true;
			_buttonFindAllSteps.IsEnabled = true;
			_imageGeneratingIcon.IsEnabled = true;
			_imageSolve.IsEnabled = true;
			_comboBoxMode.IsEnabled = true;
			_comboBoxSymmetry.IsEnabled = true;
			_comboBoxBackdoorFilteringDepth.IsEnabled = true;
			_comboBoxDifficulty.IsEnabled = true;
			_textBoxPathFilter.IsEnabled = true;

			UpdateUndoRedoControls();
		}

		/// <summary>
		/// Disable solving controls.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DisableSolvingControls()
		{
			_cacheAllSteps = null;

			_menuItemFileOpen.IsEnabled = false;
			_menuItemFileOpenDatabase.IsEnabled = false;
			_menuItemFileSave.IsEnabled = false;
			_menuItemFileSavePicture.IsEnabled = false;
			_menuItemFileSaveBatch.IsEnabled = false;
			_menuItemOptionsSettings.IsEnabled = false;
			_menuItemGenerateHardPattern.IsEnabled = false;
			_menuItemEditPaste.IsEnabled = false;
			_menuItemEditFix.IsEnabled = false;
			_menuItemEditUnfix.IsEnabled = false;
			_menuItemEditReset.IsEnabled = false;
			_menuItemEditClear.IsEnabled = false;
			_menuItemClearStack.IsEnabled = false;
			_menuItemGenerateWithSymmetry.IsEnabled = false;
			_menuItemAnalyzeSolve.IsEnabled = false;
			_menuItemAnalyzeAnalyze.IsEnabled = false;
			_menuItemAnalyzeShowFullHouses.IsEnabled = false;
			_menuItemAnalyzeShowLastDigits.IsEnabled = false;
			_menuItemAnalyzeSeMode.IsEnabled = false;
			_menuItemAnalyzeFastSearch.IsEnabled = false;
			_menuItemAnalyzeCheckConclusionValidityAfterSearched.IsEnabled = false;
			_menuItemAnalyzeCheckGurthSymmetricalPlacement.IsEnabled = false;
			_menuItemAnalyzeOptimizeApplyingOrder.IsEnabled = false;
			_menuItemAnalyzeUseCalculationPriority.IsEnabled = false;
			_menuItemAnalyzeExport.IsEnabled = false;
			_menuItemTransformMirrorAntidiagonal.IsEnabled = false;
			_menuItemTransformMirrorDiagonal.IsEnabled = false;
			_menuItemTransformMirrorLeftRight.IsEnabled = false;
			_menuItemTransformMirrorTopBotton.IsEnabled = false;
			_menuItemTransformRotateClockwise.IsEnabled = false;
			_menuItemTransformRotateCounterclockwise.IsEnabled = false;
			_menuItemTransformRotatePi.IsEnabled = false;
			_buttonFindAllSteps.IsEnabled = false;
			_imageGeneratingIcon.IsEnabled = false;
			_imageSolve.IsEnabled = false;
			_comboBoxSymmetry.IsEnabled = false;
			_comboBoxMode.IsEnabled = false;
			_comboBoxBackdoorFilteringDepth.IsEnabled = false;
			_comboBoxDifficulty.IsEnabled = false;
			_textBoxPathFilter.IsEnabled = false;

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
			_menuItemFileSave.IsEnabled = true;
			_menuItemFileSavePicture.IsEnabled = true;
			_menuItemFileSaveBatch.IsEnabled = true;
			_menuItemOptionsSettings.IsEnabled = true;
			_menuItemGenerateHardPattern.IsEnabled = true;
			_menuItemEditPaste.IsEnabled = true;
			_menuItemEditFix.IsEnabled = true;
			_menuItemEditUnfix.IsEnabled = true;
			_menuItemEditReset.IsEnabled = true;
			_menuItemEditClear.IsEnabled = true;
			_menuItemClearStack.IsEnabled = true;
			_menuItemGenerateWithSymmetry.IsEnabled = true;
			_menuItemAnalyzeSolve.IsEnabled = true;
			_menuItemAnalyzeAnalyze.IsEnabled = true;
			_menuItemAnalyzeShowFullHouses.IsEnabled = true;
			_menuItemAnalyzeShowLastDigits.IsEnabled = true;
			_menuItemAnalyzeSeMode.IsEnabled = true;
			_menuItemAnalyzeFastSearch.IsEnabled = true;
			_menuItemAnalyzeCheckConclusionValidityAfterSearched.IsEnabled = true;
			_menuItemAnalyzeCheckGurthSymmetricalPlacement.IsEnabled = true;
			_menuItemAnalyzeOptimizeApplyingOrder.IsEnabled = true;
			_menuItemAnalyzeUseCalculationPriority.IsEnabled = true;
			_menuItemAnalyzeExport.IsEnabled = true;
			_menuItemTransformMirrorAntidiagonal.IsEnabled = true;
			_menuItemTransformMirrorDiagonal.IsEnabled = true;
			_menuItemTransformMirrorLeftRight.IsEnabled = true;
			_menuItemTransformMirrorTopBotton.IsEnabled = true;
			_menuItemTransformRotateClockwise.IsEnabled = true;
			_menuItemTransformRotateCounterclockwise.IsEnabled = true;
			_menuItemTransformRotatePi.IsEnabled = true;
			_buttonFindAllSteps.IsEnabled = true;
			_imageGeneratingIcon.IsEnabled = true;
			_imageSolve.IsEnabled = true;
			_comboBoxMode.IsEnabled = true;
			_comboBoxSymmetry.IsEnabled = true;
			_comboBoxBackdoorFilteringDepth.IsEnabled = true;
			_comboBoxDifficulty.IsEnabled = true;
			_textBoxPathFilter.IsEnabled = true;

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

		/// <summary>
		/// Transform the grid.
		/// </summary>
		/// <param name="transformation">The inner function to process the transformation.</param>
		private void Transform(Func<IReadOnlyGrid, UndoableGrid> transformation)
		{
			if (_puzzle != (SudokuGrid)SudokuGrid.Empty/* && Messagings.AskWhileClearingStack() == MessageBoxResult.Yes*/)
			{
				Puzzle = transformation(_puzzle);

				UpdateUndoRedoControls();
				UpdateImageGrid();
			}
		}

		/// <summary>
		/// Change the language.
		/// </summary>
		/// <param name="globalizationString">The globalization string.</param>
		private void ChangeLanguage(string globalizationString)
		{
			Settings.LanguageCode = globalizationString;

			// Get all possible resource dictionaries.
			var dictionaries = new List<ResourceDictionary>();
			var mergedDic = LangSource.MergedDictionaries;
			foreach (var dictionary in mergedDic)
			{
				dictionaries.Add(dictionary);
			}

			// Get the specified dictionary.
			ResourceDictionary? g(string p) => dictionaries.FirstOrDefault(d => d.Source.OriginalString == p);
			if (!((g($"Lang.{globalizationString}.xaml") ?? g("Lang.en-us.xaml")) is ResourceDictionary resourceDictionary))
			{
				Messagings.FailedToLoadGlobalizationFile();
				return;
			}

			mergedDic.Remove(resourceDictionary);
			mergedDic.Add(resourceDictionary);

			// Then change the language of the library 'Sudoku.Core'.
			CoreResources.ChangeLanguage(globalizationString);
		}
	}
}
