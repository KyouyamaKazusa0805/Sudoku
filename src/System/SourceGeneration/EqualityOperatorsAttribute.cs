namespace System.SourceGeneration;

/// <summary>
/// Represents an attribute type that allows source generators generating for equality operators if the specified types marked this attribute.
/// </summary>
/// <param name="behavior">Represents a kind of behavior on generated expression on comparing equality for instances.</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
public sealed partial class EqualityOperatorsAttribute(
	[PrimaryConstructorParameter] EqualityOperatorsBehavior behavior = EqualityOperatorsBehavior.Intelligent
) : PatternOverriddenAttribute
{
	/// <summary>
	/// Indicates an extra option to tell source generators which case of nullability should be preferred.
	/// By default, not null for value types and including null for reference types.
	/// </summary>
	public NullabilityPrefer NullabilityPrefer { get; init; } = NullabilityPrefer.Default;
}
