namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attributes that allows user using it applying to a type
/// (especially for <see langword="class"/> or <see langword="struct"/>), indicating the overriden metadata
/// for method <see cref="object.ToString"/>, and make source generator generates their own <c>ToString</c>
/// method automatically.
/// </summary>
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
