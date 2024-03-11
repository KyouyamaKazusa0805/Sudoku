namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents a bindable source for collected steps.
/// </summary>
internal sealed class CollectedStepBindableSource
{
	/// <summary>
	/// Indicates the title.
	/// </summary>
	public required string Title { get; set; }

	/// <summary>
	/// Indicates the step.
	/// </summary>
	public Step? Step { get; set; }

	/// <summary>
	/// Indicates the values.
	/// </summary>
	public ObservableCollection<CollectedStepBindableSource>? Children { get; set; }

	/// <summary>
	/// Indicates the description.
	/// </summary>
	public IEnumerable<Inline>? Description { get; set; }
}
