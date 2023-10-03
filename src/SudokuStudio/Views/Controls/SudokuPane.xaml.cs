using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.SourceGeneration;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Sudoku.Analytics;
using Sudoku.Concepts;
using SudokuStudio.BindableSource;
using SudokuStudio.Collection;
using SudokuStudio.ComponentModel;
using SudokuStudio.Input;
using SudokuStudio.Interaction;
using SudokuStudio.Interaction.Conversions;
using SudokuStudio.Rendering;
using SudokuStudio.Storage;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI;
using WinRT;
using static System.Numerics.BitOperations;
using static Sudoku.Analytics.ConclusionType;
using static Sudoku.SolutionWideReadOnlyFields;
using static SudokuStudio.Strings.StringsAccessor;

namespace SudokuStudio.Views.Controls;

/// <summary>
/// Defines a sudoku pane control.
/// </summary>
[DependencyProperty<bool>("DisplayCandidates", DefaultValue = true, DocSummary = "Indicates whether the pane displays for candidates.")]
[DependencyProperty<bool>("DisplayCursors", DefaultValue = true, DocSummary = "Indicates whether the pane displays cursors that uses different colors to highlight some cells as peers of the target cell that is the one your mouse points to.")]
[DependencyProperty<bool>("UseDifferentColorToDisplayDeltaDigits", DefaultValue = true, DocSummary = "Indicates whether the pane displays for delta digits using different colors.")]
[DependencyProperty<bool>("DisableFlyout", DocSummary = "Indicates whether the pane disable flyout open.")]
[DependencyProperty<bool>("PreventConflictingInput", DefaultValue = true, DocSummary = "Indicates whether the pane prevent the simple conflict, which means, if you input a digit that is conflict with the digits in its containing houses, this pane will do nothing by this value being <see langword=\"true\"/>. If not, the pane won't check for any conflict and always allow you inputting the digit regardless of possible conflict.")]
[DependencyProperty<bool>("EnableUndoRedoStacking", DefaultValue = true, MembersNotNullWhenReturnsTrue = new[] { nameof(_redoStack), nameof(_undoStack) }, DocSummary = "Indicates whether the pane enables for undoing and redoing operation.")]
[DependencyProperty<bool>("EnableDoubleTapFilling", DefaultValue = true, DocSummary = "Indicates whether the digit will be automatically input by double tapping a candidate.")]
[DependencyProperty<bool>("EnableRightTapRemoving", DefaultValue = true, DocSummary = "Indicates whether the digit will be removed (eliminated) from the containing cell by tapping a candidate using right mouse button.")]
[DependencyProperty<bool>("EnableAnimationFeedback", DefaultValue = true, DocSummary = "Indicates whether sudoku pane enables for animation feedback.")]
[DependencyProperty<bool>("TransparentBackground", DocSummary = "Indicates whether sudoku pane does not use background color to display a sudoku puzzle.")]
[DependencyProperty<decimal>("HighlightCandidateCircleScale", DocSummary = "Indicates the scale of highlighted candidate circles. The value should generally be below 1.0.")]
[DependencyProperty<decimal>("HighlightBackgroundOpacity", DocSummary = "Indicates the opacity of the background highlighted elements. The value should generally be below 1.0.")]
[DependencyProperty<decimal>("ChainStrokeThickness", DocSummary = "Indicates the chain stroke thickness.")]
[DependencyProperty<decimal>("GivenFontScale", DocSummary = "Indicates the font scale of given digits. The value should generally be below 1.0.")]
[DependencyProperty<decimal>("ModifiableFontScale", DocSummary = "Indicates the font scale of modifiable digits. The value should generally be below 1.0.")]
[DependencyProperty<decimal>("PencilmarkFontScale", DocSummary = "Indicates the font scale of pencilmark digits (candidates). The value should generally be below 1.0.")]
[DependencyProperty<decimal>("BabaGroupLabelFontScale", DocSummary = "Indicates the font scale of baba group characters. The value should generally be below 1.0.")]
[DependencyProperty<decimal>("CoordinateLabelFontScale", DocSummary = "Indicates the coordinate label font scale. The value should generally be below 1.0.")]
[DependencyProperty<int>("HouseCompletedFeedbackDuration", DefaultValue = 800, DocSummary = "Indicates the duration of feedback when a house is completed.")]
[DependencyProperty<Cell>("SelectedCell", DocSummary = "Indicates the currently selected cell.")]
[DependencyProperty<CoordinateType>("CoordinateLabelDisplayKind", DefaultValue = CoordinateType.RxCy, DocSummary = "Indicates the displaying kind of coordinate labels.")]
[DependencyProperty<CoordinateLabelDisplayMode>("CoordinateLabelDisplayMode", DefaultValue = CoordinateLabelDisplayMode.UpperAndLeft, DocSummary = "Indicates the displaying mode of coordinate labels.", DocRemarks = "For more information please visit <see cref=\"Rendering.CoordinateLabelDisplayMode\"/>.")]
[DependencyProperty<CandidateViewNodeDisplayNode>("CandidateViewNodeDisplayMode", DefaultValue = CandidateViewNodeDisplayNode.CircleSolid, DocSummary = "Indicates the displaying mode of candidate view nodes.")]
[DependencyProperty<EliminationDisplayMode>("EliminationDisplayMode", DefaultValue = EliminationDisplayMode.CircleSolid, DocSummary = "Indicates the displaying mode of an elimination.")]
[DependencyProperty<CandidateMap>("ViewUnitUsedCandidates", DocSummary = "Indicates a map that stores candidates used in a view unit.")]
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
[DependencyProperty<Color>("HouseCompletedFeedbackColor", DocSummary = "Indicates the feedback color when a house is completed.")]
[DependencyProperty<DashArray>("StrongLinkDashStyle", DocSummary = "Indicates the dash style of the strong links.")]
[DependencyProperty<DashArray>("WeakLinkDashStyle", DocSummary = "Indicates the dash style of the weak links.")]
[DependencyProperty<DashArray>("CycleLikeLinkDashStyle", DocSummary = "Indicates the dash style of the cycle-like technique links.")]
[DependencyProperty<DashArray>("OtherLinkDashStyle", DocSummary = "Indicates the dash style of the other links.")]
[DependencyProperty<FontFamily>("GivenFont", DocSummary = "Indicates the given font.")]
[DependencyProperty<FontFamily>("ModifiableFont", DocSummary = "Indicates the modifiable font.")]
[DependencyProperty<FontFamily>("PencilmarkFont", DocSummary = "Indicates the candidate font.")]
[DependencyProperty<FontFamily>("CoordinateLabelFont", DocSummary = "Indicates the coordinate label font.")]
[DependencyProperty<FontFamily>("BabaGroupLabelFont", DocSummary = "Indicates the baba group label font.")]
[DependencyProperty<ViewUnitBindableSource>("ViewUnit", IsNullable = true, DocSummary = "Indicates the view unit used.")]
[DependencyProperty<ColorPalette>("AuxiliaryColors", DocSummary = "Indicates the auxiliary colors.")]
[DependencyProperty<ColorPalette>("DifficultyLevelForegrounds", DocSummary = "Indicates the foreground colors of all 6 kinds of difficulty levels.")]
[DependencyProperty<ColorPalette>("DifficultyLevelBackgrounds", DocSummary = "Indicates the background colors of all 6 kinds of difficulty levels.")]
[DependencyProperty<ColorPalette>("UserDefinedColorPalette", DocSummary = "Indicates the user-defined colors used by customized views.")]
[DependencyProperty<ColorPalette>("AlmostLockedSetsColors", DocSummary = "Indicates the colors applied to technique structure Almost Locked Sets.")]
public sealed partial class SudokuPane : UserControl, INotifyPropertyChanged
{
	[Default]
	private static readonly decimal HighlightCandidateCircleScaleDefaultValue = .9M;

