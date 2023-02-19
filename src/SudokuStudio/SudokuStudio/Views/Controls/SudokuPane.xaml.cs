namespace SudokuStudio.Views.Controls;

/// <summary>
/// Defines a sudoku pane control.
/// </summary>
[DependencyProperty<bool>("DisplayCandidates", DefaultValue = true, DocSummary = "Indicates whether the pane displays for candidates.")]
[DependencyProperty<bool>("DisplayCursors", DefaultValue = true, DocSummary = "Indicates whether the pane displays cursors that uses different colors to highlight some cells as peers of the target cell that is the one your mouse points to.")]
[DependencyProperty<bool>("UseDifferentColorToDisplayDeltaDigits", DefaultValue = true, DocSummary = "Indicates whether the pane displays for delta digits using different colors.")]
[DependencyProperty<bool>("DisableFlyout", DocSummary = "Indicates whether the pane disable flyout open.")]
[DependencyProperty<bool>("PreventConflictingInput", DefaultValue = true, DocSummary = "Indicates whether the pane prevent the simple confliction, which means, if you input a digit that is confilct with the digits in its containing houses, this pane will do nothing by this value being <see langword=\"true\"/>. If not, the pane won't check for any confliction and always allow you inputting the digit regardless of possible confilction.")]
[DependencyProperty<double>("GivenFontScale", DefaultValue = 1.0, DocSummary = "Indicates the font scale of given digits. The value should generally be below 1.0.")]
[DependencyProperty<double>("ModifiableFontScale", DefaultValue = 1.0, DocSummary = "Indicates the font scale of modifiable digits. The value should generally be below 1.0.")]
[DependencyProperty<double>("PencilmarkFontScale", DefaultValue = .33, DocSummary = "Indicates the font scale of pencilmark digits (candidates). The value should generally be below 1.0.")]
[DependencyProperty<double>("BabaGroupLabelFontScale", DefaultValue = .6, DocSummary = "Indicates the font scale of baba group characters. The value should generally be below 1.0.")]
[DependencyProperty<double>("CoordinateLabelFontScale", DefaultValue = .4, DocSummary = "Indicates the coordinate label font scale. The value should generally be below 1.0.")]
[DependencyProperty<double>("HighlightCandidateCircleScale", DefaultValue = .9, DocSummary = "Indicates the scale of highlighted candidate circles. The value should generally be below 1.0.")]
[DependencyProperty<double>("HighlightBackgroundOpacity", DefaultValue = .15, DocSummary = "Indicates the opacity of the background highlighted elements. The value should generally be below 1.0.")]
[DependencyProperty<double>("ChainStrokeThickness", DefaultValue = 2.0, DocSummary = "Indicates the chain stroke thickness.")]
[DependencyProperty<int>("SelectedCell", DocSummary = "Indicates the currently selected cell.")]
[DependencyProperty<CoordinateLabelDisplayKind>("CoordinateLabelDisplayKind", DefaultValue = CoordinateLabelDisplayKind.RxCy, DocSummary = "Indicates the displaying kind of coordinate labels.", DocRemarks = "For more information please visit <see cref=\"Interaction.CoordinateLabelDisplayKind\"/>.")]
[DependencyProperty<CoordinateLabelDisplayMode>("CoordinateLabelDisplayMode", DefaultValue = CoordinateLabelDisplayMode.UpperAndLeft, DocSummary = "Indicates the displaying mode of coordinate labels.", DocRemarks = "For more information please visit <see cref=\"Interaction.CoordinateLabelDisplayMode\"/>.")]
[DependencyProperty<Color>("GivenColor", DocSummary = "Indicates the given color.")]
[DependencyProperty<Color>("ModifiableColor", DocSummary = "Indicates the modifiable color.")]
[DependencyProperty<Color>("PencilmarkColor", DocSummary = "Indicates the pencilmark color.")]
[DependencyProperty<Color>("CoordinateLabelColor", DocSummary = "Indicates the coordinate label color.")]
[DependencyProperty<Color>("BabaGroupLabelColor", DocSummary = "Indicates the baba group label color.")]
[DependencyProperty<Color>("DeltaCandidateColor", DocSummary = "Indicates the color that is used for displaying candidates that are wrongly removed, but correct.")]
[DependencyProperty<Color>("DeltaCellColor", DocSummary = "Indicates the color that is used for displaying cell digits that are wrongly filled.")]
[DependencyProperty<Color>("BorderColor", DocSummary = "Indicates the border color.")]
[DependencyProperty<Color>("CursorBackgroundColor", DocSummary = "Indicates the cursor background color.")]
[DependencyProperty<Color>("LinkColor", DocSummary = "Indicates the link color.")]
[DependencyProperty<Color>("NormalColor", DocSummary = "Indicates the normal color.")]
[DependencyProperty<Color>("AssignmentColor", DocSummary = "Indicates the assignment color.")]
[DependencyProperty<Color>("OverlappedAssignmentColor", DocSummary = "Indicates the overlapped assignment color.")]
[DependencyProperty<Color>("EliminationColor", DocSummary = "Indicates the elimination color.")]
[DependencyProperty<Color>("CannibalismColor", DocSummary = "Indicates the cannibalism color.")]
[DependencyProperty<Color>("ExofinColor", DocSummary = "Indicates the exofin color.")]
[DependencyProperty<Color>("EndofinColor", DocSummary = "Indicates the endofin color.")]
[DependencyProperty<DashArray>("StrongLinkDashStyle", DocSummary = "Indicates the dash style of the strong links.")]
[DependencyProperty<DashArray>("WeakLinkDashStyle", DocSummary = "Indicates the dash style of the weak links.")]
[DependencyProperty<DashArray>("CycleLikeLinkDashStyle", DocSummary = "Indicates the dash style of the cycle-like technique links.")]
[DependencyProperty<DashArray>("OtherLinkDashStyle", DocSummary = "Indicates the dash style of the other links.")]
[DependencyProperty<FontFamily>("GivenFont", DocSummary = "Indicates the given font.")]
[DependencyProperty<FontFamily>("ModifiableFont", DocSummary = "Indicates the modifiable font.")]
[DependencyProperty<FontFamily>("PencilmarkFont", DocSummary = "Indicates the candidate font.")]
[DependencyProperty<FontFamily>("CoordinateLabelFont", DocSummary = "Indicates the coordinate label font.")]
[DependencyProperty<FontFamily>("BabaGroupLabelFont", DocSummary = "Indicates the baba group label font.")]
[DependencyProperty<ViewUnit>("ViewUnit", IsNullable = true, DocSummary = "Indicates the view unit used.")]
[DependencyProperty<ColorPalette>("AuxiliaryColors", DocSummary = "Indicates the auxiliary colors.")]
[DependencyProperty<ColorPalette>("DifficultyLevelForegrounds", DocSummary = "Indicates the foreground colors of all 6 kinds of difficulty levels.")]
[DependencyProperty<ColorPalette>("DifficultyLevelBackgrounds", DocSummary = "Indicates the background colors of all 6 kinds of difficulty levels.")]
[DependencyProperty<ColorPalette>("UserDefinedColorPalette", DocSummary = "Indicates the user-defined colors used by customized views.")]
[DependencyProperty<ColorPalette>("AlmostLockedSetsColors", DocSummary = "Indicates the colors applied to technique structure Almost Locked Sets.")]
public sealed partial class SudokuPane : UserControl, INotifyPropertyChanged
{
	[DefaultValue]
	private static readonly Color GivenColorDefaultValue = Colors.Black;

