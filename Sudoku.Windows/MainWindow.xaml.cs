using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Extensions;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Converters;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Globalization;
using Sudoku.Solving.Manual;
using Sudoku.Windows.Extensions;
using static Sudoku.Windows.Constants;
using CoreResources = Sudoku.Windows.Resources;
using StepTriplet = System.KeyedTuple<string, int, Sudoku.Solving.Manual.StepInfo>;
#if SUDOKU_RECOGNITION
using System.Diagnostics;
#endif
#if OBSOLETE
using System.Runtime.CompilerServices;
#endif

namespace Sudoku.Windows
{
	/// <summary>
	/// Interaction logic for <c>MainWindow.xaml</c>.
	/// </summary>
	public partial class MainWindow : Window
	{
		#region Constructor
		/// <inheritdoc cref="DefaultConstructor"/>
		public MainWindow() => InitializeComponent();
		#endregion


		#region Overriden methods
		/// <inheritdoc/>
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e); // Call the base method.

			LoadConfigIfWorth();
			CheckResourceDictionaryFilesExistence();
			ChangeLanguage(Settings.LanguageCode);
			PreventYouOpeningTwoSameWindows();

#if SUDOKU_RECOGNITION
			InitializeRecognizerIfWorth();
#endif

			DefineShortCuts();
			InitializeGridPainter();
			LoadDatabaseIfWorth();
			UpdateControls();
		}

		/// <inheritdoc/>
		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);

			if (sizeInfo.NewSize != sizeInfo.PreviousSize)
			{
				double w = _gridMain.ColumnDefinitions[0].ActualWidth;
				double h = _gridMain.RowDefinitions[0].ActualHeight;
				_imageGrid.Height = _imageGrid.Width = Math.Min(w, h);
				Settings.GridSize = w;
				_currentPainter = new(new(_imageGrid.RenderSize.ToDSizeF()), Settings, _puzzle);

				UpdateImageGrid();
			}
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

#if DEBUG && false
			// Serialize the resource dictionaries.
			CoreResources.Serialize(nameof(CoreResources.LangSourceEnUs), Paths.LangSourceEnUs);
			CoreResources.Serialize(nameof(CoreResources.LangSourceZhCn), Paths.LangSourceZhCn);
#endif

			// Save the configuration file.
			SaveConfig();

#if SUDOKU_RECOGNITION
			// Dispose the OCR tools.
			_recognition?.Dispose();
#endif

			// Call base.OnClosing to close the window (collect the garbage memory).
			base.OnClosing(e);

