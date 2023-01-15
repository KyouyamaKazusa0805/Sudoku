namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Indicates the type is a large structure.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, Inherited = false)]
public sealed class IsLargeStructAttribute : Attribute
{
}