	[DefaultValue]
	private static readonly Color ModifiableColorDefaultValue = Colors.Blue;

	[DefaultValue]
	private static readonly Color PencilmarkColorDefaultValue = new() { A = 255, R = 100, G = 100, B = 100 };

	[DefaultValue]
	private static readonly Color CoordinateLabelColorDefaultValue = new() { A = 255, R = 100, G = 100, B = 100 };

	[DefaultValue]
	private static readonly Color BabaGroupLabelColorDefaultValue = Colors.Red;

	[DefaultValue]
	private static readonly Color DeltaCandidateColorDefaultValue = Color.FromArgb(255, 255, 185, 185);

	[DefaultValue]
	private static readonly Color DeltaCellColorDefaultValue = new() { A = 255, R = 255 };

	[DefaultValue]
	private static readonly Color BorderColorDefaultValue = Colors.Black;

	[DefaultValue]
	private static readonly Color CursorBackgroundColorDefaultValue = new() { A = 32, B = 255 };

	[DefaultValue]
	private static readonly Color LinkColorDefaultValue = Colors.Red;

	[DefaultValue]
	private static readonly Color NormalColorDefaultValue = Color.FromArgb(255, 63, 218, 101);

	[DefaultValue]
	private static readonly Color AssignmentColorDefaultValue = Color.FromArgb(255, 63, 218, 101);

