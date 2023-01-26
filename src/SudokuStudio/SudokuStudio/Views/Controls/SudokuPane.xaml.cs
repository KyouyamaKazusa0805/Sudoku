namespace SudokuStudio.Views.Controls;

/// <summary>
/// Defines a sudoku pane control.
/// </summary>
public sealed partial class SudokuPane : UserControl, INotifyPropertyChanged
{
	/// <summary>
	/// Defines a pair of stacks that stores undo and redo steps.
	/// </summary>
	internal readonly NotifyElementChangedStack<Grid> _undoStack = new(), _redoStack = new();

	/// <summary>
	/// The easy entry to visit children <see cref="SudokuPaneCell"/> instances. This field contains 81 elements,
	/// indicating controls being displayed as 81 cells in a sudoku grid respectively.
	/// </summary>
	/// <remarks>
	/// Although this field is not marked as <see langword="readonly"/>, it will only be initialized during initialization.
	/// <b>Please do not modify any elements in this array.</b>
	/// </remarks>
	internal SudokuPaneCell[] _children;

	/// <summary>
	/// Indicates whether the pane displays for candidates.
	/// </summary>
	[NotifyBackingField]
	private bool _displayCandidates = true;

	/// <summary>
	/// Indicates whether the pane displays cursors that uses different colors to highlight some cells as peers of the target cell
	/// that is the one your mouse points to.
	/// </summary>
	[NotifyBackingField]
	private bool _displayCursors = true;

	/// <summary>
	/// Indicates whether the pane displays for delta digits using different colors.
	/// </summary>
	[NotifyBackingField]
	private bool _useDifferentColorToDisplayDeltaDigits = true;

	/// <summary>
	/// Indicates the font scale of value digits (given or modifiable ones). The value should generally be below 1.0.
	/// </summary>
	[NotifyBackingField]
	private double _valueFontScale = 1.0;

	/// <summary>
	/// Indicates the font scale of pencilmark digits (candidates). The value should generally be below 1.0.
	/// </summary>
	[NotifyBackingField]
	private double _pencilmarkFontScale = .33;

	/// <summary>
	/// Indicates the font scale of baba group characters. The value should generally be below 1.0.
	/// </summary>
	[NotifyBackingField]
	private double _babaGroupLabelFontScale = .6;

	/// <summary>
	/// Indicates the coordinate label font scale. The value should generally be below 1.0.
	/// </summary>
	[NotifyBackingField]
	private double _coordinateLabelFontScale = .4;

	/// <summary>
	/// Indicates the scale of highlighted candidate circles. The value should generally be below 1.0.
	/// </summary>
	[NotifyBackingField]
	private double _highlightCandidateCircleScale = .9;

	/// <summary>
	/// Indicates the opacity of the background highlighted elements. The value should generally be below 1.0.
	/// </summary>
	[NotifyBackingField]
	private double _highlightBackgroundOpacity = .15;

	/// <summary>
	/// Indicates the currently selected cell.
	/// </summary>
	[NotifyBackingField]
	private int _selectedCell;

	/// <summary>
	/// Indicates the displaying kind of coordinate labels.
	/// </summary>
	/// <remarks>
	/// For more information please visit <see cref="Interaction.CoordinateLabelDisplayKind"/>.
	/// </remarks>
	/// <seealso cref="Interaction.CoordinateLabelDisplayKind"/>
	[NotifyBackingField]
	private CoordinateLabelDisplayKind _coordinateLabelDisplayKind = CoordinateLabelDisplayKind.RxCy;

	/// <summary>
	/// Indicates the displaying mode of coordinate labels.
	/// </summary>
	/// <remarks>
	/// For more information please visit <see cref="Interaction.CoordinateLabelDisplayMode"/>.
	/// </remarks>
	/// <seealso cref="Interaction.CoordinateLabelDisplayMode"/>
	[NotifyBackingField]
	private CoordinateLabelDisplayMode _coordinateLabelDisplayMode = CoordinateLabelDisplayMode.UpperAndLeft;

	/// <summary>
	/// Indicates the given color.
	/// </summary>
	[NotifyBackingField]
	private Color _givenColor = Colors.Black;

