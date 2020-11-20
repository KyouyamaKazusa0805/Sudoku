using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Drawing.Extensions;
using Sudoku.Extensions;
using Sudoku.Globalization;
using Sudoku.Windows.Constants;
using Sudoku.Windows.Extensions;
using static System.Math;
using static Sudoku.Constants.Processings;
using C = Sudoku.Data.ConclusionType;
using CoreResources = Sudoku.Windows.Resources;
using K = System.Windows.Input.Key;
using M = System.Windows.Input.ModifierKeys;
using R = System.Windows.MessageBoxResult;
using StepTriplet = System.KeyedTuple<string, int, Sudoku.Solving.TechniqueInfo>;
using Sudoku.Data.Extensions;
#if SUDOKU_RECOGNITION
using System.Diagnostics;
#endif

namespace Sudoku.Windows
{
	/// <summary>
	/// Interaction logic for <c>MainWindow.xaml</c>.
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <inheritdoc cref="DefaultConstructor"/>
		public MainWindow() => InitializeComponent();


		#region Overriden methods
		/// <inheritdoc/>
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e); // Call the base method.

			LoadConfigIfWorth();
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
				_imageGrid.Height = _imageGrid.Width = Min(w, h);
				Settings.GridSize = w;
				_currentPainter = new(new(_imageGrid.RenderSize.ToDSizeF()), Settings, _puzzle);

				UpdateImageGrid();
			}
		}

		/// <inheritdoc/>
		protected override void OnClosing(CancelEventArgs e)
		{
			// Ask when worth.
			if (Settings.AskWhileQuitting && Messagings.AskWhileQuitting() == R.No)
			{
				e.Cancel = true;
				return;
			}

			SaveConfig();

			SaveCoreResources();

#if SUDOKU_RECOGNITION
			_recognition?.Dispose();
#endif

			GC.Collect();

			base.OnClosing(e);

#if SUDOKU_RECOGNITION
			// If you don't use this feature, the program won't need to use
			// this method to KILL itself... KILL... sounds terrible and dangerous, isn't it?
			// To be honest, I don't know why the program fails to exit... The background
			// threads still running after base close method executed completely. If you
			// know the detail of Emgu.CV, please tell me, thx!
			if (_recognition is { ToolIsInitialized: true })
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
				case var key when key.IsDigit():
				{
					if (_pointConverter.GetCell(Mouse.GetPosition(_imageGrid).ToDPointF()) is var cell
						&& cell == -1)
					{
						return;
					}

					if (_puzzle.GetStatus(cell) == CellStatus.Given)
					{
						return;
					}

					int digit = e.Key.IsDigitUpsideAlphabets() ? e.Key - K.D1 : e.Key - K.NumPad1;
					switch (Keyboard.Modifiers)
					{
						case M.None:
						{
							// Input a digit.
							// Input or eliminate a digit.
							if (digit != -1 && ((SudokuGrid)_puzzle).Duplicate(cell, digit))
							{
								// Input is invalid. We can't let you fill this cell with this digit.
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
						case M.Shift:
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
				case var key and (K.OemMinus or K.OemPlus):
				{
					// Get the previous view or the next view.
					if (_currentViewIndex == -1 || _currentTechniqueInfo is null)
					{
						return;
					}

					int nextIndex = key == K.OemMinus ? _currentViewIndex - 1 : _currentViewIndex + 1;
					if (nextIndex < 0 || nextIndex >= _currentTechniqueInfo.Views.Count)
					{
						return;
					}

					_currentPainter = _currentPainter with
					{
						View = _currentTechniqueInfo.Views[_currentViewIndex = nextIndex]
					};

					UpdateImageGrid();

					break;
				}
				case var key when key.IsArrow() && _focusedCells.Count == 1:
				{
					// Move the focused cell.
					int cell = _focusedCells.First;
					_focusedCells.Clear();
					_focusedCells.AddAnyway(
						e.Key switch
						{
							K.Up => cell - 9 < 0 ? cell + 72 : cell - 9,
							K.Down => cell + 9 >= 81 ? cell - 72 : cell + 9,
							K.Left => cell - 1 < 0 ? cell + 8 : cell - 1,
							K.Right => (cell + 1) % 81,
							_ => throw Throwings.ImpossibleCase
						});

					_currentPainter = _currentPainter with
					{
						Grid = _puzzle,
						FocusedCells = _focusedCells
					};

					UpdateImageGrid();

					break;
				}
				case K.Space:
				{
					// View the intersection.
					_previewMap = _focusedCells;
					_focusedCells = _focusedCells.PeerIntersection;

					_currentPainter = _currentPainter with
					{
						Grid = _puzzle,
						FocusedCells = _focusedCells
					};

					UpdateImageGrid();

					break;
				}
				case K.Tab:
				{
					// Move to next box row.
					int cell = _focusedCells.IsEmpty ? 0 : _focusedCells.First;
					_focusedCells.Clear();
					_focusedCells.AddAnyway((cell + 3) % 81);

					_currentPainter = _currentPainter with
					{
						Grid = _puzzle,
						FocusedCells = _focusedCells
					};

					UpdateImageGrid();

					break;
				}
				case K.Escape:
				{
					// Clear focused cells.
					ClearViews();

					UpdateImageGrid();

					_textBoxInfo.ClearValue(TextBox.TextProperty);

					break;
				}
			}

			GC.Collect();
		}

		/// <inheritdoc/>
		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);

			if (_previewMap.HasValue && e.Key == K.Space)
			{
				_focusedCells = _previewMap.Value;

				_currentPainter = _currentPainter with { FocusedCells = _focusedCells };

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
			catch (FileNotFoundException ex) when (ex.FileName?.EndsWith("eng.traineddata") ?? false)
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
		/// Save core resources.
		/// </summary>
		private void SaveCoreResources()
		{
			DirectoryEx.CreateIfDoesNotExist("lang");

			File.WriteAllText(@"lang\Resources.en-us.dic", JsonSerializer.Serialize(CoreResources.LangSourceEnUs));
			File.WriteAllText(@"lang\Resources.zh-cn.dic", JsonSerializer.Serialize(CoreResources.LangSourceZhCn));
		}

		/// <summary>
		/// Add short cuts during initialization.
		/// </summary>
		private void DefineShortCuts()
		{
			AddShortCut(K.C, M.Control, null, MenuItemEditCopy_Click);
			AddShortCut(K.H, M.Control, _menuItemGenerateHardPattern, MenuItemGenerateHardPattern_Click);
			AddShortCut(K.O, M.Control, _menuItemFileOpen, MenuItemFileOpen_Click);
			AddShortCut(K.P, M.Control, null, MenuItemFileGetSnapshot_Click);
			AddShortCut(K.S, M.Control, null, MenuItemFileSave_Click);
			AddShortCut(K.V, M.Control, _menuItemEditPaste, MenuItemEditPaste_Click);
			AddShortCut(K.Y, M.Control, _menuItemEditRedo, MenuItemEditRedo_Click);
			AddShortCut(K.Z, M.Control, _menuItemEditUndo, MenuItemEditUndo_Click);
			AddShortCut(K.F5, M.Control, _menuItemEditRecomputeCandidates, MenuItemEditRecomputeCandidates_Click);
			AddShortCut(K.OemTilde, M.Control, _menuItemEditFix, MenuItemEditFix_Click);
			AddShortCut(K.F9, M.None, _menuItemAnalyzeAnalyze, MenuItemAnalyzeAnalyze_Click);
			AddShortCut(K.F10, M.None, _menuItemAnalyzeSolve, MenuItemAnalyzeSolve_Click);
			AddShortCut(K.F4, M.Alt, null, MenuItemFileQuit_Click);
			AddShortCut(K.N, M.Control | M.Shift, _menuItemEditClear, MenuItemEditClear_Click);
			AddShortCut(K.C, M.Control | M.Shift, null, MenuItemEditCopyCurrentGrid_Click);
			AddShortCut(K.OemTilde, M.Control | M.Shift, _menuItemEditUnfix, MenuItemEditUnfix_Click);
		}

		/// <summary>
		/// Clear the current views (<see cref="View"/> and <see cref="MutableView"/>).
		/// </summary>
		/// <seealso cref="View"/>
		/// <seealso cref="MutableView"/>
		private void ClearViews()
		{
			_focusedCells.Clear();
			_currentPainter = _currentPainter with
			{
				Grid = _puzzle,
				Conclusions = null,
				CustomView = null,
				View = null,
				FocusedCells = null
			};
			_currentViewIndex = -1;
			_currentTechniqueInfo = null;
		}

		/// <summary>
		/// Load database if worth.
		/// </summary>
		private void LoadDatabaseIfWorth()
		{
			if (Settings.CurrentPuzzleDatabase is null
				|| Messagings.AskWhileLoadingAndCoveringDatabase() != R.Yes)
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
						is WindowsSettings settingsResult)
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
			K key, M modifierKeys, UIElement? matchControl, ExecutedRoutedEventHandler executed)
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
#if COPY_SYNC && OBSOLETE
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
#if COPY_SYNC && OBSOLETE
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
			_imageUndoIcon.Source =
				new BitmapImage(
					new(
						$@"Resources/ImageIcon-Undo{
							((_menuItemEditUndo.IsEnabled = _puzzle.HasUndoSteps) ? string.Empty : "Disable")
						}.png",
						UriKind.Relative));
			_imageRedoIcon.Source =
				new BitmapImage(
					new(
						$@"Resources/ImageIcon-Redo{
							((_menuItemEditRedo.IsEnabled = _puzzle.HasRedoSteps) ? string.Empty : "Disable")
						}.png",
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
			_imageGeneratingIcon.IsEnabled = false;
			_imageSolve.IsEnabled = false;
			_comboBoxSymmetry.IsEnabled = false;
			_comboBoxMode.IsEnabled = false;
			_comboBoxBackdoorFilteringDepth.IsEnabled = false;
			_comboBoxDifficulty.IsEnabled = false;

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
			_imageGeneratingIcon.IsEnabled = true;
			_imageSolve.IsEnabled = true;
			_comboBoxMode.IsEnabled = true;
			_comboBoxSymmetry.IsEnabled = true;
			_comboBoxBackdoorFilteringDepth.IsEnabled = true;
			_comboBoxDifficulty.IsEnabled = true;

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
			_imageGeneratingIcon.IsEnabled = false;
			_imageSolve.IsEnabled = false;
			_comboBoxSymmetry.IsEnabled = false;
			_comboBoxMode.IsEnabled = false;
			_comboBoxBackdoorFilteringDepth.IsEnabled = false;
			_comboBoxDifficulty.IsEnabled = false;

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
			_imageGeneratingIcon.IsEnabled = true;
			_imageSolve.IsEnabled = true;
			_comboBoxMode.IsEnabled = true;
			_comboBoxSymmetry.IsEnabled = true;
			_comboBoxBackdoorFilteringDepth.IsEnabled = true;
			_comboBoxDifficulty.IsEnabled = true;

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
			ResourceDictionary? g(string p) => dictionaries.FirstOrDefault(d => d.Source.OriginalString == p);
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

			if (_analyisResult.HasSolved)
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
				foreach (var step in _analyisResult.SolvingSteps!)
				{
					var (fore, back) = Settings.DiffColors[step.DifficultyLevel];
					pathList.Add(
						new()
						{
							Foreground = new SolidColorBrush(fore.ToWColor()),
							Background = new SolidColorBrush(back.ToWColor()),
							Content =
								new StepTriplet(
									$"(#{i + 1}, {step.Difficulty}) {step.ToSimpleString()}", i++, step),
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
				var collection = new List<DifficultyInfo>();
				decimal summary = 0, summaryMax = 0;
				int summaryCount = 0;
				foreach (var techniqueGroup in
					from step in _analyisResult.SolvingSteps!
					orderby step.Difficulty
					group step by step.Name)
				{
					string name = techniqueGroup.Key;
					int count = techniqueGroup.Count();
					decimal total = 0, maximum = 0;
					foreach (var step in techniqueGroup)
					{
						summary += step.Difficulty;
						summaryCount++;
						total += step.Difficulty;
						maximum = Max(step.Difficulty, maximum);
						summaryMax = Max(step.Difficulty, maximum);
					}

					collection.Add(new(name, count, total, maximum));
				}

				collection.Add(new(null, summaryCount, summary, summaryMax));

				GridView view;
				_listViewSummary.ItemsSource = collection;
				_listViewSummary.View = view = new();
				view.Columns.AddRange(
					createGridViewColumn(LangSource["TechniqueHeader"], nameof(DifficultyInfo.Technique), .6),
					createGridViewColumn(LangSource["TechniqueCount"], nameof(DifficultyInfo.Count), .1),
					createGridViewColumn(LangSource["TechniqueTotal"], nameof(DifficultyInfo.Total), .15),
					createGridViewColumn(LangSource["TechniqueMax"], nameof(DifficultyInfo.Max), .15));
				view.AllowsColumnReorder = false;

				GridViewColumn createGridViewColumn(object header, string name, double widthScale) =>
					new()
					{
						Header = header,
						DisplayMemberBinding = new Binding(name),
						Width = _tabControlInfo.ActualWidth * widthScale - 4,
					};
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
					case C.Assignment when digit != d:
					case C.Elimination when digit == d:
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
