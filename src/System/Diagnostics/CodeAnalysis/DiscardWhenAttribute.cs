namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// To mark onto a parameter, to tell the compiler that the argument must be a discard
/// and can't be used when the return value is specified value.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
[Conditional("SOLUTION_WIDE_CODE_ANALYSIS")]
[AutoGeneratePrimaryConstructor]
public sealed partial class DiscardWhenAttribute : Attribute
{
	/// <summary>
	/// Indicates the return value that makes the parameter a discard.
	/// </summary>
	public bool ReturnValue { get; }
}