	/// <summary>
	/// Indicates the modifiable color.
	/// </summary>
	[NotifyBackingField]
	private Color _modifiableColor = Colors.Blue;

	/// <summary>
	/// Indicates the pencilmark color.
	/// </summary>
	[NotifyBackingField]
	private Color _pencilmarkColor = new() { A = 255, R = 100, G = 100, B = 100 };

	/// <summary>
	/// Indicates the coordinate label color.
	/// </summary>
	[NotifyBackingField]
	private Color _coordinateLabelColor = new() { A = 255, R = 100, G = 100, B = 100 };

	/// <summary>
	/// Indicates the baba group label color.
	/// </summary>
	[NotifyBackingField]
	private Color _babaGroupLabelColor = Colors.Red;

	/// <summary>
	/// Indicates the color that is used for displaying candidates that are wrongly removed, but correct.
	/// </summary>
	[NotifyBackingField]
	private Color _deltaCandidateColor = new() { A = 255, R = 255, G = 185, B = 185 };

	/// <summary>
	/// Indicates the color that is used for displaying cell digits that are wrongly filled.
	/// </summary>
	[NotifyBackingField]
	private Color _deltaCellColor = new() { A = 255, R = 255 };

	/// <summary>
	/// Indicates the border color.
	/// </summary>
	[NotifyBackingField]
	private Color _borderColor = Colors.Black;

	/// <summary>
	/// Indicates the cursor background color.
	/// </summary>
	[NotifyBackingField]
	private Color _cursorBackgroundColor = new() { A = 32, B = 255 };

	/// <summary>
	/// Indicates the target puzzle.
	/// </summary>
	private Grid _puzzle = Grid.Empty;

	/// <summary>
	/// Indicates the value font.
	/// </summary>
	[NotifyBackingField]
	private FontFamily _valueFont = new("Tahoma");

	/// <summary>
	/// Indicates the candidate font.
	/// </summary>
	[NotifyBackingField]
	private FontFamily _pencilmarkFont = new("Tahoma");

	/// <summary>
	/// Indicates the coordinate label font.
	/// </summary>
	[NotifyBackingField]
	private FontFamily _coordinateLabelFont = new("Tahoma");

	/// <summary>
	/// Indicates the baba group label font.
	/// </summary>
	[NotifyBackingField]
	private FontFamily _babaGroupLabelFont = new("Times New Roman");

	/// <summary>
	/// Indicates the view unit used.
	/// </summary>
	private ViewUnit? _viewUnit;


	/// <summary>
	/// Initializes a <see cref="SudokuPane"/> instance.
	/// </summary>
	public SudokuPane()
	{
		InitializeComponent();
		InitializeChildrenControls();
		UpdateCellData(_puzzle);
		InitializeEvents();
	}


	/// <inheritdoc cref="_puzzle"/>
	public Grid Puzzle
	{
		get => _puzzle;

		set => SetPuzzle(value, clearStack: true);
	}

	/// <summary>
	/// Indicates the base page.
	/// </summary>
	public AnalyzePage BasePage { get; set; } = null!;

	/// <inheritdoc cref="_viewUnit"/>
	public ViewUnit? ViewUnit
	{
		get => _viewUnit;

		set
		{
			if (ReferenceEquals(_viewUnit, value))
			{
				return;
			}

			_viewUnit = value;

			ViewUnitSetterAfter(value);
		}
	}

	/// <summary>
	/// Indicates the approximately-measured width and height value of a cell.
	/// </summary>
	internal double ApproximateCellWidth => ((Width + Height) / 2 - 100 - (4 << 1)) / 10;

	/// <summary>
	/// Indicates the solution of property <see cref="Puzzle"/>.
	/// </summary>
	/// <seealso cref="Puzzle"/>
	internal Grid Solution => _puzzle.GetSolution();


	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;

	/// <summary>
	/// Indicates the event that is triggered when a file is failed to be received via dropped file.
	/// </summary>
	public event FailedReceivedDroppedFileEventHandler? FailedReceivedDroppedFile;


