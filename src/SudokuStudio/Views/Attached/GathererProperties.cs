namespace SudokuStudio.Views.Attached;

/// <summary>
/// Defines a bind behaviors on <see cref="SudokuPane"/> instances, for <see cref="Collector"/> instance's interaction.
/// </summary>
/// <seealso cref="SudokuPane"/>
/// <seealso cref="Collector"/>
[AttachedProperty<int>(SettingItemNames.StepGathererMaxStepsGathered, DefaultValue = 1000)]
[AttachedProperty<int>(SettingItemNames.DifficultyLevelMode, DefaultValue = 0)]
public static partial class GathererProperties
{
	[Callback]
	private static void StepGathererOnlySearchSameLevelTechniquesInFindAllStepsPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetStepCollector((SudokuPane)d).WithSameLevelConfigruation((CollectorDifficultyLevelMode)(int)e.NewValue);

	[Callback]
	private static void StepGathererMaxStepsGatheredPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetStepCollector((SudokuPane)d).WithMaxSteps((int)e.NewValue);
}
