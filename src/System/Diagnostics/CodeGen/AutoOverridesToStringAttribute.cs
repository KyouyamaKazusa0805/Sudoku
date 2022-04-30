namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attributes that allows user using it applying to a type
/// (especially for <see langword="class"/> or <see langword="struct"/>), indicating the overriden metadata
/// for method <see cref="object.ToString"/>, and make source generator generates their own <c>ToString</c>
/// method automatically.
/// </summary>
/// <remarks>
/// At default, the generated string value will be the concatenation of assignment styled string with key-value pairs.
/// For example, if the attribute references two properties in the type:
/// <code><![CDATA[
/// [AutoOverridesToString(nameof(A), nameof(B))]
/// public sealed class ExampleType
/// {
///     public int A { get; } = 42;
///     public int B { get; } = 108;
/// }
/// ]]></code>
/// The generated output string will be:
/// <code><![CDATA[
/// ExampleType { A = 42, B = 108 }
/// ]]></code>
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
public sealed class AutoOverridesToStringAttribute : Attribute
{
	/// <summary>
	/// Initializes an <see cref="AutoOverridesToStringAttribute"/> instance via the specified array
	/// of <see cref="string"/> elements indicating the names of the data members you want to be output
	/// in the output source file.
	/// </summary>
	/// <param name="memberNames">The name of data members, represented as a <see cref="string"/> array.</param>
	public AutoOverridesToStringAttribute(params string[] memberNames) => MemberNames = memberNames;


	/// <summary>
	/// Indicate the name of members that take part in the output.
	/// </summary>
	public string[] MemberNames { get; }
}
