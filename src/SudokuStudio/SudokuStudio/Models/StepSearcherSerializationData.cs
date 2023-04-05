namespace SudokuStudio.Models;

/// <summary>
/// Defines a serialization data of a step searcher.
/// </summary>
[DependencyProperty<bool>("IsEnabled", DefaultValue = true, DocSummary = "Indicates whether the step searcher is enabled.")]
[DependencyProperty<string>("Name", DocSummary = "Indicates the name of the step searcher.")]
[DependencyProperty<string>("TypeName", DocSummary = "Indicates the type name of the step searcher. This property can be used for creating instances via reflection using method <see cref=\"Activator.CreateInstance(Type)\"/>.")]
public sealed partial class StepSearcherSerializationData : DependencyObject
{
	/// <summary>
	/// Creates a list of <see cref="StepSearcher"/> instances.
	/// </summary>
	/// <returns>A list of <see cref="StepSearcher"/> instances.</returns>
	public StepSearcher[] CreateStepSearchers()
		=> StepSearcherPool.GetStepSearchers(typeof(StepSearcher).Assembly.GetType($"Sudoku.Analytics.StepSearchers.{TypeName}")!, true);
}