	/// <summary>
	/// Undo a step.
	/// </summary>
	public void UndoStep()
	{
		if (_undoStack.Count == 0)
		{
			// No more steps can be undone.
			return;
		}

		_redoStack.Push(_puzzle);

		var target = _undoStack.Pop();
		SetPuzzle(target, whileUndoingOrRedoing: true);
	}

	/// <summary>
	/// Redo a step.
	/// </summary>
	public void RedoStep()
	{
		if (_redoStack.Count == 0)
		{
			// No more steps can be redone.
			return;
		}

		_undoStack.Push(_puzzle);

		var target = _redoStack.Pop();
		SetPuzzle(target, whileUndoingOrRedoing: true);
	}

	/// <summary>
	/// Copies the current grid as text into the clipboard.
	/// </summary>
	public void Copy()
	{
		if (Puzzle is var puzzle and ({ IsUndefined: true } or { IsEmpty: true }))
		{
			return;
		}

		var dataPackage = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
		dataPackage.SetText(puzzle.ToString(SusserFormat.Full));

		Clipboard.SetContent(dataPackage);
	}

	/// <summary>
	/// Copy the snapshot of the sudoku grid control, to the clipboard.
	/// </summary>
	/// <returns>
	/// The typical <see langword="await"/>able instance that holds the task to copy the snapshot.
	/// </returns>
	/// <remarks>
	/// The code is referenced from
	/// <see href="https://github.com/microsoftarchive/msdn-code-gallery-microsoft/blob/21cb9b6bc0da3b234c5854ecac449cb3bd261f29/Official%20Windows%20Platform%20Sample/XAML%20render%20to%20bitmap%20sample/%5BC%23%5D-XAML%20render%20to%20bitmap%20sample/C%23/Scenario2.xaml.cs#L120">here</see>
	/// and
	/// <see href="https://github.com/microsoftarchive/msdn-code-gallery-microsoft/blob/21cb9b6bc0da3b234c5854ecac449cb3bd261f29/Official%20Windows%20Platform%20Sample/XAML%20render%20to%20bitmap%20sample/%5BC%23%5D-XAML%20render%20to%20bitmap%20sample/C%23/Scenario2.xaml.cs#L182">here</see>.
	/// </remarks>
	public async Task CopySnapshotAsync()
	{
		// Creates the stream to store the output image data.
		var stream = new InMemoryRandomAccessStream();

		// Gets the snapshot of the control.
		await this.RenderToAsync(stream);

		// Copies the data to the data package.
		var dataPackage = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
		var streamRef = RandomAccessStreamReference.CreateFromStream(stream);
		dataPackage.SetBitmap(streamRef);

		// Copies to the clipboard.
		Clipboard.SetContent(dataPackage);
	}

	/// <summary>
	/// Pastes the text, to the clipboard.
	/// </summary>
	/// <returns>
	/// The typical <see langword="await"/>able instance that holds the task to paste the text.
	/// </returns>
	public async Task PasteAsync()
	{
		var dataPackageView = Clipboard.GetContent();
		if (!dataPackageView.Contains(StandardDataFormats.Text))
		{
			return;
		}

		var gridStr = await dataPackageView.GetTextAsync();
		if (!Grid.TryParse(gridStr, out var grid))
		{
			return;
		}

		Puzzle = grid;
	}

