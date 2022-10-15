namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Indicates a field of function pointer type that cannot be invokable outside the scope of the type declaring that field.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class DisallowFunctionPointerInvocationAttribute : Attribute
{
}