#if SUDOKU_RECOGNITION
			// If you don't use this feature, the program won't need to use
			// this method to KILL itself... KILL... sounds terrible and dangerous, isn't it?
			// To be honest, I don't know why the program fails to exit... The background
			// threads still running after base close method executed completely. If you
			// know the detail of Emgu.CV, please tell me, thx!
			if (_recognition is { IsInitialized: true })
			{
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
				case var key
				when key.IsDigit()
				&& _pointConverter.GetCell(Mouse.GetPosition(_imageGrid).ToDPointF()) is var cell and not -1
				&& _puzzle.GetStatus(cell) != CellStatus.Given:
				{
					int digit = e.Key.IsDigitUpsideAlphabets() ? e.Key - Key.D1 : e.Key - Key.NumPad1;
					switch (Keyboard.Modifiers)
					{
						// Input a digit.
						case ModifierKeys.None when digit == -1 || !_puzzle.Duplicate(cell, digit):
						{
							_puzzle[cell] = digit;
							if (digit != -1 && _puzzle.GetStatus(cell) == CellStatus.Modifiable)
							{
								// This cell can be modified with other digits.
								_puzzle.RecomputeCandidates();
							}

							break;
						}
						// Eliminate a digit.
						case ModifierKeys.Shift:
						{
							_puzzle[cell, digit] = true;

							break;
						}
					}

					UpdateUndoRedoControls();
					UpdateImageGrid();

					break;
				}
				case var key and (Key.OemMinus or Key.OemPlus)
				when _currentViewIndex != -1 && _currentStepInfo is not null:
				{
					// Get the previous view or the next view.
					var views = _currentStepInfo.Views;
					int totalViewsCount = views.Count;
					ref int i = ref _currentViewIndex;
					i = Math.Abs(
					(
						key == Key.OemMinus ? i - 1 is var j && j < 0 ? j + totalViewsCount : j : i + 1
					) % totalViewsCount);

					_currentPainter.View = views[i];

					UpdateImageGrid();

					break;
				}
				case var key when key.IsArrow() && _focusedCells.Count == 1:
				{
					// Move the focused cell.
					int cell = _focusedCells[0];
					_focusedCells.Clear();
					_focusedCells.AddAnyway(e.Key switch
					{
						Key.Up => cell - 9 < 0 ? cell + 72 : cell - 9,
						Key.Down => cell + 9 >= 81 ? cell - 72 : cell + 9,
						Key.Left => cell - 1 < 0 ? cell + 8 : cell - 1,
						Key.Right => (cell + 1) % 81
					});

					_currentPainter.FocusedCells = _focusedCells;

					UpdateImageGrid();

					break;
				}
				case Key.Space:
				{
					// View the intersection.
					_previewMap = _focusedCells;
					_focusedCells = _focusedCells.PeerIntersection;

					_currentPainter.FocusedCells = _focusedCells;

					UpdateImageGrid();

					break;
				}
				case Key.Tab:
				{
					// Move to next box row.
					int cell = _focusedCells.IsEmpty ? 0 : _focusedCells[0];
					_focusedCells.Clear();
					_focusedCells.AddAnyway((cell + 3) % 81);

					_currentPainter.FocusedCells = _focusedCells;

					UpdateImageGrid();

					break;
				}
				case Key.Escape:
				{
					// Clear focused cells.
					ClearViews();

					UpdateImageGrid();

					_textBoxInfo.ClearValue(TextBox.TextProperty);

					break;
				}
			}
		}

		/// <inheritdoc/>
		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);

			if (_previewMap.HasValue && e.Key == Key.Space)
			{
				_focusedCells = _previewMap.Value;

				_currentPainter.FocusedCells = _focusedCells;

				UpdateImageGrid();
			}
		}
		#endregion


		#region Other instance methods
#if SUDOKU_RECOGNITION
		/// <summary>
		/// Initialize recognizer if worth.
		/// </summary>
		private void InitializeRecognizerIfWorth()
		{
			try
			{
				_recognition = new();
			}
#if !MUST_DOWNLOAD_TRAINED_DATA
			catch (FileNotFoundException ex) when (ex.FileName?.EndsWith(Paths.TrainedDataFileName) ?? false)
			{
				// Trained data file can't be found.
				Messagings.FailedToLoadRecognitionTool(ex);
			}
#endif
			catch (Exception ex)
			{
				// Other exceptions.
				Messagings.FailedToLoadRecognitionTool(ex);
			}
		}
