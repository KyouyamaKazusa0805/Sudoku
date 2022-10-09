namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Indicates the type is a large structure.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, Inherited = false)]
public sealed class LargeStructAttribute : Attribute
{
	/// <summary>
	/// Indicates the suggested member name.
	/// </summary>
	[DisallowNull]
	public string? SuggestedMemberName { get; init; }
}
