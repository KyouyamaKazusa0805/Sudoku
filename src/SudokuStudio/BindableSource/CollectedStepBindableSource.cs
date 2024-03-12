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
	[PrimaryConstructorParameter(Accessibility = "public required", SetterExpression = "set")] string title,
	[PrimaryConstructorParameter(Accessibility = "public required", SetterExpression = "set")] Step? step,
	[PrimaryConstructorParameter(Accessibility = "public required", SetterExpression = "set")] IEnumerable<CollectedStepBindableSource>? children,
	[PrimaryConstructorParameter(Accessibility = "public required", SetterExpression = "set")] IEnumerable<Inline>? description
);