	[Default]
	private static readonly decimal HighlightBackgroundOpacityDefaultValue = .15M;

	[Default]
	private static readonly decimal ChainStrokeThicknessDefaultValue = 2.0M;

	[Default]
	private static readonly decimal GivenFontScaleDefaultValue = 1.0M;

	[Default]
	private static readonly decimal ModifiableFontScaleDefaultValue = 1.0M;

	[Default]
	private static readonly decimal PencilmarkFontScaleDefaultValue = .33M;

	[Default]
	private static readonly decimal BabaGroupLabelFontScaleDefaultValue = .6M;

	[Default]
	private static readonly decimal CoordinateLabelFontScaleDefaultValue = .4M;

	[Default]
	private static readonly Color GivenColorDefaultValue = Colors.Black;

	[Default]
	private static readonly Color ModifiableColorDefaultValue = Colors.Blue;

	[Default]
	private static readonly Color PencilmarkColorDefaultValue = new() { A = 255, R = 100, G = 100, B = 100 };

	[Default]
	private static readonly Color CoordinateLabelColorDefaultValue = new() { A = 255, R = 100, G = 100, B = 100 };

	[Default]
	private static readonly Color BabaGroupLabelColorDefaultValue = Colors.Red;

	[Default]
	private static readonly Color DeltaCandidateColorDefaultValue = Color.FromArgb(255, 255, 185, 185);

