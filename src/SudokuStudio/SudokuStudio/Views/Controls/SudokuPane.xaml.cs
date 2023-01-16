namespace SudokuStudio.Views.Controls;

/// <summary>
/// Defines a sudoku pane control.
/// </summary>
public sealed partial class SudokuPane : UserControl, INotifyPropertyChanged
{
	/// <summary>
	/// Defines a pair of stacks that stores undo and redo steps.
	/// </summary>
	private readonly Stack<Grid> _undoStack = new(), _redoStack = new();

	/// <summary>
	/// Indicates whether the pane displays for candidates.
	/// </summary>
	[NotifyPropertyChangedBackingField]
	private bool _displayCandidates = true;

	/// <summary>
	/// Indicates whether the pane displays for delta digits using different colors.
	/// </summary>
	[NotifyPropertyChangedBackingField]
	private bool _useDifferentColorToDisplayDeltaDigits = true;

	/// <summary>
	/// Indicates the font scale of value digits (given or modifiable ones). The value should generally be below 1.0.
	/// </summary>
	[NotifyPropertyChangedBackingField]
	private double _valueFontScale = 1.0;

	/// <summary>
	/// Indicates the font scale of pencilmark digits (candidates). The value should generally be below 1.0.
	/// </summary>
	[NotifyPropertyChangedBackingField]
	private double _pencilmarkFontScale = 0.33;

	/// <summary>
	/// Indicates the coordinate label font scale.
	/// </summary>
	[NotifyPropertyChangedBackingField]
	private double _coordinateLabelFontScale = 0.4;

	/// <summary>
	/// Indicates the currently selected cell.
	/// </summary>
	[NotifyPropertyChangedBackingField]
	private int _selectedCell;

	/// <summary>
	/// Indicates the displaying kind of coordinate labels.
	/// </summary>
	/// <remarks>
	/// For more information please visit <see cref="Interaction.CoordinateLabelDisplayKind"/>.
	/// </remarks>
	/// <seealso cref="Interaction.CoordinateLabelDisplayKind"/>
	[NotifyPropertyChangedBackingField]
	private CoordinateLabelDisplayKind _coordinateLabelDisplayKind = CoordinateLabelDisplayKind.RxCy;

	/// <summary>
	/// Indicates the displaying mode of coordinate labels.
	/// </summary>
	/// <remarks>
	/// For more information please visit <see cref="Interaction.CoordinateLabelDisplayMode"/>.
	/// </remarks>
	/// <seealso cref="Interaction.CoordinateLabelDisplayMode"/>
	[NotifyPropertyChangedBackingField]
	private CoordinateLabelDisplayMode _coordinateLabelDisplayMode = CoordinateLabelDisplayMode.UpperAndLeft;

	/// <summary>
	/// Indicates the given color.
	/// </summary>
	[NotifyPropertyChangedBackingField]
	private Color _givenColor = Colors.Black;

	/// <summary>
	/// Indicates the modifiable color.
	/// </summary>
	[NotifyPropertyChangedBackingField]
	private Color _modifiableColor = Colors.Blue;

	/// <summary>
	/// Indicates the pencilmark color.
	/// </summary>
	[NotifyPropertyChangedBackingField]
	private Color _pencilmarkColor = new() { A = 255, R = 100, G = 100, B = 100 };

	/// <summary>
	/// Indicates the coordinate label color.
	/// </summary>
	[NotifyPropertyChangedBackingField]
	private Color _coordinateLabelColor = new() { A = 255, R = 100, G = 100, B = 100 };

	/// <summary>
	/// Indicates the color that is used for displaying candidates that are wrongly removed, but correct.
	/// </summary>
	[NotifyPropertyChangedBackingField]
	private Color _deltaCandidateColor = new() { A = 255, R = 255, G = 185, B = 185 };

	/// <summary>
	/// Indicates the color that is used for displaying cell digits that are wrongly filled.
	/// </summary>
	[NotifyPropertyChangedBackingField]
	private Color _deltaCellColor = new() { A = 255, R = 255 };

	/// <summary>
	/// Indicates the border color.
	/// </summary>
	[NotifyPropertyChangedBackingField]
	private Color _borderColor = Colors.Black;

	/// <inheritdoc cref="Puzzle"/>
	private Grid _puzzle = Grid.Empty;

