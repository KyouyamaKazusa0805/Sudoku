namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that is used as a parameter mark that tells source generators to route to the specified target expression.
/// </summary>
/// <param name="targetPropertyExpression">
/// Indicates the target property expression used. This property specifies and controls the target behavior
/// that output what final deconstruction assignment expression should be.
/// </param>
[AttributeUsage(AttributeTargets.Parameter)]
[Obsolete("This type is being deprecated because the future C# version will support the extension feature 'Roles & Extensions'. For more information, please visit Roslyn repo to learn more information.", false)]
public sealed class GeneratedDeconstructionArgumentAttribute(
#pragma warning disable IDE0060, CS9113
	string targetPropertyExpression
#pragma warning restore IDE0060, CS9113
) : Attribute;