	[Default]
	private static readonly Color DeltaCellColorDefaultValue = new() { A = 255, R = 255 };

	[Default]
	private static readonly Color BorderColorDefaultValue = Colors.Black;

	[Default]
	private static readonly Color CursorBackgroundColorDefaultValue = new() { A = 32, B = 255 };

	[Default]
	private static readonly Color LinkColorDefaultValue = Colors.Red;

	[Default]
	private static readonly Color NormalColorDefaultValue = Color.FromArgb(255, 63, 218, 101);

	[Default]
	private static readonly Color AssignmentColorDefaultValue = Color.FromArgb(255, 63, 218, 101);

	[Default]
	private static readonly Color OverlappedAssignmentColorDefaultValue = Color.FromArgb(255, 0, 255, 204);

	[Default]
	private static readonly Color EliminationColorDefaultValue = Color.FromArgb(255, 255, 118, 132);

	[Default]
	private static readonly Color CannibalismColorDefaultValue = new() { A = 255, R = 235 };

	[Default]
	private static readonly Color ExofinColorDefaultValue = Color.FromArgb(255, 127, 187, 255);

	[Default]
	private static readonly Color EndofinColorDefaultValue = Color.FromArgb(255, 216, 178, 255);

	[Default]
	private static readonly Color HouseCompletedFeedbackColorDefaultValue = Colors.HotPink;

	[Default]
	private static readonly CandidateMap ViewUnitUsedCandidatesDefaultValue = CandidateMap.Empty;

	[Default]
	private static readonly DashArray StrongLinkDashStyleDefaultValue = [];

	[Default]
	private static readonly DashArray WeakLinkDashStyleDefaultValue = [3, 1.5];

	[Default]
	private static readonly DashArray CycleLikeLinkDashStyleDefaultValue = [];

	[Default]
	private static readonly DashArray OtherLinkDashStyleDefaultValue = [3, 3];

	[Default]
	private static readonly FontFamily GivenFontDefaultValue = new("Tahoma");

	[Default]
	private static readonly FontFamily ModifiableFontDefaultValue = new("Tahoma");

	[Default]
	private static readonly FontFamily PencilmarkFontDefaultValue = new("Tahoma");

	[Default]
	private static readonly FontFamily CoordinateLabelFontDefaultValue = new("Tahoma");

	[Default]
	private static readonly FontFamily BabaGroupLabelFontDefaultValue = new("Times New Roman");

	[Default]
	private static readonly ColorPalette AuxiliaryColorsDefaultValue = [
		Color.FromArgb(255, 255, 192, 89),
		Color.FromArgb(255, 127, 187, 255),
		Color.FromArgb(255, 216, 178, 255)
	];

	[Default]
	private static readonly ColorPalette DifficultyLevelForegroundsDefaultValue = [
		Color.FromArgb(255, 0, 51, 204),
		Color.FromArgb(255, 0, 102, 0),
		Color.FromArgb(255, 102, 51, 0),
		Color.FromArgb(255, 102, 51, 0),
		Color.FromArgb(255, 102, 0, 0),
		Colors.Black
	];

	[Default]
	private static readonly ColorPalette DifficultyLevelBackgroundsDefaultValue = [
		Color.FromArgb(255, 204, 204, 255),
		Color.FromArgb(255, 100, 255, 100),
		Color.FromArgb(255, 255, 255, 100),
		Color.FromArgb(255, 255, 150, 80),
		Color.FromArgb(255, 255, 100, 100),
		Color.FromArgb(255, 220, 220, 220)
	];

