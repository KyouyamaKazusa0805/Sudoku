namespace System.SourceGeneration;

/// <summary>
/// Represents an attribute type that allows source generators generating for comparison operators if the specified types marked this attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class ComparisonOperatorsAttribute : PatternOverriddenAttribute;
