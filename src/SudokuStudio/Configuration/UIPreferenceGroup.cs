namespace SudokuStudio.Configuration;

/// <summary>
/// Defines a list of UI-related preference items. Some items in this group may not be found in settings page
/// because they are controlled by UI only, not by users.
/// </summary>
public sealed partial class UIPreferenceGroup : PreferenceGroup
{
	[Default]
	private static readonly decimal MainNavigationPageOpenPaneLengthDefaultValue = 200M;

	[Default]
	private static readonly decimal HighlightedPencilmarkBackgroundEllipseScaleDefaultValue = 0.9M;

	[Default]
	private static readonly decimal HighlightedBackgroundOpacityDefaultValue = .15M;

	[Default]
	private static readonly decimal ChainStrokeThicknessDefaultValue = 1.5M;

	[Default]
	private static readonly decimal GivenFontScaleDefaultValue = .85M;

	[Default]
	private static readonly decimal ModifiableFontScaleDefaultValue = .85M;

	[Default]
	private static readonly decimal PencilmarkFontScaleDefaultValue = .3M;

	[Default]
	private static readonly decimal BabaGroupingFontScaleDefaultValue = .6M;

	[Default]
	private static readonly decimal CoordinateLabelFontScaleDefaultValue = .4M;

	[Default]
	private static readonly Color GivenFontColorDefaultValue = Colors.Black;

	[Default]
	private static readonly Color GivenFontColor_DarkDefaultValue = Colors.Gray;

	[Default]
	private static readonly Color ModifiableFontColorDefaultValue = Colors.Blue;

	[Default]
	private static readonly Color ModifiableFontColor_DarkDefaultValue = Color.FromArgb(255, 86, 156, 214);

	[Default]
	private static readonly Color PencilmarkFontColorDefaultValue = Color.FromArgb(255, 100, 100, 100);

	[Default]
	private static readonly Color PencilmarkFontColor_DarkDefaultValue = Color.FromArgb(255, 80, 80, 80);

	[Default]
	private static readonly Color BabaGroupingFontColorDefaultValue = Colors.Red;

	[Default]
	private static readonly Color BabaGroupingFontColor_DarkDefaultValue = Colors.Red;

	[Default]
	private static readonly Color CoordinateLabelFontColorDefaultValue = Color.FromArgb(255, 100, 100, 100);

	[Default]
	private static readonly Color CoordinateLabelFontColor_DarkDefaultValue = Color.FromArgb(255, 155, 155, 155);

	[Default]
	private static readonly Color DeltaValueColorDefaultValue = Colors.Red;

	[Default]
	private static readonly Color DeltaValueColor_DarkDefaultValue = Colors.Red;

	[Default]
	private static readonly Color DeltaPencilmarkColorDefaultValue = Color.FromArgb(255, 255, 185, 185);

	[Default]
	private static readonly Color DeltaPencilmarkColor_DarkDefaultValue = Colors.Magenta;

	[Default]
	private static readonly Color SudokuPaneBorderColorDefaultValue = Colors.Black;

	[Default]
	private static readonly Color SudokuPaneBorderColor_DarkDefaultValue = Colors.Gray;

	[Default]
	private static readonly Color CursorBackgroundColorDefaultValue = Colors.Blue with { A = 32 };

	[Default]
	private static readonly Color CursorBackgroundColor_DarkDefaultValue = Color.FromArgb(32, 86, 156, 214);

	[Default]
	private static readonly Color ChainColorDefaultValue = Colors.Red;

	[Default]
	private static readonly Color ChainColor_DarkDefaultValue = Colors.Red;

	[Default]
	private static readonly Color NormalColorDefaultValue = Color.FromArgb(255, 63, 218, 101);

	[Default]
	private static readonly Color NormalColor_DarkDefaultValue = Color.FromArgb(255, 63, 218, 101);

	[Default]
	private static readonly Color AssignmentColorDefaultValue = Color.FromArgb(255, 63, 218, 101);

	[Default]
	private static readonly Color AssignmentColor_DarkDefaultValue = Color.FromArgb(255, 63, 218, 101);