	[Default]
	private static readonly ColorPalette UserDefinedColorPaletteDefaultValue = [
		Color.FromArgb(255, 63, 218, 101), // Green
		Color.FromArgb(255, 255, 192, 89), // Orange
		Color.FromArgb(255, 127, 187, 255), // Sky-blue
		Color.FromArgb(255, 216, 178, 255), // Purple
		Color.FromArgb(255, 197, 232, 140), // Yellow-green
		Color.FromArgb(255, 255, 203, 203), // Light red
		Color.FromArgb(255, 178, 223, 223), // Blue green
		Color.FromArgb(255, 252, 220, 165), // Light orange
		Color.FromArgb(255, 255, 255, 150), // Yellow
		Color.FromArgb(255, 247, 222, 143), // Golden yellow
		Color.FromArgb(255, 220, 212, 252), // Purple
		Color.FromArgb(255, 255, 118, 132), // Red
		Color.FromArgb(255, 206, 251, 237), // Light sky-blue
		Color.FromArgb(255, 215, 255, 215), // Light green
		Color.FromArgb(255, 192, 192, 192) // Gray
	];

	[Default]
	private static readonly ColorPalette AlmostLockedSetsColorsDefaultValue = [
		Color.FromArgb(255, 255, 203, 203),
		Color.FromArgb(255, 178, 223, 223),
		Color.FromArgb(255, 252, 220, 165),
		Color.FromArgb(255, 255, 255, 150),
		Color.FromArgb(255, 247, 222, 143)
	];


	/// <summary>
	/// Indicates the temporary selected cell.
	/// </summary>
	internal Cell _temporarySelectedCell = -1;

	/// <summary>
	/// Defines a pair of stacks that stores undo steps.
	/// This field can be used when <see cref="EnableUndoRedoStacking"/> is <see langword="true"/>.
	/// </summary>
	/// <seealso cref="EnableUndoRedoStacking"/>
	internal ObservableStack<Grid>? _undoStack;

	/// <summary>
	/// Defines a pair of stacks that stores redo steps.
	/// This field can be used when <see cref="EnableUndoRedoStacking"/> is <see langword="true"/>.
	/// </summary>
	/// <seealso cref="EnableUndoRedoStacking"/>
	internal ObservableStack<Grid>? _redoStack;

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
	/// Initializes a <see cref="SudokuPane"/> instance.
	/// </summary>
	public SudokuPane()
	{
		InitializeComponent();
		InitializeChildrenControls();

		_puzzle = Grid.Empty;

		UpdateCellData(in _puzzle);
		InitializeEvents();
	}


