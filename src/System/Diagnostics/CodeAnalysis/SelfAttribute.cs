namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Indicates the generic argument is a self type.
/// </summary>
[AttributeUsage(AttributeTargets.GenericParameter, Inherited = false)]
public sealed class SelfAttribute : Attribute
{
}