	/// <summary>
	/// Try to set puzzle, with a <see cref="bool"/> value indicating whether the stack fields <see cref="_undoStack"/>
	/// and <see cref="_redoStack"/> will be cleared.
	/// </summary>
	/// <param name="value">The newer grid.</param>
	/// <param name="clearStack">
	/// <para>Indicates whether the stack fields <see cref="_undoStack"/> and <see cref="_redoStack"/> will be cleared.</para>
	/// <para>The default value is <see langword="false"/>.</para>
	/// </param>
	/// <param name="whileUndoingOrRedoing">
	/// <para>Indicates whether the current operation occurred while undoing and redoing a grid step.</para>
	/// <para>The default value is <see langword="false"/>.</para>
	/// </param>
	/// <param name="clearAnalyzeTabData">
	/// <para>Indicates whether the puzzle-replacing operation will clear analyze data.</para>
	/// <para>The default value is <see langword="true"/>.</para>
	/// </param>
	/// <remarks>
	/// <para>
	/// This method is nearly equal to <see cref="set_Puzzle(Grid)"/>, but this method can also control undoing and redoing stacks.
	/// </para>
	/// <para>Generally, we use this method more times than covering with property <see cref="Puzzle"/>.</para>
	/// </remarks>
	/// <seealso cref="_undoStack"/>
	/// <seealso cref="_redoStack"/>
	/// <seealso cref="set_Puzzle(Grid)"/>
	/// <seealso cref="Puzzle"/>
	internal void SetPuzzle(scoped in Grid value, bool clearStack = false, bool whileUndoingOrRedoing = false, bool clearAnalyzeTabData = true)
	{
		if (_puzzle == value)
		{
			return;
		}

		// Pushes the grid into the stack if worth.
		if (!whileUndoingOrRedoing && !clearStack)
		{
			_undoStack.Push(_puzzle);
		}

		_puzzle = value;

		UpdateCellData(value);
		switch (clearStack, whileUndoingOrRedoing)
		{
			case (true, _):
			{
				ClearStacks();
				break;
			}
			case (false, false):
			{
				_redoStack.Clear();
				break;
			}
		}

		// Clears the analyze tab pages is worth.
		if (clearAnalyzeTabData)
		{
			BasePage?.ClearAnalyzeTabsData();
		}

		// Clears the view unit.
		ViewUnit = null;

		PropertyChanged?.Invoke(this, new(nameof(Puzzle)));
	}

	/// <summary>
	/// To initialize children controls for <see cref="_children"/>.
	/// </summary>
	[MemberNotNull(nameof(_children))]
	private void InitializeChildrenControls()
	{
		_children = new SudokuPaneCell[81];
		for (var i = 0; i < 81; i++)
		{
			var cellControl = new SudokuPaneCell { CellIndex = i, BasePane = this };

			GridLayout.SetRow(cellControl, i / 9 + 2);
			GridLayout.SetColumn(cellControl, i % 9 + 2);

			MainGrid.Children.Add(cellControl);
			_children[i] = cellControl;
		}
	}

	/// <summary>
	/// To initializes for stack events.
	/// </summary>
	private void InitializeEvents()
	{
		_undoStack.Changed += _ => PropertyChanged?.Invoke(this, new(nameof(_undoStack)));
		_redoStack.Changed += _ => PropertyChanged?.Invoke(this, new(nameof(_redoStack)));
	}

	/// <summary>
	/// To initialize <see cref="_children"/> values via the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <seealso cref="_children"/>
	private void UpdateCellData(scoped in Grid grid)
	{
		for (var i = 0; i < 81; i++)
		{
			var cellControl = _children[i];
			cellControl.Status = grid.GetStatus(i);
			cellControl.CandidatesMask = grid.GetCandidates(i);
		}
	}

	/// <summary>
	/// Try to clear stacks <see cref="_undoStack"/> and <see cref="_redoStack"/>.
	/// </summary>
	private void ClearStacks()
	{
		_undoStack.Clear();
		_redoStack.Clear();
	}

	private void ViewUnitSetterAfter(ViewUnit? value)
	{
		if (value is null)
		{
			ViewUnitFrameworkElementFactory.RemoveViewUnitControls(BasePage);
		}
		else
		{
			ViewUnitFrameworkElementFactory.AddViewUnitControls(BasePage, value);
		}
	}


	private void UserControl_Loaded(object sender, RoutedEventArgs e) => Focus(FocusState.Programmatic);

