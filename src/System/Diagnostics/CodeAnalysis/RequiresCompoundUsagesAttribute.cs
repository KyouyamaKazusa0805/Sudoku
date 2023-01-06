namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Defines an attribute that requires an operator usage can only be a compound one.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class RequiresCompoundUsagesAttribute : Attribute
{
}
