namespace SudokuStudio.Interaction.DataTemplateSelectors;

/// <summary>
/// Represents a template selector.
/// </summary>
public sealed class StepCollectingInfoTemplateSelector : DataTemplateSelector
{
	/// <summary>
	/// Indicates the root template.
	/// </summary>
	public DataTemplate RootTemplate { get; set; } = null!;

	/// <summary>
	/// Indicates the leaf template.
	/// </summary>
	public DataTemplate LeafTemplate { get; set; } = null!;


	/// <inheritdoc/>
	protected override DataTemplate SelectTemplateCore(object item)
		=> item is CollectedStepBindableSource { Step: not null } ? LeafTemplate : RootTemplate;
}
