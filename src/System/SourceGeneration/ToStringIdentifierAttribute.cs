namespace System.SourceGeneration;

/// <summary>
/// Indicates the identifier name displayed in generated code for <c>ToString</c> method.
/// </summary>
/// <param name="displayMemberName">The display member name.</param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false)]
public sealed class ToStringIdentifierAttribute(string displayMemberName) : Attribute;