	private void UserControl_KeyDown(object sender, KeyRoutedEventArgs e)
	{
		/**
			Please note that the parent control may use globalized hotkeys to control some behaviors.
			If <c>e.Handled</c> is not set <see langword="false"/> value before exited parent <c>KeyDown</c> method,
			this method will not be triggered and executed.
		*/
		switch (Keyboard.GetModifierStatusForCurrentThread(), SelectedCell, Keyboard.GetInputDigit(e.Key))
		{
			case (_, not (>= 0 and < 81), _):
			case (_, var cell, _) when Puzzle.GetStatus(cell) == CellStatus.Given:
			case (_, _, -2):
			{
				return;
			}
			case ((false, false, false, false), var cell, -1):
			{
				var modified = Puzzle;
				modified[cell] = -1;

				SetPuzzle(modified);

				break;
			}
			case ((false, true, false, false), var cell, var digit) when Puzzle.Exists(cell, digit) is true:
			{
				var modified = Puzzle;
				modified[cell, digit] = false;

				SetPuzzle(modified);

				break;
			}
			case ((false, false, false, false), var cell, var digit) when !Puzzle.DuplicateWith(cell, digit):
			{
				var modified = Puzzle;
				if (Puzzle.GetStatus(cell) == CellStatus.Modifiable)
				{
					// Temporarily re-compute candidates.
					modified[cell] = -1;
				}

				modified[cell] = digit;
				SetPuzzle(modified);

				break;
			}
		}
	}

	private void UserControl_DragOver(object sender, DragEventArgs e)
	{
		e.AcceptedOperation = DataPackageOperation.Copy;
		e.DragUIOverride.Caption = GetString("SudokuPane_DropSudokuFileHere");
		e.DragUIOverride.IsCaptionVisible = true;
		e.DragUIOverride.IsContentVisible = true;
	}

	private void UserControl_PointerEntered(object sender, PointerRoutedEventArgs e) => Focus(FocusState.Programmatic);

	private async void UserControl_DropAsync(object sender, DragEventArgs e)
	{
		if (e.DataView is not { } dataView)
		{
			return;
		}

		if (!dataView.Contains(StandardDataFormats.StorageItems))
		{
			return;
		}

		switch (await dataView.GetStorageItemsAsync())
		{
			case [StorageFolder folder]:
			{
				var files = await folder.GetFilesAsync(CommonFileQuery.DefaultQuery, 0, 2);
				if (files is not [StorageFile { FileType: CommonFileExtensions.Text or CommonFileExtensions.PlainText } file])
				{
					return;
				}

				await handleSudokuFileAsync(file);

				break;
			}
			case [StorageFile { FileType: CommonFileExtensions.Text or CommonFileExtensions.PlainText } file]:
			{
				await handleSudokuFileAsync(file);

				break;
			}


			async Task handleSudokuFileAsync(StorageFile file)
			{
				var filePath = file.Path;
				var fileInfo = new FileInfo(filePath);
				switch (fileInfo.Length)
				{
					case 0:
					{
						FailedReceivedDroppedFile?.Invoke(this, new(FailedReceivedDroppedFileReason.FileIsEmpty));
						return;
					}
					case > 1024:
					{
						FailedReceivedDroppedFile?.Invoke(this, new(FailedReceivedDroppedFileReason.FileIsTooLarge));
						return;
					}
					default:
					{
						var content = await FileIO.ReadTextAsync(file);
						switch (io::Path.GetExtension(filePath))
						{
							case CommonFileExtensions.PlainText:
							{
								if (string.IsNullOrWhiteSpace(content))
								{
									FailedReceivedDroppedFile?.Invoke(this, new(FailedReceivedDroppedFileReason.FileIsEmpty));
									return;
								}

								if (!Grid.TryParse(content, out var g))
								{
									FailedReceivedDroppedFile?.Invoke(this, new(FailedReceivedDroppedFileReason.FileCannotBeParsed));
									return;
								}

								Puzzle = g;
								break;
							}
							case CommonFileExtensions.Text:
							{
								switch (Deserialize<GridSerializationData[]>(content, CommonSerializerOptions.CamelCasing))
								{
									case [{ GridString: var str }]:
									{
										if (!Grid.TryParse(str, out var g))
										{
											FailedReceivedDroppedFile?.Invoke(this, new(FailedReceivedDroppedFileReason.FileCannotBeParsed));
											return;
										}

										Puzzle = g;
										break;
									}
									default:
									{
										FailedReceivedDroppedFile?.Invoke(this, new(FailedReceivedDroppedFileReason.FileCannotBeParsed));
										return;
									}
								}

								break;
							}
						}

						break;
					}
				}
			}
		}
	}
}
