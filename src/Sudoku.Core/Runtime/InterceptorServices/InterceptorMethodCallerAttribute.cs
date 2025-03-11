namespace Sudoku.Runtime.InterceptorServices;

/// <summary>
/// Indicates the target method marked this attribute will be replaced with intercepted methods.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class InterceptorMethodCallerAttribute : Attribute;
