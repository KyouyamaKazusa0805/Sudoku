namespace SudokuStudio.Views.Pages.Settings;

/// <summary>
/// Represents an analysis preference items page.
/// </summary>
public sealed partial class AnalysisPreferenceItemsPage : Page
{
	/// <summary>
	/// Initializes an <see cref="AnalysisPreferenceItemsPage"/> instance.
	/// </summary>
	public AnalysisPreferenceItemsPage() => InitializeComponent();


	private void DirectTechniquesSettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(DirectTechniquesSettingPage), true);

	private void IttoryuSettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(IttoryuSettingPage), true);

	private void FishSettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(FishSettingPage), true);

	private void SingleDigitPatternSettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(SingleDigitPatternSettingPage), true);

	private void DeadlyPatternSettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(DeadlyPatternSettingPage), true);

	private void AlmostLockedSetsAndIntersectionSettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(AlmostLockedSetAndIntersectionSettingPage), true);

	private void BabaGroupingSettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(BabaGroupingSettingPage), true);

	private void PermutationSettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(PermutationSettingPage), true);

	private void ChainSettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(ChainOrLoopSettingPage), true);

	private void DualitySettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(DualitySettingPage), true);

	private void StepSearcherBehaviorSettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(StepSearcherBehaviorsSettingPage), true);

	private void CompatibilitySettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(CompatibilitySettingPage), true);

	private void MiscellaneousBehaviorsSettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(MiscellaneousAnalysisSettingPage), true);
}
