namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attributes that allows user using it applying to a type
/// (especially for <see langword="class"/> or <see langword="struct"/>), indicating the overriden metadata
/// for method <see cref="object.GetHashCode"/>, and make source generator generates their own <c>GetHashCode</c>
/// method automatically.
/// </summary>
/// <remarks>
/// At default, the generated method will invoke <see cref="HashCode.Combine{T1}(T1)"/> and other methods
/// in this method group (i.e. the method group <c>HashCode.Combine</c>) to get the final hash code.
/// For example, if the attribute references two properties in the type:
/// <code><![CDATA[
/// [AutoOverridesGetHashCode(nameof(A), nameof(B))]
/// public sealed class ExampleType
/// {
///     public int A { get; } = 42;
///     public int B { get; } = 108;
/// }
/// ]]></code>
/// The generated code will be:
/// <code><![CDATA[
/// public override int GetHashCode() => HashCode.Combine(A, B);
/// ]]></code>
/// </remarks>
/// <seealso cref="HashCode"/>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
public sealed class AutoOverridesGetHashCodeAttribute :
	Attribute,
	IMultipleMembersBinder,
	ISealedModifierEmitter,
	IPatternProvider
{
	/// <summary>
	/// Initializes an <see cref="AutoOverridesGetHashCodeAttribute"/> instance via the specified array
	/// of <see cref="string"/> elements indicating the names of the data members you want to take part in
	/// the hash code calculation.
	/// </summary>
	/// <param name="memberNames">The name of data members, represented as a <see cref="string"/> array.</param>
	public AutoOverridesGetHashCodeAttribute(params string[] memberNames) => MemberExpressions = memberNames;


	/// <inheritdoc/>
	public bool EmitsSealedKeyword { get; init; } = false;

	/// <summary>
	/// Indicates the pattern. The default value is <see langword="null"/>.
	/// </summary>
	[DisallowNull]
	public string? Pattern { get; init; }

	/// <inheritdoc/>
	public string[] MemberExpressions { get; }
}