	/// <summary>
	/// Indicates the core-operating sudoku puzzle.
	/// </summary>
	[ImplicitField]
	public Grid Puzzle
	{
		get => _puzzle;

		set
		{
			SetPuzzleInternal(in value, PuzzleUpdatingMethod.Programmatic, true);
			GridUpdated?.Invoke(this, new(GridUpdatedBehavior.Replacing, value));
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
	internal Grid Solution => _puzzle.SolutionGrid;


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
	/// Indicates the event that is triggered when a candidate is clicked.
	/// This event can be also used for checking the clicked cell, house, chute, etc..
	/// </summary>
	public event GridClickedEventHandler? Clicked;

	/// <summary>
	/// Indicates the event that is triggered when "displaying candidates" option is toggled.
	/// </summary>
	public event CandidatesDisplayingToggledEventHandler? CandidatesDisplayingToggled;

	/// <summary>
	/// Indicates the event that is triggered when a house is completed.
	/// </summary>
	public event HouseCompletedEventHandler? HouseCompleted;

	/// <summary>
	/// Indicates the event that is triggered when the current puzzle is completed.
	/// </summary>
	public event PuzzleCompletedEventHandler? PuzzleCompleted;

	/// <summary>
	/// Indicates the event that is triggered when caching.
	/// </summary>
	public event EventHandler? Caching;


	/// <summary>
	/// Undo a step. This method requires member <see cref="EnableUndoRedoStacking"/> be <see langword="true"/>.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Throws when the property <see cref="EnableUndoRedoStacking"/> is not <see langword="true"/>.
	/// </exception>
	public void UndoStep()
	{
		if (!EnableUndoRedoStacking)
		{
			throw new InvalidOperationException($"Undoing operation requires property '{nameof(EnableUndoRedoStacking)}' be true value.");
		}

		if (_undoStack.Count == 0)
		{
			// No more steps can be undone.
			return;
		}

		_redoStack.Push(_puzzle);

		var target = _undoStack.Pop();

		SetPuzzleCore(in target, new(PuzzleUpdatingMethod.UserUpdating, false, true));

		GridUpdated?.Invoke(this, new(GridUpdatedBehavior.Undoing, target));
	}

	/// <summary>
	/// Redo a step. This method requires member <see cref="EnableUndoRedoStacking"/> be <see langword="true"/>.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Throws when the property <see cref="EnableUndoRedoStacking"/> is not <see langword="true"/>.
	/// </exception>
	public void RedoStep()
	{
		if (!EnableUndoRedoStacking)
		{
			throw new InvalidOperationException($"Redoing operation requires property '{nameof(EnableUndoRedoStacking)}' be true value.");
		}

		if (_redoStack.Count == 0)
		{
			// No more steps can be redone.
			return;
		}

		_undoStack.Push(_puzzle);

		var target = _redoStack.Pop();

		SetPuzzleCore(in target, new(PuzzleUpdatingMethod.UserUpdating, false, true));

		GridUpdated?.Invoke(this, new(GridUpdatedBehavior.Redoing, target));
	}

	/// <summary>
	/// Try to set a digit into a cell, or delete a digit from a cell.
	/// </summary>
	/// <param name="cell">The cell to be set or delete.</param>
	/// <param name="digit">The digit to be set or delete.</param>
	/// <param name="isSet">Indicates whether the operation is to set a digit into the target cell.</param>
	public void SetOrDeleteDigit(Cell cell, Digit digit, bool isSet)
	{
		var puzzle = Puzzle;
		var conclusion = (Conclusion)(isSet ? new(Assignment, cell, digit) : new(Elimination, cell, digit));
		puzzle.Apply(conclusion);

		SetPuzzleInternal(in puzzle, PuzzleUpdatingMethod.UserUpdating);
		GridUpdated?.Invoke(this, new(isSet ? GridUpdatedBehavior.Assignment : GridUpdatedBehavior.Elimination, cell * 9 + digit));
	}

	/// <summary>
	/// Try to update grid state.
	/// </summary>
	/// <param name="newGrid">The new grid to be used for assigning to the target.</param>
	public void UpdateGrid(scoped ref readonly Grid newGrid) => SetPuzzleInternal(in newGrid, PuzzleUpdatingMethod.Programmatic);

	/// <summary>
	/// <para>Triggers <see cref="GridUpdated"/> event.</para>
	/// <para>This method can only be used by internal control type <see cref="SudokuPaneCell"/> or the current type scope.</para>
	/// </summary>
	/// <param name="behavior">The behavior.</param>
	/// <param name="value">The new value to assign.</param>
	/// <seealso cref="SudokuPaneCell"/>
	internal void TriggerGridUpdated(GridUpdatedBehavior behavior, object value) => GridUpdated?.Invoke(this, new(behavior, value));

	/// <summary>
	///	<para>Triggers <see cref="Clicked"/> event.</para>
	///	<para><inheritdoc cref="TriggerGridUpdated(GridUpdatedBehavior, object)" path="//summary/para[2]"/></para>
	/// </summary>
	/// <param name="mouseButton">Indicates the mouse button clicked.</param>
	/// <param name="candidate">The candidate.</param>
	internal void TriggerClicked(MouseButton mouseButton, Candidate candidate) => Clicked?.Invoke(this, new(mouseButton, candidate));

	/// <summary>
	/// <para>Try to set puzzle, with a <see cref="bool"/> value indicating whether undoing and redoing stacks should be cleared.</para>
	/// <para><inheritdoc cref="TriggerGridUpdated(GridUpdatedBehavior, object)" path="//summary/para[2]"/></para>
	/// </summary>
	/// <param name="value">The newer grid.</param>
	/// <param name="method">The updating method.</param>
	/// <param name="clearStack">
	/// Indicates whether undoing and redoing stacks should be cleared. The default value is <see langword="false"/>.
	/// </param>
	/// <seealso cref="SudokuPaneCell"/>
	internal void SetPuzzleInternal(scoped ref readonly Grid value, PuzzleUpdatingMethod method, bool clearStack = false)
		=> SetPuzzleCore(in value, new(method, clearStack, false));

	/// <inheritdoc cref="UpdateViewUnit(ViewUnitBindableSource?)"/>
	private void UpdateViewUnit() => UpdateViewUnit(ViewUnit);

	/// <summary>
	/// Update view unit (add view nodes or remove view nodes).
	/// </summary>
	/// <param name="viewUnit">The view unit bindable source.</param>
	private void UpdateViewUnit(ViewUnitBindableSource? viewUnit)
	{
		RenderableFactory.RemoveViewUnitControls(this);

		if (viewUnit is not null)
		{
			RenderableFactory.AddViewUnitControls(this, viewUnit);
		}
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
		if (EnableUndoRedoStacking)
		{
			(_undoStack = []).Changed += _ => PropertyChanged?.Invoke(this, new(nameof(_undoStack)));
			(_redoStack = []).Changed += _ => PropertyChanged?.Invoke(this, new(nameof(_redoStack)));
		}

		HouseCompleted += static (sender, e) => sender.OnHouseCompletedAsync(e);
	}

	/// <summary>
	/// The default handler that is used by <see cref="HouseCompleted"/> event.
	/// </summary>
	/// <param name="e">The event arguments.</param>
	private async void OnHouseCompletedAsync(HouseCompletedEventArgs e)
	{
		if (e.Method == PuzzleUpdatingMethod.Programmatic)
		{
			return;
		}

		foreach (var cells in e.LastCell.GetCellsOrdered(e.House))
		{
			cells.ForEach(cell => _children[cell].LightUpAsync(250));
			await Task.Delay(100);
		}
	}

	/// <summary>
	/// To initialize <see cref="_children"/> values via the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <seealso cref="_children"/>
	private void UpdateCellData(scoped ref readonly Grid grid)
	{
		for (var i = 0; i < 81; i++)
		{
			var cellControl = _children[i];
			cellControl.State = grid.GetState(i);
			cellControl.CandidatesMask = grid.GetCandidates(i);
		}
	}

	/// <summary>
	/// Try to update the puzzle value by using the specified newer <see cref="Grid"/> instance,
	/// and two parameters indicating the details of the current updating operation.
	/// </summary>
	/// <param name="value">
	/// <inheritdoc cref="SetPuzzleInternal(ref readonly Grid, PuzzleUpdatingMethod, bool)" path="/param[@name='value']"/>
	/// </param>
	/// <param name="data">The details of updating.</param>
	private void SetPuzzleCore(scoped ref readonly Grid value, GridUpdatingDetails data)
	{
		var (method, clearStack, whileUndoingOrRedoing) = data;

		// Pushes the grid into the stack if worth.
		if (!whileUndoingOrRedoing && !clearStack && EnableUndoRedoStacking)
		{
			_undoStack.Push(_puzzle);
		}

		// Check whether a house is going to be completed.
		var housesToBeCompleted = value.FullHouses & ~_puzzle.FullHouses;
		using scoped var lastCells = new ValueList<Cell>((byte)PopCount((uint)housesToBeCompleted));
		foreach (var houseToBeCompleted in housesToBeCompleted)
		{
			foreach (var cell in HouseCells[houseToBeCompleted])
			{
				if (_puzzle.GetState(cell) == CellState.Empty && value.GetState(cell) != CellState.Empty)
				{
					lastCells.Add(cell);
					break;
				}
			}
		}

		// Assigns the puzzle.
		_puzzle = value;

		UpdateCellData(in value);
		switch (clearStack, whileUndoingOrRedoing)
		{
			case (true, _) when EnableUndoRedoStacking:
			{
				_undoStack.Clear();
				_redoStack.Clear();
				break;
			}
			case (false, false) when EnableUndoRedoStacking:
			{
				_redoStack.Clear();
				break;
			}
		}

		// Clears the view unit.
		ViewUnit = null;

		// Triggers the event.
		PropertyChanged?.Invoke(this, new(nameof(Puzzle)));

		var houses = housesToBeCompleted.GetAllSets();
		for (var i = 0; i < houses.Length; i++)
		{
			HouseCompleted?.Invoke(this, new(lastCells[i], houses[i], method));
		}

		if (value.IsSolved)
		{
			PuzzleCompleted?.Invoke(this, new(in value));
		}
	}


	[Callback]
	private static void DisplayCandidatesPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is SudokuPane pane)
		{
			pane.CandidatesDisplayingToggled?.Invoke(pane, new());

			// Update view nodes. This operation should be executed no matter whether the event is triggered.
			pane.UpdateViewUnit();
		}
	}

