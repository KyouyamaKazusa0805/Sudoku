using Microsoft.UI;
using Microsoft.UI.Xaml;
using Sudoku.Analytics.Categorization;
using Sudoku.Concepts;
using SudokuStudio.Collection;
using SudokuStudio.ComponentModel;
using SudokuStudio.Interaction;
using SudokuStudio.Rendering;
using Windows.UI;

namespace SudokuStudio.Configuration;

using E = EliminationDisplayMode;
using M = CoordinateLabelDisplayMode;
using N = CandidateViewNodeDisplayNode;

/// <summary>
/// Defines a list of UI-related preference items. Some items in this group may not be found in settings page
/// because they are controlled by UI only, not by users.
/// </summary>
[DependencyProperty<bool>("DisplayCandidates", DefaultValue = true, DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.DisplayCandidates")]
[DependencyProperty<bool>("DisplayCursors", DefaultValue = true, DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.DisplayCursors")]
[DependencyProperty<bool>("DistinctWithDeltaDigits", DefaultValue = true, DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.UseDifferentColorToDisplayDeltaDigits")]
[DependencyProperty<bool>("DisableSudokuPaneLayout", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.DisableFlyout")]
[DependencyProperty<bool>("PreventConflictingInput", DefaultValue = true, DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.PreventConflictingInput")]
[DependencyProperty<bool>("SavePuzzleGeneratingHistory", DefaultValue = true, DocSummary = "Indicates whether the program saves for puzzle-generating history.")]
[DependencyProperty<bool>("EnableDoubleTapFillingForSudokuPane", DefaultValue = true, DocSummary = "Indicates whether sudoku pane in analysis page provides with a simpler way to fill with digits via double tapping.")]
[DependencyProperty<bool>("EnableRightTapRemovingForSudokuPane", DefaultValue = true, DocSummary = "Indicates whether sudoku pane in analysis page provides with a simpler way to delete digits via right tapping.")]
[DependencyProperty<bool>("EnableAnimationFeedback", DefaultValue = true, DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.EnableAnimationFeedback")]
[DependencyProperty<bool>("TransparentBackground", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.TransparentBackground")]
[DependencyProperty<bool>("GeneratedPuzzleShouldBeMinimal", DefaultValue = false, DocSummary = "Indicates whether the generated puzzles should be minimal.")]
[DependencyProperty<bool>("GeneratedPuzzleShouldBePearl", DefaultValue = false, DocSummary = "Indicates whether the generated puzzles should be pearl.")]
[DependencyProperty<bool>("AutoCachePuzzleAndView", DefaultValue = false, DocSummary = "Indicates whether the last puzzle and its views should be cached to local path, in order to recover them after you re-start or launch the program.")]
[DependencyProperty<bool>("CanRestrictGeneratingGivensCount", DocSummary = "Indicates whether the generator will limit the number of givens. The value should be used with another property 'GeneratingGivensCount'.")]
[DependencyProperty<bool>("MakeLettersUpperCaseInRxCyNotation", DocSummary = "Indicates whether UI makes letters upper-casing on displaying coordinates if worth.")]
[DependencyProperty<bool>("MakeLettersUpperCaseInK9Notation", DocSummary = "Indicates whether UI makes letters upper-casing on displaying coordinates in K9 notation if worth.")]
[DependencyProperty<bool>("MakeLettersUpperCaseInExcelNotation", DocSummary = "Indicates whether UI makes letters upper-casing on displaying coordinates in Excel notation if worth.")]
[DependencyProperty<bool>("MakeDigitBeforeCellInRxCyNotation", DocSummary = "Indicates whether UI makes digits displaying before cells.")]
[DependencyProperty<bool>("HouseNotationOnlyDisplayCapitalsInRxCyNotation", DocSummary = "Indicates whether UI makes houses display its capital letters.")]
[DependencyProperty<bool>("AlsoSaveBatchGeneratedPuzzlesIntoHistory", DocSummary = "Indicates whether the program also save for batch generated puzzles into history.")]
[DependencyProperty<char>("EmptyCellCharacter", DefaultValue = '0', DocSummary = "Indicates the default empty character you want to use. The value can be '0' or '.'.")]
[DependencyProperty<char>("FinalRowLetterInK9Notation", DefaultValue = 'I', DocSummary = "Indicates the last letter representing the last row of the grid in displaying coordinates in K9 notation.")]
[DependencyProperty<decimal>("HighlightedPencilmarkBackgroundEllipseScale", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.HighlightCandidateCircleScale")]
[DependencyProperty<decimal>("HighlightedBackgroundOpacity", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.HighlightBackgroundOpacity")]
[DependencyProperty<decimal>("ChainStrokeThickness", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.ChainStrokeThickness")]
[DependencyProperty<decimal>("GivenFontScale")]
[DependencyProperty<decimal>("ModifiableFontScale")]
[DependencyProperty<decimal>("PencilmarkFontScale")]
[DependencyProperty<decimal>("BabaGroupingFontScale")]
[DependencyProperty<decimal>("CoordinateLabelFontScale")]
[DependencyProperty<int>("CoordinateLabelDisplayMode", DefaultValue = (int)M.FourDirection, DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.CoordinateLabelDisplayMode")]
[DependencyProperty<int>("CandidateViewNodeDisplayMode", DefaultValue = (int)N.CircleSolid, DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.CandidateViewNodeDisplayMode")]
[DependencyProperty<int>("EliminationDisplayMode", DefaultValue = (int)E.CircleSolid, DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.EliminationDisplayMode")]
[DependencyProperty<int>("DesiredPictureSizeOnSaving", DefaultValue = 1000)]
[DependencyProperty<int>("GeneratedPuzzleGivensCount", DefaultValue = -1, DocSummary = "Indicates how many the number of givens will be used for generating. The value must be -1 or between 17 and 80. -1 is for no restriction for generating a puzzle on limiting with the number of givens.")]
[DependencyProperty<int>("IttoryuLength", DefaultValue = 0, DocSummary = "Indicates the ittoryu length for the generated puzzles.")]
[DependencyProperty<string>("GivenFontName", DefaultValue = "Cascadia Code")]
[DependencyProperty<string>("ModifiableFontName", DefaultValue = "Cascadia Code")]
[DependencyProperty<string>("PencilmarkFontName", DefaultValue = "Cascadia Code")]
[DependencyProperty<string>("BabaGroupingFontName", DefaultValue = "Times New Roman")]
[DependencyProperty<string>("CoordinateLabelFontName", DefaultValue = "Cascadia Code")]
[DependencyProperty<string>("DefaultSeparatorInNotation", DefaultValue = ", ", DocSummary = "Indicates the default separators for separating with coordinates.")]
[DependencyProperty<string>("DigitsSeparatorInNotation", IsNullable = true, DocSummary = "Indicates the default digit separators for displaying digits.")]
[DependencyProperty<string>("FetchingPuzzleLibrary", IsNullable = true, DocSummary = "Indicates the file ID of the puzzle library that you want to be used for generating in analyzer page.")]
[DependencyProperty<BackdropKind>("Backdrop", DefaultValue = BackdropKind.Acrylic)]
[DependencyProperty<StepTooltipDisplayItems>("StepDisplayItems", DefaultValue = StepTooltipDisplayItems.TechniqueName | StepTooltipDisplayItems.DifficultyRating | StepTooltipDisplayItems.SimpleDescription | StepTooltipDisplayItems.ExtraDifficultyCases, DocSummary = "Indicates the tooltip display items.")]
[DependencyProperty<DifficultyLevel>("GeneratorDifficultyLevel", DefaultValue = 0, DocSummary = "Indicates the difficulty level for generated puzzles.")]
[DependencyProperty<SymmetricType>("GeneratorSymmetricPattern", DefaultValue = 0, DocSummary = "Indicates the symmetric pattern for generated puzzles.")]
[DependencyProperty<Technique>("SelectedTechnique", DefaultValue = 0, DocSummary = "Indicates the selected technique, which will be appeared in generated puzzles.")]
[DependencyProperty<CoordinateType>("ConceptNotationBasedKind", DefaultValue = CoordinateType.RxCy, DocSummary = "Indicates the based type for displaying a concept notation.")]
[DependencyProperty<Color>("GivenFontColor")]
[DependencyProperty<Color>("ModifiableFontColor")]
[DependencyProperty<Color>("PencilmarkFontColor")]
[DependencyProperty<Color>("BabaGroupingFontColor")]
[DependencyProperty<Color>("CoordinateLabelFontColor")]
[DependencyProperty<Color>("DeltaValueColor", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.DeltaCellColor")]
[DependencyProperty<Color>("DeltaPencilmarkColor", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.DeltaCandidateColor")]
[DependencyProperty<Color>("SudokuPaneBorderColor", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.BorderColor")]
[DependencyProperty<Color>("CursorBackgroundColor", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.CursorBackgroundColor")]
[DependencyProperty<Color>("ChainColor", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.LinkColor")]
[DependencyProperty<Color>("NormalColor", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.NormalColor")]
[DependencyProperty<Color>("AssignmentColor", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.AssignmentColor")]
[DependencyProperty<Color>("OverlappedAssignmentColor", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.OverlappedAssignmentColor")]
[DependencyProperty<Color>("EliminationColor", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.EliminationColor")]
[DependencyProperty<Color>("CannibalismColor", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.CannibalismColor")]
[DependencyProperty<Color>("ExofinColor", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.ExofinColor")]
[DependencyProperty<Color>("EndofinColor", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.EndofinColor")]
[DependencyProperty<Color>("HouseCompletedFeedbackColor", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.HouseCompletedFeedbackColor")]
[DependencyProperty<DashArray>("StrongLinkDashStyle", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.StrongLinkDashStyle")]
[DependencyProperty<DashArray>("WeakLinkDashStyle", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.WeakLinkDashStyle")]
[DependencyProperty<DashArray>("CyclingCellLinkDashStyle", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.CycleLikeLinkDashStyle")]
[DependencyProperty<DashArray>("OtherLinkDashStyle", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.OtherLinkDashStyle")]
[DependencyProperty<Grid>("LastGridPuzzle", DocSummary = "Indicates the last opened puzzle to be loaded or saved.")]
[DependencyProperty<UserDefinedRenderable?>("LastRenderable", DocSummary = "Indicates the renderable items produced by last opened puzzle.")]
[DependencyProperty<ColorPalette>("AuxiliaryColors", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.AuxiliaryColors")]
[DependencyProperty<ColorPalette>("DifficultyLevelForegrounds", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.DifficultyLevelForegrounds")]
[DependencyProperty<ColorPalette>("DifficultyLevelBackgrounds", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.DifficultyLevelBackgrounds")]
[DependencyProperty<ColorPalette>("UserDefinedColorPalette", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.UserDefinedColorPalette")]
[DependencyProperty<ColorPalette>("AlmostLockedSetsColors", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.AlmostLockedSetsColors")]
public sealed partial class UIPreferenceGroup : PreferenceGroup
{
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
	private static readonly Color ModifiableFontColorDefaultValue = Colors.Blue;

	[Default]
	private static readonly Color PencilmarkFontColorDefaultValue = Color.FromArgb(255, 100, 100, 100);

	[Default]
	private static readonly Color BabaGroupingFontColorDefaultValue = Colors.Red;

	[Default]
	private static readonly Color CoordinateLabelFontColorDefaultValue = Color.FromArgb(255, 100, 100, 100);

	[Default]
	private static readonly Color DeltaValueColorDefaultValue = Colors.Red;

	[Default]
	private static readonly Color DeltaPencilmarkColorDefaultValue = Color.FromArgb(255, 255, 185, 185);

	[Default]
	private static readonly Color SudokuPaneBorderColorDefaultValue = Colors.Black;

	[Default]
	private static readonly Color CursorBackgroundColorDefaultValue = Colors.Blue with { A = 32 };

	[Default]
	private static readonly Color ChainColorDefaultValue = Colors.Red;

	[Default]
	private static readonly Color NormalColorDefaultValue = Color.FromArgb(255, 63, 218, 101);

	[Default]
	private static readonly Color AssignmentColorDefaultValue = Color.FromArgb(255, 63, 218, 101);

	[Default]
	private static readonly Color OverlappedAssignmentColorDefaultValue = Color.FromArgb(255, 0, 255, 204);

	[Default]
	private static readonly Color EliminationColorDefaultValue = Color.FromArgb(255, 255, 118, 132);

	[Default]
	private static readonly Color CannibalismColorDefaultValue = Color.FromArgb(255, 235, 0, 0);

	[Default]
	private static readonly Color ExofinColorDefaultValue = Color.FromArgb(255, 127, 187, 255);

	[Default]
	private static readonly Color EndofinColorDefaultValue = Color.FromArgb(255, 216, 178, 255);

	[Default]
	private static readonly Color HouseCompletedFeedbackColorDefaultValue = Colors.HotPink;

	[Default]
	private static readonly DashArray StrongLinkDashStyleDefaultValue = [];

	[Default]
	private static readonly DashArray WeakLinkDashStyleDefaultValue = [3, 1.5];

	[Default]
	private static readonly DashArray CyclingCellLinkDashStyleDefaultValue = [];

	[Default]
	private static readonly DashArray OtherLinkDashStyleDefaultValue = [3, 3];

	[Default]
	private static readonly Grid LastGridPuzzleDefaultValue = Grid.Empty;

	[Default]
	private static readonly ColorPalette AuxiliaryColorsDefaultValue = [
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
