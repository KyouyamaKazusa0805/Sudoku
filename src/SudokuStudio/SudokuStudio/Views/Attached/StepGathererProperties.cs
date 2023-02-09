#pragma warning disable CS1591

namespace SudokuStudio.Views.Attached;

using static SudokuPaneBindable;

/// <summary>
/// Defines a bind behaviors on <see cref="SudokuPane"/> instances, for <see cref="StepsGatherer"/> instance's interaction.
/// </summary>
/// <seealso cref="SudokuPane"/>
/// <seealso cref="StepsGatherer"/>
public static class StepGathererProperties
{
	public static readonly DependencyProperty StepGathererOnlySearchSameLevelTechniquesInFindAllStepsProperty =
		DependencyProperty.RegisterAttached(
			"StepGathererOnlySearchSameLevelTechniquesInFindAllSteps",
			typeof(bool),
			typeof(SudokuPaneBindable),
			new(true, static (d, e) => GetProgramStepGatherer((SudokuPane)d).OnlyShowSameLevelTechniquesInFindAllSteps = (bool)e.NewValue)
		);

	public static readonly DependencyProperty StepGathererMaxStepsGatheredProperty =
		DependencyProperty.RegisterAttached(
			"StepGathererMaxStepsGathered",
			typeof(int),
			typeof(SudokuPaneBindable),
			new(true, static (d, e) => GetProgramStepGatherer((SudokuPane)d).MaxStepsGathered = (int)e.NewValue)
		);


	public static void SetStepGathererOnlySearchSameLevelTechniquesInFindAllSteps(DependencyObject obj, bool value)
		=> obj.SetValue(StepGathererOnlySearchSameLevelTechniquesInFindAllStepsProperty, value);

	public static void SetStepGathererMaxStepsGathered(DependencyObject obj, int value)
		=> obj.SetValue(StepGathererMaxStepsGatheredProperty, value);

	public static bool GetStepGathererOnlySearchSameLevelTechniquesInFindAllSteps(DependencyObject obj)
		=> (bool)obj.GetValue(StepGathererOnlySearchSameLevelTechniquesInFindAllStepsProperty);

	public static int GetStepGathererMaxStepsGathered(DependencyObject obj) => (int)obj.GetValue(StepGathererMaxStepsGatheredProperty);
}
