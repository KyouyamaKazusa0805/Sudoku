namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that is used as a parameter mark that tells source generators to route to the specified target expression.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
[Obsolete("This type is being deprecated because the future C# version will support the extension feature 'Roles & Extensions'. For more information, please visit Roslyn repo to learn more information.", false)]
public sealed class GeneratedDeconstructionArgumentAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="GeneratedDeconstructionArgumentAttribute"/> instance via the specified target property.
	/// </summary>
	/// <param name="targetPropertyExpression">
	/// Indicates the target property expression used. This property specifies and controls the target behavior
	/// that output what final deconstruction assignment expression should be.
	/// </param>
	public GeneratedDeconstructionArgumentAttribute(
		[SuppressMessage("Style", IDE0060, Justification = Pending)] string targetPropertyExpression
	)
	{
	}
}
