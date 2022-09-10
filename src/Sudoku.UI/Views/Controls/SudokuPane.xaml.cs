namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Defines a user control that interacts with a sudoku grid.
/// </summary>
public sealed partial class SudokuPane : UserControl, INotifyPropertyChanged
{
	/// <summary>
	/// Indicates the delta that is used for checking whether two <see cref="double"/> values are same
	/// or their difference is below to the delta value.
	/// </summary>
	private const double Epsilon = 1E-2;


	/// <summary>
	/// Indicates the inner collection that stores the drawing elements, and also influences the controls
	/// displaying in the window.
	/// </summary>
	private readonly DrawingElementBag _drawingElements = new();

	/// <summary>
	/// Indicates the compositor.
	/// </summary>
	private readonly Compositor _compositor = CompositionTarget.GetCompositorForCurrentThread();

	/// <summary>
	/// Indicates whether the control is loading at the first time.
	/// </summary>
	private bool _isFirstLoading = true;

	/// <summary>
	/// Indicates the size that the current pane is, which is the backing field
	/// of the property <see cref="Size"/>.
	/// </summary>
	/// <seealso cref="Size"/>
	private double _size;

	/// <summary>
	/// Indicates the outside offset value, which is the backing field
	/// of the property <see cref="OutsideOffset"/>.
	/// </summary>
	/// <seealso cref="OutsideOffset"/>
	private double _outsideOffset;

	/// <summary>
	/// Indicates the cell being focused.
	/// </summary>
	/// <remarks>
	/// The variable is used for the following members:
	/// <list type="bullet">
	/// <item><see cref="OnPointerMoved(PointerRoutedEventArgs)"/></item>
	/// <item><see cref="OnKeyDown(KeyRoutedEventArgs)"/></item>
	/// </list>
	/// </remarks>
	private int _cell;

	/// <summary>
	/// Indicates the candidate being focused.
	/// </summary>
	/// <remarks>
	/// The variable is used for the following members:
	/// <list type="bullet">
	/// <item><see cref="OnPointerMoved(PointerRoutedEventArgs)"/></item>
	/// <item><see cref="OnKeyDown(KeyRoutedEventArgs)"/></item>
	/// </list>
	/// </remarks>
	private int _candidate;

	/// <summary>
	/// Indicates the spring animation.
	/// </summary>
	private SpringVector3NaturalMotionAnimation _springAnimation = null!;


	/// <summary>
	/// Initializes a <see cref="SudokuPane"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SudokuPane() => InitializeComponent();


	/// <summary>
	/// Gets or sets the size of the pane.
	/// </summary>
	public double Size
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _size;

