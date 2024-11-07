namespace Sudoku.Analytics.Intercepting;

/// <summary>
/// (<b>Interceptor-related attribute</b>)<br/>
/// Represents an attribute type that can be applied to a method, indicating the method uses interceptor
/// (contains interceptor method invocation).
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class InterceptorMethodCallerAttribute : Attribute;