	[DefaultValue]
	private static readonly Color OverlappedAssignmentColorDefaultValue = Color.FromArgb(255, 0, 255, 204);

	[DefaultValue]
	private static readonly Color EliminationColorDefaultValue = Color.FromArgb(255, 255, 118, 132);

	[DefaultValue]
	private static readonly Color CannibalismColorDefaultValue = new() { A = 255, R = 235 };

	[DefaultValue]
	private static readonly Color ExofinColorDefaultValue = Color.FromArgb(255, 127, 187, 255);

	[DefaultValue]
	private static readonly Color EndofinColorDefaultValue = Color.FromArgb(255, 216, 178, 255);

	[DefaultValue]
	private static readonly DashArray StrongLinkDashStyleDefaultValue = new();

	[DefaultValue]
	private static readonly DashArray WeakLinkDashStyleDefaultValue = new(3, 1.5);

	[DefaultValue]
	private static readonly DashArray CycleLikeLinkDashStyleDefaultValue = new();

	[DefaultValue]
	private static readonly DashArray OtherLinkDashStyleDefaultValue = new(3, 3);

	[DefaultValue]
	private static readonly FontFamily GivenFontDefaultValue = new("Tahoma");

	[DefaultValue]
	private static readonly FontFamily ModifiableFontDefaultValue = new("Tahoma");

	[DefaultValue]
	private static readonly FontFamily PencilmarkFontDefaultValue = new("Tahoma");

	[DefaultValue]
	private static readonly FontFamily CoordinateLabelFontDefaultValue = new("Tahoma");

	[DefaultValue]
	private static readonly FontFamily BabaGroupLabelFontDefaultValue = new("Times New Roman");

	[DefaultValue]
	private static readonly ColorPalette AuxiliaryColorsDefaultValue = new()
	{
		Color.FromArgb(255, 255, 192,  89),
		Color.FromArgb(255, 127, 187, 255),
		Color.FromArgb(255, 216, 178, 255)
	};

	[DefaultValue]
	private static readonly ColorPalette DifficultyLevelForegroundsDefaultValue = new()
	{
		Color.FromArgb(255,   0,  51, 204),
		Color.FromArgb(255,   0, 102,   0),
		Color.FromArgb(255, 102,  51,   0),
		Color.FromArgb(255, 102,  51,   0),
		Color.FromArgb(255, 102,   0,   0),
		Colors.Black
	};

	[DefaultValue]
	private static readonly ColorPalette DifficultyLevelBackgroundsDefaultValue = new()
	{
		Color.FromArgb(255, 204, 204, 255),
		Color.FromArgb(255, 100, 255, 100),
		Color.FromArgb(255, 255, 255, 100),
		Color.FromArgb(255, 255, 150,  80),
		Color.FromArgb(255, 255, 100, 100),
		Color.FromArgb(255, 220, 220, 220)
	};

	[DefaultValue]
	private static readonly ColorPalette UserDefinedColorPaletteDefaultValue = new()
	{
		Color.FromArgb(255,  63, 218, 101), // Green
		Color.FromArgb(255, 255, 192,  89), // Orange
		Color.FromArgb(255, 127, 187, 255), // Skyblue
		Color.FromArgb(255, 216, 178, 255), // Purple
		Color.FromArgb(255, 197, 232, 140), // Yellowgreen
		Color.FromArgb(255, 255, 203, 203), // Light red
		Color.FromArgb(255, 178, 223, 223), // Blue green
		Color.FromArgb(255, 252, 220, 165), // Light orange
		Color.FromArgb(255, 255, 255, 150), // Yellow
		Color.FromArgb(255, 247, 222, 143), // Golden yellow
		Color.FromArgb(255, 220, 212, 252), // Purple
		Color.FromArgb(255, 255, 118, 132), // Red
		Color.FromArgb(255, 206, 251, 237), // Light skyblue
		Color.FromArgb(255, 215, 255, 215), // Light green
		Color.FromArgb(255, 192, 192, 192) // Gray
	};

