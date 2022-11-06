namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that is used as a parameter mark that tells source generators to route to the specified target expression.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class GeneratedDeconstructionArgumentAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="GeneratedDeconstructionArgumentAttribute"/> instance via the specified target property.
	/// </summary>
	/// <param name="targetPropertyExpression">The target property expression.</param>
	public GeneratedDeconstructionArgumentAttribute(string targetPropertyExpression) => TargetPropertyExpression = targetPropertyExpression;

	/// <summary>
	/// Indicates the target property expression used. This property specifies and controls the target behavior
	/// that output what final deconstruction assignment expression should be.
	/// </summary>
	public string TargetPropertyExpression { get; }
}
