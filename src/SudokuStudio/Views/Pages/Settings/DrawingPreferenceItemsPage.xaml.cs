namespace SudokuStudio.Views.Pages.Settings;

/// <summary>
/// Represents drawing preferences page.
/// </summary>
public sealed partial class DrawingPreferenceItemsPage : Page
{
	/// <summary>
	/// The default view unit.
	/// </summary>
	private readonly ViewUnitBindableSource _defaultViewUnit = new()
	{
		View = [
			new CandidateViewNode(WellKnownColorIdentifier.Normal, 8),
			new CandidateViewNode(WellKnownColorIdentifier.Normal, 17),
			new CandidateViewNode(WellKnownColorIdentifier.Exofin, 49 * 9 + 7),
			new CandidateViewNode(WellKnownColorIdentifier.Exofin, 50 * 9 + 7),
			new CandidateViewNode(WellKnownColorIdentifier.Endofin, 30 * 9 + 1),
			new CandidateViewNode(WellKnownColorIdentifier.Endofin, 31 * 9 + 1),
			new CandidateViewNode(WellKnownColorIdentifier.Auxiliary1, 8 * 9 + 3),
			new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, 17 * 9 + 6),
			new CandidateViewNode(WellKnownColorIdentifier.Auxiliary3, 26 * 9 + 0),
			new CandidateViewNode(WellKnownColorIdentifier.AlmostLockedSet1, 58 * 9 + 6),
			new CandidateViewNode(WellKnownColorIdentifier.AlmostLockedSet1, 58 * 9 + 7),
			new CandidateViewNode(WellKnownColorIdentifier.AlmostLockedSet2, 67 * 9 + 0),
			new CandidateViewNode(WellKnownColorIdentifier.AlmostLockedSet2, 67 * 9 + 1),
			new CandidateViewNode(WellKnownColorIdentifier.AlmostLockedSet3, 76 * 9 + 0),
			new CandidateViewNode(WellKnownColorIdentifier.AlmostLockedSet3, 76 * 9 + 1),
			new CandidateViewNode(WellKnownColorIdentifier.AlmostLockedSet4, 77 * 9 + 1),
			new CandidateViewNode(WellKnownColorIdentifier.AlmostLockedSet4, 77 * 9 + 4),
			new CandidateViewNode(WellKnownColorIdentifier.AlmostLockedSet5, 72 * 9 + 2),
			new CandidateViewNode(WellKnownColorIdentifier.AlmostLockedSet5, 72 * 9 + 5),
			new CandidateViewNode(WellKnownColorIdentifier.Auxiliary1, 36 * 9 + 2),
			new CandidateViewNode(WellKnownColorIdentifier.Normal, 37 * 9 + 2),
			new CandidateViewNode(WellKnownColorIdentifier.Auxiliary1, 44 * 9 + 6),
			new CandidateViewNode(WellKnownColorIdentifier.Normal, 43 * 9 + 6),
			new CandidateViewNode(WellKnownColorIdentifier.Normal, 45 * 9 + 5),
			new CandidateViewNode(WellKnownColorIdentifier.Auxiliary1, 46 * 9 + 5),
			new CandidateViewNode(WellKnownColorIdentifier.Normal, 35 * 9 + 3),
			new CandidateViewNode(WellKnownColorIdentifier.Auxiliary1, 34 * 9 + 3),
			new LinkViewNode(WellKnownColorIdentifier.Normal, new(2, CellsMap[36]), new(2, CellsMap[37]), Inference.Strong),
			new LinkViewNode(WellKnownColorIdentifier.Normal, new(6, CellsMap[44]), new(6, CellsMap[43]), Inference.Strong),
			new LinkViewNode(WellKnownColorIdentifier.Normal, new(5, CellsMap[45]), new(5, CellsMap[46]), Inference.Weak),
			new LinkViewNode(WellKnownColorIdentifier.Normal, new(3, CellsMap[35]), new(3, CellsMap[34]), Inference.Weak)
		],
		Conclusions = [new(Assignment, 1, 8), new(Assignment, 79, 0), new(Elimination, 0, 8), new(Elimination, 80, 0)]
	};


	/// <summary>
	/// Initializes a <see cref="DrawingPreferenceItemsPage"/> instance.
	/// </summary>
	public DrawingPreferenceItemsPage() => InitializeComponent();


	/// <summary>
	/// Try to set color to the specified <see cref="ColorPalette"/> instance.
	/// </summary>
	/// <param name="palette">The instance.</param>
	/// <param name="index">The index to be set.</param>
	/// <param name="newColor">The new color to be set.</param>
	private void ChangeColor(ColorPalette palette, int index, Color newColor) => palette[index] = newColor;


	private void Page_Loaded(object sender, RoutedEventArgs e) => SampleSudokuGrid.ViewUnit = _defaultViewUnit;

	private void SampleSudokuGrid_Loaded(object sender, RoutedEventArgs e)
		=> ((App)Application.Current).CoverSettingsToSudokuPaneViaApplicationTheme(SampleSudokuGrid);

	private void SampleSudokuGrid_ActualThemeChanged(FrameworkElement sender, object args)
		=> ((App)Application.Current).CoverSettingsToSudokuPaneViaApplicationTheme(SampleSudokuGrid);

	private void DeltaCellColorSelector_ColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.DeltaValueColor = e;

	private void DeltaCandidateColorSelector_ColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.DeltaPencilmarkColor = e;

	private void BorderColorSelector_ColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.SudokuPaneBorderColor = e;

	private void CursorBackgroundColorSelector_ColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.CursorBackgroundColor = e;

	private void ChainColorSelector_ColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.ChainColor = e;

	private void NormalColorSelector_ColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.NormalColor = e;

	private void AssignmentColorSelector_ColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.AssignmentColor = e;

	private void OverlappedAssignmentColorSelector_ColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.OverlappedAssignmentColor = e;

	private void EliminationColorSelector_ColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.EliminationColor = e;

	private void CannibalismColorSelector_ColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.CannibalismColor = e;

	private void ExofinColorSelector_ColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.ExofinColor = e;

	private void EndofinColorSelector_ColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.EndofinColor = e;

	private void AuxiliaryColor1Selector_ColorChanged(object sender, Color e)
		=> ChangeColor(((App)Application.Current).Preference.UIPreferences.AuxiliaryColors, 0, e);

	private void AuxiliaryColor2Selector_ColorChanged(object sender, Color e)
		=> ChangeColor(((App)Application.Current).Preference.UIPreferences.AuxiliaryColors, 1, e);

	private void AuxiliaryColor3Selector_ColorChanged(object sender, Color e)
		=> ChangeColor(((App)Application.Current).Preference.UIPreferences.AuxiliaryColors, 2, e);

	private void AlmostLockedSetsColor1Selector_ColorChanged(object sender, Color e)
		=> ChangeColor(((App)Application.Current).Preference.UIPreferences.AlmostLockedSetsColors, 0, e);

	private void AlmostLockedSetsColor2Selector_ColorChanged(object sender, Color e)
		=> ChangeColor(((App)Application.Current).Preference.UIPreferences.AlmostLockedSetsColors, 1, e);

	private void AlmostLockedSetsColor3Selector_ColorChanged(object sender, Color e)
		=> ChangeColor(((App)Application.Current).Preference.UIPreferences.AlmostLockedSetsColors, 2, e);

	private void AlmostLockedSetsColor4Selector_ColorChanged(object sender, Color e)
		=> ChangeColor(((App)Application.Current).Preference.UIPreferences.AlmostLockedSetsColors, 3, e);

	private void AlmostLockedSetsColor5Selector_ColorChanged(object sender, Color e)
		=> ChangeColor(((App)Application.Current).Preference.UIPreferences.AlmostLockedSetsColors, 4, e);

	private void GivenFontPicker_SelectedFontChanged(object sender, string e)
		=> ((App)Application.Current).Preference.UIPreferences.GivenFontName = e;

	private void GivenFontPicker_SelectedFontScaleChanged(object sender, decimal e)
		=> ((App)Application.Current).Preference.UIPreferences.GivenFontScale = e;

	private void GivenFontPicker_SelectedFontColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.GivenFontColor = e;

	private void ModifiableFontPicker_SelectedFontChanged(object sender, string e)
		=> ((App)Application.Current).Preference.UIPreferences.ModifiableFontName = e;

	private void ModifiableFontPicker_SelectedFontScaleChanged(object sender, decimal e)
		=> ((App)Application.Current).Preference.UIPreferences.ModifiableFontScale = e;

	private void ModifiableFontPicker_SelectedFontColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.ModifiableFontColor = e;

	private void PencilmarkFontPicker_SelectedFontChanged(object sender, string e)
		=> ((App)Application.Current).Preference.UIPreferences.PencilmarkFontName = e;

	private void PencilmarkFontPicker_SelectedFontScaleChanged(object sender, decimal e)
		=> ((App)Application.Current).Preference.UIPreferences.PencilmarkFontScale = e;

	private void PencilmarkFontPicker_SelectedFontColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.PencilmarkFontColor = e;

	private void CoordinateFontPicker_SelectedFontChanged(object sender, string e)
		=> ((App)Application.Current).Preference.UIPreferences.CoordinateLabelFontName = e;

	private void CoordinateFontPicker_SelectedFontScaleChanged(object sender, decimal e)
		=> ((App)Application.Current).Preference.UIPreferences.CoordinateLabelFontScale = e;

	private void CoordinateFontPicker_SelectedFontColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.CoordinateLabelFontColor = e;

	private void BabaGroupingFontPicker_SelectedFontChanged(object sender, string e)
		=> ((App)Application.Current).Preference.UIPreferences.BabaGroupingFontName = e;

	private void BabaGroupingFontPicker_SelectedFontScaleChanged(object sender, decimal e)
		=> ((App)Application.Current).Preference.UIPreferences.BabaGroupingFontScale = e;

	private void BabaGroupingFontPicker_SelectedFontColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.BabaGroupingFontColor = e;

	private void StrongLinkDashStyleBox_DashArrayChanged(object sender, DashArray e)
		=> ((App)Application.Current).Preference.UIPreferences.StrongLinkDashStyle = e;

	private void WeakLinkDashStyleBox_DashArrayChanged(object sender, DashArray e)
		=> ((App)Application.Current).Preference.UIPreferences.WeakLinkDashStyle = e;

	private void CycleLikeCellLinkDashStyleBox_DashArrayChanged(object sender, DashArray e)
		=> ((App)Application.Current).Preference.UIPreferences.CyclingCellLinkDashStyle = e;
}
