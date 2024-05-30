namespace System.SourceGeneration;

/// <summary>
/// Represents an attribute type that describes source generators can generate extra source code into the target type,
/// with the specified generation mode.
/// </summary>
/// <param name="flags">Indicates the flags whose corresponding member will be generated.</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
public sealed partial class TypeImplAttribute([PrimaryConstructorParameter] TypeImplFlag flags) : Attribute
{
	/// <summary>
	/// Indicates whether source generators will generate source code with modifiers <see langword="ref readonly"/>
	/// or <see langword="in"/> onto parameters if the parameter type is the current type.
	/// </summary>
	/// <remarks>
	/// The value is <see langword="false"/> by default.
	/// </remarks>
	public bool IsLargeStructure { get; init; } = false;

	/// <summary>
	/// Indicates the extra modifiers can be applied to <see cref="object.Equals(object?)"/> method.
	/// </summary>
	public string? OtherModifiersOnEquals { get; init; }

	/// <summary>
	/// Represents a kind of behavior on generated expression on comparing equality for instances.
	/// </summary>
	/// <remarks>
	/// The value is <see cref="EqualsBehavior.Intelligent"/> by default.
	/// </remarks>
	public EqualsBehavior EqualsBehavior { get; init; } = EqualsBehavior.Intelligent;
}
