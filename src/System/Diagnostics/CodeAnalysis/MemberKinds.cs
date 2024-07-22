namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Indicates a data member kind.
/// </summary>
public static class MemberKinds
{
	/// <summary>
	/// Indicates the member kind is field.
	/// </summary>
	public const string Field = nameof(Field);

	/// <summary>
	/// Indicates the member kind is property.
	/// </summary>
	public const string Property = nameof(Property);
}