	[Callback]
	private static void ViewUnitPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is not SudokuPane pane)
		{
			return;
		}

		pane.UpdateViewUnit();

		pane.Caching?.Invoke(pane, EventArgs.Empty);
	}

	[Callback]
	private static void BabaGroupLabelColorPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> ((SudokuPane)d).UpdateViewUnit();

	[Callback]
	private static void LinkColorPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((SudokuPane)d).UpdateViewUnit();

	[Callback]
	private static void NormalColorPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((SudokuPane)d).UpdateViewUnit();

	[Callback]
	private static void AssignmentColorPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> ((SudokuPane)d).UpdateViewUnit();

	[Callback]
	private static void OverlappedAssignmentColorPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> ((SudokuPane)d).UpdateViewUnit();

	[Callback]
	private static void EliminationColorPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> ((SudokuPane)d).UpdateViewUnit();

	[Callback]
	private static void CannibalismColorPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> ((SudokuPane)d).UpdateViewUnit();

	[Callback]
	private static void ExofinColorPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((SudokuPane)d).UpdateViewUnit();

	[Callback]
	private static void EndofinColorPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((SudokuPane)d).UpdateViewUnit();

	[Callback]
	private static void BabaGroupLabelFontPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> ((SudokuPane)d).UpdateViewUnit();

	[Callback]
	private static void AuxiliaryColorsPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> ((SudokuPane)d).UpdateViewUnit();

	[Callback]
	private static void UserDefinedColorPalettePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> ((SudokuPane)d).UpdateViewUnit();

	[Callback]
	private static void AlmostLockedSetsColorsPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> ((SudokuPane)d).UpdateViewUnit();

	[Callback]
	private static void StrongLinkDashStylePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> ((SudokuPane)d).UpdateViewUnit();

	[Callback]
	private static void WeakLinkDashStylePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> ((SudokuPane)d).UpdateViewUnit();

	[Callback]
	private static void CycleLikeLinkDashStylePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> ((SudokuPane)d).UpdateViewUnit();

	[Callback]
	private static void OtherLinkDashStylePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> ((SudokuPane)d).UpdateViewUnit();

	[Callback]
	private static void HighlightCandidateCircleScalePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> ((SudokuPane)d).UpdateViewUnit();

	[Callback]
	private static void HighlightBackgroundOpacityPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> ((SudokuPane)d).UpdateViewUnit();

	[Callback]
	private static void ChainStrokeThicknessPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> ((SudokuPane)d).UpdateViewUnit();

	[Callback]
	private static void CandidateViewNodeDisplayModePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> ((SudokuPane)d).UpdateViewUnit();

	[Callback]
	private static void EliminationDisplayModePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> ((SudokuPane)d).UpdateViewUnit();

	[Callback]
	private static void HouseCompletedFeedbackDurationPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		foreach (var element in ((SudokuPane)d)._children)
		{
			element.HouseCompletedFeedbackDuration = (int)e.NewValue;
		}
	}

	[Callback]
	private static void CoordinateLabelDisplayKindPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (e.NewValue is not CoordinateType value)
		{
			return;
		}

		var i = 0;
		foreach (var element in ((SudokuPane)d).MainGrid.Children)
		{
			if (element is not TextBlock t)
			{
				continue;
			}

			t.Text = CoordinateLabelConversion.ToCoordinateLabelText(value, i % 9, i < 18);
			i++;
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
				if (files is not [StorageFile { FileType: FileExtensions.Text or FileExtensions.PlainText } file])
				{
					return;
				}

				await handleSudokuFileAsync(file);

				break;
			}
			case [StorageFile { FileType: FileExtensions.Text or FileExtensions.PlainText } file]:
			{
				await handleSudokuFileAsync(file);

				break;
			}
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
				case > 1024 * 64:
				{
					FailedReceivedDroppedFile?.Invoke(this, new(FailedReceivedDroppedFileReason.FileIsTooLarge));
					return;
				}
				default:
				{
					switch (Path.GetExtension(filePath))
					{
						case FileExtensions.PlainText:
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
						case FileExtensions.Text:
						{
							switch (SudokuFileHandler.Read(filePath))
							{
								case [{ BaseGrid: var g }]:
								{
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
		switch (Keyboard.GetModifierStateForCurrentThread(), SelectedCell, e.Key, Keyboard.GetInputDigit(e.Key))
		{
			default:
			case (_, not (>= 0 and < 81), _, _):
			case var (_, cell, _, _) when Puzzle.GetState(cell) == CellState.Given:
			case (_, _, _, -2):
			{
				return;
			}
			case ({ AllFalse: true }, var cell, _, -1):
			{
				var modified = Puzzle;
				modified.SetDigit(cell, -1);

				GridUpdated?.Invoke(this, new(GridUpdatedBehavior.Clear, cell));

				SetPuzzleInternal(in modified, PuzzleUpdatingMethod.UserUpdating);

				DigitInput?.Invoke(this, new(cell, -1));

				break;
			}
			case ((false, true, false, false), var cell, _, var digit) when Puzzle.Exists(cell, digit) is true:
			{
				var modified = Puzzle;
				modified.SetCandidateIsOn(cell, digit, false);

				SetPuzzleInternal(in modified, PuzzleUpdatingMethod.UserUpdating);

				GridUpdated?.Invoke(this, new(GridUpdatedBehavior.Elimination, cell * 9 + digit));

				break;
			}
			case ({ AllFalse: true }, var cell, _, var digit)
			when PreventConflictingInput && !Puzzle.DuplicateWith(cell, digit) || !PreventConflictingInput:
			{
				var modified = Puzzle;
				if (Puzzle.GetState(cell) == CellState.Modifiable)
				{
					// Temporarily re-compute candidates.
					modified.SetDigit(cell, -1);
				}

				modified.SetDigit(cell, digit);

				SetPuzzleInternal(in modified, PuzzleUpdatingMethod.UserUpdating);

				DigitInput?.Invoke(this, new(cell, digit));
				GridUpdated?.Invoke(this, new(GridUpdatedBehavior.Assignment, cell * 9 + digit));

				break;
			}
		}
	}
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	public static CellMap[] GetCellsOrdered(this Cell @this, House house)
	{
		var cells = HouseCells[house];
		switch (house.ToHouseType())
		{
			case HouseType.Row:
			{
				var pos = Array.FindIndex(cells, cell => cell % 9 == @this % 9);
				var result = new List<CellMap>(5);
				for (var i = 1; ; i++)
				{
					var map = CellMap.Empty;
					if (pos - i >= 0)
					{
						map.Add(cells[pos - i]);
					}
					if (pos + i < cells.Length)
					{
						map.Add(cells[pos + i]);
					}

					if (map)
					{
						result.Add(map);
						continue;
					}

					break;
				}

				return [.. result];
			}
			case HouseType.Column:
			{
				var pos = Array.FindIndex(cells, cell => cell / 9 == @this / 9);
				var result = new List<CellMap>(5);
				for (var i = 1; ; i++)
				{
					var map = CellMap.Empty;
					if (pos - i >= 0)
					{
						map.Add(cells[pos - i]);
					}
					if (pos + i < cells.Length)
					{
						map.Add(cells[pos + i]);
					}

					if (map)
					{
						result.Add(map);
						continue;
					}

					break;
				}

				return [.. result];
			}
			case HouseType.Block:
			{
				return from cell in cells select CellsMap[cell];
			}
			default:
			{
				throw new InvalidOperationException("The value is invalid.");
			}
		}
	}
}
