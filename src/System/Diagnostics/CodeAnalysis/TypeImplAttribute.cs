namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents an attribute type that can help developers declare types simpler and easier,
/// through generating particular code, like automatically implementing <see cref="IEquatable{T}"/>
/// by using source generator.
/// </summary>
/// <param name="flags">Indicates the flags whose corresponding member will be generated.</param>
/// <remarks>
/// For example, we have defined a record-like type <c>MyColor</c> declared like this:
/// <code><![CDATA[
/// public readonly struct MyColor(byte a, byte r, byte g, byte b) : IEquatable<MyColor>
/// {
///     public byte A { get; } = a;
///     public byte R { get; } = r;
///     public byte G { get; } = g;
///     public byte B { get; } = b;
///     private int RawValue => A << 24 | R << 16 | G << 8 | B;
/// 
///     public override bool Equals([NotNullWhen(true)] object? other)
///         => other is MyColor comparer && Equals(comparer);
/// 
///     public bool Equals(MyColor other) => RawValue == other.RawValue;
/// 
///     public override int GetHashCode() => RawValue;
/// }
/// ]]></code>
/// By using <see cref="TypeImplAttribute"/>, the code can be simplified to this:
/// <code><![CDATA[
/// [TypeImpl(TypeImplFlag.Equals | TypeImplFlag.GetHashCode | TypeImplFlag.Equatable)]
/// public readonly partial struct MyColor([Property] byte a, [Property] byte r, [Property] byte g, [Property] byte b) : IEquatable<MyColor>
/// {
///     [HashCodeMember]
///     [EquatableMember]
///     private int RawValue => A << 24 | R << 16 | G << 8 | B;
/// }
/// ]]></code>
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed partial class TypeImplAttribute([Property] TypeImplFlag flags) : Attribute
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
	/// Indicates whether the source generator will emit <c>(Interface)<see langword="this"/></c> on implementation,
	/// especially for cases on calling overloads.
	/// </summary>
	/// <remarks>
	/// The value is <see langword="false"/> by default.
	/// </remarks>
	public bool EmitThisCastToInterface { get; init; } = false;

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
	/// Indicates the extra modifiers can be applied to <see cref="IEquatable{T}.Equals(T)"/> method.
	/// </summary>
	public string? OtherModifiersOnEquatableEquals { get; init; }

	/// <summary>
	/// <para>
	/// Indicates the modifier that will be applied to implement <c>Equals</c>, as the parameter modifier,
	/// if the corresponding type (containing type) is a large structure.
	/// </para>
	/// <para>
	/// By default, the modifier is <see langword="ref readonly"/>.
	/// Supported values can be <see langword="ref"/>, <see langword="ref readonly"/> or <see langword="in"/>.
	/// </para>
	/// </summary>
	public string EquatableLargeStructModifier { get; init; } = "ref readonly";

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
