namespace System.SourceGeneration;

/// <summary>
/// Represents an attribute type that allows source generators controlling behaviors on generating <see cref="object.Equals(object?)"/> method.
/// </summary>
/// <param name="behavior">Represents a kind of behavior on generated expression on comparing equality for instances.</param>
/// <seealso cref="object.Equals(object?)"/>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed partial class EqualsAttribute([Data] EqualsBehavior behavior = EqualsBehavior.Intelligent) : PatternOverriddenAttribute;