#endif

		/// <summary>
		/// Add short cuts during initialization.
		/// </summary>
		private void DefineShortCuts()
		{
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
		}

		/// <summary>
		/// Clear the current views (<see cref="View"/> and <see cref="MutableView"/>).
		/// </summary>
		/// <seealso cref="View"/>
		/// <seealso cref="MutableView"/>
		private void ClearViews()
		{
			_focusedCells.Clear();

			_currentPainter = new(_currentPainter.Converter, _currentPainter.Preferences, _puzzle);
			_currentViewIndex = -1;
			_currentStepInfo = null;
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
			_puzzlesText = sr.ReadToEnd().SplitByNewLine();

			int current = Settings.CurrentPuzzleNumber;
			int max = _puzzlesText.Length;
			LoadPuzzle(_puzzlesText[current].TrimEndNewLine());
			_labelPuzzleNumber.Content = $"{current + 1}/{max}";
			_textBoxJumpTo.IsEnabled = true;
			UpdateDatabaseControls(current != 0, current != 0, current != max - 1, current != max - 1);
		}

		/// <summary>
		/// Repaint the <see cref="_imageGrid"/> to show the newer grid image.
		/// </summary>
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
		private void UpdateDatabaseControls(bool first, bool prev, bool next, bool last)
		{
			_buttonFirst.IsEnabled = first;
			_buttonPrev.IsEnabled = prev;
			_buttonNext.IsEnabled = next;
			_buttonLast.IsEnabled = last;
		}

		/// <summary>
		/// Check the existence of language resource dictionary files.
		/// </summary>
		private void CheckResourceDictionaryFilesExistence(string folderPath = "lang")
		{
			// Check whether the folder exists.
			const string pathDoesNotContainLanguageFiles = "The language resource dictionary file doesn't exist.";
			if (!Directory.Exists(folderPath))
			{
				MessageBox.Show(
					messageBoxText: pathDoesNotContainLanguageFiles,
					caption: "Error",
					button: MessageBoxButton.OK,
					icon: MessageBoxImage.Error);

				Environment.FailFast(pathDoesNotContainLanguageFiles);
			}

			// Check whether the resource dictionary files exist.
			const string defaultLanguageFileName = "Resources.en-us.dic";
			const string defaultLanguageFileIsRequired = "The file named '" + defaultLanguageFileName + "' is required.";
			if (!File.Exists($@"{folderPath}\{defaultLanguageFileName}"))
			{
				MessageBox.Show(
					messageBoxText: defaultLanguageFileIsRequired,
					caption: "Error",
					button: MessageBoxButton.OK,
					icon: MessageBoxImage.Error);

				Environment.FailFast(defaultLanguageFileIsRequired);
			}

			// Check whether the resource dictionary files are vaild.
			const string languageFileIsInvalid = "The required resource dictionary file is invalid.";
			if (CoreResources.LangSourceEnUs is null)
			{
				MessageBox.Show(
					messageBoxText: languageFileIsInvalid,
					caption: "Error",
					button: MessageBoxButton.OK,
					icon: MessageBoxImage.Error);

				Environment.FailFast(languageFileIsInvalid);
			}
		}

		/// <summary>
		/// Save configurations if worth.
		/// </summary>
		/// <param name="path">
		/// The path of the configuration file. The default value is <c>"configurations.json"</c>.
		/// </param>
		private void LoadConfigIfWorth(string path = "configurations.json")
		{
			Settings = new();
			if (File.Exists(path))
			{
				try
				{
					string s = File.ReadAllText(path);
					if (
						JsonSerializer.Deserialize<WindowsSettings>(s, _serializerOptions)
						is var settingsResult and not null)
					{
						Settings = settingsResult;
					}
					else
					{
						Settings.CoverBy(WindowsSettings.DefaultSetting);
					}
				}
				catch
				{
					Messagings.FailedToLoadSettings();
				}
			}
			else
			{
				Settings.CoverBy(WindowsSettings.DefaultSetting);
			}
		}

		/// <summary>
		/// Save configurations.
		/// </summary>
		/// <param name="path">The path to save. The default value is <c>"configurations.json"</c>.</param>
		private void SaveConfig(string path = "configurations.json")
		{
			try
			{
				File.WriteAllText(path, JsonSerializer.Serialize(Settings, _serializerOptions));
			}
			catch (Exception ex)
			{
				Messagings.FailedToSaveConfig(ex);
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
		private void AddShortCut(
			Key key, ModifierKeys modifierKeys, UIElement? matchControl, ExecutedRoutedEventHandler executed)
		{
			var command = new RoutedCommand();
			command.InputGestures.Add(new KeyGesture(key, modifierKeys));
			CommandBindings.Add(
				new(
					command,
					(sender, e) => ((matchControl?.IsEnabled ?? true) ? executed : null)?.Invoke(sender, e)));
		}

		/// <summary>
		/// The internal copy method to process the operation of copying value to clipboard.
		/// </summary>
		/// <param name="format">The grid format.</param>
		private void InternalCopy(string format)
		{
			try
			{
#if OBSOLETE
				// This may throw exceptions being called while solving and generating puzzles.
				SystemClipboard.Text = _puzzle.ToString(format);
#else
				SystemClipboard.DataObject = _puzzle.ToString(format);
#endif
			}
			catch (ArgumentNullException ex)
			{
				Messagings.FailedToSaveToClipboardDueToArgumentNullException(ex);
			}
#if OBSOLETE
			catch (COMException ex) when (ex.HResult == unchecked((int)2147746256))
			{
				Messagings.FailedToSaveToClipboardDueToAsyncCalling();
			}
#endif
		}

		/// <summary>
		/// To update the control status.
		/// </summary>
		private void UpdateControls()
		{
#if !SUDOKU_RECOGNITION
			_menuItemFileLoadPicture.IsEnabled = false;
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

			_gridMain.ColumnDefinitions[0].Width = new(Settings.GridSize);

			_comboBoxSymmetry.SelectedIndex = Settings.GeneratingSymmetryModeComboBoxSelectedIndex;
			_comboBoxMode.SelectedIndex = Settings.GeneratingModeComboBoxSelectedIndex;
			_comboBoxDifficulty.SelectedIndex = Settings.GeneratingDifficultyLevelSelectedIndex;
			SwitchOnGeneratingComboBoxesDisplaying();

			UpdateImageGrid();
		}

		/// <summary>
		/// Switch on displaying view of generating combo boxes.
		/// </summary>
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
		/// Initializes grid painter.
		/// </summary>
		private void InitializeGridPainter()
		{
			var (w, h) = _imageGrid;
			_currentPainter = new(_pointConverter = new((float)w, (float)h), Settings, _puzzle);
		}

		/// <summary>
		/// To load a puzzle with a specified possible puzzle string.
		/// </summary>
		/// <param name="puzzleStr">The puzzle string.</param>
		private void LoadPuzzle(string puzzleStr)
		{
			try
			{
				Puzzle = SudokuGrid.Parse(puzzleStr, Settings.PmGridCompatible);

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
		private void UpdateUndoRedoControls()
		{
			_menuItemEditUndo.IsEnabled = _puzzle.HasUndoSteps;
			_menuItemEditRedo.IsEnabled = _puzzle.HasRedoSteps;
			_imageUndoIcon.Source =
				new BitmapImage(
					new(
						$@"Resources/ImageIcon-Undo{(_puzzle.HasUndoSteps ? string.Empty : "Disable")}.png",
						UriKind.Relative));
			_imageRedoIcon.Source =
				new BitmapImage(
					new(
						$@"Resources/ImageIcon-Redo{(_puzzle.HasRedoSteps ? string.Empty : "Disable")}.png",
						UriKind.Relative));
		}

		/// <summary>
		/// Disable generating controls.
		/// </summary>
		private void DisableGeneratingControls()
		{
			_analyisResult = null;

			_listBoxTechniques.ClearValue(ItemsControl.ItemsSourceProperty);
			_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);

			_textBoxInfo.Text = (string)LangSource["WhileGenerating"];
			_menuItemFileOpen.IsEnabled = false;
			_menuItemFileOpenDatabase.IsEnabled = false;
			_menuItemFileSave.IsEnabled = false;
			_menuItemFileSavePicture.IsEnabled = false;
			_menuItemFileSaveBatch.IsEnabled = false;
			_menuItemOptionsSettings.IsEnabled = false;
			_menuItemEditPaste.IsEnabled = false;
			_menuItemEditPasteAsSukaku.IsEnabled = false;
			_menuItemEditFix.IsEnabled = false;
			_menuItemEditUnfix.IsEnabled = false;
			_menuItemEditReset.IsEnabled = false;
			_menuItemEditClear.IsEnabled = false;
			_menuItemEditRecomputeCandidates.IsEnabled = false;
			_menuItemEditClearStack.IsEnabled = false;
			_menuItemGenerateWithSymmetry.IsEnabled = false;
			_menuItemGenerateHardPattern.IsEnabled = false;
			_menuItemGenerateWithTechniqueFiltering.IsEnabled = false;
			_menuItemAnalyzeAnalyze.IsEnabled = false;
			_menuItemAnalyzeSolve.IsEnabled = false;
			_menuItemAnalyzeExport.IsEnabled = false;
			_menuItemViewsBugNView.IsEnabled = false;
			_menuItemViewsBackdoorView.IsEnabled = false;
			_menuItemViewsGspView.IsEnabled = false;
			_menuItemTransformMirrorAntidiagonal.IsEnabled = false;
			_menuItemTransformMirrorDiagonal.IsEnabled = false;
			_menuItemTransformMirrorLeftRight.IsEnabled = false;
			_menuItemTransformMirrorTopBotton.IsEnabled = false;
			_menuItemTransformRotateClockwise.IsEnabled = false;
			_menuItemTransformRotateCounterclockwise.IsEnabled = false;
			_menuItemTransformRotatePi.IsEnabled = false;
			_imageGeneratingIcon.IsEnabled = false;
			_imageSolve.IsEnabled = false;
			_comboBoxSymmetry.IsEnabled = false;
			_comboBoxMode.IsEnabled = false;
			_comboBoxBackdoorFilteringDepth.IsEnabled = false;
			_comboBoxDifficulty.IsEnabled = false;
			_buttonFindAllSteps.IsEnabled = false;

			_imageGrid.Visibility = Visibility.Hidden;

			UpdateDatabaseControls(false, false, false, false);
			_textBoxJumpTo.IsEnabled = false;
			_labelPuzzleNumber.ClearValue(ContentProperty);

			UpdateUndoRedoControls();
		}

		/// <summary>
		/// Enable generating controls.
		/// </summary>
		private void EnableGeneratingControls()
		{
			_textBoxInfo.ClearValue(TextBox.TextProperty);
			_menuItemFileOpen.IsEnabled = true;
			_menuItemFileOpenDatabase.IsEnabled = true;
			_menuItemFileSave.IsEnabled = true;
			_menuItemFileSavePicture.IsEnabled = true;
			_menuItemFileSaveBatch.IsEnabled = true;
			_menuItemOptionsSettings.IsEnabled = true;
			_menuItemEditPaste.IsEnabled = true;
			_menuItemEditPasteAsSukaku.IsEnabled = true;
			_menuItemEditFix.IsEnabled = true;
			_menuItemEditUnfix.IsEnabled = true;
			_menuItemEditReset.IsEnabled = true;
			_menuItemEditClear.IsEnabled = true;
			_menuItemEditClearStack.IsEnabled = true;
			_menuItemEditRecomputeCandidates.IsEnabled = true;
			_menuItemGenerateWithSymmetry.IsEnabled = true;
			_menuItemGenerateHardPattern.IsEnabled = true;
			_menuItemGenerateWithTechniqueFiltering.IsEnabled = true;
			_menuItemAnalyzeSolve.IsEnabled = true;
			_menuItemAnalyzeAnalyze.IsEnabled = true;
			_menuItemAnalyzeExport.IsEnabled = true;
			_menuItemViewsBugNView.IsEnabled = true;
			_menuItemViewsBackdoorView.IsEnabled = true;
			_menuItemViewsGspView.IsEnabled = true;
			_menuItemTransformMirrorAntidiagonal.IsEnabled = true;
			_menuItemTransformMirrorDiagonal.IsEnabled = true;
			_menuItemTransformMirrorLeftRight.IsEnabled = true;
			_menuItemTransformMirrorTopBotton.IsEnabled = true;
			_menuItemTransformRotateClockwise.IsEnabled = true;
			_menuItemTransformRotateCounterclockwise.IsEnabled = true;
			_menuItemTransformRotatePi.IsEnabled = true;
			_imageGeneratingIcon.IsEnabled = true;
			_imageSolve.IsEnabled = true;
			_comboBoxMode.IsEnabled = true;
			_comboBoxSymmetry.IsEnabled = true;
			_comboBoxBackdoorFilteringDepth.IsEnabled = true;
			_comboBoxDifficulty.IsEnabled = true;
			_buttonFindAllSteps.IsEnabled = true;

			_imageGrid.Visibility = Visibility.Visible;

			UpdateUndoRedoControls();
		}

		/// <summary>
		/// Disable solving controls.
		/// </summary>
		private void DisableSolvingControls()
		{
			_listBoxTechniques.ClearValue(ItemsControl.ItemsSourceProperty);
			_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);

			_menuItemFileOpen.IsEnabled = false;
			_menuItemFileOpenDatabase.IsEnabled = false;
			_menuItemFileSave.IsEnabled = false;
			_menuItemFileSavePicture.IsEnabled = false;
			_menuItemFileSaveBatch.IsEnabled = false;
			_menuItemOptionsSettings.IsEnabled = false;
			_menuItemEditPaste.IsEnabled = false;
			_menuItemEditPasteAsSukaku.IsEnabled = false;
			_menuItemEditFix.IsEnabled = false;
			_menuItemEditUnfix.IsEnabled = false;
			_menuItemEditReset.IsEnabled = false;
			_menuItemEditClear.IsEnabled = false;
			_menuItemEditClearStack.IsEnabled = false;
			_menuItemEditRecomputeCandidates.IsEnabled = false;
			_menuItemGenerateWithSymmetry.IsEnabled = false;
			_menuItemGenerateHardPattern.IsEnabled = false;
			_menuItemGenerateWithTechniqueFiltering.IsEnabled = false;
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
			_menuItemViewsBugNView.IsEnabled = false;
			_menuItemViewsBackdoorView.IsEnabled = false;
			_menuItemViewsGspView.IsEnabled = false;
			_menuItemTransformMirrorAntidiagonal.IsEnabled = false;
			_menuItemTransformMirrorDiagonal.IsEnabled = false;
			_menuItemTransformMirrorLeftRight.IsEnabled = false;
			_menuItemTransformMirrorTopBotton.IsEnabled = false;
			_menuItemTransformRotateClockwise.IsEnabled = false;
			_menuItemTransformRotateCounterclockwise.IsEnabled = false;
			_menuItemTransformRotatePi.IsEnabled = false;
			_imageGeneratingIcon.IsEnabled = false;
			_imageSolve.IsEnabled = false;
			_comboBoxSymmetry.IsEnabled = false;
			_comboBoxMode.IsEnabled = false;
			_comboBoxBackdoorFilteringDepth.IsEnabled = false;
			_comboBoxDifficulty.IsEnabled = false;
			_buttonFindAllSteps.IsEnabled = false;

			_imageGrid.Visibility = Visibility.Hidden;

			UpdateUndoRedoControls();
		}

		/// <summary>
		/// Enable solving controls.
		/// </summary>
		private void EnableSolvingControls()
		{
			_textBoxInfo.ClearValue(TextBox.TextProperty);
			_menuItemFileOpen.IsEnabled = true;
			_menuItemFileOpenDatabase.IsEnabled = true;
			_menuItemFileSave.IsEnabled = true;
			_menuItemFileSavePicture.IsEnabled = true;
			_menuItemFileSaveBatch.IsEnabled = true;
			_menuItemOptionsSettings.IsEnabled = true;
			_menuItemEditPaste.IsEnabled = true;
			_menuItemEditPasteAsSukaku.IsEnabled = true;
			_menuItemEditFix.IsEnabled = true;
			_menuItemEditUnfix.IsEnabled = true;
			_menuItemEditReset.IsEnabled = true;
			_menuItemEditClear.IsEnabled = true;
			_menuItemEditClearStack.IsEnabled = true;
			_menuItemEditRecomputeCandidates.IsEnabled = true;
			_menuItemGenerateWithSymmetry.IsEnabled = true;
			_menuItemGenerateHardPattern.IsEnabled = true;
			_menuItemGenerateWithTechniqueFiltering.IsEnabled = true;
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
			_menuItemViewsBugNView.IsEnabled = true;
			_menuItemViewsBackdoorView.IsEnabled = true;
			_menuItemViewsGspView.IsEnabled = true;
			_menuItemTransformMirrorAntidiagonal.IsEnabled = true;
			_menuItemTransformMirrorDiagonal.IsEnabled = true;
			_menuItemTransformMirrorLeftRight.IsEnabled = true;
			_menuItemTransformMirrorTopBotton.IsEnabled = true;
			_menuItemTransformRotateClockwise.IsEnabled = true;
			_menuItemTransformRotateCounterclockwise.IsEnabled = true;
			_menuItemTransformRotatePi.IsEnabled = true;
			_imageGeneratingIcon.IsEnabled = true;
			_imageSolve.IsEnabled = true;
			_comboBoxMode.IsEnabled = true;
			_comboBoxSymmetry.IsEnabled = true;
			_comboBoxBackdoorFilteringDepth.IsEnabled = true;
			_comboBoxDifficulty.IsEnabled = true;
			_buttonFindAllSteps.IsEnabled = true;

			_imageGrid.Visibility = Visibility.Visible;

			UpdateUndoRedoControls();
		}

		/// <summary>
		/// Clear item sources when generated.
		/// </summary>
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
		private void SwitchOnTabItemWhenGeneratedOrSolving() => _tabControlInfo.SelectedIndex = 0;

		/// <summary>
		/// Set a digit.
		/// </summary>
		private void SetADigit(int cell, int digit)
		{
			_puzzle[cell] = digit;

			UpdateUndoRedoControls();
			UpdateImageGrid();
		}

		/// <summary>
		/// Delete a digit.
		/// </summary>
		private void DeleteADigit(int cell, int digit)
		{
			_puzzle[cell, digit] = true;

			UpdateUndoRedoControls();
			UpdateImageGrid();
		}

		/// <summary>
		/// Transform the grid.
		/// </summary>
		/// <param name="transformation">The inner function to process the transformation.</param>
		private unsafe void Transform(delegate*<in SudokuGrid, SudokuGrid> transformation)
		{
			if (_puzzle != SudokuGrid.Empty/* && Messagings.AskWhileClearingStack() == MessageBoxResult.Yes*/)
			{
				Puzzle = transformation((SudokuGrid)_puzzle);

				UpdateUndoRedoControls();
				UpdateImageGrid();
			}
		}

		/// <summary>
		/// Change the language.
		/// </summary>
		/// <param name="countryCode">The country code.</param>
		private void ChangeLanguage(CountryCode countryCode)
		{
			if (countryCode == CountryCode.Default)
			{
				return;
			}

			Settings.LanguageCode = countryCode;

			// Get all possible resource dictionaries.
			var dictionaries = new List<ResourceDictionary>();
			var mergedDic = LangSource.MergedDictionaries;
			foreach (var dictionary in mergedDic)
			{
				dictionaries.Add(dictionary);
			}

			// Get the specified dictionary.
			ResourceDictionary? g(string p) => dictionaries.Find(d => d.Source.OriginalString == p);
			if ((g($"Lang{countryCode}.xaml") ?? g("LangEnUs.xaml")) is not ResourceDictionary rd)
			{
				Messagings.FailedToLoadGlobalizationFile();
				return;
			}

			mergedDic.Remove(rd);
			mergedDic.Add(rd);

			// Then change the language of the library 'Sudoku.Core'.
			CoreResources.ChangeLanguage(countryCode);
		}

		/// <summary>
		/// Display difficulty information after analyzed a puzzle.
		/// </summary>
		private void DisplayDifficultyInfoAfterAnalyzed()
		{
			if (_tabControlInfo is not { ActualWidth: > 50, ActualHeight: > 50 } || _analyisResult is null)
			{
				return;
			}

			if (_analyisResult.IsSolved)
			{
				_textBoxInfo.Text =
					$"{_analyisResult.SolvingStepsCount} " +
					$@"{(LangSource[_analyisResult.SolvingStepsCount == 1 ? "StepSingular" : "StepPlural"])}" +
					$"{(Settings.LanguageCode == CountryCode.EnUs ? " " : string.Empty)}" +
					$"{LangSource["Comma"]}" +
					$"{(Settings.LanguageCode == CountryCode.EnUs ? " " : string.Empty)}" +
					$"{LangSource["TimeElapsed"]}" +
					$"{_analyisResult.ElapsedTime:hh\\:mm\\.ss\\.fff}" +
					$"{LangSource["Period"]}";

				int i = 0;
				var pathList = new List<ListBoxItem>();
				foreach (var step in _analyisResult.Steps!)
				{
					var (fore, back) = Settings.DiffColors[step.DifficultyLevel];
					var content =
						new StepTriplet(
							(Settings.ShowStepLabel, Settings.ShowStepDifficulty) switch
							{
								(true, true) => $"(#{i + 1}, {step.Difficulty}) {step.ToSimpleString()}",
								(true, false) => $"(#{i + 1}) {step.ToSimpleString()}",
								(false, true) => $"({step.Difficulty}) {step.ToSimpleString()}",
								_ => step.ToSimpleString()
							},
							i++,
							step);

					pathList.Add(new()
					{
						Foreground = new SolidColorBrush(fore.ToWColor()),
						Background = new SolidColorBrush(back.ToWColor()),
						Content = content,
						BorderThickness = default,
						HorizontalContentAlignment = HorizontalAlignment.Left,
						VerticalContentAlignment = VerticalAlignment.Center
					});
				}
				_listBoxPaths.ItemsSource = pathList;

				// Gather the information.
				// GridView should list the instance with each property, not fields,
				// even if fields are public.
				// Therefore, here may use anonymous type is okay, but using value tuples
				// is bad.
				var puzzleDifficultyLevel = DifficultyLevel.Unknown;
				var collection = new List<DifficultyInfo>();
				decimal summary = 0, summaryMax = 0;
				int summaryCount = 0;
				foreach (var techniqueGroup in
					from step in _analyisResult.Steps!
					orderby step.Difficulty
					group step by step.Name)
				{
					string name = techniqueGroup.Key;
					int count = techniqueGroup.Count();
					decimal total = 0, minimum = decimal.MaxValue, maximum = 0;
					var minDifficultyLevel = DifficultyLevel.LastResort;
					var maxDifficultyLevel = DifficultyLevel.Unknown;
					foreach (var step in techniqueGroup)
					{
						summary += step.Difficulty;
						summaryCount++;
						total += step.Difficulty;
						minimum = Math.Min(step.Difficulty, minimum);
						maximum = Math.Max(step.Difficulty, maximum);
						minDifficultyLevel = Algorithms.Min(step.DifficultyLevel, minDifficultyLevel);
						maxDifficultyLevel = Algorithms.Max(step.DifficultyLevel, maxDifficultyLevel);
					}

					summaryMax = Math.Max(summaryMax, maximum);
					puzzleDifficultyLevel = Algorithms.Max(puzzleDifficultyLevel, maxDifficultyLevel);

					if (minimum == maximum)
					{
						collection.Add(
							new(
								name, count, total, minimum.ToString("0.0"),
								minDifficultyLevel | maxDifficultyLevel));
					}
					else
					{
						collection.Add(
							new(
								name, count, total, $"{minimum:0.0} - {maximum:0.0}",
								minDifficultyLevel | maxDifficultyLevel));
					}
				}

				collection.Add(
					new(null, summaryCount, summary, summaryMax.ToString("0.0"), puzzleDifficultyLevel));

				_listViewSummary.ItemsSource = collection;
			}
			else
			{
				Messagings.FailedToSolveWithMessage(_analyisResult);
			}
		}
		#endregion


		#region Event-delegated methods
		/// <inheritdoc cref="Events.SizeChanged(object?, EventArgs)"/>
		private void Window_SizeChanged(object sender, SizeChangedEventArgs e) =>
			DisplayDifficultyInfoAfterAnalyzed();
		#endregion


		#region Static methods
		/// <summary>
		/// To prevent you opening two same windows.
		/// </summary>
		private static void PreventYouOpeningTwoSameWindows()
		{
			var mutex = new Mutex(true, LangSource["SolutionName"] as string, out bool mutexIsNew);
			if (mutexIsNew)
			{
				mutex.ReleaseMutex();
			}
			else
			{
				Messagings.YouCanOnlyOpenOneProgram();
				Environment.Exit(0);
			}
		}

		/// <summary>
		/// To check the validity of all conclusions.
		/// </summary>
		/// <param name="solution">(<see langword="in"/> parameter) The solution.</param>
		/// <param name="conclusions">The conclusions.</param>
		/// <returns>A <see cref="bool"/> indicating that.</returns>
		private static bool CheckConclusionsValidity(in SudokuGrid solution, IEnumerable<Conclusion> conclusions)
		{
			foreach (var (t, c, d) in conclusions)
			{
				int digit = solution[c];
				switch (t)
				{
					case ConclusionType.Assignment when digit != d:
					case ConclusionType.Elimination when digit == d:
					{
						return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Create a default JSON serializer options instance while initializing
		/// the field <see cref="_serializerOptions"/>.
		/// </summary>
		/// <returns>The instance.</returns>
		/// <seealso cref="_serializerOptions"/>
		private static JsonSerializerOptions CreateDefaultJsonSerializerOptionsInstance()
		{
			var result = new JsonSerializerOptions() { WriteIndented = true };
			result.Converters.Add(new ColorJsonConverter());
			return result;
		}
		#endregion
	}
}
