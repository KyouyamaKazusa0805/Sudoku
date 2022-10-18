namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Indicates the specified field can be accessed only in the current file where the field is declared.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Constructor, Inherited = false)]
public sealed class FileAccessOnlyAttribute : Attribute
{
}
