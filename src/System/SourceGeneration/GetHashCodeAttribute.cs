namespace System.SourceGeneration;

/// <summary>
/// Represents an attribute type that allows source generators controlling behaviors on generating <see cref="object.GetHashCode"/> method.
/// </summary>
/// <param name="behavior">Represents a kind of behavior on generated expression on comparing equality for instances.</param>
/// <seealso cref="object.GetHashCode"/>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed partial class GetHashCodeAttribute([PrimaryCosntructorParameter] GetHashCodeBehavior behavior = GetHashCodeBehavior.Intelligent) : PatternOverriddenAttribute;