	[Default]
	private static readonly Color OverlappedAssignmentColorDefaultValue = Color.FromArgb(255, 0, 255, 204);

	[Default]
	private static readonly Color OverlappedAssignmentColor_DarkDefaultValue = Color.FromArgb(255, 0, 255, 204);

	[Default]
	private static readonly Color EliminationColorDefaultValue = Color.FromArgb(255, 255, 118, 132);

	[Default]
	private static readonly Color EliminationColor_DarkDefaultValue = Color.FromArgb(255, 255, 118, 132);

	[Default]
	private static readonly Color CannibalismColorDefaultValue = Color.FromArgb(255, 235, 0, 0);

	[Default]
	private static readonly Color CannibalismColor_DarkDefaultValue = Color.FromArgb(255, 235, 0, 0);

	[Default]
	private static readonly Color ExofinColorDefaultValue = Color.FromArgb(255, 127, 187, 255);

	[Default]
	private static readonly Color ExofinColor_DarkDefaultValue = Color.FromArgb(255, 127, 187, 255);

	[Default]
	private static readonly Color EndofinColorDefaultValue = Color.FromArgb(255, 216, 178, 255);

	[Default]
	private static readonly Color EndofinColor_DarkDefaultValue = Color.FromArgb(255, 216, 178, 255);

	[Default]
	private static readonly Color GroupedNodeStrokeColorDefaultValue = Colors.Orange;

	[Default]
	private static readonly Color GroupedNodeStrokeColor_DarkDefaultValue = Color.FromArgb(64, 67, 53, 25);

	[Default]
	private static readonly Color GroupedNodeBackgroundColorDefaultValue = Colors.Yellow with { A = 64 };

	[Default]
	private static readonly Color GroupedNodeBackgroundColor_DarkDefaultValue = Color.FromArgb(255, 157, 93, 0);

	[Default]
	private static readonly Color HouseCompletedFeedbackColorDefaultValue = Colors.HotPink;

	[Default]
	private static readonly Color HouseCompletedFeedbackColor_DarkDefaultValue = Colors.DarkMagenta;

	[Default]
	private static readonly DashArray StrongLinkDashStyleDefaultValue = [];

	[Default]
	private static readonly DashArray WeakLinkDashStyleDefaultValue = [3, 1.5];

	[Default]
	private static readonly Grid LastGridPuzzleDefaultValue = Grid.Empty;

	[Default]
	private static readonly ColorPalette AuxiliaryColorsDefaultValue = [
		Color.FromArgb(255, 255, 192, 89),
		Color.FromArgb(255, 127, 187, 255),
		Color.FromArgb(255, 216, 178, 255)
	];

	[Default]
	private static readonly ColorPalette AuxiliaryColors_DarkDefaultValue = [
		Color.FromArgb(255, 255, 192, 89),
		Color.FromArgb(255, 127, 187, 255),
		Color.FromArgb(255, 216, 178, 255)
	];

	[Default]
	private static readonly ColorPalette AlmostLockedSetsColorsDefaultValue = [
		Color.FromArgb(255, 255, 203, 203),
		Color.FromArgb(255, 178, 223, 223),
		Color.FromArgb(255, 252, 220, 165),
		Color.FromArgb(255, 255, 255, 150),
		Color.FromArgb(255, 247, 222, 143)
	];

