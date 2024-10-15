namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents an attribute type that can be applied to a field or a property,
/// indicating the member can be used and called by methods
/// <see cref="IDisposable.Dispose"/> and <see cref="IAsyncDisposable.DisposeAsync"/>.
/// </summary>
/// <seealso cref="IDisposable.Dispose"/>
/// <seealso cref="IAsyncDisposable.DisposeAsync"/>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false)]
public sealed class DisposableMemberAttribute : ComponentMemberAttribute;
