namespace Sudoku.Runtime.DisplayingServices;

/// <summary>
/// Represents an attribute that can modify step displaying features,
/// which can control the target displaying UI controls.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class StepDisplayingFeatureAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="StepDisplayingFeatureAttribute"/> instance via the specified runtime features.
	/// </summary>
	/// <param name="features">
	/// The runtime features. If you want to apply multiple different kinds of features,
	/// just use <c><see langword="operator"/> |</c>, such as the expression <c></c>.
	/// </param>
	public StepDisplayingFeatureAttribute(StepDisplayingFeature features) => Features = features;


	/// <summary>
	/// Indicates the features that can be used on rendering and displaying runtime UI controls.
	/// </summary>
	public StepDisplayingFeature Features { get; }

	/// <summary>
	/// Indicates the extra member to be verified.
	/// </summary>
	[NotNullIfNotNull(nameof(VerifyMemberValue))]
	public string? VerifyMemberName { get; init; }

	/// <summary>
	/// Indicates the value to the extra member to be verified.
	/// </summary>
	[NotNullIfNotNull(nameof(VerifyMemberName))]
	public object? VerifyMemberValue { get; init; }
}
