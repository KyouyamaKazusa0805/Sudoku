namespace System.SourceGeneration;

/// <summary>
/// Indicates the display name on generated code of <c>ToString</c> method.
/// </summary>
/// <param name="displayMemberName">The display member name.</param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false)]
public sealed class GeneratedDisplayNameAttribute(string displayMemberName) : Attribute;