		set
		{
			if (_size.NearlyEquals(value, Epsilon))
			{
				return;
			}

			_size = value;
			foreach (var drawingElement in _drawingElements.OfType<CellLine, BlockLine>())
			{
				drawingElement.DynamicAssign(
					instance =>
					{
						instance.OutsideOffset = OutsideOffset;
						instance.PaneSize = value;
					}
				);
			}
		}
	}

	/// <summary>
	/// Gets or sets the outside offset to the view model.
	/// </summary>
	public double OutsideOffset
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _outsideOffset;

		set
		{
			if (_outsideOffset.NearlyEquals(value, Epsilon))
			{
				return;
			}

			_outsideOffset = value;
			foreach (var drawingElement in _drawingElements.OfType<CellLine, BlockLine>())
			{
				drawingElement.DynamicAssign(
					instance =>
					{
						instance.OutsideOffset = value;
						instance.PaneSize = Size;
					}
				);
			}
		}
	}

	/// <summary>
	/// Indicates the current cell focused. The default value is -1.
	/// </summary>
	public int CurrentCell { get; internal set; } = -1;

	/// <summary>
	/// Indicates the number of the total undo steps.
	/// </summary>
	public int UndoStepsCount => GetSudokuGridViewModel().UndoStepsCount;

	/// <summary>
	/// Indicates the number of the total redo steps.
	/// </summary>
	public int RedoStepsCount => GetSudokuGridViewModel().RedoStepsCount;

	/// <summary>
	/// Gets or sets the current used grid.
	/// </summary>
	public Grid Grid
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => GetSudokuGridViewModel().Grid;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			GetSudokuGridViewModel().Grid = value;

			GridRefreshed?.Invoke(this, null);
		}
	}


	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;

	/// <summary>
	/// Indicates an event that is triggered when the grid is refreshed (changed, covered, etc.).
	/// </summary>
	public event EventHandler<object?>? GridRefreshed;

	/// <summary>
	/// Indicates an event that is triggered when a file is successfully received using drag and drop operation.
	/// </summary>
	public event EventHandler<object?>? SuccessfullyReceivedDroppedFile;

	/// <summary>
	/// Indicates an event that is triggered when a file is failed to be received using drag and drop operation.
	/// </summary>
	public event EventHandler<FailedReceivedDroppedFileEventArgs>? FailedReceivedDroppedFile;


	/// <summary>
	/// Undo a step.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void UndoStep() => GetSudokuGridViewModel().Undo();

	/// <summary>
	/// Redo a step.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RedoStep() => GetSudokuGridViewModel().Redo();

	/// <summary>
	/// To make the cell fill the digit.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void MakeDigit(int cell, int digit) => GetSudokuGridViewModel().MakeDigit(cell, digit);

	/// <summary>
	/// To eliminate the digit from the grid.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void EliminateDigit(int cell, int digit) => GetSudokuGridViewModel().EliminateDigit(cell, digit);

	/// <summary>
	/// To set the mark at the specified cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="colorPaletteIndex">
	/// The index that corresponds to the real color stored in the palette.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetCellMark(int cell, int colorPaletteIndex)
	{
		var (a, r, g, b) = ((App)Application.Current).UserPreference.PaletteColors[colorPaletteIndex];
		var node = new CellViewNode(Identifier.FromColor(a, r, g, b), cell);
		switch (GetDisplayableUnit())
		{
			case null:
			{
				SetVisual(new UserDefinedVisual { node });

				break;
			}
			case UserDefinedVisual view:
			{
				view.Add(node);

				// Refresh the view. This is just a trick.
				SetVisual(view);

				break;
			}
		}
	}

	/// <summary>
	/// To set the mark at the specified candidate.
	/// </summary>
	/// <param name="candidate">The candidate.</param>
	/// <param name="colorPaletteIndex">
	/// The index that corresponds to the real color stored in the palette.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetCandidateMark(int candidate, int colorPaletteIndex)
	{
		var (a, r, g, b) = ((App)Application.Current).UserPreference.PaletteColors[colorPaletteIndex];
		var node = new CandidateViewNode(Identifier.FromColor(a, r, g, b), candidate);
		switch (GetDisplayableUnit())
		{
			case null:
			{
				SetVisual(new UserDefinedVisual { node });

				break;
			}
			case UserDefinedVisual view:
			{
				view.Add(node);

				// Refresh the view. This is just a trick.
				SetVisual(view);

				break;
			}
		}
	}

	/// <summary>
	/// To fix the grid, to change all modifiable values to given ones.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FixGrid() => GetSudokuGridViewModel().FixGrid();

	/// <summary>
	/// To unfix the grid, to change all given values to modifiable ones.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void UnfixGrid() => GetSudokuGridViewModel().UnfixGrid();

	/// <summary>
	/// To reset the grid, to revert the grid to the initial status.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ResetGrid() => GetSudokuGridViewModel().ResetGrid();

	/// <summary>
	/// To replace the grid with the new one, in order to make the current operation undoable.
	/// </summary>
	/// <param name="grid">The grid to be replaced with.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ReplaceGridUndoable(scoped in Grid grid) => GetSudokuGridViewModel().ReplaceGrid(grid);

	/// <summary>
	/// Mask all value cells.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Mask() => GetSudokuGridViewModelNullable()?.Mask();

	/// <summary>
	/// Unmask all value cells.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Unmask() => GetSudokuGridViewModelNullable()?.Unmask();

	/// <summary>
	/// Shows the candidates.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ShowCandidates() => GetSudokuGridViewModel().UserShowCandidates = true;

	/// <summary>
	/// Hides the candidates.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void HideCandidates() => GetSudokuGridViewModel().UserShowCandidates = false;

	/// <summary>
	/// Sets the visual unit to be shown. The method will automatically displays for the first view
	/// in the view array.
	/// </summary>
	/// <param name="visual">The visual unit to be displayed.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetVisual(IVisual visual) => GetSudokuGridViewModel().Visual = visual;

	/// <inheritdoc cref="SudokuGrid.SetPreviousView"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetPreviousView() => GetSudokuGridViewModel().SetPreviousView();

	/// <inheritdoc cref="SudokuGrid.SetNextView"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetNextView() => GetSudokuGridViewModel().SetNextView();

	/// <inheritdoc cref="SudokuGrid.SkipToViewIndex"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SkipToViewIndex(int index) => GetSudokuGridViewModel().SkipToViewIndex(index);

	/// <summary>
	/// Clear all view nodes.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ClearViews() => GetSudokuGridViewModel().ClearViewNodes();

	/// <summary>
	/// Gets the view index.
	/// </summary>
	/// <returns>The view index.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetViewIndex() => GetSudokuGridViewModel().ViewIndex;

	/// <summary>
	/// Gets the current displaying unit.
	/// </summary>
	/// <returns>The current displaying unit.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IVisual? GetDisplayableUnit() => GetSudokuGridViewModel().Visual;

	/// <inheritdoc/>
	protected override void OnPointerMoved(PointerRoutedEventArgs e)
	{
		base.OnPointerMoved(e);

		var position = e.GetCurrentPoint(this).Position;
		_cell = PointConversions.GetCell(position, Size, OutsideOffset);
		_candidate = PointConversions.GetCandidate(position, Size, OutsideOffset);
		if (GetSudokuGridViewModelNullable() is { } sudokuGrid)
		{
			sudokuGrid.FocusedCell = _cell;
		}
	}

	/// <inheritdoc/>
	protected override void OnKeyDown(KeyRoutedEventArgs e)
	{
		base.OnKeyDown(e);

		// Here we just suppose a mode to fill with value in a cell:
		//     1. Gets the mouse position, and then get the position of the cell.
		//     2. Gets the pressed key from the argument 'e'.
		//     3. Fill the cell with the value.
		if (_cell == -1)
		{
			e.Handled = true;
			return;
		}

		var pressedData = ModifierKeyDownData.FromCurrentState();
		switch (e.Key)
		{
			case var key and (>= Key.Number0 and <= Key.Number9 or >= Key.NumberPad0 and <= Key.NumberPad9 or Key.Back):
			{
				handleDigitOperation(
					key,
					key is >= Key.Number0 and <= Key.Number9 ? Key.Number0 : Key.NumberPad0,
					pressedData
				);

				break;
			}
			default: // Other cases.
			{
				// Here we must set the value to 'false', in order to process keyboard accelerators.
				// If we set 'e.Handled = true', the keyboard accelerators in the base page won't be triggered.
				e.Handled = false;
				return;
			}


			void handleDigitOperation(Key key, Key firstDigit, ModifierKeyDownData pressedData)
			{
				var digit = key - firstDigit - 1;
				switch (pressedData)
				{
					case (false, true, false) when key != Key.Back: // Shift.
					{
						EliminateDigit(_cell, digit);
						break;
					}
					case (false, false, true): // Alt.
					{
						SetCellMark(_cell, digit);
						break;
					}
					case (false, true, true): // Shift + Alt.
					{
						SetCandidateMark(_candidate, digit);
						break;
					}
					case (false, false, false):
					{
						MakeDigit(_cell, key == Key.Back ? -1 : digit);
						break;
					}
				}
			}
		}
	}

	/// <inheritdoc/>
	protected override void OnRightTapped(RightTappedRoutedEventArgs e)
	{
		base.OnRightTapped(e);

		// Gets the mouse point and converts to the real cell value.
		var point = e.GetPosition(this);
		var cell = PointConversions.GetCell(point, Size, OutsideOffset);

		// Checks whether the cell value is valid.
		if (cell == -1)
		{
			return;
		}

		// Sets the tag with the cell value, in order to get the details by other methods later.
		CurrentCell = cell;

		// Try to create the menu flyout and show the items.
		for (var i = 0; i < 9; i++)
		{
			var digitExists = Grid.Exists(cell, i) is true;
			((Button)FindName($"_cButtonMake{i + 1}")).IsEnabled = digitExists;
			((Button)FindName($"_cButtonDelete{i + 1}")).IsEnabled = digitExists;
		}
	}

	/// <summary>
	/// Loads the grid if the program is opened via opening a file.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void LoadFirstGridIfWorth()
	{
		switch (((App)Application.Current).InitialInfo)
		{
			case { FirstGrid: { } grid } initialInfo:
			{
				// Sets the value.
				Grid = grid;

				// Remove the value to avoid re-triggering.
				initialInfo.FirstGrid = null;

				break;
			}
			case { DrawingDataRawValue: { } drawingDataRawValue } initialInfo:
			{
				var visual = Deserialize<UserDefinedVisual>(drawingDataRawValue, CamelCasing);
				if (visual is null)
				{
					// Something goes wrong.
					break;
				}

				// Sets the value.
				SetVisual(visual);

				// Remove the value to avoid re-triggering.
				initialInfo.DrawingDataRawValue = null;

				break;
			}
		}
	}

	/// <summary>
	/// Makes the loading operation completed, sets the field <see cref="_isFirstLoading"/>
	/// to <see langword="false"/> value.
	/// </summary>
	/// <seealso cref="SudokuPane_Loaded(object, RoutedEventArgs)"/>
	/// <seealso cref="_isFirstLoading"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void MakeLoadingOperationCompleted() => _isFirstLoading = false;

	/// <summary>
	/// Initializes <see cref="OutsideRectangle"/> instances, and append
	/// into the collection <see cref="_drawingElements"/>.
	/// </summary>
	/// <param name="up">The user preference instance.</param>
	/// <seealso cref="OutsideRectangle"/>
	/// <seealso cref="_drawingElements"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void InitializeOutsideRectangle(Preference up)
		=> _drawingElements.Add(
			new OutsideRectangle
			{
				PaneSize = Size,
				StrokeColor = up.OutsideBorderColor,
				FillColor = up.GridBackgroundFillColor,
				StrokeThickness = up.OutsideBorderWidth
			}
		);

	/// <summary>
	/// Initializes <see cref="CandidateLine"/>, <see cref="CellLine"/> and <see cref="BlockLine"/> instances,
	/// and append into the collection <see cref="_drawingElements"/>.
	/// </summary>
	/// <param name="up">The user preference instance.</param>
	/// <seealso cref="CandidateLine"/>
	/// <seealso cref="CellLine"/>
	/// <seealso cref="BlockLine"/>
	/// <seealso cref="_drawingElements"/>
	private void InitializeBorderLines(Preference up)
	{
		if (up.ShowCandidateBorderLines)
		{
			for (byte i = 0; i < 28; i++)
			{
				if (i % 3 == 0)
				{
					// Skips the overlapping lines.
					continue;
				}

				_drawingElements.Add(
					new CandidateLine
					{
						Order = i,
						PaneSize = Size,
						OutsideOffset = OutsideOffset,
						StrokeColor = up.CandidateBorderColor,
						StrokeThickness = up.CandidateBorderWidth
					}
				);
				_drawingElements.Add(
					new CandidateLine
					{
						Order = (byte)(i + 28),
						PaneSize = Size,
						OutsideOffset = OutsideOffset,
						StrokeColor = up.CandidateBorderColor,
						StrokeThickness = up.CandidateBorderWidth
					}
				);
			}
		}

		for (byte i = 0; i < 10; i++)
		{
			if (i % 3 == 0)
			{
				// Skips the overlapping lines.
				continue;
			}

			_drawingElements.Add(
				new CellLine
				{
					Order = i,
					PaneSize = Size,
					OutsideOffset = OutsideOffset,
					StrokeColor = up.CellBorderColor,
					StrokeThickness = up.CellBorderWidth
				}
			);
			_drawingElements.Add(
				new CellLine
				{
					Order = (byte)(i + 10),
					PaneSize = Size,
					OutsideOffset = OutsideOffset,
					StrokeColor = up.CellBorderColor,
					StrokeThickness = up.CellBorderWidth
				}
			);
		}

		for (byte i = 0; i < 4; i++)
		{
			_drawingElements.Add(
				new BlockLine
				{
					Order = i,
					PaneSize = Size,
					OutsideOffset = OutsideOffset,
					StrokeColor = up.BlockBorderColor,
					StrokeThickness = up.BlockBorderWidth
				}
			);
			_drawingElements.Add(
				new BlockLine
				{
					Order = (byte)(i + 4),
					PaneSize = Size,
					OutsideOffset = OutsideOffset,
					StrokeColor = up.BlockBorderColor,
					StrokeThickness = up.BlockBorderWidth
				}
			);
		}
	}

	/// <summary>
	/// Initializes <see cref="SudokuGrid"/> instances, and append into the collection <see cref="_drawingElements"/>.
	/// </summary>
	/// <param name="up">The user preference instance.</param>
	/// <seealso cref="SudokuGrid"/>
	/// <seealso cref="_drawingElements"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void InitializeSudokuGrid(Preference up)
		=> _drawingElements.Add(
			new SudokuGrid
			{
				Preference = up,
				PaneSize = Size,
				OutsideOffset = OutsideOffset,
				Grid = Grid.Empty,
				UndoRedoStepsUpdatedCallback = () =>
				{
					PropertyChanged?.Invoke(this, new(nameof(UndoStepsCount)));
					PropertyChanged?.Invoke(this, new(nameof(RedoStepsCount)));
				},
				FocusedCell = -1
			}
		);

	/// <summary>
	/// Adds the controls into the canvas.
	/// </summary>
	private void AddIntoCanvas()
	{
		foreach (var control in from drawingElement in _drawingElements select drawingElement.GetControl())
		{
			_cCanvasMain.Children.Add(control);
		}
	}

	/// <summary>
	/// To create or update the spring animation, updating the field <see cref="_springAnimation"/>.
	/// </summary>
	/// <param name="finalValue">The final value.</param>
	/// <seealso cref="_springAnimation"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CreateOrUpdateSpringAnimation(float finalValue)
	{
		if ((_springAnimation, _compositor) is (null, not null))
		{
			_springAnimation = _compositor.CreateSpringVector3Animation();
			_springAnimation.Target = nameof(Scale);
		}

		_springAnimation!.FinalValue = new(finalValue);
	}

	/// <summary>
	/// Gets the <see cref="SudokuGrid"/> instance as the view model.
	/// </summary>
	/// <returns>The <see cref="SudokuGrid"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private SudokuGrid GetSudokuGridViewModel() => _drawingElements.OfType<SudokuGrid>().Single();

	/// <summary>
	/// Gets the <see cref="SudokuGrid"/> instance as the view model, and return <see langword="null"/>
	/// if the collection doesn't contain any <see cref="SudokuGrid"/> elements.
	/// </summary>
	/// <returns>
	/// The <see cref="SudokuGrid"/> instance, or <see langword="null"/>
	/// if the collection doesn't contain any <see cref="SudokuGrid"/> elements.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private SudokuGrid? GetSudokuGridViewModelNullable() => _drawingElements.OfType<SudokuGrid>().FirstOrDefault();


	/// <summary>
	/// Triggers when the pane is loaded.
	/// </summary>
	/// <param name="sender">The object to trigger the event. The instance is always itself.</param>
	/// <param name="e">The event arguments provided.</param>
	private void SudokuPane_Loaded(object sender, RoutedEventArgs e)
	{
		// If the initialization operation is not the first time, we should skip the initialization.
		if (!_isFirstLoading)
		{
			return;
		}

		// Initialize controls.
		var up = ((App)Application.Current).UserPreference;

		InitializeOutsideRectangle(up);
		InitializeBorderLines(up);
		InitializeSudokuGrid(up);
		AddIntoCanvas();
		LoadFirstGridIfWorth();
		MakeLoadingOperationCompleted();
	}

	/// <summary>
	/// Triggers when the mouse is moved into the current control.
	/// </summary>
	/// <param name="sender">The object to trigger the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void SudokuPane_PointerEntered(object sender, PointerRoutedEventArgs e)
		// Set the focus to the control in order to trigger the 'KeyDown' event.
		=> Focus(FocusState.Programmatic);

	/// <summary>
	/// Triggers when a thing is dragging over the control.
	/// </summary>
	/// <param name="sender">The object to trigger the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void SudokuPane_DragOver(object sender, DragEventArgs e)
	{
		e.AcceptedOperation = DataPackageOperation.Copy;
		e.DragUIOverride.Caption = R["DropSudokuFileHere"]!;
		e.DragUIOverride.IsCaptionVisible = true;
		e.DragUIOverride.IsContentVisible = true;
	}

	/// <summary>
	/// Triggers when a thing is dropping over the control.
	/// </summary>
	/// <param name="sender">The object to trigger the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private async void SudokuPane_DropAsync(object sender, DragEventArgs e)
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
				var z = await folder.GetFilesAsync(CommonFileQuery.DefaultQuery, 0, 2);
				if (z is not [StorageFile { FileType: CommonFileExtensions.Text or CommonFileExtensions.Sudoku, Path: var path }])
				{
					break;
				}

				await handleSudokuFile(path);

				break;
			}
			case [StorageFile { FileType: CommonFileExtensions.Text or CommonFileExtensions.Sudoku, Path: var path }]:
			{
				await handleSudokuFile(path);

				break;
			}


			async Task handleSudokuFile(string path)
			{
				switch (new FileInfo(path).Length)
				{
					case 0:
					{
						FailedReceivedDroppedFile?.Invoke(this, new(FailedReceivedDroppedFileReason.FileIsEmpty));
						break;
					}
					case > 1024:
					{
						FailedReceivedDroppedFile?.Invoke(this, new(FailedReceivedDroppedFileReason.FileIsTooLarge));
						break;
					}
					default:
					{
						var content = await SioFile.ReadAllTextAsync(path);
						if (string.IsNullOrWhiteSpace(content))
						{
							return;
						}

						if (!Grid.TryParse(content, out var grid))
						{
							return;
						}

						Grid = grid;

						SuccessfullyReceivedDroppedFile?.Invoke(this, null);
						break;
					}
				}
			}
		}
	}

	/// <summary>
	/// Triggers when the target <see cref="MenuFlyoutItem"/> is clicked.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void MakeOrDeleteMenuItem_Click(object sender, RoutedEventArgs e)
	{
		if (sender is not Button { Tag: string s } || !int.TryParse(s, out var possibleDigit))
		{
			return;
		}

		if (possibleDigit > 0)
		{
			MakeDigit(CurrentCell, possibleDigit - 1);
		}
		else if (possibleDigit < 0)
		{
			EliminateDigit(CurrentCell, ~possibleDigit);
		}
	}

	/// <summary>
	/// Triggers when the cursor is entered into the control.
	/// </summary>
	/// <param name="sender">The object having triggered this event.</param>
	/// <param name="e">The event arguments provided.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void MakeOrDeleteMenuItem_PointerEntered(object sender, PointerRoutedEventArgs e)
	{
		if (sender is not Control { IsEnabled: true } uiElement)
		{
			return;
		}

		CreateOrUpdateSpringAnimation(1.1F);
		uiElement.StartAnimation(_springAnimation);
	}

	/// <summary>
	/// Triggers when the cursor is entered into the control.
	/// </summary>
	/// <param name="sender">The object having triggered this event.</param>
	/// <param name="e">The event arguments provided.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void MakeOrDeleteMenuItem_PointerExited(object sender, PointerRoutedEventArgs e)
	{
		if (sender is not Control { IsEnabled: true } uiElement)
		{
			return;
		}

		CreateOrUpdateSpringAnimation(1);
		uiElement.StartAnimation(_springAnimation);
	}
}
