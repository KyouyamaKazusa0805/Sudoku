namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents an enumeration type that tells source generators which case of nullability should be preferred.
/// By default, <see langword="not null"/> for value types and including <see langword="null"/> for reference types.
/// </summary>
public enum NullabilityPrefer
{
	/// <summary>
	/// Indicates the preferring is default.
	/// </summary>
	Default,

	/// <summary>
	/// Indicates the source generator will prefer <see langword="not null"/>.
	/// </summary>
	NotNull,

	/// <summary>
	/// Indicates the source generator will prefer including <see langword="null"/>.
	/// </summary>
	IncludeNull
}
