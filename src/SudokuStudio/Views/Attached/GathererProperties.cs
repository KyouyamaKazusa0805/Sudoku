using Microsoft.UI.Xaml;
using Sudoku.Analytics;
using Sudoku.Analytics.Metadata;
using SudokuStudio.ComponentModel;
using SudokuStudio.Views.Controls;

namespace SudokuStudio.Views.Attached;

/// <summary>
/// Defines a bind behaviors on <see cref="SudokuPane"/> instances, for <see cref="StepCollector"/> instance's interaction.
/// </summary>
/// <seealso cref="SudokuPane"/>
/// <seealso cref="StepCollector"/>
[AttachedProperty<int>(RuntimeIdentifier.StepGathererMaxStepsGathered, DefaultValue = 1000)]
[AttachedProperty<int>(RuntimeIdentifier.DifficultyLevelMode, DefaultValue = 0)]
public static partial class GathererProperties
{
	[Callback]
	private static void StepGathererOnlySearchSameLevelTechniquesInFindAllStepsPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetStepCollector((SudokuPane)d).WithSameLevelConfigruation((StepCollectorDifficultyLevelMode)(int)e.NewValue);

	[Callback]
	private static void StepGathererMaxStepsGatheredPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetStepCollector((SudokuPane)d).WithMaxSteps((int)e.NewValue);
}
