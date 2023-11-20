namespace System.SourceGeneration;

/// <summary>
/// Represents an attribute type that allows source generators controlling behaviors on generating <see cref="object.ToString"/> method.
/// </summary>
/// <param name="behavior">Represents a kind of behavior on generated expression on comparing equality for instances.</param>
/// <seealso cref="object.ToString"/>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed partial class ToStringAttribute([Data] ToStringBehavior behavior = ToStringBehavior.Intelligent) : PatternOverriddenAttribute;
