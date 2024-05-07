namespace SudokuStudio.Configuration;

/// <summary>
/// Defines a serialization data of a step searcher.
/// </summary>
[DependencyProperty<bool>("IsEnabled", DefaultValue = true, DocSummary = "Indicates whether the step searcher is enabled.")]
[DependencyProperty<string>("Name", DocSummary = "Indicates the name of the step searcher.")]
[DependencyProperty<string>("TypeName", DocSummary = "Indicates the type name of the step searcher. This property can be used for creating instances via reflection using getMetaProperties <see cref=\"Activator.CreateInstance(Type)\"/>.")]
public sealed partial class StepSearcherInfo : DependencyObject
{
	/// <summary>
	/// Indicates whether the technique option is not fixed and can be used for drag-and-drop operation.
	/// </summary>
	public bool CanDrag => !CreateStepSearchers()[0].Metadata.IsOrderingFixed;

	/// <summary>
	/// Indicates whether the technique option is not read-only and can be used for toggle operation.
	/// </summary>
	public bool CanToggle => !CreateStepSearchers()[0].Metadata.IsReadOnly;

	/// <summary>
	/// Creates a list of <see cref="StepSearcher"/> instances.
	/// </summary>
	/// <returns>A list of <see cref="StepSearcher"/> instances.</returns>
	public StepSearcher[] CreateStepSearchers() => StepSearcherPool.GetStepSearchers(TypeName, true);


	/// <inheritdoc/>
	public override string ToString()
		=> $$"""{{nameof(StepSearcherInfo)}} { {{nameof(IsEnabled)}} = {{IsEnabled}}, {{nameof(Name)}} = {{Name}}, {{nameof(TypeName)}} = {{TypeName}} }""";
}
