namespace System.SourceGeneration;

/// <summary>
/// Represents an attribute type that describes source generators can generate extra source code into the target type,
/// with the specified generation mode.
/// </summary>
/// <param name="flags">Indicates the flags whose corresponding member will be generated.</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
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
	/// Indicates the extra modifiers can be applied to <see cref="object.GetHashCode"/> method.
	/// </summary>
	public string? OtherModifiersOnGetHashCode { get; init; }

	/// <summary>
	/// Indicates the extra modifiers can be applied to <see cref="object.ToString"/> method.
	/// </summary>
	public string? OtherModifiersOnToString { get; init; }

	/// <summary>
	/// Defines a behavior by source generator on generating <see cref="object.Equals(object?)"/> overridden methods.
	/// </summary>
	/// <remarks>
	/// The value is <see cref="EqualsBehavior.Intelligent"/> by default.
	/// </remarks>
	public EqualsBehavior EqualsBehavior { get; init; } = EqualsBehavior.Intelligent;

	/// <summary>
	/// Defines a behavior by source generator on generating <see cref="object.GetHashCode"/> overridden methods.
	/// </summary>
	/// <remarks>
	/// The value is <see cref="GetHashCodeBehavior.Intelligent"/> by default.
	/// </remarks>
	public GetHashCodeBehavior GetHashCodeBehavior { get; init; } = GetHashCodeBehavior.Intelligent;

	/// <summary>
	/// Defines a behavior by source generator on generating <see cref="object.ToString"/> overridden methods.
	/// </summary>
	/// <remarks>
	/// The value is <see cref="ToStringBehavior.Intelligent"/> by default.
	/// </remarks>
	public ToStringBehavior ToStringBehavior { get; init; } = ToStringBehavior.Intelligent;

	/// <summary>
	/// Indicates an extra option to tell source generators which case of nullability should be preferred.
	/// By default, not null for value types and including null for reference types.
	/// </summary>
	public NullabilityPrefer OperandNullabilityPrefer { get; init; } = NullabilityPrefer.Default;
}