	[Default]
	private static readonly ColorPalette AlmostLockedSetsColors_DarkDefaultValue = [
		Color.FromArgb(255, 255, 203, 203),
		Color.FromArgb(255, 178, 223, 223),
		Color.FromArgb(255, 252, 220, 165),
		Color.FromArgb(255, 255, 255, 150),
		Color.FromArgb(255, 247, 222, 143)
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
	private static readonly ColorPalette DifficultyLevelForegrounds_DarkDefaultValue = [
		Color.FromArgb(255, 0, 51, 204),
		Color.FromArgb(255, 0, 102, 0),
		Color.FromArgb(255, 102, 51, 0),
		Color.FromArgb(255, 102, 51, 0),
		Color.FromArgb(255, 102, 0, 0),
		Colors.White
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
	private static readonly ColorPalette DifficultyLevelBackgrounds_DarkDefaultValue = [
		Color.FromArgb(255, 204, 204, 255),
		Color.FromArgb(255, 100, 255, 100),
		Color.FromArgb(255, 255, 255, 100),
		Color.FromArgb(255, 255, 150, 80),
		Color.FromArgb(255, 255, 100, 100),
		Color.FromArgb(255, 220, 220, 220)
	];

	[Default]
	private static readonly ColorPalette UserDefinedColorPaletteDefaultValue = [
		Color.FromArgb(255, 63, 218, 101),
		Color.FromArgb(255, 255, 192, 89),
		Color.FromArgb(255, 127, 187, 255),
		Color.FromArgb(255, 216, 178, 255),
		Color.FromArgb(255, 197, 232, 140),
		Color.FromArgb(255, 255, 203, 203),
		Color.FromArgb(255, 178, 223, 223),
		Color.FromArgb(255, 252, 220, 165),
		Color.FromArgb(255, 255, 255, 150),
		Color.FromArgb(255, 247, 222, 143),
		Color.FromArgb(255, 220, 212, 252),
		Color.FromArgb(255, 255, 118, 132),
		Color.FromArgb(255, 206, 251, 237),
		Color.FromArgb(255, 215, 255, 215),
		Color.FromArgb(255, 192, 192, 192)
	];

	[Default]
	private static readonly ColorPalette UserDefinedColorPalette_DarkDefaultValue = [
		Color.FromArgb(255, 63, 218, 101),
		Color.FromArgb(255, 255, 192, 89),
		Color.FromArgb(255, 127, 187, 255),
		Color.FromArgb(255, 216, 178, 255),
		Color.FromArgb(255, 197, 232, 140),
		Color.FromArgb(255, 255, 203, 203),
		Color.FromArgb(255, 178, 223, 223),
		Color.FromArgb(255, 252, 220, 165),
		Color.FromArgb(255, 255, 255, 150),
		Color.FromArgb(255, 247, 222, 143),
		Color.FromArgb(255, 220, 212, 252),
		Color.FromArgb(255, 255, 118, 132),
		Color.FromArgb(255, 206, 251, 237),
		Color.FromArgb(255, 215, 255, 215),
		Color.FromArgb(255, 192, 192, 192)
	];


	/// <inheritdoc cref="SudokuPane.DisplayCandidates"/>
	[AutoDependencyProperty(DefaultValue = true)]
	public partial bool DisplayCandidates { get; set; }

	/// <inheritdoc cref="SudokuPane.DisplayCursors"/>
	[AutoDependencyProperty(DefaultValue = true)]
	public partial bool DisplayCursors { get; set; }

	/// <inheritdoc cref="SudokuPane.UseDifferentColorToDisplayDeltaDigits"/>
	[AutoDependencyProperty(DefaultValue = true)]
	public partial bool DistinctWithDeltaDigits { get; set; }

	/// <inheritdoc cref="SudokuPane.DisableFlyout"/>
	[AutoDependencyProperty]
	public partial bool DisableSudokuPaneLayout { get; set; }

	/// <inheritdoc cref="SudokuPane.PreventConflictingInput"/>
	[AutoDependencyProperty(DefaultValue = true)]
	public partial bool PreventConflictingInput { get; set; }

	/// <summary>
	/// Indicates whether the program saves for puzzle-generating history.
	/// </summary>
	[AutoDependencyProperty(DefaultValue = true)]
	public partial bool SavePuzzleGeneratingHistory { get; set; }

	/// <summary>
	/// Indicates whether sudoku pane in analysis page provides with a simpler way to fill with Digits via double tapping.
	/// </summary>
	[AutoDependencyProperty]
	public partial bool EnableDoubleTapFillingForSudokuPane { get; set; }

	/// <summary>
	/// Indicates whether sudoku pane in analysis page provides with a simpler way to delete Digits via right tapping.
	/// </summary>
	[AutoDependencyProperty]
	public partial bool EnableRightTapRemovingForSudokuPane { get; set; }

	/// <inheritdoc cref="SudokuPane.EnableAnimationFeedback"/>
	[AutoDependencyProperty(DefaultValue = true)]
	public partial bool EnableAnimationFeedback { get; set; }

	/// <inheritdoc cref="SudokuPane.TransparentBackground"/>
	[AutoDependencyProperty]
	public partial bool TransparentBackground { get; set; }

	/// <summary>
	/// Indicates whether the last puzzle and its views should be cached to local path,
	/// in order to recover them after you re-start or launch the program.
	/// </summary>
	[AutoDependencyProperty]
	public partial bool AutoCachePuzzleAndView { get; set; }

	/// <summary>
	/// Indicates whether UI makes letters upper-casing on displaying coordinates if worth.
	/// </summary>
	[AutoDependencyProperty]
	public partial bool MakeLettersUpperCaseInRxCyNotation { get; set; }

	/// <summary>
	/// Indicates whether UI makes letters upper-casing on displaying coordinates in K9 notation if worth.
	/// </summary>
	[AutoDependencyProperty]
	public partial bool MakeLettersUpperCaseInK9Notation { get; set; }

	/// <summary>
	/// Indicates whether UI makes letters upper-casing on displaying coordinates in Excel notation if worth.
	/// </summary>
	[AutoDependencyProperty]
	public partial bool MakeLettersUpperCaseInExcelNotation { get; set; }

	/// <summary>
	/// Indicates whether UI makes Digits displaying before cells.
	/// </summary>
	[AutoDependencyProperty]
	public partial bool MakeDigitBeforeCellInRxCyNotation { get; set; }

	/// <summary>
	/// Indicates whether UI makes houses display its capital letters.
	/// </summary>
	[AutoDependencyProperty]
	public partial bool HouseNotationOnlyDisplayCapitalsInRxCyNotation { get; set; }

	/// <summary>
	/// Indicates whether the program also save for batch generated puzzles into history.
	/// </summary>
	[AutoDependencyProperty]
	public partial bool AlsoSaveBatchGeneratedPuzzlesIntoHistory { get; set; }

	/// <summary>
	/// Indicates whether the program uses corner radius property to apply to sudoku panes.
	/// </summary>
	[AutoDependencyProperty(DefaultValue = true)]
	public partial bool EnableCornerRadiusForSudokuPanes { get; set; }

	/// <summary>
	/// Indicates the default empty character you want to use. The value can be '0' or '.'.
	/// </summary>
	[AutoDependencyProperty(DefaultValue = '0')]
	public partial char EmptyCellCharacter { get; set; }

	/// <summary>
	/// Indicates the last letter representing the last row of the grid in displaying coordinates in K9 notation.
	/// </summary>
	[AutoDependencyProperty(DefaultValue = 'I')]
	public partial char FinalRowLetterInK9Notation { get; set; }

	/// <summary>
	/// Indicates the open-pane length of main navigation page.
	/// </summary>
	[AutoDependencyProperty]
	public partial decimal MainNavigationPageOpenPaneLength { get; set; }

	/// <inheritdoc cref="SudokuPane.HighlightCandidateCircleScale"/>
	[AutoDependencyProperty]
	public partial decimal HighlightedPencilmarkBackgroundEllipseScale { get; set; }

	/// <inheritdoc cref="SudokuPane.HighlightBackgroundOpacity"/>
	[AutoDependencyProperty]
	public partial decimal HighlightedBackgroundOpacity { get; set; }

	/// <inheritdoc cref="SudokuPane.ChainStrokeThickness"/>
	[AutoDependencyProperty]
	public partial decimal ChainStrokeThickness { get; set; }

	/// <summary>
	/// Indicates the given font scale.
	/// </summary>
	[AutoDependencyProperty]
	public partial decimal GivenFontScale { get; set; }

	/// <summary>
	/// Indicates the modifiable font scale.
	/// </summary>
	[AutoDependencyProperty]
	public partial decimal ModifiableFontScale { get; set; }

	/// <summary>
	/// Indicates the pencilmark font scale.
	/// </summary>
	[AutoDependencyProperty]
	public partial decimal PencilmarkFontScale { get; set; }

	/// <summary>
	/// Indicates the babe grouping font scale.
	/// </summary>
	[AutoDependencyProperty]
	public partial decimal BabaGroupingFontScale { get; set; }

	/// <summary>
	/// Indicates the coordinate label font scale.
	/// </summary>
	[AutoDependencyProperty]
	public partial decimal CoordinateLabelFontScale { get; set; }

	/// <inheritdoc cref="SudokuPane.CoordinateLabelDisplayMode"/>
	[AutoDependencyProperty(DefaultValue = (int)CoordinateLabelDisplay.FourDirection)]
	public partial int CoordinateLabelDisplayMode { get; set; }

	/// <inheritdoc cref="SudokuPane.CandidateViewNodeDisplayMode"/>
	[AutoDependencyProperty(DefaultValue = (int)CandidateViewNodeDisplay.CircleSolid)]
	public partial int CandidateViewNodeDisplayMode { get; set; }

	/// <inheritdoc cref="SudokuPane.EliminationDisplayMode"/>
	[AutoDependencyProperty(DefaultValue = (int)EliminationDisplay.CircleSolid)]
	public partial int EliminationDisplayMode { get; set; }

	/// <inheritdoc cref="SudokuPane.AssignmentDisplayMode"/>
	[AutoDependencyProperty(DefaultValue = (int)EliminationDisplay.CircleSolid)]
	public partial int AssignmentDisplayMode { get; set; }

	/// <summary>
	/// Indicates the desired size of output picture while saving.
	/// </summary>
	[AutoDependencyProperty(DefaultValue = 1000)]
	public partial int DesiredPictureSizeOnSaving { get; set; }

	/// <summary>
	/// Indicates the ittoryu length for the generated puzzles.
	/// </summary>
	[AutoDependencyProperty(DefaultValue = 0)]
	public partial int IttoryuLength { get; set; }

	/// <summary>
	/// Indicates the language of UI.
	/// </summary>
	[AutoDependencyProperty(DefaultValue = 0)]
	public partial int Language { get; set; }

	/// <summary>
	/// Indicates the given font name.
	/// </summary>
	[AutoDependencyProperty(DefaultValue = "Cascadia Code")]
	public partial string GivenFontName { get; set; }

	/// <summary>
	/// Indicates the modifiable font name.
	/// </summary>
	[AutoDependencyProperty(DefaultValue = "Cascadia Code")]
	public partial string ModifiableFontName { get; set; }

	/// <summary>
	/// Indicates the pencilmark font name.
	/// </summary>
	[AutoDependencyProperty(DefaultValue = "Cascadia Code")]
	public partial string PencilmarkFontName { get; set; }

	/// <summary>
	/// Indicates the baba grouping font name.
	/// </summary>
	[AutoDependencyProperty(DefaultValue = "Times New Roman")]
	public partial string BabaGroupingFontName { get; set; }

	/// <summary>
	/// Indicates the coordinate label font name.
	/// </summary>
	[AutoDependencyProperty(DefaultValue = "Cascadia Code")]
	public partial string CoordinateLabelFontName { get; set; }

	/// <summary>
	/// Indicates the default separators for separating with coordinates.
	/// </summary>
	[AutoDependencyProperty(DefaultValue = ", ")]
	public partial string DefaultSeparatorInNotation { get; set; }

	/// <summary>
	/// Indicates the default digit separators for displaying Digits.
	/// </summary>
	[AutoDependencyProperty]
	public partial string? DigitsSeparatorInNotation { get; set; }

	/// <summary>
	/// Indicates the file ID of the puzzle library that you want to be used for generating in analyzer page.
	/// </summary>
	[AutoDependencyProperty]
	public partial string? FetchingPuzzleLibrary { get; set; }

	/// <summary>
	/// Indicates the backdrop.
	/// </summary>
	[AutoDependencyProperty(DefaultValue = BackdropKind.Acrylic)]
	public partial BackdropKind Backdrop { get; set; }

	/// <summary>
	/// Indicates the tooltip display items.
	/// </summary>
	[AutoDependencyProperty(
		DefaultValue = StepTooltipDisplayItems.TechniqueName | StepTooltipDisplayItems.DifficultyRating
		| StepTooltipDisplayItems.SimpleDescription | StepTooltipDisplayItems.ExtraDifficultyCases)]
	public partial StepTooltipDisplayItems StepDisplayItems { get; set; }

	/// <summary>
	/// Indicates the based type for displaying a concept notation.
	/// </summary>
	[AutoDependencyProperty(DefaultValue = CoordinateType.RxCy)]
	public partial CoordinateType ConceptNotationBasedKind { get; set; }

	/// <summary>
	/// Indicates the theme used in this program.
	/// </summary>
	[AutoDependencyProperty(DefaultValue = Theme.Default)]
	public partial Theme CurrentTheme { get; set; }

	/// <summary>
	/// Indicates the given font color.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color GivenFontColor { get; set; }

	/// <summary>
	/// Indicates the given font color, in dark theme.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color GivenFontColor_Dark { get; set; }

	/// <summary>
	/// Indicates the modifiable font color.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color ModifiableFontColor { get; set; }

	/// <summary>
	/// Indicates the modifiable font color, in dark theme.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color ModifiableFontColor_Dark { get; set; }

	/// <summary>
	/// Indicates the pencilmark font color.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color PencilmarkFontColor { get; set; }

	/// <summary>
	/// Indicates the pencilmark font color, in dark theme.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color PencilmarkFontColor_Dark { get; set; }

	/// <summary>
	/// Indicates the baba grouping font color.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color BabaGroupingFontColor { get; set; }

	/// <summary>
	/// Indicates the baba grouping font color, in dark theme.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color BabaGroupingFontColor_Dark { get; set; }

	/// <summary>
	/// Indicates the coordinate label font color.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color CoordinateLabelFontColor { get; set; }

	/// <summary>
	/// Indicates the coordinate label font color, in dark theme.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color CoordinateLabelFontColor_Dark { get; set; }

	/// <summary>
	/// Indicates the default value color.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color DeltaValueColor { get; set; }

	/// <summary>
	/// Indicates the default value color, in dark theme.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color DeltaValueColor_Dark { get; set; }

	/// <summary>
	/// Indicates the delta pencilmark color.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color DeltaPencilmarkColor { get; set; }

	/// <summary>
	/// Indicates the delta pencilmark color, in dark theme.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color DeltaPencilmarkColor_Dark { get; set; }

	/// <summary>
	/// Indicates the sudoku pane border color.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color SudokuPaneBorderColor { get; set; }

	/// <summary>
	/// Indicates the sudoku pane border color, in dark theme.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color SudokuPaneBorderColor_Dark { get; set; }

	/// <summary>
	/// Indicates the cursor background color.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color CursorBackgroundColor { get; set; }

	/// <summary>
	/// Indicates the cursor background color, in dark theme.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color CursorBackgroundColor_Dark { get; set; }

	/// <summary>
	/// Indicates the chain color.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color ChainColor { get; set; }

	/// <summary>
	/// Indicates the chain color, in dark theme.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color ChainColor_Dark { get; set; }

	/// <summary>
	/// Indicates the normal color.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color NormalColor { get; set; }

	/// <summary>
	/// Indicates the normal color, in dark theme.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color NormalColor_Dark { get; set; }

	/// <summary>
	/// Indicates the assignment color.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color AssignmentColor { get; set; }

	/// <summary>
	/// Indicates the assignment color, in dark theme.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color AssignmentColor_Dark { get; set; }

	/// <summary>
	/// Indicates the overlapped assignment color.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color OverlappedAssignmentColor { get; set; }

	/// <summary>
	/// Indicates the overlapped assignment color, in dark theme.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color OverlappedAssignmentColor_Dark { get; set; }

	/// <summary>
	/// Indicates the elimination color.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color EliminationColor { get; set; }

	/// <summary>
	/// Indicates the elimination color, in dark theme.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color EliminationColor_Dark { get; set; }

	/// <summary>
	/// Indicates the cannibalism color.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color CannibalismColor { get; set; }

	/// <summary>
	/// Indicates the cannibalism color, in dark theme.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color CannibalismColor_Dark { get; set; }

	/// <summary>
	/// Indicates the exo-fin color.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color ExofinColor { get; set; }

	/// <summary>
	/// Indicates the exo-fin color, in dark theme.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color ExofinColor_Dark { get; set; }

	/// <summary>
	/// Indicates the endo-fin color.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color EndofinColor { get; set; }

	/// <summary>
	/// Indicates the endo-fin color, in dark theme.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color EndofinColor_Dark { get; set; }

	/// <summary>
	/// Indicates grouped node stroke color.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color GroupedNodeStrokeColor { get; set; }

	/// <summary>
	/// Indicates grouped node stroke color, in dark theme.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color GroupedNodeStrokeColor_Dark { get; set; }

	/// <summary>
	/// Indicates grouped node background color.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color GroupedNodeBackgroundColor { get; set; }

	/// <summary>
	/// Indicates grouped node background color, in dark theme.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color GroupedNodeBackgroundColor_Dark { get; set; }

	/// <summary>
	/// Indicates house completed feedback color.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color HouseCompletedFeedbackColor { get; set; }

	/// <summary>
	/// Indicates house completed feedback color, in dark theme.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color HouseCompletedFeedbackColor_Dark { get; set; }

	/// <inheritdoc cref="SudokuPane.StrongLinkDashStyle"/>
	[AutoDependencyProperty]
	public partial DashArray StrongLinkDashStyle { get; set; }

	/// <inheritdoc cref="SudokuPane.WeakLinkDashStyle"/>
	[AutoDependencyProperty]
	public partial DashArray WeakLinkDashStyle { get; set; }

	/// <summary>
	/// Indicates the last opened puzzle to be loaded or saved.
	/// </summary>
	[AutoDependencyProperty]
	public partial Grid LastGridPuzzle { get; set; }

	/// <summary>
	/// Indicates the drawable items produced by last opened puzzle.
	/// </summary>
	[AutoDependencyProperty]
	public partial UserDefinedDrawable? LastRenderable { get; set; }

	/// <inheritdoc cref="SudokuPane.AuxiliaryColors"/>
	[AutoDependencyProperty]
	public partial ColorPalette AuxiliaryColors { get; set; }

	/// <inheritdoc cref="SudokuPane.AuxiliaryColors"/>
	[AutoDependencyProperty]
	public partial ColorPalette AuxiliaryColors_Dark { get; set; }

	/// <inheritdoc cref="SudokuPane.DifficultyLevelForegrounds"/>
	[AutoDependencyProperty]
	public partial ColorPalette DifficultyLevelForegrounds { get; set; }

	/// <inheritdoc cref="SudokuPane.DifficultyLevelForegrounds"/>
	[AutoDependencyProperty]
	public partial ColorPalette DifficultyLevelForegrounds_Dark { get; set; }

	/// <inheritdoc cref="SudokuPane.DifficultyLevelBackgrounds"/>
	[AutoDependencyProperty]
	public partial ColorPalette DifficultyLevelBackgrounds { get; set; }

	/// <inheritdoc cref="SudokuPane.DifficultyLevelBackgrounds"/>
	[AutoDependencyProperty]
	public partial ColorPalette DifficultyLevelBackgrounds_Dark { get; set; }

	/// <inheritdoc cref="SudokuPane.UserDefinedColorPalette"/>
	[AutoDependencyProperty]
	public partial ColorPalette UserDefinedColorPalette { get; set; }

	/// <inheritdoc cref="SudokuPane.UserDefinedColorPalette"/>
	[AutoDependencyProperty]
	public partial ColorPalette UserDefinedColorPalette_Dark { get; set; }

	/// <inheritdoc cref="SudokuPane.AlmostLockedSetsColors"/>
	[AutoDependencyProperty]
	public partial ColorPalette AlmostLockedSetsColors { get; set; }

	/// <inheritdoc cref="SudokuPane.AlmostLockedSetsColors"/>
	[AutoDependencyProperty]
	public partial ColorPalette AlmostLockedSetsColors_Dark { get; set; }


	[Callback]
	private static void BackdropPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (e.NewValue is not BackdropKind value)
		{
			return;
		}

		foreach (var window in ((App)Application.Current).WindowManager.ActiveWindows)
		{
			window.SystemBackdrop = value.GetBackdrop();
		}
	}
}
