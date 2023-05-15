namespace System.SourceGeneration;

/// <summary>
/// Defines an attribute that specifies a parameter used by a deconstruction method.
/// </summary>
/// <param name="referencedMemberName">The referenced member name.</param>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
public sealed class DeconstructionMethodArgumentAttribute(string referencedMemberName) : Attribute;
