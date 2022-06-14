namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attributes that allows user using it applying to a type
/// (especially for <see langword="class"/> or <see langword="struct"/>), indicating the overridden metadata
/// for method <see cref="object.Equals(object?)"/>, and make source generator generates their own <c>Equals</c>
/// method automatically.
/// </summary>
/// <remarks>
/// At default, the generated method will emit operator <see langword="is"/> to determine whether the instance
/// is of the current type, and compares the inner values one by one using <c>operator ==</c> if implemented,
/// or invoking <see cref="EqualityComparer{T}.Default"/> to compare two values.
/// For example, if the attribute references two properties in the type:
/// <code><![CDATA[
/// [AutoOverridesEquals(nameof(A), nameof(B))]
/// public sealed class ExampleType
/// {
///     public int A { get; } = 42;
///     public int B { get; } = 108;
/// }
/// ]]></code>
/// The generated code will be:
/// <code><![CDATA[
/// public override bool Equals(object? obj) => Equals(comparer as ExampleType);
/// 
/// public bool Equals(ExampleType? other) => other is not null && A == other.A && B == other.B;
/// ]]></code>
/// </remarks>
/// <seealso cref="EqualityComparer{T}.Default"/>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class AutoOverridesEqualsAttribute :
	SourceGeneratorOptionProviderAttribute,
	IMultipleMembersBinder,
	IInModifierEmitter,
	ISealedModifierEmitter,
	IInterfaceImplementingCaseController
{
	/// <summary>
	/// Initializes an <see cref="AutoOverridesEqualsAttribute"/> instance via the specified array
	/// of <see cref="string"/> elements indicating the names of the data members you want to take part in
	/// the hash code calculation.
	/// </summary>
	/// <param name="memberExpressions">The name of data members, represented as a <see cref="string"/> array.</param>
	public AutoOverridesEqualsAttribute(params string[] memberExpressions) => MemberExpressions = memberExpressions;


	/// <inheritdoc/>
	public bool UseExplicitImplementation { get; init; } = false;

	/// <inheritdoc/>
	public bool EmitsSealedKeyword { get; init; } = false;

	/// <inheritdoc/>
	public bool EmitsInKeyword { get; init; } = false;

	/// <inheritdoc/>
	public string[] MemberExpressions { get; }
}
