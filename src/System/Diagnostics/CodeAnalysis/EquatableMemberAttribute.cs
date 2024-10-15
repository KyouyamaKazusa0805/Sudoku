namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Indicates the marked member will participate equality comparison operation, especially for <see cref="IEquatable{T}.Equals(T)"/>.
/// </summary>
/// <seealso cref="IEquatable{T}.Equals(T)"/>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false)]
public sealed class EquatableMemberAttribute : ComponentMemberAttribute;
