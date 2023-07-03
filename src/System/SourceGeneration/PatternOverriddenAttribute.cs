namespace System.SourceGeneration;

/// <summary>
/// Represents a base type that provides base properties used by derived attribute types.s
/// </summary>
public abstract class PatternOverriddenAttribute : Attribute
{
	/// <summary>
	/// <para>
	/// Indicates the other modifiers should be modified. For example, if the method should be <see langword="sealed"/>
	/// in a <see langword="class"/> type, assign <c>"Sealed"</c> or <c>"sealed"</c> to this property. Casing are ignored.
	/// Multiple modifiers should be separated by whitespaces.
	/// </para>
	/// <para>By default, this property is <see langword="null"/>.</para>
	/// </summary>
	/// <remarks>
	/// Please note, sometimes some extra keywords will be automatically added into signature without manually assigning them to this property.
	/// For example, in a non-<see langword="readonly struct"/>, due to the implementation not caring
	/// about its <see langword="readonly"/>-ability, a <see langword="readonly"/> modifier will be implicitly added.
	/// </remarks>
	public string? OtherModifiers { get; init; }
}