	[DefaultValue]
	private static readonly ColorPalette AlmostLockedSetsColorsDefaultValue = new()
	{
		Color.FromArgb(255, 255, 203, 203),
		Color.FromArgb(255, 178, 223, 223),
		Color.FromArgb(255, 252, 220, 165),
		Color.FromArgb(255, 255, 255, 150),
		Color.FromArgb(255, 247, 222, 143)
	};


	/// <summary>
	/// Defines a pair of stacks that stores undo and redo steps.
	/// </summary>
	internal readonly ObservableStack<Grid> _undoStack = new(), _redoStack = new();

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
	/// Indicates the target puzzle.
	/// </summary>
	private Grid _puzzle = Grid.Empty;


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

		set => SetPuzzle(value, true, true);
	}

	/// <summary>
	/// Indicates the base page.
	/// </summary>
	public AnalyzePage BasePage { get; set; } = null!;

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
	/// Indicates the event that is triggered when a digit is input (that cause a change in a cell).
	/// </summary>
	public event DigitInputEventHandler? DigitInput;

	/// <summary>
	/// Indicates the event that is triggered when the internal grid is updated with the specified behavior,
	/// such as removed a candidate, filled with a digit, etc..
	/// </summary>
	public event GridUpdatedEventHandler? GridUpdated;

	/// <summary>
	/// Indicates the event that is triggered when the mouse wheel is changed.
	/// </summary>
	public event SudokuPaneMouseWheelChangedEventHandler? MouseWheelChanged;


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

		GridUpdated?.Invoke(this, new(GridUpdatedBehavior.Undoing, target));
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

		GridUpdated?.Invoke(this, new(GridUpdatedBehavior.Redoing, target));
	}

	/// <summary>
	/// Copies the current grid as text into the clipboard.
	/// </summary>
	public void Copy()
	{
		if (FocusState == FocusState.Unfocused)
		{
			return;
		}

		if (Puzzle is var puzzle and ({ IsUndefined: true } or { IsEmpty: true }))
		{
			return;
		}

		var dataPackage = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
		dataPackage.SetText(puzzle.ToString(SusserFormat.Full));

		Clipboard.SetContent(dataPackage);
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
	public void SetPuzzle(scoped in Grid value, bool clearStack = false, bool clearAnalyzeTabData = true)
		=> SetPuzzle(value, clearStack: clearStack, whileUndoingOrRedoing: false, clearAnalyzeTabData: clearAnalyzeTabData);

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
		if (FocusState == FocusState.Unfocused)
		{
			return;
		}

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
		if (FocusState == FocusState.Unfocused)
		{
			return;
		}

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
	/// Triggers <see cref="GridUpdated"/> event. This method can only be called by internal control type <see cref="SudokuPaneCell"/>.
	/// </summary>
	/// <param name="behavior">The behavior.</param>
	/// <param name="value">The new value to assign.</param>
	/// <seealso cref="SudokuPaneCell"/>
	internal void TriggerGridUpdated(GridUpdatedBehavior behavior, object value) => GridUpdated?.Invoke(this, new(behavior, value));

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

	/// <summary>
	/// <inheritdoc cref="SetPuzzle(in Grid, bool, bool)" path="/summary"/>
	/// </summary>
	/// <param name="value">
	/// <inheritdoc cref="SetPuzzle(in Grid, bool, bool)" path="/param[@name='value']"/>
	/// </param>
	/// <param name="clearStack">
	/// <inheritdoc cref="SetPuzzle(in Grid, bool, bool)" path="/param[@name='clearStack']"/>
	/// </param>
	/// <param name="whileUndoingOrRedoing">
	/// <para>Indicates whether the method is called while undoing and redoing.</para>
	/// <para>The default value is <see langword="false"/>.</para>
	/// </param>
	/// <param name="clearAnalyzeTabData">
	/// <inheritdoc cref="SetPuzzle(in Grid, bool, bool)" path="/param[@name='clearAnalyzeTabData']"/>
	/// </param>
	private void SetPuzzle(scoped in Grid value, bool clearStack = false, bool whileUndoingOrRedoing = false, bool clearAnalyzeTabData = true)
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

		GridUpdated?.Invoke(this, new(GridUpdatedBehavior.Replacing, value));
	}


	[Callback]
	private static void DisplayCandidatesPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (e is not { NewValue: bool value })
		{
			return;
		}

		((App)Application.Current).Preference.UIPreferences.DisplayCandidates = value;
	}

	[Callback]
	private static void ViewUnitPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is not (SudokuPane pane, { NewValue: var rawValue and (null or ViewUnit _) }))
		{
			return;
		}

		ViewUnitFrameworkElementFactory.RemoveViewUnitControls(pane);

		if (rawValue is ViewUnit value)
		{
			ViewUnitFrameworkElementFactory.AddViewUnitControls(pane, value);
		}
	}


	private void UserControl_PointerEntered(object sender, PointerRoutedEventArgs e) => Focus(FocusState.Programmatic);

	private void UserControl_DragOver(object sender, DragEventArgs e)
	{
		e.AcceptedOperation = DataPackageOperation.Copy;
		e.DragUIOverride.Caption = GetString("SudokuPane_DropSudokuFileHere");
		e.DragUIOverride.IsCaptionVisible = true;
		e.DragUIOverride.IsContentVisible = true;
	}

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
						switch (SystemPath.GetExtension(filePath))
						{
							case CommonFileExtensions.PlainText:
							{
								var content = await FileIO.ReadTextAsync(file);
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
								switch (SudokuFileHandler.Read(filePath))
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

	private void UserControl_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
	{
		var pointerPoint = e.GetCurrentPoint((UIElement)sender);
		if (pointerPoint.Properties.MouseWheelDelta is not (var delta and not 0))
		{
			return;
		}

		MouseWheelChanged?.Invoke(this, new(delta < 0));

		e.Handled = true;
	}

	private void UserControl_KeyDown(object sender, KeyRoutedEventArgs e)
	{
		switch (Keyboard.GetModifierStatusForCurrentThread(), SelectedCell, e.Key, Keyboard.GetInputDigit(e.Key))
		{
			default:
			case (_, not (>= 0 and < 81), _, _):
			case (_, var cell, _, _) when Puzzle.GetStatus(cell) == CellStatus.Given:
			case (_, _, _, -2):
			{
				return;
			}
			case ({ AllFalse: true }, var cell, _, -1):
			{
				var modified = Puzzle;
				modified[cell] = -1;

				GridUpdated?.Invoke(this, new(GridUpdatedBehavior.Clear, cell));

				SetPuzzle(modified, false, true);

				DigitInput?.Invoke(this, new(cell, -1));

				break;
			}
			case ((false, true, false, false), var cell, _, var digit) when Puzzle.Exists(cell, digit) is true:
			{
				var modified = Puzzle;
				modified[cell, digit] = false;

				SetPuzzle(modified, false, true);

				GridUpdated?.Invoke(this, new(GridUpdatedBehavior.Elimination, cell * 9 + digit));

				break;
			}
			case ({ AllFalse: true }, var cell, _, var digit)
			when PreventConflictingInput && !Puzzle.DuplicateWith(cell, digit) || !PreventConflictingInput:
			{
				var modified = Puzzle;
				if (Puzzle.GetStatus(cell) == CellStatus.Modifiable)
				{
					// Temporarily re-compute candidates.
					modified[cell] = -1;
				}

				modified[cell] = digit;

				SetPuzzle(modified, false, true);

				DigitInput?.Invoke(this, new(cell, digit));
				GridUpdated?.Invoke(this, new(GridUpdatedBehavior.Assignment, cell * 9 + digit));

				break;
			}
		}
	}
}
