namespace SudokuStudio.Views.Attached;

/// <summary>
/// Defines a bind behaviors on <see cref="SudokuPane"/> instances, for <see cref="Collector"/> instance's interaction.
/// </summary>
/// <seealso cref="SudokuPane"/>
/// <seealso cref="Collector"/>
public static partial class CollectorProperties
{
	/// <summary>
	/// Indicates the number of maximal steps to be collected.
	/// </summary>
	[DependencyProperty(DefaultValue = 1000)]
	public static partial int CollectorMaxStepsCollected { get; set; }

	/// <summary>
	/// Indicates the difficulty level mode. Casted from <see cref="CollectorDifficultyLevelMode"/>.
	/// </summary>
	/// <seealso cref="CollectorDifficultyLevelMode"/>
	[DependencyProperty(DefaultValue = 0)]
	public static partial int DifficultyLevelMode { get; set; }


	[Callback]
	private static void DifficultyLevelModePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetStepCollector((SudokuPane)d).WithSameLevelConfiguration((CollectorDifficultyLevelMode)(int)e.NewValue);

	[Callback]
	private static void CollectorMaxStepsCollectedPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetStepCollector((SudokuPane)d).WithMaxSteps((int)e.NewValue);
}
