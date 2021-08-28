namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// To mark onto a parameter or the return value, to tell the user that the memory must be released after used,
/// due to the unmanaged memory allocation.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
[Conditional("SOLUTION_WIDE_CODE_ANALYSIS")]
public sealed class CallerNeedReleaseAttribute : Attribute
{
}
