namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Indicates a field of function pointer type that cannot be invokable. 
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class DisallowFunctionPointerInvocationAttribute : Attribute
{
}
