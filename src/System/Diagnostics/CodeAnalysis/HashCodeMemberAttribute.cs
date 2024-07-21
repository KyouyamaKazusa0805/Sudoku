namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Indicates the marked member will participate hashing operation.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter, Inherited = false)]
public sealed class HashCodeMemberAttribute : Attribute;