	/// <summary>
	/// Indicates the value font.
	/// </summary>
	[NotifyPropertyChangedBackingField]
	private FontFamily _valueFont = new("Tahoma");

	/// <summary>
	/// Indicates the candidate font.
	/// </summary>
	[NotifyPropertyChangedBackingField]
	private FontFamily _pencilmarkFont = new("Tahoma");

	/// <summary>
	/// Indicates the coordinate label font.
	/// </summary>
	[NotifyPropertyChangedBackingField]
	private FontFamily _coordinateLabelFont = new("Tahoma");

	/// <summary>
	/// The easy entry to visit children <see cref="SudokuPaneCell"/> instances. This field contains 81 elements,
	/// indicating controls being displayed as 81 cells in a sudoku grid respectively.
	/// </summary>
	private SudokuPaneCell[] _children;


	/// <summary>
	/// Initializes a <see cref="SudokuPane"/> instance.
	/// </summary>
	public SudokuPane()
	{
		InitializeComponent();
		InitializeChildrenControls();
		UpdateCellData(_puzzle);
	}


	/// <summary>
	/// Indicates the target grid puzzle.
	/// </summary>
	public Grid Puzzle
	{
		get => _puzzle;

		set => SetPuzzle(value, true);
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
	/// Indicates the event that is triggered when a file is successfully received via dropped file.
	/// </summary>
	public event SuccessfullyReceivedDroppedFileEventHandler? SuccessfullyReceivedDroppedFile;

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
		SetPuzzle(target, whileOndoingOrRedoing: true);
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
		SetPuzzle(target, whileOndoingOrRedoing: true);
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
	/// Try to set puzzle, with a <see cref="bool"/> value indicating whether the stack fields <see cref="_undoStack"/>
	/// and <see cref="_redoStack"/> will be cleared.
	/// </summary>
	/// <param name="value">The newer grid.</param>
	/// <param name="clearStack">
	/// <para>Indicates whether the stack fields <see cref="_undoStack"/> and <see cref="_redoStack"/> will be cleared.</para>
	/// <para>The default value is <see langword="false"/>.</para>
	/// </param>
	/// <param name="whileOndoingOrRedoing">Indicates whether the current operation occurred while undoing and redoing a grid step.</param>
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
	internal void SetPuzzle(scoped in Grid value, bool clearStack = false, bool whileOndoingOrRedoing = false)
	{
		if (_puzzle == value)
		{
			return;
		}

		if (!whileOndoingOrRedoing && !clearStack)
		{
			_undoStack.Push(_puzzle);
		}

		_puzzle = value;

		UpdateCellData(value);
		switch (clearStack, whileOndoingOrRedoing)
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

		PropertyChanged?.Invoke(this, new(nameof(Puzzle)));
	}

	/// <summary>
	/// To initialize <see cref="GridCellData"/> values via the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
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


	private void UserControl_Loaded(object sender, RoutedEventArgs e) => Focus(FocusState.Programmatic);

	private void UserControl_KeyDown(object sender, KeyRoutedEventArgs e)
	{
		/// Please note that the parent control may use globalized hotkeys to control some behaviors.
		/// If <c>e.Handled</c> is not set <see langword="false"/> value before exited parent <c>KeyDown</c> method,
		/// this method will not be triggered and executed.
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
			case ((false, false, false, false), var cell, var digit) when !Puzzle.DupliateWith(cell, digit):
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
			case [StorageFolder folder]
#pragma warning disable format
			when await folder.GetFilesAsync(CommonFileQuery.DefaultQuery, 0, 2) is
			[
				StorageFile
				{
					FileType: CommonFileExtensions.Text or CommonFileExtensions.PlainText,
					Path: var path
				}
			]:
#pragma warning restore format
			{
				await handleSudokuFile(path);

				break;
			}
			case [StorageFile { FileType: CommonFileExtensions.Text or CommonFileExtensions.PlainText, Path: var path }]:
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
						var content = await File.ReadAllTextAsync(path);
						if (string.IsNullOrWhiteSpace(content))
						{
							return;
						}

						if (!Grid.TryParse(content, out var grid))
						{
							return;
						}

						Puzzle = grid;

						SuccessfullyReceivedDroppedFile?.Invoke(this, new());
						break;
					}
				}
			}
		}
	}
}
