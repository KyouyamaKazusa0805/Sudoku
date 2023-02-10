namespace SudokuStudio.Views.Attached;

/// <summary>
/// Defines a bind behaviors on <see cref="SudokuPane"/> instances, for <see cref="StepsGatherer"/> instance's interaction.
/// </summary>
/// <seealso cref="SudokuPane"/>
/// <seealso cref="StepsGatherer"/>
[AttachedProperty<bool>("StepGathererOnlySearchSameLevelTechniquesInFindAllSteps", DefaultValue = true, CallbackMethodName = nameof(StepGathererOnlySearchSameLevelTechniquesInFindAllStepsPropertyCallback))]
[AttachedProperty<int>("StepGathererMaxStepsGathered", DefaultValue = 1000, CallbackMethodName = nameof(StepGathererMaxStepsGatheredPropertyCallback))]
public static partial class StepGathererProperties
{
	private static void StepGathererOnlySearchSameLevelTechniquesInFindAllStepsPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramStepGatherer((SudokuPane)d).OnlyShowSameLevelTechniquesInFindAllSteps = (bool)e.NewValue;

	private static void StepGathererMaxStepsGatheredPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramStepGatherer((SudokuPane)d).MaxStepsGathered = (int)e.NewValue;
}
