namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents a bindable source for collected steps.
/// </summary>
/// <param name="title">Indicates the title.</param>
/// <param name="step">Indicates the step.</param>
/// <param name="children">Indicates the values.</param>
/// <param name="description">Indicates the description.</param>
[method: SetsRequiredMembers]
internal sealed partial class CollectedStepBindableSource(
	[Property(Accessibility = "public required", Setter = PropertySetters.Set)] string title,
	[Property(Accessibility = "public required", Setter = PropertySetters.Set)] Step? step,
	[Property(Accessibility = "public required", Setter = PropertySetters.Set)] IEnumerable<CollectedStepBindableSource>? children,
	[Property(Accessibility = "public required", Setter = PropertySetters.Set)] IEnumerable<Inline>? description
);
